using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentsGuidExtractor.ClassLibrary.Enums
{
    public enum SearchType
    {
        /// <summary>
        /// Search by file name only.
        /// </summary>
        FileName = 0,
        /// <summary>
        /// Search by full path of the file.
        /// </summary>
        FilePath = 1
    }
}
