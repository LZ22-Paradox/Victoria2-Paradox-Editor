using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Class_Types
{
    public class ProvinceIDDictionaries
    {
        public Dictionary<string, string> ToFile { get; set; }
        public Dictionary<string, string> ToName { get; set; }
        public Dictionary<string, HistoryFile> ToHistoryFile { get; set; }
    }
}
