namespace Paradox_Editor.Extensions.Types;

public class Core
{
    private string _tag { get; set; }

    public Core(string TAG)
    {
        this.TAG = TAG;
    }

    public string TAG
    {
        get => _tag;
        set => _tag = value;
    }
}