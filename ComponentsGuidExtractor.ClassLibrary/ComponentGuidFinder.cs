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
        const string componentsRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components";
        const string productsRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products";
        const string sharedDllsRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SharedDLLs";

        public static Dictionary<string, List<Component>> FindComponentGuids(List<string> list, SearchType searchType)
        {
            return SortAndFormatComponentGuidDictionary(FindComponents(list, searchType));
        }

        private static Dictionary<string, List<Component>> FindComponents(List<string> list, SearchType searchType)
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

                RegistryKey products = localMachine.OpenSubKey(productsRoot);
                RegistryKey components = localMachine.OpenSubKey(componentsRoot);
                RegistryKey sharedDlls = Registry.LocalMachine.OpenSubKey(sharedDllsRoot);

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
                            string productName = string.Empty;

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
                                RegistryKey productProperties = null;
                                int sharedDllRefCount;

                                productProperties = products.OpenSubKey(productId + @"\InstallProperties");
                                
                                sharedDllRefCount = (int)(sharedDlls.GetValue(valueData.Replace("?", "")) ?? sharedDlls.GetValue(valueData.Replace("?", "").Replace("SysWOW64", "system32", StringComparison.OrdinalIgnoreCase)) ?? 0);
                                
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

                                if (productProperties != null)
                                {
                                    productName = productProperties.GetValue("DisplayName")?.ToString();

                                    if (productName == null)
                                    {
                                        System.Diagnostics.Debugger.Break();
                                    }
                                }
                                else
                                {
                                    productName = "";
                                }

                                existingComponent.Products.Add(new Product { ProductGuid = productId, ProductName = productName, FilePath = valueData, SharedDllRefCount = sharedDllRefCount });
                            }
                        }
                    }

                }
            }
            return fileComponentsDictionary;
        }

        private static Dictionary<string, List<Component>> SortAndFormatComponentGuidDictionary(Dictionary<string, List<Component>> componentGuidDict)
        {
            if (componentGuidDict != null && componentGuidDict.Count > 0)
            {
                foreach (var key in componentGuidDict.Keys)
                {
                    foreach (var component in componentGuidDict[key])
                    {
                        component.ComponentGuid = component.ComponentGuid.FormatGuid();

                        SortedList<string, Product> Products;

                        Products = new SortedList<string, Product>();

                        foreach (var product in component.Products)
                        {
                            product.ProductGuid = product.ProductGuid.FormatGuid();
                            Products.Add(product.ProductGuid, product);
                        }

                        component.Products = Products.Values;
                    }
                }
            }
            return componentGuidDict;
        }
    }
}
