using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Paradox_Editor.DataAcquisition;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Parsers;

public class ProvincesDefinitionsFileParser : ParserCommon
{
    /// <summary>
    /// Populates existing provinces with CSV colour and filename data.
    /// </summary>
    public override T Parse<T>(string directory, params string[] fileParts)
    {
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = false,
            MissingFieldFound = null
        };
        
        var path = GetGameFilePath(directory, Path.Combine(fileParts));
        using var reader = new StreamReader(path);

        using var csv = new CsvReader(reader, cfg);
        csv.Context.RegisterClassMap<CSVProvinceMap>();

        var records = csv.GetRecords<ProvinceCSVDefinition>();
        return (T)records;
    }
}