namespace Paradox_Editor.Types;

public sealed class Party
{
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;

    public string Ideology { get; set; } = string.Empty;
    public string EconomicPolicy { get; set; } = string.Empty;
    public string TradePolicy { get; set; } = string.Empty;
    public string ReligiousPolicy { get; set; } = string.Empty;
    public string CitizenshipPolicy { get; set; } = string.Empty;
    public string WarPolicy { get; set; } = string.Empty;
}