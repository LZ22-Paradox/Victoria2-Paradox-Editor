using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Paradox_Editor.Parsers;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ProvinceDefinitionsFileParser(string Directory) : ParserCommon(Directory)
{
    /// <summary>
    /// Populates existing provinces with CSV colour and filename data.
    /// </summary>
    public List<T> ParseList<T>(params string[] fileParts)
    {
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = false,
            MissingFieldFound = null
        };

        var path = GetGameFilePath(Path.Combine(fileParts));
        using var reader = new StreamReader(
            path,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            detectEncodingFromByteOrderMarks: true
        );

        using var csv = new CsvReader(reader, cfg);
        csv.Context.RegisterClassMap<ProvinceCSVDefinitionMap>();

        var records = csv
            .GetRecords<ProvinceCSVDefinition>()
            .Where(x => int.TryParse(x.Id, out _))
            .ToList();

        List<ProvinceCSVDefinition> result = [];

        foreach (ProvinceCSVDefinition record in records.ToList())
        {
            if (!record.Name.Equals("x", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrEmpty(record.RiverName))
            {
                result.Add(record with { IsRiver = false });
                continue;
            }

            result.Add(record with { IsRiver = true, Name = record.RiverName });
        }

        // Sort by ID.
        result.Sort((a, b)
            => int.Parse(a.Id!).CompareTo(int.Parse(b.Id!)));

        return (List<T>)(object)result;
    }

    public override T Parse<T>(params string[] fileParts) => throw new System.NotImplementedException();

    public struct ProvinceCSVDefinition
    {
        public string? Id { get; set; }
        public string Red { get; set; }
        public string Green { get; set; }
        public string Blue { get; set; }
        public string Name { get; set; }
        public string? RiverName { get; set; }
        internal bool IsRiver { get; set; }
    }
}

/// <summary>
/// For use in important CSV file reading.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ProvinceCSVDefinitionMap : ClassMap<ProvinceDefinitionsFileParser.ProvinceCSVDefinition>
{
    public ProvinceCSVDefinitionMap()
    {
        Map(m => m.Id).Index(0);
        Map(m => m.Red).Index(1);
        Map(m => m.Green).Index(2);
        Map(m => m.Blue).Index(3);
        Map(m => m.Name).Index(4);
        Map(m => m.RiverName).Index(5);
    }
}