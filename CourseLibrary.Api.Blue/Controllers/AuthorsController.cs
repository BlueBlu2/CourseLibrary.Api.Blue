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

        public AuthorsController(ICourseLibraryRepository context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        public ActionResult<IEnumerable<Author>> GetAuthors()
        {
            return Ok(_context.GetAuthors());
        }
        [HttpGet("{authorId:guid}")]
        public ActionResult<Author> GetAuthor(Guid authorId)
        {
            var author = _context.GetAuthor(authorId);
            return author != null ? Ok(author) : (ActionResult)NotFound();
        }
    }
}
