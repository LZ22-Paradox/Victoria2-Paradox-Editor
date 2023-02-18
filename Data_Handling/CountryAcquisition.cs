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

namespace Paradox_Editor.Data_Handling
{
    //For use in important CSV file reading.
    public class CountryAcquisition
    {
        private Dictionary<string,string> TempCountriesCommonFiles = new(); //IN DEVELOPMENT

        private string[] CountriesCommonFiles;
        private string CountriesCommonTextFile;
        private Dictionary<string, Country> Countries = new(); 
        public CountryAcquisition(string directory)
        {
            foreach (var file in Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories)) { 
                var splitFile = file.Split('\\');
                TempCountriesCommonFiles.Add(splitFile[splitFile.Length - 1].Replace(".txt", ""), file);
            }


            CountriesCommonFiles = Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
			CountriesCommonTextFile = Path.Combine(directory, "common", "countries.txt"); //Find countries from vanilla that are overwritten

            LoadCountries();
		}

        public Dictionary<string, Country> GetCountries() => Countries;
        public string GetCountryTextFile() => CountriesCommonTextFile;
        public string[] GetCountriesCommonFiles() => CountriesCommonFiles; //May need changing to access specific country data, later
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
                    if (TempCountriesCommonFiles.TryGetValue(name, out string commonFilePath))
                    {
                        Country country = new Country(tag, name, commonFilePath);
                        Countries.Add(tag, country);
                    }
                }
            }


        }

        public bool VerifyCountryFlags()
        {
            //Check the game files for all of the flags needed. Differentiate between vic2 and eu4
            return true; //temp
        }

	}

}
