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


        //Cultures & Pops Data
        public static CulturesFile CULTURES_DATA;



        public static void SetCulturesData(string culturesTextFilePath) => CULTURES_DATA = CulturesFile.Parse(culturesTextFilePath);
        public static void AcquisitionIOData(string modFolder) => IO_DATA = new IODataAcquisition(modFolder);
        public static void AcquisitionProvinceData(string modFolder) => PROVINCE_DATA = new ProvinceDataAcquisition(modFolder);

    }
}
