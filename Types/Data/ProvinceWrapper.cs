namespace Paradox_Editor.Types.Data;

public struct ProvinceWrapper
{
    public string ProvinceID { get; set; }
    public string ProvinceName { get; set; }
    public string File { get; set; }

    public ProvinceWrapper(string provinceId, string name, string file)
    {
        ProvinceID = provinceId;
        ProvinceName = name;
        File = file;
    }
}