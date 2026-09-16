using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Paradox_Editor.Extensions;
using Color = System.Windows.Media.Color;

namespace Paradox_Editor.Types;
// TODO: ADD READING FOR COUNTRY HISTORY DATA
// TODO: SEE CULTURE PARSER FOR INSPIRATIONS

public partial class Country
{
    // META DATA
    public readonly string TAG;
    private bool isInMod = false;

    // COMMON DATA
    public readonly string Name;
    private Color CountryColor;
    public Color GetColor() => CountryColor;
    private string GraphicalCulture;

    // TODO: Get Graphical Cultures??

    // HISTORY DATA
    private int Capital; // Province ID.
    private string PrimaryCulture;
    private readonly List<string> Cultures = new();
    private string Religion;
    private string Government; //Make a governments type
    private double Plurality = 0.0;
    private string National_Value; //Make national-value type
    private decimal Literacy = 0.00m; //Decimal so that values > 0.1 are available
    private string Non_State_Culture_Literacy; //Entirely optional; not common in most countries
    private string Civilized;
    private string IsReleasableVassal;
    private int Prestige;
    private string TAG_oob; //For the TAG_oob.txt; not all countries have them
    private readonly List<string> SetCountryFlags = new();

    ///Not Implemented
    //Reforms (get reforms)

    //Ruling Party & Upper House
    private string Ruling_Party;

    private string Last_Election;

    ///Not Implemented
    public Dictionary<string, Ideology> Upper_House { get; set; } = new();

    //Starting Consciousness
    private int Consciousness = 0;
    private int NonState_Consciousness = 0;

    //Technologies ///Need technologies type

    //Inventions


    public Country(string tag, string name, string commonFilePath, string historyFilePath)
    {
        TAG = tag;
        Name = name;
        AssignCommonFileData(commonFilePath);
        ParseHistoryData(historyFilePath); //Formerly AssignHistoryFileData
    }

    private void ParseHistoryData(string historyFilePath)
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
            string unused = reader.ReadUntil(' ');
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
                    PrimaryCulture = reader.ReadUntilWhitespace();
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
                    var tempIdeologies = Ideology.Parse(reader);
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

    private void AssignCommonFileData(string commonFilePath)
    {
        foreach (string line in File.ReadAllLines(commonFilePath))
        {
            string[] splitLine = line.Split("=", StringSplitOptions.TrimEntries);
            if (line.Contains("color =", StringComparison.Ordinal) ||
                line.Contains("color=", StringComparison.Ordinal)) //color = { #  #  # }
            {
                #region Color Handling

                string fixedColorData = splitLine[1].Replace("{", "").Replace("}", "").Trim();
                var splitColours = fixedColorData.Split(" ").ToList();
                var newSplitColors = new List<string>();

                foreach (string color in splitColours)
                    if (!string.IsNullOrEmpty(color))
                        newSplitColors.Add(color);

                #endregion

                //If the country's colour is invalid.
                if (string.IsNullOrEmpty(splitColours[0])) CountryColor = Colors.Black;
                else
                {
                    //ColorAsStrings = splitColors.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    CountryColor = Color.FromRgb(
                        (byte)Convert.ToInt32(Regex.Replace(newSplitColors[0], "[A-Za-z ]", "")),
                        (byte)Convert.ToInt32(Regex.Replace(newSplitColors[1], "[A-Za-z ]", "")),
                        (byte)Convert.ToInt32(Regex.Replace(newSplitColors[2], "[A-Za-z ]", "")));
                }
            }
            else if (line.Contains("graphical_culture", StringComparison.Ordinal))
            {
                GraphicalCulture = splitLine[1];
            }
        }
    }
}