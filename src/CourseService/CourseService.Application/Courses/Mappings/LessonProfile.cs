using AutoMapper;
using CourseService.Application.Courses.DTO;
using CourseService.Domain.Entities;

namespace CourseService.Application.Courses.Mappings;

public class LessonProfile : Profile
{
    public LessonProfile()
    {
        CreateMap<LessonDTO, Lesson>();
        CreateMap<Lesson, LessonResponse>();
    }
}
