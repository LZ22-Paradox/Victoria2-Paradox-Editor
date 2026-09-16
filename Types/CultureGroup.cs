using System.Collections.Generic;
using System.IO;
using Paradox_Editor.Extensions;
using Paradox_Editor.Parsers;

namespace Paradox_Editor.Types;

public sealed class CultureGroup
{
    public string Leader { get; set; }
    public string Unit { get; set; }
    public string IsOverseas { get; set; }
    public Dictionary<string, Culture> Cultures { get; } = new();
    public string Union { get; set; }
}