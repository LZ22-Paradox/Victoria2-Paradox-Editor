using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

namespace Paradox_Editor.DataAcquisition;

public class MapFileDataAcquisition
{
    private readonly Dictionary<string, Continent> _continents;

    public readonly DefaultMap DefaultMapFile;

    // ReSharper disable once RedundantAssignment
    public MapFileDataAcquisition(ref DatabaseProvinces databaseProvincesReference, string directory)
    {
        string[] continentPathParts = [directory, "map", "continent.txt"];
        _continents = new ContinentParser(directory).Parse<Dictionary<string, Continent>>(continentPathParts);

        DefaultMapFile = new DefaultMapParser(directory).Parse<DefaultMap>("map", "default.map");

        // TODO: Create regions

        CreateProvinceDatabase(out databaseProvincesReference, directory);
    }

    private void CreateProvinceDatabase(out DatabaseProvinces databaseProvincesReference, string directory)
    {
        var database = new DatabaseProvinces(DefaultMapFile.MaxProvinces);

        // All begins as ocean before being marked with land provinces.
        // The reason this is being done here and nowhere else is that this is the earliest, most convenient point to
        //  check for this property before it becomes "too late" during runtime. "continents.txt" is the only file that
        //  specifies what is an ocean, and what is not outside the game's regions file, which I do not trust.
        bool[] isOceanProvinceArray = new bool[DefaultMapFile.MaxProvinces];
        Array.Fill(isOceanProvinceArray, true);
        Parallel.ForEach(_continents.Values, continent =>
        {
            foreach (var province in continent.Provinces)
                isOceanProvinceArray[province] = false;
        });


        // Create provinces in memory from the "definition.csv" file. At this stage, they are still missing COMMON
        //  HISTORY, and POP information, which is handled a layer above this aquisition layer..
        Task task = Task.Run(() =>
        {
            var records = new ProvinceDefinitionsFileParser(directory)
                .ParseList<ProvinceDefinitionsFileParser.ProvinceCSVDefinition>("map", "definition.csv");

            Parallel.ForEach(records, record =>
            {
                // Avoid parsing null / blank provinces, and especially entries that begin with entries that aren't
                //  numerical.
                if (string.IsNullOrWhiteSpace(record.Id) ||
                    Regex.IsMatch(record.Id, @"\bprovince\b") ||
                    !int.TryParse(record.Id, out int _))
                    return;

                if (!uint.TryParse(record.Red.Replace(".", ""), out var red) ||
                    !uint.TryParse(record.Green.Replace(".", ""), out var green) ||
                    !uint.TryParse(record.Blue.Replace(".", ""), out var blue))
                    return;

                // Create the province entry in the database. Order of operations demands this be done before Common
                //  and History data fill-in.
                database.CreateProvince(
                    /*Province ID*/ id: Convert.ToUInt32(record.Id),
                    /*Color*/ red, green, blue,
                    /*Province Name*/ record.Name,
                    /*Ocean Array*/ isOceanProvinceArray
                );
            });
        });
        task.Wait();
        databaseProvincesReference = database;
    }

    public ImmutableDictionary<string, Continent> GetContinents() => _continents.ToImmutableDictionary();
}