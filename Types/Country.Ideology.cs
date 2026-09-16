using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Paradox_Editor.Extensions;

namespace Paradox_Editor.Types;

public partial class Country
{
    public sealed class Ideology
    {
        public string Name { get; set; }
        public string Percentage { get; set; }

        public static List<Ideology> Parse(StreamReader reader)
        {
            List<Ideology> ideologyList = [];
            reader.SkipUntil('{');
            reader.SkipWhitespace();
            while (reader.Peek() is not -1 and not '}')
            {
                Ideology ideology = new();
                string? line = reader.ReadLine();
                var regex = new Regex("\t|\\s+");
                if (!string.IsNullOrEmpty(line))
                {
                    string[] cleanedLine = regex.Replace(line, "").Split('=');
                    ideology.Name = cleanedLine[0];
                    ideology.Percentage = cleanedLine[1];
                }

                ideologyList.Add(ideology);
                reader.SkipWhitespace();
            }

            reader.Read();
            return ideologyList;
        }
    }
}