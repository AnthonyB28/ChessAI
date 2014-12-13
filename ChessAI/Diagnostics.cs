using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Diagnostics
    {
        public static long singleTime = 0;
        public static long multiTime = 0;
        public static long maxMulti = 0;
        public static int searches = 0;

        public static double getAvgSingle(){
            return (1.0 * singleTime) / searches;
        }

        public static double getAvgMulti()
        {
            return (1.0 * multiTime) / searches;
        }

        public static void setMaxMulti(long ms)
        {
            if (ms > maxMulti)
            {
                maxMulti = ms;
            }
            Console.WriteLine("Current Max Multi: " + maxMulti);
        }
    }
}
