using System;
using System.Collections.Generic;

namespace ChessAI
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[,] state = new byte[8, 8];
            //state[5, 6] = 12;
            //state[6, 6] = 1;
            //state[0, 2] = 6;
            //state[0, 1] = 1;
            //state[2, 1] = 1;
            //state[7, 1] = 1;
            //state[6, 1] = 5;
            //Board board = new Board(state, new byte[12], new Stack<Move>(), 6, 2, false, false, 1, 1, 1, 1, 1, 1);
            //Console.WriteLine(board);
            //String move;
            //board.PlayNegaMaxMoveMultiThreaded(out move, false, 9);
            //Console.WriteLine(move);
            GameState s = new GameState(Convert.ToBoolean(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), args[3]);
            s.Run();
        }
    }
}
