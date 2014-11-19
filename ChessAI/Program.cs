using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Network.BeginPollingServer();
            Network.MakeMove("Pd2d3");
            while(true)
            {

            }
        }
    }
}
