using Paradox_Editor.D_Static_Classes_Types;
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
        public static GameSoundHandler SoundHandler { get; set; } = new GameSoundHandler();
        public string SoundSet { get; set; }
        public string ValidClick { get; set; }
        public string Gamemode { get; set; }

        public void PlayClickSound() //Updates & Disables the other existing map modes
        {
            if (Gamemode == "VIC2")
            {
                var splayer = new SoundPlayer(Properties.Resources.VIC2_ValidClick);
                splayer.Play();
            }
            else if (Gamemode == "EU4")
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
            if (Gamemode == "VIC2")
            {
                var splayer = new SoundPlayer(Properties.Resources.VIC2_Connecting);
                splayer.Play();
            }
            else if (Gamemode == "EU4")
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
