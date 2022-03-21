using ComponentsGuidExtractor.ClassLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CompStringToGuidConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> filesList = new()
            {
                "comctl32.Ocx",
                "ComDlg32.OCX",
                "mscomct2.ocx",
                "mscomctl.OCX",
                "msstdfmt.dll",
                "tabctl32.ocx",
                "Tlbinf32.dll",
                "UnRAR.dll",
                "vsjitdebuggerps.dll"
            };

            string componentsRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components";

            Console.WriteLine("Reading components registry...");
            var extractedComponentsGuidDictionary = ComponentGuidFinder.FindComponentGuids(componentsRoot, filesList,
                ComponentsGuidExtractor.ClassLibrary.Enums.SearchType.FileName);
            Console.WriteLine("Read components registry.");
            if (extractedComponentsGuidDictionary?.Count > 0)
            {
                string outputFileName = @"Extracted Components Guids.json";
                
                Console.WriteLine($"Writing to file '{outputFileName}'...");

                File.WriteAllText(outputFileName, JsonSerializer.Serialize(extractedComponentsGuidDictionary
                    , new JsonSerializerOptions { WriteIndented = true }));
                
                Console.WriteLine($"Components Guid written to file '{outputFileName}' successfully.");
            }
            else
            {
                Console.WriteLine("No Component Guid found.");
            }
        }
    }
}
