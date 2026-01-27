using AutoMapper;
using CourseService.Application.DTO;
using CourseService.Application.Interfaces.Repositories;
using CourseService.Application.Interfaces.Services;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Domain.Results;

namespace CourseService.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<Result<bool>> ArchiveCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course.AuthorId != authorId)
        {
            return Result<bool>.Failure($"Forbidden! {authorId} is not the creator of the course");
        }

        await _courseRepository.ArchiveCourseAsync(course);

        return Result<bool>.Success(true);
    }

    public async Task<Result<CourseResponse>> CreateCourseAsync(CourseDTO courseDTO, Guid authorId)
    {
        var course = _mapper.Map<Course>(courseDTO);
        var createdCourse = await _courseRepository.CreateCourseAsync(course);
        var response = _mapper.Map<CourseResponse>(createdCourse);
        
        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<List<CourseResponse>>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
        if(courses == null)
        {
            return Result<List<CourseResponse>>.Failure("List is empty");
        }

        var response = _mapper.Map<List<CourseResponse>>(courses);

        return Result<List<CourseResponse>>.Success(response);
    }

    public async Task<Result<CourseResponse>> GetCourseByIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if(course == null)
        {
            return Result<CourseResponse>.Failure($"Course with {courseId} doesn`t exist");
        }

        var response = _mapper.Map<CourseResponse>(course);

        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<bool>> PublishCourseAsync(Guid courseId, Guid authorId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if(course.AuthorId != authorId)
        {
            return Result<bool>.Failure($"Forbidden! {authorId} is not the creator of the course");
        }

        await _courseRepository.PublishCourseAsync(course);

        return Result<bool>.Success(true);
    }

    public async Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid authorId, Guid courseId)
    {
        var isAuthor = await _courseRepository.GetCourseByIdAsync(authorId);

        if(isAuthor.AuthorId != authorId)
        {
            return Result<CourseResponse>.Failure($"{authorId} is not the creator of the course");
        }

        var course = _mapper.Map<Course>(courseDTO);
        var updatedCourse = await _courseRepository.UpdateCourseAsync(course);
        var response = _mapper.Map<CourseResponse>(updatedCourse);

        return Result<CourseResponse>.Success(response);
    }
}
