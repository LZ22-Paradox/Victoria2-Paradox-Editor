using CSVFile;
using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Paradox_Editor.Extensions.Types;

namespace Paradox_Editor.A_Map_Functions
{
    //For use in important CSV file reading.
    public sealed class MainCsvIndexSyntax : ClassMap<ProvinceFile>
    {
        public MainCsvIndexSyntax()
        {
            Map(m => m.ProvinceID).Index(0);
            Map(m => m.red).Index(1);
            Map(m => m.green).Index(2);
            Map(m => m.blue).Index(3);
            Map(m => m.ProvinceName).Index(4);
        }
    }

    public class DataAcquisition
    {
        private string GameDirectory;
        private string ModDirectory;
        private DirectoryStructure Directories = new();

        private Dictionary<uint, ProvinceFile> Provinces = new();

        //Temporary lists that may be sorted later
        private Dictionary<uint, int> ColorsToProvinceIDs = new();
        private Dictionary<string, string> TagsToCountryNames = new();
        private Dictionary<string, Color> CountryNamesToColors = new();

        public DataAcquisition() { }
        public DataAcquisition(string directory)
        {
            if (!directory.Equals(MainWindow.CurrentGameMode)) ///May need fixing later
            {
                GameDirectory = Directory.GetParent(directory).Parent.ToString();
            }
            ModDirectory = directory;
            Directories = CollectDirectoryData(directory);

            CollectDirectoryData(directory);
            GetHistoryFiles();

            PopulateAppendProvinceCSVData();
            PopulateProvinceColorsToIDs();
            PopulateTagsToCountryNames();
            PopulateCountryNamesToColours();
        }

        public string GetModDirectory() => ModDirectory;
        public string GetGameDirectory() => GameDirectory;

        public Dictionary<uint, ProvinceFile> GetProvinces() => Provinces;
        public Dictionary<uint, int> GetColorsToProvinceIDs() => ColorsToProvinceIDs;
        public Dictionary<string, string> GetTagsToCountryNames() => TagsToCountryNames;
        public Dictionary<string, Color> GetCountryNamesToColours() => CountryNamesToColors;
        public ProvinceFile GetProvince(uint provinceID)
        {
            _ = Provinces.TryGetValue(provinceID, out ProvinceFile province);
            return province;
        }
        public void ReplaceProvince(ProvinceFile province)
        {
            Provinces[(uint)province.ProvinceID] = province;
        }


