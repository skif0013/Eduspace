using AutoMapper;
using CourseService.Application.DTO;
using CourseService.Domain.Entities;

namespace CourseService.Application.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile() 
    { 
        CreateMap<CourseDTO, Course>();
        CreateMap<Course, CourseResponse>();
        CreateMap<List<Course>, List<CourseResponse>>();
    }
}
