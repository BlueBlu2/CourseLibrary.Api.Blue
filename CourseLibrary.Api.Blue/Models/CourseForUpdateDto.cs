using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Models
{
    public class CourseForUpdateDto : CourseForManipulationDto
    {
        [Required]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
