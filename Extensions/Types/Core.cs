using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.Extensions.Types
{
    public class Core
    {
        private string tag { get; set; }

        public Core(string TAG)
        {
            this.TAG = TAG;
        }

        public string TAG
        {
            get => tag;
            set => tag = value;
        }
    }
}
