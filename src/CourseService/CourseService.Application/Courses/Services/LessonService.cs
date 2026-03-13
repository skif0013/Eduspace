using AutoMapper;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Events;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using System.Text.Json;

namespace CourseService.Application.Courses.Services;

public class LessonService : ILessonService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IMapper _mapper;
    private readonly IMessagePublisher _publisher;

    public LessonService(
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        IMapper mapper,
        IMessagePublisher publisher)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _mapper = mapper;
        _publisher = publisher;
    }

    public async Task<Result<LessonResponse>> CreateLessonAsync(LessonDTO lessonDTO, Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<LessonResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            return Result<LessonResponse>.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            return Result<LessonResponse>.Failure(CourseErrors.CourseArchived);
        }

        var lesson = _mapper.Map<Lesson>(lessonDTO);
        lesson.CourseId = courseId;
        var createdLesson = await _lessonRepository.CreateLessonAsync(lesson);

        var response = _mapper.Map<LessonResponse>(createdLesson);

        var @event = new LessonCreatedEvent(lesson.Id, lesson.CourseId, authorId);
        var json = JsonSerializer.Serialize(@event);
        await _publisher.PublishAsync("lesson.created", json);

        return Result<LessonResponse>.Success(response);
    }

    public async Task<Result> DeleteLessonAsync(Guid lessonId, Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<LessonResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            return Result<LessonResponse>.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            return Result<LessonResponse>.Failure(CourseErrors.CourseArchived);
        }

        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        if (lesson == null || lesson.CourseId != courseId)
        {
            return Result<LessonResponse>.Failure(LessonErrors.LessonNotFound);
        }

        await _lessonRepository.DeleteLessonAsync(lesson);

        return Result.Success();
    }

    public async Task<Result<LessonResponse>> GetLessonByIdAsync(Guid lessonId)
    {
        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        if (lesson == null)
        {
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
            return Result<LessonResponse>.Failure(CourseErrors.CourseNotFound);
        }

        if (course.AuthorId != authorId)
        {
            return Result<LessonResponse>.Failure(CourseErrors.NotCourseAuthor);
        }

        if (course.Status == CourseStatus.Archived)
        {
            return Result<LessonResponse>.Failure(CourseErrors.CourseArchived);
        }

        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        if (lesson == null || lesson.CourseId != courseId)
        {
            return Result<LessonResponse>.Failure(LessonErrors.LessonNotFound);
        }

        lesson.LessonNumber = lessonDTO.LessonNumber;
        lesson.Name = lessonDTO.Name;
        lesson.Description = lessonDTO.Description;
        lesson.VideoUrl = lessonDTO.VideoUrl;

        await _lessonRepository.UpdateLessonAsync(lesson);

        var resopnse = _mapper.Map<LessonResponse>(lesson);

        return Result<LessonResponse>.Success(resopnse);
    }
}
