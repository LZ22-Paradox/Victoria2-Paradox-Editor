using System.Threading;
using System.Threading.Tasks;
using Paradox_Editor.DataAcquisition;
using Paradox_Editor.Extensions;
using Paradox_Editor.Types;
using Paradox_Editor.Types.Data;

namespace Paradox_Editor;

public class ModData
{
    public ModInfoAcquisition MOD_DATA = null!;
    public MapFileDataAcquisition MAP_DATA = null!;
    public CommonDataAcquisition COMMON_DATA = null!;
    public LocalizationAcquisition LOCALIZATION_DATA = null!;

    /// <summary>
    /// Collective database of all merged common and historical province data. It is a big old SoA class.
    /// </summary>
    public DatabaseProvinces DatabaseProvinces = null!;

    /// <summary>
    /// SoA database of all country data. It's also pretty large, and full of mixed content.
    /// </summary>
    public DatabaseCountries DatabaseCountries = null!;

    public static ModData Instance = null!;

    public ModData(string modFolder)
    {
        Instance = this;
        Utilities.ModDirectory = modFolder;

        // WARN: DOES NOT HANDLE MAPS W. CUSTOM PROVINCES. FIX THAT.

        /* TODO: SUPPORT COMMENTS
         *   If the line starts with a hashtag, read it as a comment.
         *   If the line starts with a number, read it as a date with its own tree / scope. */

        var thread = new Thread(() =>
        {
            // VITAL FRAMEWORK DATA
            MOD_DATA = new ModInfoAcquisition(modFolder);
            // -> Game Directory

            MAP_DATA = new MapFileDataAcquisition(ref DatabaseProvinces, modFolder);
            // -> map/continents.txt
            // -> map/default.map
            // -> map/definition.csv
            // -> TODO: map/region.txt
            // -> TODO: map/climate.txt
            // -> TODO: map/positions.txt (Very important for making new provinces)
            // -> TODO: map/terrain.txt

            // LOCALIZATION
            // TODO: Deal with this.

            // COMMON (DEFINITION) FILES
            COMMON_DATA = new CommonDataAcquisition(ref DatabaseCountries, modFolder);
            // -> common/goods.txt
            // -> common/cultures.txt
            // -> common/countries.txt
            // -> common/countries/*.txt

            // ReSharper disable once JoinDeclarationAndInitializer
            // HISTORY (DATA) FILES
            Task task;
            task = Task.Run(() => HistoryDataAcquisition.PopulateHistoryProvinces(DatabaseProvinces, modFolder));
            task.Wait(); // -> history/provinces/**/*.txt
            task = Task.Run(() => HistoryDataAcquisition.PopulateHistoryCountries(DatabaseCountries, modFolder));
            task.Wait(); // -> history/countries/*.txt
        });
        thread.Start();
        thread.Join();
    }

    public static void AcquisitionData()
    {
    }
}