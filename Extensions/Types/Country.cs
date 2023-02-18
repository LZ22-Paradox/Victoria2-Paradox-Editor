using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paradox_Editor.Extensions.Types
{
    /*
     * TODO:
     * - ADD READING FORCOUNTRY HISTORY DATA
     * - SEE CULTURE PARSER FOR INSPIRATIONS
     */

    public class Country
    {
        //Meta Data
        string TAG;
        public string GetTAG() => TAG;
        //Common Data
        string Name;
        public string GetName() => Name;
        Color Color;
        public Color GetColor() => Color;
        string Graphical_Gulture; ///Get Graphical Cultures??

        //History Data
        int Capital;
        string Primary_Culture;
        string []Cultures;
        string Religion;
        string Government; //Make a governments type
        double Plurality = 0.0;
        string National_Value; //Make national-value type
        decimal Literacy = 0.00m; //Decimal so that values > 0.1 are available
        string Non_State_Culture_Literacy; //Entirely optional; not common in most countries
        bool IsCivilized; //Is boolean, but "yes" and "no" are written in-file replace "true" and "false".
        string TAG_oob; //For the TAG_oob.txt; not all countries have them

        //Reforms (get reforms)

        //Ruling Party & Upper House
        string Ruling_Party;
        object Upper_House; //Upper-house consists of multiple ideologies, which could be changed.

        //Starting Consciousness
        int Consciousness = 0;
        int NonState_Consciousness = 0;

        //Technologies ///Need technologies type

        //Inventions

        //Set Country Flag?

        public Country(string tag, string name, string commonFilePath)
        {
            TAG = tag;
            Name = name;
            AssignCommonFileData(commonFilePath);
        }

        private void AssignCommonFileData(string commonFilePath)
        {
            string name = Path.GetFileName(commonFilePath).Replace(".txt", "");
            foreach (string line in File.ReadAllLines(commonFilePath))
            {
                string[] splitLine = line.Split("=", StringSplitOptions.TrimEntries);
                if (line.Contains("color =", StringComparison.Ordinal) || line.Contains("color=", StringComparison.Ordinal)) //color = { #  #  # }
                {
                    #region Color Handling
                    string fixedColorData = splitLine[1].Replace("{", "").Replace("}", "").Trim();
                    List<string> splitColours = fixedColorData.Split(" ").ToList();
                    List<string> newSplitColors = new List<string>();

                    foreach (string color in splitColours)
                        if (!string.IsNullOrEmpty(color))                       
                            newSplitColors.Add(color);
                    #endregion

                    if (!string.IsNullOrEmpty(splitColours[0]))
                    { //ColorAsStrings = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        this.Color = Color.FromRgb(
                            (byte)Convert.ToInt32(Regex.Replace(newSplitColors[0], "[A-Za-z ]", "")),
                            (byte)Convert.ToInt32(Regex.Replace(newSplitColors[1], "[A-Za-z ]", "")),
                            (byte)Convert.ToInt32(Regex.Replace(newSplitColors[2], "[A-Za-z ]", "")));
                    } else //If the country's colour is invalid.
                        Color = Colors.Black;

                } else if (line.Contains("graphical_culture", StringComparison.Ordinal)) {
                    Graphical_Gulture = splitLine[1];
                }
                
            }
        }


    }
}
