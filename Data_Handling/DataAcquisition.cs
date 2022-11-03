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
using Paradox_Editor.Extensions;
using System.Windows;
using Paradox_Editor.Cultures;

namespace Paradox_Editor.Data_Handling
{
    //For use in important CSV file reading.
    public class DataAcquisition
    {
        protected string DotModFile;
        protected string ModName;
        protected string GameFolder { get; set; }
        protected string ModFolder { get; set; }
        protected DirectoryStructure ModDirectories = new();

        public DataAcquisition() { }
        public DataAcquisition(string directory)
        {
            ModFolder = directory;
            ModDirectories = CollectModData(directory);
            if (!directory.Equals(MainWindow.CurrentGameMode))  ///May need fixing
            {
                DirectoryInfo modFolder = Directory.GetParent(directory);
                GameFolder = modFolder.Parent.ToString();
                var modName = Path.GetFileName(ModFolder);
                var dotMod = (modFolder + "\\" + modName + ".mod");
                if (File.Exists(dotMod))
                {
                    var dotModLines = File.ReadAllLines(dotMod);
                    var reg = new Regex(@"(?<=([\'\""])).*?(?=\1)");
                    foreach (var line in dotModLines)
                    {
                        switch (line)
                        {
                            case string when line.Contains("name ="):
                                ModName = reg.Match(line).ToString();
                                break;
                            case string when line.Contains("nuts"):
                                break;
                            case string when line.Contains("nuts"):
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Missing mod file : " + DotModFile);
                }
            }
            else
            {
                //Handling for if directory equals game mode IE Victoria or EU4 main folder
            }


        }

        public string GetModFolder() => ModFolder;
        public string GetGameDirectory() => GameFolder;
        public CulturesFile CulturesData;

        /// <summary>
        /// Gets Victoria 2's directory paths.
        /// </summary>
        /// <param name="directory"></param>
        public DirectoryStructure CollectModData(string directory)
        {
            Debug.WriteLine("Collect Data Called! (Should be only once.)");
            string[] masterFolder = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
            string[] historyProvincePaths = Directory.GetFiles(Path.Combine(directory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            string[] countryCommonFiles = Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
            string countriesCommonFilePath = Path.Combine(directory, "common", "countries.txt"); //Find if overwritten
            string culturesCommonFilePath = Path.Combine(directory, "common", "cultures.txt"); //Find if overwritten
            string CSVFilePath;
            if (File.Exists(Path.Combine(directory, "map", "definition.csv")))
                CSVFilePath = Path.Combine(directory, "map", "definition.csv");
            else
                CSVFilePath = Path.Combine(GameFolder, "map", "definition.csv");

            CulturesData = CulturesFile.Parse(culturesCommonFilePath);
            
            return new DirectoryStructure()
            {
                PrimaryDirectories = masterFolder,
                CulturesTxt = culturesCommonFilePath,
                Countries = countryCommonFiles,
                CountriesTxt = countriesCommonFilePath,
                DefinitionCSVPath = CSVFilePath,
                HistoryProvincePaths = historyProvincePaths
            };
        }


    }

}
