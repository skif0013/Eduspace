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

    public CourseService(
        ICourseRepository courseRepository,
        ICourseCache cache,
        ILogger<CourseService> logger,
        IMapper mapper,
        IMessagePublisher publisher)
    {
        _courseRepository = courseRepository;
        _cache = cache;
        _logger = logger;
        _mapper = mapper;
        _publisher = publisher;
    }

    public async Task<Result<string>> ArchiveCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            //_logger.LogTrace("TRACE");
            //_logger.LogDebug("DEBUG");
            //_logger.LogInformation("INFO");
            //_logger.LogWarning("WARNING");
            //_logger.LogError("ERROR");
            _logger.LogInformation("Course with {CourseId} not found.", courseId);

            return Result<string>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            _logger.LogWarning("Archive denied. Author {AuthorId} is not the owner of course {CourseId}", authorId, courseId);

            return Result<string>.Failure(CourseErrors.NotCourseAuthor);
        }

        var wasPublished = course.Status == CourseStatus.Published;

        course.Status = CourseStatus.Archived;
        await _courseRepository.UpdateCourseAsync(course);

        if (wasPublished)
        {
            await _cache.IncrementCatalogVersionAsync();
            await _cache.RemoveAsync($"course:{course.Id}");
        }

        var @event = new CourseArchivedEvent(course.Id, course.AuthorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("course.archived", json);

        _logger.LogInformation("Course {CourseId} was archived by author {AuthorId}", course.Id, course.AuthorId);

        return Result<string>.Success("Course has been archived");
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

        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<PagedCoursesResponse>> GetPagedCoursesAsync(int page, int pageSize)
    {
        var version = await _cache.GetCatalogVersionAsync();

        var cacheKey = $"courses:v{version}:page:{page}:size:{pageSize}";

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
        var cacheKey = $"course:{courseId}";
        var cacheData = await _cache.GetAsync<CourseResponse>(cacheKey);
        if (cacheData != null)
        {
            return Result<CourseResponse>.Success(cacheData);
        }

        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<CourseResponse>.Failure(CourseErrors.CourseNotFound);
        }

        var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);
        var response = _mapper.Map<CourseResponse>(course);
        response.AverageRating = average;
        response.AmountRatings = amount;

        await _cache.SetAsync(cacheKey, response, CacheEntryType.Course);

        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<string>> PublishCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<string>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            return Result<string>.Failure(CourseErrors.NotCourseAuthor);
        }

        course.Status = CourseStatus.Published;
        await _courseRepository.UpdateCourseAsync(course);

        await _cache.IncrementCatalogVersionAsync();
        await _cache.RemoveAsync($"course:{course.Id}");

        var @event = new CoursePublishedEvent(course.Id, course.AuthorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("course.published", json);

        return Result<string>.Success("Course has been published.");
    }

    public async Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<CourseResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            return Result<CourseResponse>.Failure(CourseErrors.NotCourseAuthor);
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
        await _cache.RemoveAsync($"course:{course.Id}");

        return Result<CourseResponse>.Success(response);
    }
}
