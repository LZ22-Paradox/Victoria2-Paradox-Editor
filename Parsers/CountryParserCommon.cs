using Paradox_Editor.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

namespace Paradox_Editor.Parsers;

public sealed class CountryParserCommon(DatabaseCountries database, string Directory, string tag)
    : ParserCommon(Directory)
{
    public override T Parse<T>(params string[] fileParts)
    {
        string countryFilePath = database.GetCommonFile(tag);
        using StreamReader reader = new(File.OpenRead(countryFilePath));
        while (true)
        {
            reader.SkipWhitespace();
            if (reader.Peek() == -1)
                break;

            string item = reader.ReadUntilWhitespace();

            switch (item)
            {
                case "color":
                    reader.SkipUntil('=');
                    Color color = ColorParser.Parse(reader);
                    database.SetColor(tag, color);
                    break;
                case "graphical_culture":
                    database.SetGraphicalCulture(tag, reader.ReadAssignmentValue());
                    break;
                case "party":
                    database.AddParty(tag, ReadParty(reader));
                    break;
                case "unit_names":
                    database.SetUnitNames(tag, ReadUnitNames(reader));
                    break;
            }

            reader.SkipWhitespace();
        }

        // Dummy object.
        return (T)new object();
    }

    private static Party ReadParty(StreamReader reader)
    {
        Party party = new();

        reader.SkipUntil('{');
        reader.SkipWhitespace();

        while (reader.Peek() is not -1 and not '}')
        {
            string item = reader.ReadUntilWhitespace();

            switch (item)
            {
                case "name":
                    party.Name = reader.ReadAssignmentValue();
                    break;
                case "start_date":
                    party.StartDate = reader.ReadAssignmentValue();
                    break;
                case "end_date":
                    party.EndDate = reader.ReadAssignmentValue();
                    break;
                case "ideology":
                    party.Ideology = reader.ReadAssignmentValue();
                    break;
                case "economic_policy":
                    party.EconomicPolicy = reader.ReadAssignmentValue();
                    break;
                case "trade_policy":
                    party.TradePolicy = reader.ReadAssignmentValue();
                    break;
                case "religious_policy":
                    party.ReligiousPolicy = reader.ReadAssignmentValue();
                    break;
                case "citizenship_policy":
                    party.CitizenshipPolicy = reader.ReadAssignmentValue();
                    break;
                case "war_policy":
                    party.WarPolicy = reader.ReadAssignmentValue();
                    break;
            }

            reader.SkipWhitespace();
        }

        reader.Read();
        return party;
    }

    private static UnitNames ReadUnitNames(StreamReader reader)
    {
        UnitNames unitNames = new();

        reader.SkipUntil('{');
        reader.SkipWhitespace();

        while (reader.Peek() is not -1 and not '}')
        {
            string unitType = reader.ReadUntilWhitespace();

            reader.SkipUntil('{');
            reader.SkipWhitespace();

            List<string> names = [];

            while (reader.Peek() is not -1 and not '}')
            {
                names.Add(reader.ReadValue());
                reader.SkipWhitespace();
            }

            reader.Read();

            unitNames[unitType] = names;
            reader.SkipWhitespace();
        }

        reader.Read();
        return unitNames;
    }
}