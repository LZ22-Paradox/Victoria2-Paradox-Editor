using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public class MapFileDataAcquisition
{
    private readonly Dictionary<string, Continent> _continents;

    public readonly DefaultMap DefaultMapFile;

    // ReSharper disable once RedundantAssignment
    public MapFileDataAcquisition(ref DatabaseProvinces databaseProvincesReference, string directory)
    {
        _continents = new ContinentParser(directory)
            .Parse<Dictionary<string, Continent>>(directory, "map", "continent.txt");
        DefaultMapFile = new DefaultMapParser(directory).Parse<DefaultMap>("map", "default.map");

        CreateProvinceDatabase(out databaseProvincesReference, directory);
    }

    private void CreateProvinceDatabase(out DatabaseProvinces databaseProvincesReference, string directory)
    {
        var database = new DatabaseProvinces(DefaultMapFile.MaxProvinces);
        Task task = Task.Run(() =>
        {
            var records = new ProvinceDefinitionsFileParser(directory)
                .Parse<List<ProvinceCSVDefinition>>("map", "definition.csv");

            Parallel.ForEach(records, record =>
            {
                // Avoid parsing null / blank provinces, and especially entries that begin with entries that aren't
                //  numerical.
                if (string.IsNullOrWhiteSpace(record.Province) ||
                    Regex.IsMatch(record.Province, @"\bprovince\b") ||
                    !int.TryParse(record.Province, out int _))
                    return;

                if (!uint.TryParse(record.Red.Replace(".", ""), out var red) ||
                    !uint.TryParse(record.Green.Replace(".", ""), out var green) ||
                    !uint.TryParse(record.Blue.Replace(".", ""), out var blue))
                    return;

                // Create the province entry in the database. Order of operations demands this be done before Common
                //  and History data fill-in.
                database.CreateProvince(
                    /*Province ID*/ id: Convert.ToUInt32(record.Province),
                    /*Color*/ red, green, blue,
                    /*Province Name*/ record.Name
                );
            });
        });
        task.Wait();
        databaseProvincesReference = database;
    }

    public ImmutableDictionary<string, Continent> GetContinents() => _continents.ToImmutableDictionary();
}