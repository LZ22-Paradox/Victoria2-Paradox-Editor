using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Paradox_Editor.Extensions.Types
{
    /*
     * TODO:
     * - ADD READING FORCOUNTRY HISTORY DATA
     * - SEE CULTURE PARSER FOR INSPIRATIONS
     */

    public class Country
    {
        #region Meta Data
        string TAG;
        public string GetTAG() => TAG;
        bool isInMod = false;
        #endregion

        # region Common Data
        string Name;
        public string GetName() => Name;
        Color Country_Color;
        public Color GetColor() => Country_Color;
        string Graphical_Gulture; ///Get Graphical Cultures??
        #endregion

        #region History Data
        int Capital;
        string Primary_Culture;
        List<string> Cultures = new();
        string Religion;
        string Government; //Make a governments type
        double Plurality = 0.0;
        string National_Value; //Make national-value type
        decimal Literacy = 0.00m; //Decimal so that values > 0.1 are available
        string Non_State_Culture_Literacy; //Entirely optional; not common in most countries
        string Civilized;
        string IsReleasableVassal;
        int Prestige;
        string TAG_oob; //For the TAG_oob.txt; not all countries have them
        List<string> SetCountryFlags = new();
        #endregion

        ///Not Implemented
        //Reforms (get reforms)

        //Ruling Party & Upper House
        string Ruling_Party;
        string Last_Election; ///Not Implemented
        public Dictionary<string, Ideology> Upper_House { get; set; } = new();

        //Starting Consciousness
        int Consciousness = 0;
        int NonState_Consciousness = 0;

        //Technologies ///Need technologies type

        //Inventions


        public Country(string tag, string name, string commonFilePath, string historyFilePath)
        {
            TAG = tag;
            Name = name;
            AssignCommonFileData(commonFilePath);
            TestHistoryFileData(historyFilePath); //Formerly AssignHistoryFileData
        }
        public Country() { }

        private void TestHistoryFileData(string historyFilePath)
        {
            using StreamReader reader = new(File.OpenRead(historyFilePath));
            while (true)
            {
                int c = reader.Peek();
                if (c == -1)
                {
                    break;
                }
                reader.SkipWhitespace();
                string name = reader.ReadUntil(' ');
                Parse(reader);
            }
        }

        public void Parse(StreamReader reader)
        {
            reader.SkipWhitespace();
            while (reader.Peek() is not -1 and not '}')
            {
                string item = reader.ReadUntilWhitespace();
                switch (item)
                {
                    case "capital":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Capital = int.Parse(reader.ReadUntilWhitespace());
                        break;
                    case "primary_culture":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Primary_Culture = reader.ReadUntilWhitespace();
                        break;
                    case "culture":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Cultures.Add(reader.ReadUntilWhitespace());
                        break;
                    case "religion":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Religion = reader.ReadUntilWhitespace();
                        break;
                    case "government=":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Government = reader.ReadUntilWhitespace();
                        break;
                    case "plurality":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Plurality = double.Parse(reader.ReadUntilWhitespace());
                        break;
                    case "nationalvalue":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        National_Value = reader.ReadUntilWhitespace();
                        break;
                    case "literacy":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Literacy = decimal.Parse(reader.ReadUntilWhitespace());
                        break;
                    case "non_state_culture_literacy":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Non_State_Culture_Literacy = reader.ReadUntilWhitespace();
                        break;
                    case "civilized":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Civilized = reader.ReadUntilWhitespace();
                        break;
                    case "is_releasable_vassal":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        IsReleasableVassal = reader.ReadUntilWhitespace();
                        break;
                    case "prestige":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        Prestige = int.Parse(reader.ReadUntilWhitespace());
                        break;
                    case "set_country_flag":
                        reader.SkipUntil('=');
                        reader.SkipWhitespace();
                        SetCountryFlags.Add(reader.ReadUntilWhitespace());
                        break;
                    case "upper_house":
                        List<Ideology> tempIdeologies = new();
                        tempIdeologies = Ideology.Parse(reader);
                        foreach (Ideology ideology in tempIdeologies)
                            Upper_House[ideology.Name] = ideology;
                        break;
                    default: //Add implementation for dates
                        break;
                }
                reader.SkipWhitespace();
            }
            reader.Read();
        }

        public sealed class Ideology
        {
            public string Name { get; set; }
            public string Percentage { get; set; }
            public static List<Ideology> Parse(StreamReader reader)
            {
                List<Ideology> ideologyList = new();
                reader.SkipUntil('{');
                reader.SkipWhitespace();
                while (reader.Peek() is not -1 and not '}')
                {
                    Ideology ideology = new();
                    string line = reader.ReadLine();
                    Regex rgx2 = new Regex("\t|\\s+");
                    string[] cleanedLine = rgx2.Replace(line, "").Split('=');
                    ideology.Name = cleanedLine[0];
                    ideology.Percentage = cleanedLine[1];
                    ideologyList.Add(ideology);
                    reader.SkipWhitespace();
                }
                reader.Read();
                return ideologyList;
            }
        }




        private void AssignCommonFileData(string commonFilePath)
        {
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
                        this.Country_Color = Color.FromRgb(
                            (byte)Convert.ToInt32(Regex.Replace(newSplitColors[0], "[A-Za-z ]", "")),
                            (byte)Convert.ToInt32(Regex.Replace(newSplitColors[1], "[A-Za-z ]", "")),
                            (byte)Convert.ToInt32(Regex.Replace(newSplitColors[2], "[A-Za-z ]", "")));
                    }
                    else //If the country's colour is invalid.
                        Country_Color = Colors.Black;

                }
                else if (line.Contains("graphical_culture", StringComparison.Ordinal))
                {
                    Graphical_Gulture = splitLine[1];
                }
            }
        }


    }
}
