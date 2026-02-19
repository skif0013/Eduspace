using AutoMapper;
using CourseService.Application.DTO;
using CourseService.Application.Events;
using CourseService.Application.Extentions;
using CourseService.Application.Interfaces.Repositories;
using CourseService.Application.Interfaces.Services;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Domain.Results;
using System.Text.Json;

namespace CourseService.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly IMessagePublisher _publisher;

    public CourseService(
        ICourseRepository courseRepository, 
        IMapper mapper, 
        IMessagePublisher publisher)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
        _publisher = publisher;
    }

    public async Task<Result<string>> ArchiveCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<string>.Failure($"Course with {courseId} not found.");
        }

        if (course.AuthorId != authorId)
        {
            return Result<string>.Failure($"Forbidden! {authorId} is not the creator of the course");
        }
        course.Status = CourseStatus.Archived;
        await _courseRepository.UpdateCourseAsync(course);

        var @event = new CourseArchivedEvent(course.Id, course.AuthorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("course.archived", json);

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
            TotalCount = totalPages,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };

        return Result<PagedCoursesResponse>.Success(response);
    }

    public async Task<Result<CourseResponse>> GetCourseByIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if(course == null)
        {
            return Result<CourseResponse>.Failure($"Course with {courseId} doesn`t exist");
        }

        var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);
        var response = _mapper.Map<CourseResponse>(course);
        response.AverageRating = average;
        response.AmountRatings = amount;

        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<string>> PublishCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if(course == null)
        {
            return Result<string>.Failure($"Course with {courseId} not found.");
        }

        if (course.AuthorId != authorId)
        {
            return Result<string>.Failure($"Forbidden! {authorId} is not the creator of the course.");
        }

        course.Status = CourseStatus.Published;
        await _courseRepository.UpdateCourseAsync(course);

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
            return Result<CourseResponse>.Failure($"Course with {courseId} not found.");
        }

        if (course.AuthorId != authorId)
        {
            return Result<CourseResponse>.Failure($"{authorId} is not the creator of the course");
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

        return Result<CourseResponse>.Success(response);
    }
}
