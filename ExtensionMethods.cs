using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TripCalculator
{
    public static class ExtensionMethods
    {
        public static bool IsNullOrEmpty(this string value)
        {
            if (value == null) return true;
            return string.IsNullOrEmpty(value.Trim());
        }


        public static bool IsNumeric(this String s)
        {
            int dummy;
            return (Int32.TryParse(s, out dummy));
        }

    }
}
