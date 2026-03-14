using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Events;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Extentions;
using CourseService.Application.Messaging;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CourseService.Application.Courses.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseCache _cache;
    private readonly ILogger<CourseService> _logger;
    private readonly IMapper _mapper;
    private readonly IMessagePublisher _publisher;
    private readonly IRedisKeyBuilder _keyBuilder;

    public CourseService(
        ICourseRepository courseRepository,
        ICourseCache cache,
        ILogger<CourseService> logger,
        IMapper mapper,
        IMessagePublisher publisher,
        IRedisKeyBuilder keyBuilder)
    {
        _courseRepository = courseRepository;
        _cache = cache;
        _logger = logger;
        _mapper = mapper;
        _publisher = publisher;
        _keyBuilder = keyBuilder;
    }

    public async Task<Result> ArchiveCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogInformation("Course {CourseId} not found", courseId);

            return Result.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            _logger.LogWarning(
                "Archive denied. Author {AuthorId} is not the owner of course {CourseId}", 
                authorId, 
                courseId);

            return Result.Failure(CourseErrors.NotCourseAuthor);
        }

        var wasPublished = course.Status == CourseStatus.Published;

        course.Status = CourseStatus.Archived;
        await _courseRepository.UpdateCourseAsync(course);

        if (wasPublished)
        {
            await _cache.IncrementCatalogVersionAsync();
            var cackeKey = _keyBuilder.GetCourseKey(courseId);
            await _cache.RemoveAsync(cackeKey);
        }

        var @event = new CourseArchivedEvent(course.Id, course.AuthorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("course.archived", json);

        _logger.LogInformation(
            "Course {CourseId} archived by author {AuthorId}", 
            course.Id, 
            course.AuthorId);

        return Result.Success();
    }

    public async Task<Result<CourseResponse>> CreateCourseAsync(CourseDTO courseDTO, Guid authorId)
    {
        var course = _mapper.Map<Course>(courseDTO);
        course.AuthorId = authorId;
        course.Status = CourseStatus.Draft;
        var createdCourse = await _courseRepository.CreateCourseAsync(course);

        var response = _mapper.Map<CourseResponse>(createdCourse);

        var @event = new CourseCreatedEvent(course.Id, course.AuthorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("course.created", json);

        _logger.LogInformation(
            "Course {CourseId} created by Author {AuthorId}",
            createdCourse.Id,
            createdCourse.AuthorId);

        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<PagedCoursesResponse>> GetPagedCoursesAsync(int page, int pageSize)
    {
        var version = await _cache.GetCatalogVersionAsync();

        var cacheKey = _keyBuilder.GetCoursesPageKey(version, page, pageSize);

        var cachedData = await _cache.GetAsync<PagedCoursesResponse>(cacheKey);
        if (cachedData != null)
        {
            return Result<PagedCoursesResponse>.Success(cachedData);
        }

        var pagedCourses = await _courseRepository.GetPagedCoursesAsync(page, pageSize);

        var coursesRating = pagedCourses.Items.Select(course =>
        {
            var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);
            var dto = _mapper.Map<CourseResponse>(course);
            dto.AverageRating = average;
            dto.AmountRatings = amount;

            return dto;
        }).ToList();

        var totalPages = (int)Math.Ceiling(pagedCourses.TotalCount / (double)pageSize);

        var response = new PagedCoursesResponse
        {
            Courses = coursesRating,
            Page = page,
            PageSize = pageSize,
            TotalCount = pagedCourses.TotalCount,
            TotalPages = totalPages
        };

        await _cache.SetAsync(cacheKey, response, CacheEntryType.Catalog);

        return Result<PagedCoursesResponse>.Success(response);
    }

    public async Task<Result<CourseResponse>> GetCourseByIdAsync(Guid courseId)
    {
        var cacheKey = _keyBuilder.GetCourseKey(courseId);
        var cacheData = await _cache.GetAsync<CourseResponse>(cacheKey);
        if (cacheData != null)
        {
            return Result<CourseResponse>.Success(cacheData);
        }

        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogWarning("Course {CourseId} not found", courseId);

            return Result<CourseResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (!course.IsFree)
        {
            // TODO Implement Payment
            return Result<CourseResponse>.Failure(CourseErrors.CourseRequiresPayment);
        }

        var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);
        var response = _mapper.Map<CourseResponse>(course);
        response.AverageRating = average;
        response.AmountRatings = amount;

        await _cache.SetAsync(cacheKey, response, CacheEntryType.Course);

        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result> PublishCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogInformation("Course {CourseId} not found", courseId);

            return Result.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            _logger.LogWarning(
                "Publish denied. Author {AuthorId} is not the owner of course {CourseId}",
                authorId,
                courseId);

            return Result.Failure(CourseErrors.NotCourseAuthor);
        }

        course.Status = CourseStatus.Published;
        await _courseRepository.UpdateCourseAsync(course);

        await _cache.IncrementCatalogVersionAsync();
        var key = _keyBuilder.GetCourseKey(courseId);
        await _cache.RemoveAsync(key);

        var @event = new CoursePublishedEvent(course.Id, course.AuthorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("course.published", json);

        _logger.LogInformation(
            "Course {CourseId} published by Author {AuthorId}",
            courseId,
            authorId);

        return Result.Success();
    }

    public async Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogInformation("Course {CourseId} not found", courseId);
            
            return Result<CourseResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            _logger.LogWarning(
                "Update denied. Author {AuthorId} is not the owner of the Course {CourseId}",
                authorId,
                courseId);

            return Result<CourseResponse>.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            return Result<CourseResponse>.Failure(CourseErrors.CourseArchived);
        }

        course.Name = courseDTO.Name;
        course.Description = courseDTO.Description;
        course.Price = courseDTO.Price;
        course.AvatarURL = courseDTO.AvatarURL;
        await _courseRepository.UpdateCourseAsync(course);

        var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);
        var response = _mapper.Map<CourseResponse>(course);
        response.AverageRating = average;
        response.AmountRatings = amount;

        await _cache.IncrementCatalogVersionAsync();
        var cackeKey = _keyBuilder.GetCourseKey(courseId);
        await _cache.RemoveAsync(cackeKey);

        _logger.LogInformation(
            "Course {CureseId} updated by Author {AuthorId}",
            response.Id,
            authorId);

        return Result<CourseResponse>.Success(response);
    }
}
