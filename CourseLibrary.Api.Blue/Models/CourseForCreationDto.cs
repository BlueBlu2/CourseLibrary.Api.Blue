using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Models
{
    public class CourseForCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
    }
}
