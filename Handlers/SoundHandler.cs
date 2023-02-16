using System;
using System.IO;
using System.Media;

namespace Paradox_Editor.Handlers
{
    public static class SoundHandler
    {
        public static string SoundSet { get; set; }
        public static string ValidClick { get; set; }
        public static string Gamemode { get; set; }
        public static string SoundAssetsPath = @"/Assets/VIC2/Sounds/"; //The Resource Path for the Icons
        //private static string WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static string WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static void SoundAssetChange(string Game)
        {
            if (Game == "VIC2")
                SoundAssetsPath = @"\Assets\VIC2\Sounds\";
            else if (Game == "EU4")
                SoundAssetsPath = @"\Assets\EU4\Sounds\";
            else
            {
            }
            Gamemode = Game;
        }

        public static void PlayClick()
        {
            var soundDirectory = new Uri(WorkingDirectory + SoundAssetsPath + "validClick.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);

            splayer.Play();
        }
        public static void PlayConnecting()
        {
            var soundDirectory = new Uri(WorkingDirectory + SoundAssetsPath + "connecting.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);
            splayer.Play();
        }

        public static void PlayError()
        {
            var soundDirectory = new Uri(WorkingDirectory + SoundAssetsPath + "error.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);
            splayer.Play();
        }
    }
}

