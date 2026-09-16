using Paradox_Editor.Extensions;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Paradox_Editor.Data_Handling;

//For use in important CSV file reading.
public class ModInfoAcquisition
{
    private string DotModFile; //Unused
    private string ModName;
    private string GameDirectory { get; set; }
    private string ModFolder { get; set; }
    private readonly DirectoryStructure ModDirectories = new();

    public ModInfoAcquisition(string directory)
    {
        ModFolder = directory;
        ModDirectories = CollectModData(directory);
        if (!directory.Equals(MainWindow.CurrentGameMode)) //May Need Fixing
        {
            DirectoryInfo modFolder = Directory.GetParent(directory);
            GameDirectory = modFolder?.Parent?.ToString();
            ModName = Path.GetFileName(ModFolder);
            var dotMod = (modFolder + "\\" + ModName + ".mod");
            if (File.Exists(dotMod))
            {
                var dotModLines = File.ReadAllLines(dotMod);
                var reg = new Regex(@"(?<=([\'\""])).*?(?=\1)");
                foreach (var line in dotModLines) //Unfinished
                {
                    if (string.IsNullOrEmpty(line))
                        continue;
                        
                    switch (line)
                    {
                        case not null when line.Contains("name ="):
                            ModName = reg.Match(line).ToString();
                            break;
                        case not null when line.Contains("nuts"): // TEMP: Nuts?
                        case not null when line.Contains("nuts"): // TEMP: Nuts?
                        default:
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show(@"Missing mod file : " + DotModFile);
            }
        }
        else
        {
            // TODO: Handling for if directory equals game mode IE Victoria or EU4 main folder
        }
    }

    public string GetModFolder() => ModFolder;
    public string GetGameDirectory() => GameDirectory;
    public DirectoryStructure GetModDirectories() => ModDirectories;

    /// <summary>
    /// Gets Victoria 2's directory paths.
    /// </summary>
    /// <param name="directory"></param>
    public DirectoryStructure CollectModData(string directory)
    {
        Debug.WriteLine("Collect Data Called! (Should be only once.)");
        string[] masterFolder = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
        string CSVFilePath;
        if (File.Exists(Path.Combine(directory, "map", "definition.csv")))
            CSVFilePath = Path.Combine(directory, "map", "definition.csv");
        else
            CSVFilePath = Path.Combine(GameDirectory, "map", "definition.csv"); //GameDirectory can somehow end up null here. Implement better check / fallback.

        return new DirectoryStructure()
        {
            PrimaryDirectories = masterFolder,
            DefinitionCSVPath = CSVFilePath
        };
    }


}