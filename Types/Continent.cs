using System.Collections.Generic;

namespace Paradox_Editor.Types;

public sealed class Continent
{
    public List<uint> Provinces { get; set; } = [];
    public string AssimilationRate = string.Empty;
    public string RGOSizeFarm = string.Empty;
    public string RGOSizeMine = string.Empty;
}