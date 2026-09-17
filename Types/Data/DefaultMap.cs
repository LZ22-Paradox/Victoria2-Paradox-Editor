using System.Collections.Generic;

namespace Paradox_Editor.Types;

public class DefaultMap
{
    public int MaxProvinces { get; set; } = 0;

    public List<int> SeaStarts { get; set; } = [];

    public string DefinitionsPath { get; set; } = string.Empty;

    // Image Files
    public string ProvincesMap { get; set; } = string.Empty;
    public string PositionsMap { get; set; } = string.Empty;
    public string TerrainMap { get; set; } = string.Empty;
    public string RiversMap { get; set; } = string.Empty;

    public string TerrainDefinitions { get; set; } = string.Empty;
    public string TreeDefinitions { get; set; } = string.Empty;
    public string ContinentFile { get; set; } = string.Empty;
    public string AdjacenciesFile { get; set; } = string.Empty;
    public string RegionFile { get; set; } = string.Empty;
    public string SeaRegionsFile { get; set; } = string.Empty;

    public string ProvinceFlagSpritesFile { get; set; } = string.Empty;

    public int[] BorderHeights { get; set; } = [];
    public int[] TerrainSheetHeights { get; set; } = [];

    public int TreeValue { get; set; }

    public float BorderCutoffValue { get; set; }
}