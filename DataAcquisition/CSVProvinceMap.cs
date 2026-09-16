using CsvHelper.Configuration;

namespace Paradox_Editor.DataAcquisition;

/// <summary>
/// For use in important CSV file reading.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CSVProvinceMap : ClassMap<CSVProvince>
{
    public CSVProvinceMap()
    {
        Map(m => m.ProvinceID).Index(0);
        Map(m => (m.Color >> 16) & 0xFF).Index(1);
        Map(m => (m.Color >> 8) & 0xFF).Index(2);
        Map(m => m.Color & 0xFF).Index(3);
        Map(m => m.ProvinceName).Index(4);
    }
}

public struct CSVProvince
{
    public uint ProvinceID;
    public byte Red;
    public byte Green;
    public byte Blue;
    public uint Color;
    public string ProvinceName;
}