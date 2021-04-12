using AutoMapper;
using CourseLibrary.Api.Blue.Models;
using CourseLibrary.API.Blue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<CourseForCreationDto, Course>();
        }
    }
}
