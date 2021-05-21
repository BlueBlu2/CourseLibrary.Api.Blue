using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Models
{
    public class AuthorForCreationWithDateOfDeathDto:AuthorForCreationDto
    {
        public DateTimeOffset? DateOfDeath { get; set; }
    }
}
