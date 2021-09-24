using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor
{
    public class TextFileReader : Dictionary<string, string>
    {
        public TextFileReader(string config)
        {
            config.Split(Environment.NewLine).ToList().ForEach(line =>
            {
                var split = line.Split("-");
                Add(split[0], split[1]);
            });
        }
    }
}
