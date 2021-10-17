using Paradox_Editor.D_Static_Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D__Static_Classes_Types
{
    public class ProvinceIDDictionaries
    {
        public Dictionary<string, string> ToFile { get; set; }
        public Dictionary<string, string> ToName { get; set; }
        public Dictionary<string, HistoryFile> ToHistoryFile { get; set; }

    }
}
