using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

namespace Paradox_Editor.Parsers;

public class CountryParserHistory(DatabaseCountries database, string Directory, Tag tag) : ParserCommon(Directory)
{
    public override T Parse<T>(params string[] fileParts)
    {
        var historyFilePath = database.GetHistoryFile(tag);
        using StreamReader reader = new(File.OpenRead(historyFilePath));
        while (true)
        {
            int c = reader.Peek();
            if (c == -1)
                break;

            reader.SkipWhitespace();
            string unused = reader.ReadUntil(' ');
            ReadCountryHistoryFile(reader);
        }

        return (T)new object();
    }

    private void ReadCountryHistoryFile(StreamReader reader)
    {
        Dictionary<string, bool> technologies = [];
        Dictionary<string, bool> inventions = [];
        reader.SkipWhitespace();
        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();
            switch (item)
            {
                case "capital":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetCapital(tag, int.Parse(reader.ReadUntilWhitespace()));
                    break;
                case "primary_culture":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetPrimaryCulture(tag, reader.ReadUntilWhitespace());
                    break;
                case "culture":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.AddCulture(tag, reader.ReadUntilWhitespace());
                    break;
                case "religion":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetReligion(tag, reader.ReadUntilWhitespace());
                    break;
                case "government=":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetGovernment(tag, reader.ReadUntilWhitespace());
                    break;
                case "plurality":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetPlurality(tag, double.Parse(reader.ReadUntilWhitespace()));
                    break;
                case "nationalvalue":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetNational_Value(tag, reader.ReadUntilWhitespace());
                    break;
                case "literacy":
                    var literacyString = reader.ReadAssignmentValue();
                    var literacy = double.Parse(literacyString);
                    database.SetLiteracy(tag, literacy);
                    break;
                case "non_state_culture_literacy":
                    literacyString = reader.ReadAssignmentValue();
                    literacy = double.Parse(literacyString);
                    database.SetNon_State_Culture_Literacy(tag, literacy);
                    break;
                case "civilized":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetCivilized(tag, reader.ReadUntilWhitespace());
                    break;
                case "is_releasable_vassal":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetIsReleasableVassal(tag, reader.ReadUntilWhitespace());
                    break;
                case "prestige":
                    reader.SkipUntil('=');
                    reader.SkipWhitespace();
                    database.SetPrestige(tag, int.Parse(reader.ReadUntilWhitespace()));
                    break;
                case "set_country_flag":
                    database.AddCountryFlag(tag, reader.ReadAssignmentValue());
                    break;
                case "oob":
                    database.AddOOB(tag, reader.ReadAssignmentValue());
                    break;
                case "upper_house":
                    database.SetUpperHouse(tag, reader.ReadStringIntDictionary());
                    break;
                case "schools":
                    database.SetTechSchool(tag, reader.ReadAssignmentValue());
                    break;
                default:
                    var key = reader.ReadUntilWhitespace();
                    var value = reader.ReadAssignmentValue();

                    // Convert values to booleans for terminology clarity. Re-serialization will matter a lot.
                    // Technologies use 1/0, whilst inventions use yes/no.
                    switch (value)
                    {
                        case "1":
                            value = "true";
                            break;
                        case "0":
                            value = "false";
                            break;
                    }

                    // Assume by default that if it can be turned into a boolean but doesn't have an explicit key, that
                    //  it is a technology.
                    if (bool.TryParse(value, out bool result))
                    {
                        technologies[key] = result;
                        break;
                    }

                    // By this point, that entries are likely inventions (which use yes/no).
                    switch (value)
                    {
                        case "yes":
                            value = "true";
                            break;
                        case "no":
                            value = "false";
                            break;
                    }

                    if (bool.TryParse(value, out result))
                    {
                        inventions[key] = result;
                        break;
                    }

                    // If the program reaches this point, then the entry is not supported.
#if DEBUG
                    Debug.WriteLine($"Could not read key \'{key}\' as an invention or technology.");
#endif

                    break;
            }

            reader.SkipWhitespace();
        }

        database.SetInventions(tag, inventions);
        database.SetTechnologies(tag, technologies);
        reader.Read();
    }
}