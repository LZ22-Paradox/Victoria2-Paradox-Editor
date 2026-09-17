namespace Paradox_Editor.Types.Data;

public readonly struct ProvinceWrapper
{
    public readonly string ProvinceID;
    public readonly string ProvinceName;
    public readonly string File;

    public ProvinceWrapper(string provinceId, string name, string file)
    {
        ProvinceID = provinceId;
        ProvinceName = name;
        File = file;
    }
}