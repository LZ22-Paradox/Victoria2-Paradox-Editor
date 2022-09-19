using System;
using System.IO;
using System.Media;

namespace Paradox_Editor.C_Window_Functions
{
    public static class SoundHandler
    {
        public static string SoundSet { get; set; }
        public static string ValidClick { get; set; }
        public static string Gamemode { get; set; }
        public static string SoundAssetsPath = @"/Preloaded_Assets/VIC2/Sounds/"; //The Resource Path for the Icons
        
        public static void SoundAssetChange(string Game)
        {
            if (Game == "VIC2")
            {
                SoundAssetsPath = @"\Preloaded_Assets\VIC2\Sounds\";
            }
            else if (Game == "EU4")
            {
                SoundAssetsPath = @"\Preloaded_Assets\EU4\Sounds\";
            }
            else
            {
            }
            Gamemode = Game;
        }

        public static void PlayClickSound() //Updates & Disables the other existing map modes
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            var soundDirectory = new Uri(projectDirectory + SoundAssetsPath + "validClick.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);

            splayer.Play();
        }
        public static void PlayConnectingSound()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            var soundDirectory = new Uri(projectDirectory + SoundAssetsPath + "connecting.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);
            splayer.Play();
        }
    }
}

