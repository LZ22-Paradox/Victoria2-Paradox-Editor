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
        {
            builder.Append((char)c);
        }
        return builder.ToString();
    }
    public static string ReadUntil(this StreamReader reader, params char[] end)
    {
        StringBuilder builder = new();
        int c;
        while ((c = reader.Read()) != -1 && !end.Contains((char)c))
        {
            builder.Append((char)c);
        }
        return builder.ToString();
    }
    public static string ReadUntilWhitespace(this StreamReader reader)
    {
        StringBuilder builder = new();
        while (true)
        {
            int c = reader.Read();
            if (c == -1)
            {
                return builder.ToString();
            }
            if (c == '#')
            {
                reader.SkipUntil('\n');
            }
            if (c is ' ' or '\n' or '\t' or '\r')
            {
                return builder.ToString();
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
            if (c == -1)
            {
                return;
            }
            if (c == '#')
            {
                reader.SkipUntil('\n');
            }
            else if (c is ' ' or '\n' or '\t' or '\r')
            {
                reader.Read();
            }
            else
            {
                return;
            }
        }
    }
}