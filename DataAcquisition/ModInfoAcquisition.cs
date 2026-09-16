using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Paradox_Editor.DataAcquisition;

//For use in important CSV file reading.
public class ModInfoAcquisition
{
    private string GameDirectory { get; set; }
    private string ModFolder { get; set; }

    public ModInfoAcquisition(string directory)
    {
        ModFolder = directory;

        // COLLECT MOD DATA
        Debug.WriteLine("Collect Data Called! (Should be only once.)");

        if (directory.Equals(MainWindow.CurrentGameMode))
            throw new Exception("This program is not allowing you to edit your base game.");

        if (string.IsNullOrEmpty(directory))
            throw new NullReferenceException("Null or empty directory provided.");

        DirectoryInfo modFolder = Directory.GetParent(directory);
        GameDirectory = modFolder?.Parent?.ToString();
        string modName = Path.GetFileName(ModFolder);
        var dotMod = (modFolder + "\\" + modName + ".mod");
        if (File.Exists(dotMod))
        {
            var dotModLines = File.ReadAllLines(dotMod);
            //var reg = new Regex(@"(?<=([\'\""])).*?(?=\1)");
            foreach (var line in dotModLines) //Unfinished
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                switch (line)
                {
                    case not null when line.Contains("name ="):
                        //modName = reg.Match(line).ToString();
                        break;
                    case not null when line.Contains("nuts"): // TEMP: Nuts?
                    default:
                        break;
                }
            }
        }
        else
        {
            MessageBox.Show(@"Missing mod file!");
        }
    }

    public string GetModFolder() => ModFolder;
    public string GetGameDirectory() => GameDirectory;
}