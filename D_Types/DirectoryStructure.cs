using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Types
{
    public class DirectoryStructure
    {
        public string[] MasterDirectory { get; set; }
        public string[] Countries { get; set; }
        public string[] HistoryProvincePaths { get; set; }
        public string CountriesTxt { get; set; }
        public string DefinitionCSVPath { get; set; }

    }
}
