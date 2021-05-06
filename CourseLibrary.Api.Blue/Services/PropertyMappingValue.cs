using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; private set; }
        public bool Revert { get; private set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperies, bool revert = false)
        {
            DestinationProperties = destinationProperies ?? throw new ArgumentNullException(nameof(destinationProperies));
            Revert = revert;
        }
    }
}
