using System.Collections.Generic;

namespace Paradox_Editor.Parsers;

public sealed class Continent
{
    public List<string> Provinces { get; set; } = [];
    public string AssimilationRate;
    public string RGOSizeFarm;
    public string RGOSizeMine;
}