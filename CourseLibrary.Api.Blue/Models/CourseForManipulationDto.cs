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
    public abstract class CourseForManipulationDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public virtual string Description { get; set; }
    }
}
