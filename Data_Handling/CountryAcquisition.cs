using System;
using Paradox_Editor.Extensions.Types;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Paradox_Editor.Data_Handling
{
    /*
     * TODO:
     * - ADD READING FORCOUNTRY HISTORY DATA
     * - SEE CULTURE PARSER FOR INSPIRATIONS
     * - DONT FORGET CHECKING FOR FALLBACKS
     * - READ IDEOLOGIES FOR IDEOLOGICAL FLAG COMPARISONS
     */
    public class CountryAcquisition
    {
        private readonly string CountriesCommonTextFile;
        private readonly Dictionary<string, string> CountriesCommonFiles = new();
        private readonly Dictionary<string, string> CountriesHistoryFiles = new(); //IN DEVELOPMENT
        private readonly Dictionary<string, Country> Countries = new();

        public CountryAcquisition(string directory)
        {
            //Common Text File
            CountriesCommonTextFile =
                Path.Combine(directory, "common", "countries.txt"); //Find countries from vanilla that are overwritten

            #region Common Files

            foreach (string file in Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt",
                         SearchOption.AllDirectories))
            {
                string[] splitFile = file.Split('\\');
                CountriesCommonFiles.Add(splitFile[^1].Replace(".txt", ""), file);
            }

            #endregion

            #region History Files

            var countryFiles = Directory.GetFiles(Path.Combine(directory, "history", "countries"), 
                "*.txt",SearchOption.AllDirectories);
            foreach (string file in countryFiles)
            {
                string[] splitFile = file.Split('\\');
                string tag = splitFile[^1][..3];
                if (CountriesHistoryFiles.TryAdd(tag, file))
                    continue;
                
                string[] testLine = File.ReadAllLines(file);
                MessageBox.Show("Duplicate Tag in History Files: " + tag);
                if (testLine.Length == 0)
                    continue;
                    
                CountriesHistoryFiles.TryGetValue(tag, out var alreadyInsertedCountry);
                if (alreadyInsertedCountry != null)
                {
                    string[] newTestLine = File.ReadAllLines(alreadyInsertedCountry);
                    if (newTestLine.Length != 0)
                        continue;
                }

                CountriesHistoryFiles.Remove(tag);
                CountriesHistoryFiles.Add(tag, file);
            }

            #endregion

            LoadCountries();
        }

        public Dictionary<string, Country> GetCountries() => Countries;
        public string GetCountryTextFile() => CountriesCommonTextFile;

        /// <summary>
        /// Reads through the country data from the countries.txt file in the Common Folder.
        /// </summary>
        public void LoadCountries()
        {
            foreach (var line in File.ReadAllLines(CountriesCommonTextFile))
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

                if (Countries.ContainsKey(tag))
                    continue; // Possible implementation for fallbacks HERE

                if (!CountriesCommonFiles.TryGetValue(name, out string commonFilePath) ||
                    !CountriesHistoryFiles.TryGetValue(tag, out string historyFilePath))
                    continue;

                var country = new Country(tag, name, commonFilePath, historyFilePath);
                Countries.Add(tag, country);
                //Possible check / display message for missing common or history file
            }
        }

        public bool VerifyCountryFlags()
        {
            //Check the game files for all of the flags needed. Differentiate between vic2 and eu4
            MessageBox.Show("Missing Country Flags!");
            return true; //temp
        }
    }
}