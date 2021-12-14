using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Class_Types
{
    public class DirectoryStructure
    {
        public string[] MasterDirectory { get; set; }
        public string[] Countries { get; set; }
        public string CountriesTxt { get; set; }
        public string DefinitionCSV { get; set; }
        public string[] HistoryProvinces { get; set; }
    }
}
