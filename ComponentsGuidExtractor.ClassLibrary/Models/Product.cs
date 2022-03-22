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
        /// Represents the product display name in the registry based on the product id the component refers to.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Represents the value name field in the registry. 
        /// Which actually is the product Guid that the component is referenced to.
        /// </summary>
        public string ProductGuid { get; set; }
        
        /// <summary>
        /// Represents value data field in the registry.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Represents the reference count value for the specified FilePath in the SharedDlls registry location.
        /// </summary>
        public int SharedDllRefCount { get; set; }
    }
}
