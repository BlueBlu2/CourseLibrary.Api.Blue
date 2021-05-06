using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Blue.Services
{
    public class PropertyMapping<TSource,TDestination>:IPropertyMapping
    {
        public Dictionary<string,PropertyMappingValue> _mappingDictionary { get; private set; }
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
