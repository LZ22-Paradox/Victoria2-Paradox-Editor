using Paradox_Editor.D__Static_Classes_Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Paradox_Editor.C_Window_Functions
{
    public class GameSoundHandler
    {
        public string SoundSet { get; set; }
        public string ValidClick { get; set; }
        public void SoundHandler() //Updates & Disables the other existing map modes
        {
            if (GameMode.gameMode == "VIC2")
            {

                SoundSet = "Vic2_";
                string path = Path.Combine(Environment.CurrentDirectory, SoundSet, ValidClick);

            }
            else if (GameMode.gameMode == "EU4")
            {
                SoundSet = "Eu4_";
            }
        }
        public void PlayClickSound() //Updates & Disables the other existing map modes
        {
            if (GameMode.gameMode == "VIC2")
            {
                var splayer = new SoundPlayer(Properties.Resources.VIC2_ValidClick);
                splayer.Play();
            }
            else if (GameMode.gameMode == "EU4")
            {
                var splayer = new SoundPlayer(Properties.Resources.EU4_ValidClick);
                splayer.Play();
            }
            else
            {
                var splayer = new SoundPlayer(Properties.Resources.VIC2_ValidClick);
                splayer.Play();
            }
        }
        public void PlayConnectingSound()
        {
            if (GameMode.gameMode == "VIC2")
            {
                var splayer = new SoundPlayer(Properties.Resources.VIC2_Connecting);
                splayer.Play();
            }
            else if (GameMode.gameMode == "EU4")
            {
                var splayer = new SoundPlayer(Properties.Resources.EU4_Connecting);
                splayer.Play();
            }
            else
            {
                var splayer = new SoundPlayer(Properties.Resources.VIC2_Connecting);
                splayer.Play();
            }
        }
    }
}
