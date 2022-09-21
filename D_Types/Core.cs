using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Types
{

    public class Core
    {
        private string TAG { get; set; }

        public Core(string TAG)
        {
            tag = TAG;
        }

        public string tag
        {
            get => TAG;
            set => TAG = value;
        }

        /*public string TAG
        {
            get { return tag; }
            set { tag = value; }
        }*/

    }
}