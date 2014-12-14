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
            //data[7, 7] = 8;
            //data[0, 7] = 8;
            //data[2, 6] = 10;
            //data[1, 6] = 7;
            //data[6, 6] = 7;
            //data[5, 6] = 7;
            //data[2, 5] = 7;
            //data[7, 5] = 7;
            //data[0, 4] = 7;
            //data[5, 1] = 1;
            //data[0, 2] = 1;
            //data[3, 2] = 1;
            //data[3, 3] = 1;
            //data[1, 4] = 1;
            //data[6, 2] = 1;
            //data[7, 2] = 1;
            //data[4, 0] = 6;
            //data[0, 0] = 2;
            //data[6, 0] = 2;
            //data[4, 7] = 12;
            //data[2, 4] = 3;
            //byte[] pieceCount = new byte[12];
            //pieceCount[1] = 7;
            //pieceCount[2] = 2;
            //pieceCount[3] = 1;
            ////pieceCount[6] = 1;
            //pieceCount[7] = 6;
            //pieceCount[8] = 2;
            //pieceCount[10] = 1;
            ////pieceCount[12] = 1;
            //Board board = new Board(data, pieceCount, new Stack<Move>(), 20,  true, false, false);
            //Console.WriteLine(board);
            //String s;
            //board.PlayNegaMaxMoveMultiThreaded(out s, false, 6);
            //List<Move> moves = board.GetAllStates(true, true);
            //foreach (Move move in moves)
            //{
            //    Console.WriteLine(move);
            //}
            GameState s = new GameState(Convert.ToBoolean(args[0]), Convert.ToInt32(args[3]), Convert.ToInt32(args[1]), args[2]);
            s.Run();
        }
    }
}
