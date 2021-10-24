using System;
using System.IO;
using System.Media;

namespace Paradox_Editor.C_Window_Functions
{
    public class GameSoundHandler
    {
        public static GameSoundHandler SoundHandler { get; set; } = new GameSoundHandler();
        public string SoundSet { get; set; }
        public string ValidClick { get; set; }
        public string Gamemode { get; set; }
        public string SoundAssetsPath = @"/Preloaded_Assets/VIC2/Sounds/"; //The Resource Path for the Icons

        public void SoundAssetChange(string Game)
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

        public void PlayClickSound() //Updates & Disables the other existing map modes
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            var soundDirectory = new Uri(projectDirectory + SoundAssetsPath + "validClick.wav", UriKind.Relative);
            var soundFileStream = File.OpenRead(soundDirectory.ToString());
            var splayer = new SoundPlayer(soundFileStream);

            splayer.Play();
        }
        public void PlayConnectingSound()
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

