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

    public async Task<Result<CourseResponse>> CreateCourseAsync(CreateCourseDTO courseDTO, Guid ownerId)
    {
        var course = _mapper.Map<Course>(courseDTO);

        var createdCourse = await _courseRepository.CreateCourseAsync(course);

        var response = _mapper.Map<CourseResponse>(createdCourse);
        
        return Result<CourseResponse>.Success(response);
    }

    public async Task<Result<List<Course>>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
        if(courses == null)
        {
            return Result<List<Course>>.Failure("List is empty");
        }

        return Result<List<Course>>.Success(courses);
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

    public async Task<Result<CourseResponse>> UpdateCourseAsync(UpdateCourseDTO courseDTO, Guid ownerId)
    {
        var isOwner = await _courseRepository.GetCourseByIdAsync(courseDTO.Id);

        if(isOwner.OwnerId != ownerId)
        {
            return Result<CourseResponse>.Failure($"{ownerId} is not the creator of the course");
        }
            
        var course = new Course()
        {
            Name = courseDTO.Name,
            Description = courseDTO.Description,
            Price = courseDTO.Price,
            AvatarURL = courseDTO.AvatarURL,
            Status = CourseStatus.Draft
        };

        var updatedCourse = await _courseRepository.UpdateCourseAsync(course);

        var response = new CourseResponse()
        {
            Id = updatedCourse.Id,
            Name = updatedCourse.Name,
            Description = updatedCourse.Description,
            Price = updatedCourse.Price,
            AvatarURL = updatedCourse.AvatarURL,
            Status = updatedCourse.Status,
            CreatedAt = updatedCourse.CreatedAt
        };

        return Result<CourseResponse>.Success(response);
    }
}
