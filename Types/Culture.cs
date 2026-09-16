using System.Collections.Generic;
using System.Windows.Media;

namespace Paradox_Editor.Parsers;

public sealed class Culture
{
    public Color Color { get; set; }
    public List<string> FirstNames { get; } = new();
    public List<string> LastNames { get; } = new();
}