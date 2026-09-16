using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public class MapFileDataAcquisition
{
    private readonly Dictionary<string, Continent> _continents;

    public readonly DefaultMap DefaultMapFile;

    public MapFileDataAcquisition(string directory)
    {
        _continents = new ContinentParser()
            .Parse<Dictionary<string, Continent>>(directory, "map", "continent.txt");
        DefaultMapFile = new DefaultMapParser().Parse<DefaultMap>(directory, "map", "default.map");
    }

    public ImmutableDictionary<string, Continent> GetContinents() => _continents.ToImmutableDictionary();
}