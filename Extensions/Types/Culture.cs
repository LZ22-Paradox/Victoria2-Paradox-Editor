using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.Extensions.Types
{
    public class Culture 
    {
        string CultureName { get; set; }
        string Color { get; set; }
        List<string> FirstNames { get; set; }
        List<string> LastNames { get; set; }

    }
    public class CultureGroup
    {
        string CultureLeader { get; set; }
        string UnitType { get; set; }
        List<string> Cultures;
    }
}
