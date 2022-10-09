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
        private static string PorkingDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

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

        public static void PlayClick()
        {
            var soundDirectory = new Uri(PorkingDirectory + SoundAssetsPath + "validClick.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);

            splayer.Play();
        }
        public static void PlayConnecting()
        {
            var soundDirectory = new Uri(PorkingDirectory + SoundAssetsPath + "connecting.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);
            splayer.Play();
        }

        public static void PlayError()
        {
            var soundDirectory = new Uri(PorkingDirectory + SoundAssetsPath + "error.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);
            splayer.Play();
        }
    }
}

