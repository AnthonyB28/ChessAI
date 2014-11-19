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
            GameState s = new GameState(true, 1283, 1, "32c68cae");
            s.PollForTurn();
        }
    }
}
