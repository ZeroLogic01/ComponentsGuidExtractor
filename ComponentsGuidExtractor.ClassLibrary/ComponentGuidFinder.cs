using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentsGuidExtractor.ClassLibrary.Models;
using ComponentsGuidExtractor.ClassLibrary.Enums;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using ComponentsGuidExtractor.ClassLibrary.Extensions;

namespace ComponentsGuidExtractor.ClassLibrary
{
    public static class ComponentGuidFinder
    {
        public static Dictionary<string, List<Component>> FindComponentGuids(string componentsRoot, List<string> list, SearchType searchType)
        {
            return SortComponentGuidDictionary(FindComponents(componentsRoot, list, searchType));
        }

        private static Dictionary<string, List<Component>> FindComponents(string componentsRoot, List<string> list, SearchType searchType)
        {
            Dictionary<string, List<Component>> fileComponentsDictionary = new();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Registry Code
                RegistryKey localMachine;
                if (Directory.Exists("C:\\Windows\\SysWOW64"))
                {
                    localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                }
                else
                {
                    localMachine = Registry.LocalMachine;
                }

                RegistryKey components = localMachine.OpenSubKey(componentsRoot);
                string[] componentIds = components.GetSubKeyNames();



                foreach (var componentId in componentIds)
                {
                    RegistryKey componentKey = components.OpenSubKey(componentId);

                    var productIds = componentKey.GetValueNames();
                    foreach (var productId in productIds)
                    {
                        if (!string.IsNullOrWhiteSpace(productId))
                        {
                            var valueData = (string)componentKey.GetValue(productId);

                            if (string.IsNullOrWhiteSpace(valueData)) { continue; }

                            string matchedFile = string.Empty;

                            switch (searchType)
                            {
                                case SearchType.FileName:
                                    matchedFile = list.FirstOrDefault(a => a.Equals(Path.GetFileName(valueData), StringComparison.OrdinalIgnoreCase));
                                    break;
                                case SearchType.FilePath:
                                    matchedFile = list.FirstOrDefault(a => Path.GetFullPath(a).Equals(Path.GetFullPath(valueData), StringComparison.OrdinalIgnoreCase));
                                    break;
                                default:
                                    break;
                            }

                            if (!string.IsNullOrWhiteSpace(matchedFile))
                            {

                                if (!fileComponentsDictionary.TryGetValue(matchedFile, out List<Component> existingComponentList))
                                {
                                    // Create if not exists in dictionary
                                    existingComponentList = fileComponentsDictionary[matchedFile] = new List<Component>();
                                }

                                var existingComponent = existingComponentList.FirstOrDefault(comp => comp.ComponentGuid.Equals(componentId, StringComparison.OrdinalIgnoreCase));
                                if (existingComponent == null)
                                {
                                    existingComponent = new Component
                                    {
                                        ComponentKey = componentKey.ToString(),
                                        ComponentGuid = componentId,
                                        Products = new List<Product>()
                                    };
                                    existingComponentList.Add(existingComponent);
                                }

                                existingComponent.Products.Add(new Product { ProductGuid = productId, Value = valueData });
                            }
                        }
                    }

                }
            }
            return fileComponentsDictionary;
        }

        private static Dictionary<string, List<Component>> SortComponentGuidDictionary(Dictionary<string, List<Component>> componentGuidDict)
        {
            if (componentGuidDict?.Count > 0)
            {
                foreach (var key in componentGuidDict.Keys)
                {
                    foreach (var component in componentGuidDict[key])
                    {
                        component.ComponentGuid = component.ComponentGuid.FormatGuid();

                        foreach (var product in component.Products)
                        {
                            product.ProductGuid = product.ProductGuid.FormatGuid();
                        }
                    }
                }
            }
            return componentGuidDict;
        }
    }
}
