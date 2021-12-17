using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Types
{

    public class CoreData
    {
        public CoreData(string tag) => TAG = tag;

        private string tag { get; set; }

        public string TAG
        {
            get { return tag; }
            set { tag = value; }
        }

        /*public string TAG
        {
            get { return tag; }
            set { tag = value; }
        }*/

    }
}