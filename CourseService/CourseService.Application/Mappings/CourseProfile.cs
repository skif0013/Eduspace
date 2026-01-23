using AutoMapper;
using CourseService.Application.DTO;
using CourseService.Domain.Entities;

namespace CourseService.Application.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile() 
    { 
        CreateMap<CreateCourseDTO, Course>();
        CreateMap<Course, CourseResponse>();
    }
}
