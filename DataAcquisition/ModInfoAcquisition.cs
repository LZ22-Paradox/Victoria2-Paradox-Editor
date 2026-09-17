using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Paradox_Editor.DataAcquisition;

//For use in important CSV file reading.
public class ModInfoAcquisition
{
    private string GameDirectory { get; }
    private string ModFolder { get; }

    public ModInfoAcquisition(string directory)
    {
        ModFolder = directory;

        // COLLECT MOD DATA
        Debug.WriteLine("Collect Data Called! (Should be only once.)");

        // Weird edge case.
        if (directory.Equals(MainWindow.CurrentGameMode))
            throw new Exception("This program is not allowing you to edit your base game.");

        // Somehow if there's no directory... uhh... do nothing. Just crash. Sucks to suck for the user.
        if (string.IsNullOrEmpty(directory))
            throw new NullReferenceException("Null or empty directory provided.");

        // Make sure that a mod directory was selected, and -not- the game.
        DirectoryInfo? modFolder = Directory.GetParent(directory);
        if (string.IsNullOrEmpty(modFolder?.ToString()) || string.IsNullOrEmpty(modFolder.Parent?.ToString()))
            throw new Exception("Failed to find parent game directory. Make sure to select the mod directory.");

        GameDirectory = modFolder.Parent?.ToString()!;
        string modName = Path.GetFileName(ModFolder);

        // TODO: Read the mod folder from the `.mod` file for consistency.
        var dotMod = modFolder + "\\" + modName + ".mod";
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
        else MessageBox.Show(@"Missing <mod name>.mod file!");
    }

    public string GetModFolder() => ModFolder;
    public string GetGameDirectory() => GameDirectory;
}