using Paradox_Editor.Cultures;
using Paradox_Editor.Data_Handling;
using Paradox_Editor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor
{
    public static class ModData
    {
        public static IODataAcquisition IO_DATA;
        public static ProvinceDataAcquisition PROVINCE_DATA;
		public static CountryAcquisition COUNTRY_DATA;

		//Cultures & Pops Data
		public static CulturesFile CULTURES_DATA;

        public static void AcquisitionData(string modFolder)
        {
            IO_DATA = new IODataAcquisition(modFolder);
            PROVINCE_DATA = new ProvinceDataAcquisition(modFolder);
		    COUNTRY_DATA = new CountryAcquisition(modFolder);
        }
        public static void SetCulturesData(string culturesTextFilePath) => CULTURES_DATA = CulturesFile.Parse(culturesTextFilePath);




        private static void test()
        {
            
        }
	}
}
