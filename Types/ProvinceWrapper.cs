namespace Paradox_Editor.Types;

public readonly struct ProvinceWrapper
{
    public readonly string Id;
    public readonly string File;

    public ProvinceWrapper(string id, string file)
    {
        Id = id;
        File = file;
    }
}