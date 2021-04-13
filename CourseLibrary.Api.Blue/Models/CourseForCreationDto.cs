using CourseLibrary.Api.Blue.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Models
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute]
    public class CourseForCreationDto// : IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        public Guid AuthorId { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Title == Description)
        //    {
        //        yield return new ValidationResult("The provided description should be different from the title."
        //            , new[] { "CourseForCreationDto" });
        //    }
        //}
    }
}
