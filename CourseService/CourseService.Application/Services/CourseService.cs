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

    public Task<Result<bool>> ArchiveCourse(Guid courseId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CourseResponse>> CreateCourseAsync(CourseDTO courseDTO, Guid ownerId)
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

        var response = _mapper.Map<CourseResponse>(courses);

        return Result<List<CourseResponse>>.Success(response);
    }

    public async Task<Result<Course>> GetCourseByIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if(course == null)
        {
            return Result<Course>.Failure($"Course with {courseId} doesn`t exist");
        }

        return Result<Course>.Success(course);
    }

    public Task<Result<bool>> PublishCourse(Guid courseId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid ownerId, Guid courseId)
    {
        var isOwner = await _courseRepository.GetCourseByIdAsync(ownerId);

        if(isOwner.OwnerId != ownerId)
        {
            return Result<CourseResponse>.Failure($"{ownerId} is not the creator of the course");
        }

        var course = _mapper.Map<Course>(courseDTO);
        var updatedCourse = await _courseRepository.UpdateCourseAsync(course);
        var response = _mapper.Map<CourseResponse>(updatedCourse);

        return Result<CourseResponse>.Success(response);
    }
}
