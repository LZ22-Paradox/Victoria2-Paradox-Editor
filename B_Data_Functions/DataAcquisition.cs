using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.B_Map_Functions;
using Paradox_Editor.D_Class_Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace Paradox_Editor
{
    public class DataAcquisition
    {


        public DataAcquisition()
        {
        }

        public sealed class MainCsvIndexSyntax : ClassMap<ProvinceDefinition>
        {
            public MainCsvIndexSyntax()
            {
                Map(m => m.province).Index(0);
                Map(m => m.red).Index(1);
                Map(m => m.green).Index(2);
                Map(m => m.blue).Index(3);
            }
        }

        public DirectoryStructure CollectDirectoryData(string directory)
        {
            var MasterFolder = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
            var Path_Provinces = Directory.GetFiles(Path.Combine(directory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            var Path_CSV = Path.Combine(directory, "map", "definition.csv");
            var Path_CountriesTxt = Path.Combine(directory, "common", "countries.txt");
            var Path_Countries = Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
            return new DirectoryStructure()
            {
                MasterDirectory = MasterFolder,
                Countries = Path_Countries,
                CountriesTxt = Path_CountriesTxt,
                DefinitionCSV = Path_CSV,
                HistoryProvinces = Path_Provinces
            };
        }

        public Dictionary<string, string> GetProvinceColorToID(string pathToCSVFile)
        {
            var colorToProvinceId = new Dictionary<string, string>();

            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
            };
            using (var reader = new StreamReader(pathToCSVFile))
            using (var csv = new CsvReader(reader, cfg))
            {
                csv.Context.RegisterClassMap<MainCsvIndexSyntax>();
                var records = csv.GetRecords<ProvinceDefinition>();
                {
                    colorToProvinceId = csv.GetRecords<ProvinceDefinition>().ToDictionary(c => c.red + " " + c.green + " " + c.blue, c => c.province); //The source of error
                }
            }
            return colorToProvinceId;
        }

        public Dictionary<string, string> GetTagToCountryName(string pathToCountriesTxt)
        {
            var tagToCountryName = new Dictionary<string, string>();

            foreach (var line in File.ReadAllLines(pathToCountriesTxt)) //Check Path_CountriesTxt
            {
                var input = line;
                var index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);

                if (string.IsNullOrEmpty(input))
                    continue;

                var removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
                var words = removal.Split('=');
                var trimmedTagToCountry = new string[] { words[0].Trim(), words[1].Trim() };
                Debug.WriteLine(trimmedTagToCountry);
                if (!tagToCountryName.ContainsKey(trimmedTagToCountry[0]))
                {
                    tagToCountryName.Add(trimmedTagToCountry[0], trimmedTagToCountry[1]);
                }
                else
                {
                    Debug.WriteLine("Repeated TAG in common/countries/: " + trimmedTagToCountry[0]);
                }
            }
            return tagToCountryName;
        }

        public ProvinceOutputData GetProvinceIDToData(string[] pathToHistoryFile)
        {

            var provinceIDToFile = new Dictionary<string, string>();
            var provinceIDToProvinceName = new Dictionary<string, string>();
            var provinceIDToHistoryFile = new Dictionary<string, HistoryFile>();
            var ExtractedData = new TextFileExtractor(pathToHistoryFile, provinceIDToFile, provinceIDToProvinceName, provinceIDToHistoryFile).ExtractForDictionary();

            var provinceIDToCoreTAGs = new Dictionary<string, List<string>>();

            var provinceIDToOwnerTAG = new Dictionary<string, string>(); //Stripey lines; controlled by country
            var provinceIDToControllerTAG = new Dictionary<string, string>(); //Stripey lines; controlled by country
            foreach (KeyValuePair<string, HistoryFile> entry in ExtractedData.ToHistoryFile) //split into new class or into textfilextract
            {
                if (entry.Value.Controller.Count != 0)
                {
                    provinceIDToOwnerTAG.Add(entry.Key, entry.Value.Owner[0]);
                    provinceIDToControllerTAG.Add(entry.Key, entry.Value.Controller[0]);
                }
                else
                {
                    provinceIDToOwnerTAG.Add(entry.Key, "noOwner");
                    provinceIDToControllerTAG.Add(entry.Key, "noController");
                }
            }


            foreach (KeyValuePair<string, HistoryFile> entry in ExtractedData.ToHistoryFile) //split into new class or into textfilextract
            {
                if (entry.Value.Core.Count != 0)
                {
                    var coreList = new List<string>();


                    for (int i = 0; i < entry.Value.Core.Count; i++)
                    {
                        coreList.Add(entry.Value.Core[i]);
                    }

                    provinceIDToCoreTAGs.Add(entry.Key, coreList);
                    
                }
                else
                {
                    provinceIDToCoreTAGs.Add(entry.Key, null);
                }

            }

            return new ProvinceOutputData()
            {
                IDToFile = ExtractedData.ToFile,
                IDToName = ExtractedData.ToName,
                IDToHistory = ExtractedData.ToHistoryFile, //IDToCores = Text.provinceIDToCoresDictionary,
                IDToOwner = provinceIDToOwnerTAG,
                IDToController = provinceIDToControllerTAG,
                IDToCores = provinceIDToCoreTAGs
            };
        }

        //

    }
}