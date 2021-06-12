using AutoMapper;
using CourseLibrary.Api.Blue.Models;
using CourseLibrary.API.Blue.Entities;
using CourseLibrary.API.Blue.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    //[ResponseCache(CacheProfileName = "240SecondsCacheProfile")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _context;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }    
        [HttpGet(Name = "GetCoursesForAuthor")]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            return _context.AuthorExists(authorId) ? Ok(_mapper.Map<IEnumerable<CourseDto>>(_context.GetCourses(authorId))) : (ActionResult)NotFound();
        }
        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        //[ResponseCache(Duration = 120)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_context.AuthorExists(authorId))
            {
                return NotFound();
            }
            var course = _context.GetCourse(authorId, courseId);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CourseDto>(course));
        }
        [HttpPost(Name = "CreateCourseForAuthor")]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if (!_context.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseEntity = _mapper.Map<Course>(course);
            _context.AddCourse(authorId, courseEntity);
            _context.Save();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);
            return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId = authorId, courseId = courseToReturn.Id}, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (!_context.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAuthorFromRepo = _context.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;

                _context.AddCourse(authorId, courseToAdd);
                _context.Save();
                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);
                return CreatedAtRoute("GetCourseForAuthor",
                        new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }
            _mapper.Map(course, courseForAuthorFromRepo);
            _context.UpdateCourse(courseForAuthorFromRepo);
            _context.Save();

            return NoContent();
        }
        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!_context.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAuthorFromRepo = _context.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null)
            {
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }

                var courseToAdd = _mapper.Map<Course>(courseDto);
                courseToAdd.Id = courseId;

                _context.AddCourse(authorId, courseToAdd);
                _context.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }
            var courseToPatch = _mapper.Map<CourseForUpdateDto>(courseForAuthorFromRepo);
            patchDocument.ApplyTo(courseToPatch, ModelState);

            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(courseToPatch, courseForAuthorFromRepo);
            _context.UpdateCourse(courseForAuthorFromRepo);
            _context.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_context.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _context.GetCourse(authorId, courseId);

            if (courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            _context.DeleteCourse(courseForAuthorFromRepo);
            _context.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var option = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)option.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
