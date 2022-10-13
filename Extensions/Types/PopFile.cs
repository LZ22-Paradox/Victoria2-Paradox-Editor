using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.Extensions.Types
{
    public struct Pop
    {
        public string Profession { get; set; }
        public string Culture;
        public string Religion;
        public uint Size;
    }

    public class PopFile
    {
        List<Pop> Pop;
        public void TestPop()
        {
            Pop.Add(new Pop{Profession = "aristocrats", Culture = "scandanavian", Religion = "protestant", Size = 1400000});

            Debug.WriteLine(Pop[0].Culture);

        }
    }
}
