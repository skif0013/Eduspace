using AutoMapper;
using CourseService.Application.Courses.DTO;
using CourseService.Domain.Entities;

namespace CourseService.Application.Courses.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CourseDTO, Course>();
        CreateMap<Course, CourseResponse>();
    }
}
