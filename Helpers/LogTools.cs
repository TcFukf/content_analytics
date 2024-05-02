using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Helpers
{
    public static class LogTools
    {
        public static void PrintIE<T>(IEnumerable<T> arr)
        {
            string outpur = string.Join(", ",arr);
            Console.WriteLine(outpur);
        }
    }
}
