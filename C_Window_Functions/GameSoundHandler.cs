using Paradox_Editor.D__Static_Classes_Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;


namespace Paradox_Editor.C_Window_Functions
{
    public class GameSoundHandler
    {
        public string SoundSet { get; set; }

        public void SoundHandler() //Updates & Disables the other existing map modes
        {
            if (GameMode.gameMode == "VIC2")
            {
                SoundSet = "";
            }
            else if (GameMode.gameMode == "EU4")
            {
                SoundSet = "";
            }
        }
        public void PlaySound() //Updates & Disables the other existing map modes
        {
            SoundPlayer splayer = new SoundPlayer(Properties.Resources.VIC2_ValidClick);
            splayer.Play();
        }
    }
}
