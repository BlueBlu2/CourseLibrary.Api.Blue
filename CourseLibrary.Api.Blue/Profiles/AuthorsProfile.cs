using AutoMapper;
using CourseLibrary.Api.Blue.Models;
using CourseLibrary.API.Blue.Entities;
using CourseLibrary.API.Blue.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.GetCurrentAge()))
                .ForMember(d => d.Name, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));
            CreateMap<AuthorForCreationDto, Author>();
        }
    }
}
