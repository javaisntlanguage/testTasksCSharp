using System;
using System.Collections.Generic;
using System.Text;

namespace task3
{
    static class Parser
    {
        //финкция определения наименьшего типа 
        public static dynamic TryParse(string str)
        {
            sbyte sByte = 0;
            short Short = 0;
            int Int = 0;
            double Double = 0;
            long Long = 0;
            if (sbyte.TryParse(str, out sByte))
            {
                return sByte;
            }
            else if (short.TryParse(str, out Short))
            {
                return Short;
            }
            else if (int.TryParse(str, out Int))
            {
                return Int;
            }
            else if (long.TryParse(str, out Long))
            {
                return Long;
            }
            else if (double.TryParse(str, out Double))
            {
                return Double;
            }
            return str;
        }
    }
}
