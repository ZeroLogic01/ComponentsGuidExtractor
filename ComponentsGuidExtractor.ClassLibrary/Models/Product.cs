using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentsGuidExtractor.ClassLibrary.Models
{
    public class Product
    {
        /// <summary>
        /// Represents the value name field in the registry. 
        /// Which actually is the product Guid that the component is referenced to.
        /// </summary>
        public string ProductGuid { get; set; }
        
        /// <summary>
        /// Represents value data field in the registry.
        /// </summary>
        public string Value { get; set; }
    }
}
