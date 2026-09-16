using System.Collections.Generic;

namespace Paradox_Editor.Types;

public class DefaultMap
{
    public int MaxProvinces { get; set; }

    public List<int> SeaStarts { get; set; }

    public string DefinitionsPath { get; set; }

    // Image Files
    public string ProvincesMap { get; set; }
    public string PositionsMap { get; set; }
    public string TerrainMap { get; set; }
    public string RiversMap { get; set; }

    public string TerrainDefinitions { get; set; }
    public string TreeDefinitions { get; set; }
    public string ContinentFile { get; set; }
    public string AdjacenciesFile { get; set; }
    public string RegionFile { get; set; }
    public string SeaRegionsFile { get; set; }

    public string ProvinceFlagSpritesFile { get; set; }

    public int[] BorderHeights { get; set; }
    public int[] TerrainSheetHeights { get; set; }

    public int TreeValue { get; set; }

    public float BorderCutoffValue { get; set; }
}