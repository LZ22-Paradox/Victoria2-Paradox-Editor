using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Parallel.ForEach(database.Tags,
                tag => new CountryParserCommon(database, directory, tag).Parse<object>());
        });
        task.Wait();
    }

    private static void LoadCommonCountriesFile(DatabaseCountries database, string directory)
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

            // Clean tabs, normalize path, and split.
            var halves = input.Replace("\t", "")
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace("\"", "")
                .Split("=");
            string tag = halves[0].Trim();
            string path = halves[1].Trim();

            // Extract the common path in advance.
            var commonPath = Path.Combine(directory, "common", path);

            bool isVanilla = false;
            string? historyPath = // Check the first three letters of all modded history country files.
                Directory.EnumerateFiles(Path.Combine(directory, "history", "countries"))
                    .FirstOrDefault(file =>
                        Path.GetFileName(file)[..3]
                            .Equals(tag, StringComparison.OrdinalIgnoreCase));
            
            if (historyPath == null)
            {
                isVanilla = true;
                
                // It failed to find the history data for a country. It will need to fall back to vanilla, which is
                //  incredibly dangerous.
                historyPath = Directory.EnumerateFiles(Path.Combine(ModData.Instance.MOD_DATA.GetGameDirectory(),
                        "history", "countries"))
                    .FirstOrDefault(file =>
                        Path.GetFileName(file)[..3].Equals(tag, StringComparison.OrdinalIgnoreCase));
            }

            if (historyPath == null)
                throw new Exception($"The country \'{tag}\' does not exist in Modded nor Vanilla history files.");

            // Extract country name from here.
            // TODO: Go it from Localization instead using the tag.
            string name = path
                .Replace("countries","")
                .Replace(Path.DirectorySeparatorChar.ToString(), "")
                .Replace(".txt", "");;
            
            //Possible check / display message for missing common or history file
            database.CreateCountry(tag, name, commonPath, historyPath, isVanilla);
        }
    }
}