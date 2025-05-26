using AutoMapper;
using Uyg.API.DTOs;
using Uyg.API.Models;

namespace Uyg.API.Mapping
{
    public class MapProfile : Profile
    {
       public MapProfile() 
        {
            CreateMap<Lessons,LessonsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ReverseMap();
            CreateMap<Category,CategoryDto>().ReverseMap();
            CreateMap<AppUser,UserDto>().ReverseMap();
            CreateMap<Course,CourseDto>().ReverseMap();
            CreateMap<Lessons,CourseLessonDto>();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<StudentCreateDto, Student>();
        }
    }
}
