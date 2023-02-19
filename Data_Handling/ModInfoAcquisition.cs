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
    public class ModInfoAcquisition
    {
        private string DotModFile; //Unused
		private string ModName;
		private string GameFolder { get; set; }
		private string ModFolder { get; set; }
		private DirectoryStructure ModDirectories = new();

        public ModInfoAcquisition() { }
        public ModInfoAcquisition(string directory)
        {
            ModFolder = directory;
            ModDirectories = CollectModData(directory);
            if (!directory.Equals(MainWindow.CurrentGameMode)) //May Need Fixing
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
                            case string when line.Contains("nuts"): //Nuts?
                                break;
                            case string when line.Contains("nuts"): //Nuts?
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
		public DirectoryStructure GetModDirectories() => ModDirectories;
		/// <summary>
		/// Gets Victoria 2's directory paths.
		/// </summary>
		/// <param name="directory"></param>
		public DirectoryStructure CollectModData(string directory)
        {
            Debug.WriteLine("Collect Data Called! (Should be only once.)");
            string[] masterFolder = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
            string[] historyProvincePaths = Directory.GetFiles(Path.Combine(directory, "history", "provinces"), "*.txt", SearchOption.AllDirectories);
            string CSVFilePath;
            if (File.Exists(Path.Combine(directory, "map", "definition.csv")))
                CSVFilePath = Path.Combine(directory, "map", "definition.csv");
            else
                CSVFilePath = Path.Combine(GameFolder, "map", "definition.csv");

          
            return new DirectoryStructure()
            {
                PrimaryDirectories = masterFolder,
                DefinitionCSVPath = CSVFilePath,
                HistoryProvincePaths = historyProvincePaths
            };
        }


    }

}
