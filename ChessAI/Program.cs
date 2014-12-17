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
            //byte[,] data = new byte[8, 8];
            //data[7, 7] = 12;
            //data[0, 4] = 7;
            //data[7, 4] = 7;
            //data[7, 2] = 1;
            //data[5, 1] = 1;
            //data[1, 0] = 6;
            //data[1, 1] = 8;
            //data[5, 2] = 2;
            //Board board = new Board(data, new Stack<Move>(), true, false, false);
            //Console.WriteLine(board);
            //List<Move> moves = board.GetAllStates(true, true);
            //foreach (Move move in moves)
            //{
            //    Console.WriteLine(move);
            //}
            GameState s = new GameState(Convert.ToBoolean(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), args[3]);
            s.Run();
        }
    }
}
