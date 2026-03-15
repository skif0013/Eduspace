using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Events;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CourseService.Application.Courses.Services;

public class LessonService : ILessonService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseCache _cache;
    private readonly ILessonRepository _lessonRepository;
    private readonly ILogger<LessonService> _logger;
    private readonly IMapper _mapper;
    private readonly IMessagePublisher _publisher;
    private readonly IRedisKeyBuilder _keyBuilder;

    public LessonService(
        ICourseRepository courseRepository,
        ICourseCache cache,
        ILessonRepository lessonRepository,
        ILogger<LessonService> logger,
        IMapper mapper,
        IMessagePublisher publisher,
        IRedisKeyBuilder keyBuilder)
    {
        _courseRepository = courseRepository;
        _cache = cache;
        _lessonRepository = lessonRepository;
        _logger = logger;
        _mapper = mapper;
        _publisher = publisher;
        _keyBuilder = keyBuilder;
    }

    public async Task<Result<LessonResponse>> CreateLessonAsync(LessonDTO lessonDTO, Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogInformation("Course {CourseId} not found", courseId);

            return Result<LessonResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            _logger.LogWarning(
                "Create lesson denied. Author {AuthorId} is not the owner of course {CourseId}",
                authorId,
                courseId);

            return Result<LessonResponse>.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            _logger.LogWarning("Create lesson denied. Course {CourseId} archived", courseId);

            return Result<LessonResponse>.Failure(CourseErrors.CourseArchived);
        }

        var lesson = _mapper.Map<Lesson>(lessonDTO);
        lesson.CourseId = courseId;
        var createdLesson = await _lessonRepository.CreateLessonAsync(lesson);

        var response = _mapper.Map<LessonResponse>(createdLesson);

        var @event = new LessonCreatedEvent(createdLesson.Id, createdLesson.CourseId, authorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("lesson.created", json);

        var cacheKey = _keyBuilder.GetCourseKey(courseId);
        await _cache.RemoveAsync(cacheKey);

        _logger.LogInformation(
            "Lesson {LessonId} created in Course {CourseId} by Author {AuthorId}",
            createdLesson.Id,
            createdLesson.CourseId,
            authorId);

        return Result<LessonResponse>.Success(response);
    }

    public async Task<Result> DeleteLessonAsync(Guid lessonId, Guid courseId, Guid authorId)
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
                "Delete lesson denied. Author {AuthorId} is not the owner of course {CourseId}",
                authorId,
                courseId);

            return Result.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            _logger.LogWarning("Delete lesson denied. Course {CourseId} archived", courseId);

            return Result.Failure(CourseErrors.CourseArchived);
        }

        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        if (lesson == null || lesson.CourseId != courseId)
        {
            _logger.LogInformation(
                "Lesson {LessonId} not found in Course {CourseId}",
                lessonId,
                courseId);

            return Result.Failure(LessonErrors.LessonNotFound);
        }

        await _lessonRepository.DeleteLessonAsync(lesson);

        var cacheKey = _keyBuilder.GetCourseKey(courseId);
        await _cache.RemoveAsync(cacheKey);

        _logger.LogInformation(
            "Lesson {LessonId} deleted in Course {CourseId} by Author {AuthorId}",
            lesson.Id,
            lesson.CourseId,
            authorId);

        return Result.Success();
    }

    public async Task<Result<LessonResponse>> GetLessonByIdAsync(Guid lessonId)
    {
        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        if (lesson == null)
        {
            _logger.LogInformation("Lesson {LessonId} not found", lessonId);

            return Result<LessonResponse>.Failure(LessonErrors.LessonNotFound);
        }

        var response = _mapper.Map<LessonResponse>(lesson);

        return Result<LessonResponse>.Success(response);
    }

    public async Task<Result<LessonResponse>> UpdateLessonAsync(LessonDTO lessonDTO, Guid lessonId, Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogInformation("Course {CourseId} not found", courseId);

            return Result<LessonResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            _logger.LogWarning(
                "Update lesson denied. Author {AuthorId} is not the owner of course {CourseId}",
                authorId,
                courseId);

            return Result<LessonResponse>.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            _logger.LogWarning("Update lesson denied. Course {CourseId} archived", courseId);

            return Result<LessonResponse>.Failure(CourseErrors.CourseArchived);
        }

        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        if (lesson == null || lesson.CourseId != courseId)
        {
            _logger.LogInformation(
                "Lesson {LessonId} not found in Course {CourseId}",
                lessonId,
                courseId);

            return Result<LessonResponse>.Failure(LessonErrors.LessonNotFound);
        }

        lesson.LessonNumber = lessonDTO.LessonNumber;
        lesson.Name = lessonDTO.Name;
        lesson.Description = lessonDTO.Description;
        lesson.VideoUrl = lessonDTO.VideoUrl;

        await _lessonRepository.UpdateLessonAsync(lesson);

        var response = _mapper.Map<LessonResponse>(lesson);

        var cacheKey = _keyBuilder.GetCourseKey(courseId);
        await _cache.RemoveAsync(cacheKey);

        _logger.LogInformation(
            "Lesson {LessonId} updated in Course {CourseId} by Author {AuthorId}",
            lesson.Id,
            lesson.CourseId,
            authorId);

        return Result<LessonResponse>.Success(response);
    }
}
