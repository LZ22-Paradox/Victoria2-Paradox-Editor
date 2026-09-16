using System.Threading;
using Paradox_Editor.DataAcquisition;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types;

namespace Paradox_Editor;

public class ModData
{
    public ModInfoAcquisition MOD_DATA = null!;
    public ProvinceDatabase PROVINCE_DATA = null!;
    public CountryAcquisition COUNTRY_DATA = null!;
    public CultureDataAcquisition CULTURES_DATA = null!;
    public MapFileDataAcquisition MAP_DATA = null!;
    public GoodsFileDataAcquisition GOODS_DATA = null!;
    public LocalizationAcquisition LOCALIZATION_DATA = null!;

    public static ModData Instance = null!;

    public ModData(string modFolder)
    {
        Instance = this;
        Utilities.ModDirectory = modFolder;

        // WARN: DOES NOT HANDLE MAPS W. CUSTOM PROVINCES. FIX THAT.

        var thread = new Thread(() =>
        {
            // VITAL FRAMEWORK DATA
            MOD_DATA = new ModInfoAcquisition(modFolder);
            // -> Game 
            
            MAP_DATA = new MapFileDataAcquisition(modFolder);
            // -> map/continents.txt
            // -> map/default.map
            
            // LOCALIZATION
            
            // COMMON (DEFINITION) FILES
            GOODS_DATA = new GoodsFileDataAcquisition(modFolder);
            CULTURES_DATA = new CultureDataAcquisition(modFolder);

            // HISTORY (DATA) FILES
            PROVINCE_DATA = ProvinceDataAcquisition.CreateDatabase(MAP_DATA.DefaultMapFile, modFolder);
            COUNTRY_DATA = new CountryAcquisition(modFolder);
        });
        thread.Start();
        thread.Join();
    }


    public static void AcquisitionData()
    {
    }
}