using Paradox_Editor.D_Static_Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D__Static_Classes_Types
{
    public class ProvinceOutputData
    {
        public Dictionary<string, string> IDToFile { get; set; }
        public Dictionary<string, string> IDToName { get; set; }
        public Dictionary<string, HistoryFile> IDToHistory { get; set; }
        public Dictionary<string, string> IDToCores { get; set; }
        public Dictionary<string, string> IDToController { get; set; }
    }
}
