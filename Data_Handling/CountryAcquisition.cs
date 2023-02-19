using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.D_Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Paradox_Editor.Extensions.Types;
using Paradox_Editor.Extensions;
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
        private string CountriesCommonTextFile;
        private Dictionary<string,string> CountriesCommonFiles = new();
        private Dictionary<string, string> CountriesHistoryFiles = new(); //IN DEVELOPMENT
        private Dictionary<string, Country> Countries = new(); 
        public CountryAcquisition(string directory)
        {
            //Common Text File
            CountriesCommonTextFile = Path.Combine(directory, "common", "countries.txt"); //Find countries from vanilla that are overwritten

            #region Common Files
            foreach (string file in Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories)) { 
                string[] splitFile = file.Split('\\');
                CountriesCommonFiles.Add(splitFile[splitFile.Length - 1].Replace(".txt", ""), file);
            }
            #endregion

            #region History Files
            foreach (string file in Directory.GetFiles(Path.Combine(directory, "history", "countries"), "*.txt", SearchOption.AllDirectories))
            {
                string[] splitFile = file.Split('\\');
                string tag = splitFile[splitFile.Length - 1].Substring(0,3);
                if (!CountriesHistoryFiles.ContainsKey(tag))
                    CountriesHistoryFiles.Add(tag, file);
                else
                {
                    string[] testLine = File.ReadAllLines(file);
                    MessageBox.Show("Duplicate Tag in History Files: " + tag);
                    if (testLine.Length == 0)
                        continue;
                    else
                    {
                        CountriesHistoryFiles.TryGetValue(tag, out var alreadyInsertedCountry);
                        string[] newTestLine = File.ReadAllLines(alreadyInsertedCountry);
                        if (newTestLine.Length != 0)
                            continue;
                        else
                        {
                            CountriesHistoryFiles.Remove(tag);
                            CountriesHistoryFiles.Add(tag, file);
                        }
                    }
                }
            }
            #endregion

            LoadCountries();
		}

        public Dictionary<string, Country> GetCountries() => Countries;
        public string GetCountryTextFile() => CountriesCommonTextFile;
        public void LoadCountries()
        {
            foreach (var line in File.ReadAllLines(CountriesCommonTextFile))
            {
                var input = line.Trim();
                var index = input.IndexOf("#");
                if (index >= 0)
                    input = input.Substring(0, index);
                index = input.IndexOf("dynamic_tags");
                if (index >= 0)
                    input = input.Substring(0, index);
                if (string.IsNullOrEmpty(input))
                    continue;

                var removal = input.Replace("\t", "").Replace("\"countries/", "").Replace(".txt\"", "");
                var words = removal.Split('='); //Has the actual country names
                string tag = words[0].Trim();
                string name = words[1].Trim();
                if (!Countries.ContainsKey(tag)) {
                    if (CountriesCommonFiles.TryGetValue(name, out string commonFilePath) && CountriesHistoryFiles.TryGetValue(tag, out string historyFilePath))
                    {
                        Country country = new Country(tag, name, commonFilePath, historyFilePath);
                        Countries.Add(tag, country);
                    } else {
                        //Possible check for missing common or history file
                    }
                }
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
