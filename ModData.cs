using Paradox_Editor.Data_Handling;
using Paradox_Editor.Parsers;
using System.Security.Authentication;

namespace Paradox_Editor
{
    public static class ModData
    {
        public static ModInfoAcquisition MOD_DATA;
        public static ProvinceDataAcquisition PROVINCE_DATA;
        public static CountryAcquisition COUNTRY_DATA;
        public static CultureDataAcquisition CULTURES_DATA;
        public static MapFileDataAcquisition MAP_DATA;
        public static LocalizationAcquisition LOCALIZATION_DATA;

		public static void AcquisitionData(string modFolder)
        {
            MOD_DATA = new ModInfoAcquisition(modFolder);
            PROVINCE_DATA = new ProvinceDataAcquisition(modFolder);
            COUNTRY_DATA = new CountryAcquisition(modFolder);
            CULTURES_DATA = new CultureDataAcquisition(modFolder);
            MAP_DATA = new MapFileDataAcquisition(modFolder);
        }

    }
}
