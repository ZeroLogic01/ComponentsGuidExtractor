using ComponentsGuidExtractor.ClassLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                "UnRAR.dll"
            };

            string componentsRoot = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components";


            var result = ComponentGuidFinder.FindComponentGuids(componentsRoot, filesList, 
                ComponentsGuidExtractor.ClassLibrary.Enums.SearchType.FileName);

            // to-do: export result to a text or a json file

            Console.ReadLine();
        }
    }
}
