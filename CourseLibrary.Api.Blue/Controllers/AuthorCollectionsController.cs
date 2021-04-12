using AutoMapper;
using CourseLibrary.Api.Blue.Helpers;
using CourseLibrary.Api.Blue.Models;
using CourseLibrary.API.Blue.Entities;
using CourseLibrary.API.Blue.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Controllers
{
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionsController: ControllerBase
    {
        private readonly ICourseLibraryRepository _context;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ICourseLibraryRepository context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name ="GetAuthorCollection")]
        public IActionResult GetAuthorCollection([FromRoute] 
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _context.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(
            IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);
            foreach (var author in authorEntities)
            {
                _context.AddAuthor(author);
            }
            _context.Save();

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorCollectionToReturn);
        }
    }
}
