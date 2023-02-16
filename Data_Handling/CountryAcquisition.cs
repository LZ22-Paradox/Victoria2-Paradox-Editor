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
        private string[] CountriesCommonFiles;
        private string CountriesCommonTextFile;
        public CountryAcquisition(string directory)
        {
			CountriesCommonFiles = Directory.GetFiles(Path.Combine(directory, "common", "countries"), "*.txt", SearchOption.AllDirectories);
			CountriesCommonTextFile = Path.Combine(directory, "common", "countries.txt"); //Find countries from vanilla that are overwritten
		}

        public string GetCountryTextFile() => CountriesCommonTextFile;
        public string[] GetCountriesCommonFiles() => CountriesCommonFiles;

        public bool DoesCountrysHaveItsFlags()
        {
            //Check the game files for all of the flags needed. Differentiate between vic2 and eu4
            return true; //temp
        }

	}

}
