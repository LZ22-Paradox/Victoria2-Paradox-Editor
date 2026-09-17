using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Paradox_Editor.Extensions;

public static class ReaderExtensions
{
    public static string ReadUntil(this StreamReader reader, char end)
    {
        StringBuilder builder = new();
        int c;
        while ((c = reader.Read()) != -1 && (char)c != end)
            builder.Append((char)c);
        return builder.ToString();
    }

    public static string ReadUntil(this StreamReader reader, params char[] end)
    {
        StringBuilder builder = new();
        int c;
        while ((c = reader.Read()) != -1 && !end.Contains((char)c))
            builder.Append((char)c);
        return builder.ToString();
    }

    public static string ReadUntilWhitespace(this StreamReader reader)
    {
        StringBuilder builder = new();
        while (true)
        {
            int c = reader.Read();
            switch (c)
            {
                case -1: return builder.ToString();
                case '#': reader.SkipUntil('\n'); break;
                case ' ' or '\n' or '\t' or '\r': return builder.ToString();
            }

            builder.Append((char)c);
        }
    }

    public static void SkipUntil(this StreamReader reader, char end)
    {
        while (true)
        {
            int c = reader.Read();
            if (c == -1 || (char)c == end) return;
        }
    }

    public static void SkipWhitespace(this StreamReader reader)
    {
        while (true)
        {
            int c = reader.Peek();
            switch (c)
            {
                case -1: return;
                case '#': reader.SkipUntil('\n'); break;
                case ' ' or '\n' or '\t' or '\r': reader.Read(); break;
                default: return;
            }
        }
    }

    public static string ReadAssignmentValue(this StreamReader reader)
    {
        reader.SkipUntil('=');
        reader.SkipWhitespace();
        return ReadValue(reader);
    }

    internal static string ReadValue(this StreamReader reader)
    {
        reader.SkipWhitespace();
        if (reader.Peek() != '"')
            return reader.ReadUntilWhitespace();

        reader.Read();
        return reader.ReadUntil('"');
    }
    
    public static Dictionary<string, int> ReadStringIntDictionary(this StreamReader reader)
    {
        var result = new Dictionary<string, int>();
        reader.SkipUntil('{');

        while (!reader.EndOfStream)
        {
            reader.SkipWhitespace();
            if (reader.Peek() == '}')
            {
                reader.Read();
                break;
            }

            string key = reader.ReadUntil('=').Trim();
            reader.SkipWhitespace();

            string value = reader.ReadUntil('\n', '}').Trim();
            if (int.TryParse(value, out int number))
                result[key] = number;
        }

        return result;
    }
}