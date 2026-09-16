using System.Collections.Generic;
using Paradox_Editor.Parsers;
using Paradox_Editor.Types;

namespace Paradox_Editor.DataAcquisition;

public class CultureDataAcquisition
{
    private readonly List<string> cultures = [];

    public CultureDataAcquisition(string directory)
    {
        var cultureGroups = new CultureParser()
            .Parse<Dictionary<string, CultureGroup>>(directory, "common", "cultures.txt");
        foreach (var cultureGroup in cultureGroups)
        foreach (var culture in cultureGroup.Value.Cultures.Keys)
        {
            cultures.Add(culture);
        }

        cultures.Sort();
    }

    public List<string> GetCultures() => cultures;
}