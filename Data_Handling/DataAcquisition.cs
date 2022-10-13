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
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Data_Handling
{
    //For use in important CSV file reading.
    public class DataAcquisition
    {
        protected string GameDirectory;
        protected string ModDirectory;
        protected DirectoryStructure Directories = new();

        public DataAcquisition() { }
        public DataAcquisition(string directory)
        {
            if (!directory.Equals(MainWindow.CurrentGameMode)) ///May need fixing later
            {
                GameDirectory = Directory.GetParent(directory).Parent.ToString();
            }
            ModDirectory = directory;
            Directories = CollectDirectoryData(directory);
        }

        public string GetModDirectory() => ModDirectory;
        public string GetGameDirectory() => GameDirectory;

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


    }

}
