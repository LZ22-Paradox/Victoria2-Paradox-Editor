using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Paradox_Editor.B_Map_Functions
{
    public class CountryNameToColor
    {
        public Dictionary<string, Color> NameToColor { get; set; } = new Dictionary<string, Color>();
        public string[] SeperatedColors { get; set; }

        public Dictionary<string, Color> GetCountryColor(string[] Folder)
        {
            foreach (var countryFile in Folder)
            {
                var name = Path.GetFileName(countryFile).Replace(".txt", "");
                foreach (var line in File.ReadAllLines(countryFile)) //Where the magic happens
                {
                    if (line.Contains("color =", StringComparison.Ordinal)) //color = { #  #  # }
                    {
                        var splitData = line.Split("=", StringSplitOptions.TrimEntries);
                        var fixedColorData = splitData[1].Replace("{", "").Replace("}", "").Trim();
                        var splitColors = fixedColorData.Split(" ");
                        SeperatedColors = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    }
                    else { }
                }
                if (!NameToColor.ContainsKey(name))
                {
                    NameToColor.Add(name, Color.FromRgb((byte)Convert.ToInt32(SeperatedColors[0]), (byte)Convert.ToInt32(SeperatedColors[1]), (byte)Convert.ToInt32(SeperatedColors[2])));
                }

            }
            return NameToColor;
        }       
    }
}
