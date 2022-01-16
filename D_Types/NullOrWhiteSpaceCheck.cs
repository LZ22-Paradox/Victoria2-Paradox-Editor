using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradox_Editor.D_Types
{
    static class NullOrWhiteSpaceCheck
    {
        public static bool IsEmptyOrWhiteSpace(this string value)
        {
            if (value.All(char.IsWhiteSpace))
            {
                return true;
            }
            return false;
        }

        public static bool IsNotEmptyOrWhiteSpace(this string value)
        {
            if (value.All(char.IsWhiteSpace))
            {
                return false;
            }
            return true;
        }
    }
}
