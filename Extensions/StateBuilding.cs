namespace Paradox_Editor.Extensions;

public class Building
{
}

public class StateBuilding : Building
{
    public StateBuilding()
    {
        Level = null;
        Building = null;
        Upgrade = null;
    }

    private string _level { get; set; }
    private string _upgrade { get; set; }
    private string _building { get; set; }

    public string Level
    {
        get => _level;
        set => _level = value;
    }

    public string Upgrade
    {
        get => _upgrade;
        set => _upgrade = value;
    }

    public string Building
    {
        get => _building;
        set => _building = value;
    }
}