using AutoMapper;
using CourseLibrary.Api.Blue.Helpers;
using CourseLibrary.Api.Blue.Models;
using CourseLibrary.Api.Blue.Services;
using CourseLibrary.API.Blue.Entities;
using CourseLibrary.API.Blue.ResourceParameters;
using CourseLibrary.API.Blue.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _context;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public AuthorsController(ICourseLibraryRepository context, IMapper mapper, 
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService 
                ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        [HttpGet(Name = "GetAuthors"),HttpHead]
        public IActionResult GetAuthors([FromQuery]AuthorsResourceParameters authorsResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }
            if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
            {
                return BadRequest();
            }
            var authorsFromRepo = _context.GetAuthors(authorsResourceParameters);
            

            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages
            };
            var option = new JsonSerializerOptions();
            option.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata,option ));

            var links = CreateLinksForAuthors(authorsResourceParameters,
                authorsFromRepo.HasNext,authorsFromRepo.HasPrevious);
            var shapedAutors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo)
                .ShapeData(authorsResourceParameters.Fields);

            var shapedAuthorWithLinks = shapedAutors.Select(author => {
                var authorAsDictionary = author as IDictionary<string, object>;
                var authorLinks = CreateLinksForAuthor((Guid)authorAsDictionary["Id"], null);
                authorAsDictionary.Add("links", authorLinks);
                return authorAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedAuthorWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }
        [HttpGet("{authorId:guid}",Name = "GetAuthor"),HttpHead("{authorId:guid}")]
        public IActionResult GetAuthor(Guid authorId, string fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }
            var author = _context.GetAuthor(authorId);
            if(author == null)
            {
                return NotFound();
            }
            var links = CreateLinksForAuthor(authorId, fields);
            var linkedResourceToReturn = _mapper.Map<AuthorDto>(author).ShapeData(fields)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return Ok(linkedResourceToReturn); 
        }
        [HttpPost(Name = "CreateAuthor")]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = _mapper.Map<Author>(author);
            _context.AddAuthor(authorEntity);
            _context.Save();
            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            var links = CreateLinksForAuthor(authorToReturn.Id, null);
            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return CreatedAtRoute("GetAuthor", new { authorId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{authorId}",Name ="DeleteAuthor")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _context.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _context.DeleteAuthor(authorFromRepo);

            _context.Save();

            return NoContent();
        }
        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                      new
                      {
                          fields = authorsResourceParameters.Fields,
                          orderBy = authorsResourceParameters.OrderBy,
                          pageNumber = authorsResourceParameters.PageNumber - 1,
                          pageSize = authorsResourceParameters.PageSize,
                          mainCategory = authorsResourceParameters.MainCategory,
                          searchQuery = authorsResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                      new
                      {
                          fields = authorsResourceParameters.Fields,
                          orderBy = authorsResourceParameters.OrderBy,
                          pageNumber = authorsResourceParameters.PageNumber + 1,
                          pageSize = authorsResourceParameters.PageSize,
                          mainCategory = authorsResourceParameters.MainCategory,
                          searchQuery = authorsResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetAuthors",
                    new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        pageNumber = authorsResourceParameters.PageNumber,
                        pageSize = authorsResourceParameters.PageSize,
                        mainCategory = authorsResourceParameters.MainCategory,
                        searchQuery = authorsResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor (Guid authorId, string fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetAuthor", new { authorId }),"self","GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetAuthor", new { authorId, fields}), "self", "GET"));
            }
            links.Add(new LinkDto(Url.Link("DeleteAuthor", new { authorId }), "delete_author", "DELETE"));
            links.Add(new LinkDto(
                Url.Link("CreateCourseForAuthor", new { authorId }), "create_course_for_author", "POST"));
            links.Add(new LinkDto(Url.Link("GetCoursesForAuthor", new { authorId }), "courses", "GET"));
            return links;
        }
        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters,
           bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateAuthorsResourceUri(
                   authorsResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateAuthorsResourceUri(
                      authorsResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(
                        authorsResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
