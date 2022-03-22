using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentsGuidExtractor.ClassLibrary.Models
{
    public class Component
    {
        /// <summary>
        /// The component key string in the registry.
        /// </summary>
        public string ComponentKey { get; set; }

        /// <summary>
        /// The Component Guid
        /// </summary>
        public string ComponentGuid { get; set; }

        /// <summary>
        /// Product list using this component
        /// </summary>
        public IList<Product> Products { get; set; }
    }
}
