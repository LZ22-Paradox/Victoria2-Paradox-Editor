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
        public static ModInfoAcquisition MOD_DATA;
        public static ProvinceDataAcquisition PROVINCE_DATA;
		public static CountryAcquisition COUNTRY_DATA;
		public static CulturesFile CULTURES_DATA;

        public static void AcquisitionData(string modFolder)
        {
            MOD_DATA = new ModInfoAcquisition(modFolder);
            PROVINCE_DATA = new ProvinceDataAcquisition(modFolder);
		    COUNTRY_DATA = new CountryAcquisition(modFolder);
            CULTURES_DATA = CulturesFile.Parse(modFolder);
        }

	}
}
