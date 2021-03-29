using AutoMapper;
using CourseLibrary.Api.Blue.Models;
using CourseLibrary.API.Blue.Entities;
using CourseLibrary.API.Blue.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _context;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet,HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors()
        {
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(_context.GetAuthors()));
        }
        [HttpGet("{authorId:guid}"),HttpHead("{authorId:guid}")]
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _context.GetAuthor(authorId);
            return author != null ? Ok(_mapper.Map<AuthorDto>(author)) : (ActionResult)NotFound();
        }
    }
}