        /// <summary>
        /// Gets Victoria 2's directory paths.
        /// </summary>
        /// <param name="directory"></param>
        public DirectoryStructure CollectDirectoryData(string directory)
        {
            string[] masterFolder = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
            string[] historyProvincePaths = Directory.GetFiles(Path.Combine(directory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            string[] countryCommonFiles = Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
            string countriesCommonFilePath = Path.Combine(directory, "common", "countries.txt");
            string CSVFilePath;
            if (File.Exists(Path.Combine(directory, "map", "definition.csv")))
                CSVFilePath = Path.Combine(directory, "map", "definition.csv");
            else
                CSVFilePath = Path.Combine(GameDirectory, "map", "definition.csv");
            
            return new DirectoryStructure()
            {
                PrimaryDirectories = masterFolder,
                Countries = countryCommonFiles,
                CountriesTxt = countriesCommonFilePath,
                DefinitionCSVPath = CSVFilePath,
                HistoryProvincePaths = historyProvincePaths
            };
        }

        /// <summary>
        /// Populates an initial list of provinces.
        /// </summary>
        public void GetHistoryFiles()
        {
            ///<!!!!!!DOES NOT READ FILES THAT ONLY HAVE SPACES (IE, LIKE "1337 Prome")!!!!! : performance-instease this>
            foreach (string fileEntry in Directories.HistoryProvincePaths)
            {
                var fileName = Path.GetFileName(fileEntry).Replace(".txt", ""); //FileEntry = Filepath
                string[] splitName = null;
                if (!fileEntry.Contains("-"))
                    splitName = fileName.Split(' ');
                else
                    splitName = fileName.Split('-'); //SplitName[1] = Province Name
                if (int.TryParse(splitName[0], out int IDValue)) //IDValue = Province ID
                {
                    var tempProvinceFile = new ProvinceFile();
                    tempProvinceFile.ProvinceID = IDValue;
                    tempProvinceFile.HistoryFilePath = fileEntry;

                    if (splitName.Length == 1) //Fix for in case name is only a number
                    {
                        tempProvinceFile.ProvinceFileName = splitName[0].Trim(); 
                    } else
                    {
                        tempProvinceFile.ProvinceFileName = splitName[1].Trim();
                    }

                    if (!Provinces.ContainsKey((uint)IDValue))
                    {
                        Provinces.Add((uint)IDValue, tempProvinceFile);
                        Provinces[(uint)IDValue].PopulateHistoryData();
                    }
                }
                else
                    Debug.WriteLine("Error listing history provinces. File {0} could not be split.", fileName);
                _ = Provinces.OrderBy(x => x.Key);
            }

        }

        /// <summary>
        /// Populates existing provinces with CSV colour & filename data.
        /// </summary>
        /// <param name="pathToCSVFile"></param>
        /// <returns name="Dictionary<uint, string>"></returns>
        public void PopulateAppendProvinceCSVData()
        {
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
                MissingFieldFound = null
            };
            using var reader = new StreamReader(Directories.DefinitionCSVPath);
            using var csv = new CsvReader(reader, cfg);
            csv.Context.RegisterClassMap<MainCsvIndexSyntax>();
            var records = csv.GetRecords<ProvinceCSVDefinition>().Skip(1);
            {
                Parallel.ForEach(records, record =>
                {
                    if (!string.IsNullOrWhiteSpace(record.province))
                    {
                        if (Provinces.ContainsKey(Convert.ToUInt32(record.province)))
                            Provinces[Convert.ToUInt32(record.province)].AppendFromCSV(record);
                    }
                });
            }
            csv.Dispose();
        }


        /// <summary>
        /// Populates dictionary of province colour keys with province ID's.
        /// </summary>
        public void PopulateProvinceColorsToIDs()
        {
            foreach (ProvinceFile province in Provinces.Values)
            {
                if (!ColorsToProvinceIDs.ContainsKey(province.color))
                    ColorsToProvinceIDs.Add(province.color, province.ProvinceID);
            }
        }

        /// <summary>
        /// Gets given game TAG's and country names, based on the common/...name.txt files.
        /// </summary>
        public void PopulateTagsToCountryNames() 
        {
            foreach (var line in File.ReadAllLines(Directories.CountriesTxt))
            {
                var input = line.Trim();
                var index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);
                if (string.IsNullOrEmpty(input))
                    continue;

                var removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
                var words = removal.Split('='); //Has the actual country names
                var trimmedTagToCountry = new string[] { words[0].Trim(), words[1].Trim() };
                //Debug.WriteLine(trimmedTagToCountry);
                if (!TagsToCountryNames.ContainsKey(trimmedTagToCountry[0]))
                    TagsToCountryNames.Add(trimmedTagToCountry[0], trimmedTagToCountry[1]);
                else
                    Debug.WriteLine("Repeated TAG in common/countries/: " + trimmedTagToCountry[0]);
            }
        }

        /// <summary>
        /// Populates the dictionary of country name keys with their respective country-colours.
        /// </summary>
        public void PopulateCountryNamesToColours()
        {
            foreach (var countryFile in Directories.Countries)
            {
                string[] seperatedColors = new string[3];
                string name = Path.GetFileName(countryFile).Replace(".txt", "");
                string firstLine = File.ReadLines(countryFile).First();
                if (firstLine.StartsWith("color"))
                {
                    var splitData = firstLine.Split("=", StringSplitOptions.TrimEntries);
                    var fixedColorData = splitData[1].Replace("{", "").Replace("}", "").Trim();
                    var splitColors = fixedColorData.Split(" ");
                    seperatedColors = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                }
                else
                {
                    foreach (var line in File.ReadAllLines(countryFile))
                    {
                        if (line.Contains("color =", StringComparison.Ordinal)) //color = { #  #  # }
                        {
                            var splitData = line.Split("=", StringSplitOptions.TrimEntries);
                            var fixedColorData = splitData[1].Replace("{", "").Replace("}", "").Trim();
                            var splitColors = fixedColorData.Split(" ");
                            seperatedColors = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();


                        }

                    }
                }
                if (!CountryNamesToColors.ContainsKey(name))
                {
                    if (!String.IsNullOrEmpty(seperatedColors[0]))
                    CountryNamesToColors.Add(name,
                        Color.FromRgb(
                            (byte)Convert.ToInt32(Regex.Replace(seperatedColors[0], "[A-Za-z ]", "")),
                            (byte)Convert.ToInt32(Regex.Replace(seperatedColors[1], "[A-Za-z ]", "")),
                            (byte)Convert.ToInt32(Regex.Replace(seperatedColors[2], "[A-Za-z ]", ""))
                        ));
                }
            }
        }

        //

    }

}
