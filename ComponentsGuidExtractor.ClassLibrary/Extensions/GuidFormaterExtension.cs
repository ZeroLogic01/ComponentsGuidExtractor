using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentsGuidExtractor.ClassLibrary.Extensions
{
    public static class GuidFormaterExtension
    {
        /// <summary>
        /// Converts the component string to a correctly formatted Guid.
        /// </summary>
        /// <param name="componentString"></param>
        /// <returns></returns>
        public static string FormatGuid(this string componentString)
        {
            if (componentString.Length != 32)
            {
                throw new ArgumentException("Invalid component string length.");
            }

            StringBuilder output = new(componentString.Length);
            output.Append($"{string.Concat(componentString.Take(8).Reverse())}-" +
                $"{string.Concat(componentString.Skip(8).Take(4).Reverse())}-" +
                $"{string.Concat(componentString.Skip(12).Take(4).Reverse())}-");

            for (int i = 16; i < componentString.Length; i += 2)
            {
                if (i == 20)
                {
                    _ = output.Append('-');
                }
                output.Append(string.Concat(componentString.Skip(i).Take(2).Reverse()));
            }
            return output.ToString();
        }
    }
}
