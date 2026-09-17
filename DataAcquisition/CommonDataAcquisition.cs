using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public class CommonDataAcquisition
{
    // Cultures
    public readonly List<string> Cultures = [];

    // Goods
    public readonly Dictionary<string, GoodGroup> Goods;

    // ReSharper disable once RedundantAssignment
    public CommonDataAcquisition(ref DatabaseCountries databaseCountries, string directory)
    {
        // Populate goods.
        Goods = new GoodsParser(directory).Parse<Dictionary<string, GoodGroup>>("common", "goods.txt");

        // Populate cultures.
        var cultureGroups = new CultureParser(directory)
            .Parse<Dictionary<string, CultureGroup>>("common", "cultures.txt");
        foreach (var cultureGroup in cultureGroups)
        foreach (var culture in cultureGroup.Value.Cultures.Keys)
            Cultures.Add(culture);

        Cultures.Sort();

        // WIP: May not need this atm.
        /*foreach (string file in Directory.GetFiles(foundCountries, "*.txt", SearchOption.AllDirectories))
        {
            string[] splitFile = file.Split('\\');
            CountryFiles.Add(splitFile[^1].Replace(".txt", ""), file);
        }*/

        // Countries.
        databaseCountries = new DatabaseCountries();

        // TODO: Check the game files that the flags for these countries exist, maybe even in vanilla.
        PopulateCountriesDatabase(databaseCountries, directory);
    }


    private static void PopulateCountriesDatabase(DatabaseCountries database, string directory)
    {
        Task task = Task.Run(() =>
        {
            // Pre-build the countries expected to exist.
            LoadCommonCountriesFile(database, directory);
            Parallel.ForEach(database.CommonFile,
                tag => new CountryParserCommon(database, directory, tag).Parse<object>());
        });
        task.Wait();
    }

    private static void LoadCommonCountriesFile(DatabaseCountries databaseCountries, string directory)
    {
        var countriesFile = Path.Combine(directory, "common", "countries.txt");
        foreach (var line in File.ReadAllLines(countriesFile))
        {
            var input = line.Trim();
            var index = input.IndexOf("#", StringComparison.Ordinal);
            if (index >= 0)
                input = input[..index];
            index = input.IndexOf("dynamic_tags", StringComparison.Ordinal);
            if (index >= 0)
                input = input[..index];
            if (string.IsNullOrEmpty(input))
                continue;

            var removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
            var words = removal.Split('='); //Has the actual country names
            string tag = words[0].Trim();
            string name = words[1].Trim();

            string commonFilePath = "";
            string historyFilePath = "";

            // WIP: Check this to make sure it actually exists.
            //  - Check Common, then check vanilla.
            //  - Check history files to ensure that a file begins with the TAG, and only the tag.
            //      - If no exist, check vanilla.

            //Possible check / display message for missing common or history file
            databaseCountries.CreateCountry(tag, name, commonFilePath, historyFilePath);
        }
    }
}