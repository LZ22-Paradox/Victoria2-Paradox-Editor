using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Paradox_Editor.Extensions.Types
{
    public class CulturesFile
    {
        public CulturesFile(string culturesTextFile)
        {
            ProcessCultureFile(culturesTextFile);

            void ProcessCultureFile(string textFile)
            {
                var reg = new Regex(@"[^#]*");
                string testString = "	north_german = { #Elbian";

                ///Begin parsing the text file here

                Debug.WriteLine(reg.Match(testString).ToString());
                //using TextReader reader = new StreamReader(File.OpenRead(Path.Combine(filepath, "cultures.txt")));
            }
        }

    }
    public class Culture 
    {
        string CultureName { get; set; }
        string Color { get; set; }
        List<string> FirstNames { get; set; }
        List<string> LastNames { get; set; }

    }
    public class CultureGroup
    {
        string CultureLeader { get; set; }
        string UnitType { get; set; }
        List<string> Cultures;
    }


}
