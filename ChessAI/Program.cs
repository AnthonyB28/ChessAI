using System;
using System.Collections.Generic;

namespace ChessAI
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[,] state = new byte[8, 8];
            //state[0, 6] = 5;
            //state[4, 5] = 5;
            //state[6, 2] = 1;
            //state[5, 1] = 1;
            //state[6,4] = 12;
            //state[6, 0] = 6;
            //state[1, 4] = 7;
            //state[0, 1] = 7;
            //state[2, 5] = 7;
            //state[4, 5] = 12;
            //state[4, 2] = 1;
            //state[1, 7] = 11;
            //state[1, 2] = 5;
            //state[6, 6] = 10;
            //state[1, 6] = 7;
            ////state[6, 6] = 10;
            //state[1, 1] = 1;
            //state[2, 1] = 1;
            //state[4, 1] = 1;
            //state[6, 1] = 1;
            //state[7, 1] = 1;
            //state[2, 0] = 6;
            //state[5, 0] = 4;
            //state[7, 0] = 2;
            ////Console.WriteLine(board);
            //Stack<Move> moves = new Stack<Move>();
            //new Move()
            //moves.Push(new Move)
            //Board board = new Board(state, new byte[12], moves, 6, 2, false, false, 1, 1, 1, 1, 1, 1);
            //Console.WriteLine(board);
            //String moveS;
            //Move move = board.PlayNegaMaxMoveMultiThreaded(out moveS, false, 8);
            //Console.WriteLine(moveS);
            //List<Move> moves = board.GetAllStates(false, false);
            //foreach (Move m in moves)
            //{
            //    Console.WriteLine(m);
            //}
            //String move;
            //Board board = new Board();
            //CreateMove(board, "Ng1f3"); 
            //CreateMove(board, "Ng8f6");
 
            //CreateMove(board, "Pd2d4"); 
            //CreateMove(board, "Pd7d5"); 
            
            //CreateMove(board, "Bc1e3"); 
            //CreateMove(board, "Bc8e6"); 
            
            //CreateMove(board, "Nf3e5"); 
            //CreateMove(board, "Nb8d7"); 
            
            //CreateMove(board, "Nb1c3"); 
            //CreateMove(board, "Nd7e5"); 
            
            //CreateMove(board, "Pd4e5"); 
            //CreateMove(board, "Nf6g4"); 
            
            //CreateMove(board, "Qd1d4"); 
            //CreateMove(board, "Ng4e3"); 
            
            //CreateMove(board, "Pf2e3"); 
            //CreateMove(board, "Pf7f6"); 
            
            //CreateMove(board, "Ke1c1"); 
            //CreateMove(board, "Qd8b8"); 
            
            //CreateMove(board, "Nc3d5"); 
            //CreateMove(board, "Pc7c6"); 
            
            //CreateMove(board, "Pe5f6"); 
            //CreateMove(board, "Ke8f7"); 
            
            //CreateMove(board, "Nd5f4"); 
            //CreateMove(board, "Be6a2"); 
            //CreateMove(board, "Qd4a4"); 
            //CreateMove(board, "Ba2e6"); 
            //CreateMove(board, "Pf6g7"); 
            //CreateMove(board, "Bf8g7"); 
            //CreateMove(board, "Nf4e6"); 
            //CreateMove(board, "Kf7e6"); 
            //CreateMove(board, "Qa4b3");
            //CreateMove(board, "Ke6f6");
            //CreateMove(board, "Rd1d7");
            //CreateMove(board, "Bg7h6");
            //CreateMove(board, "Rd7b7");
            //CreateMove(board, "Bh6e3");
            //CreateMove(board, "Kc1b1");
            //CreateMove(board, "Qb8e5");
            //CreateMove(board, "Qb3a3");
            //Console.WriteLine(board);
            //List<Move> moves = board.GetAllStates(false, false);
            //foreach (Move m in moves)
            //{
            //    Console.WriteLine(m);
            //}
            //board = board.PlayNegaMaxMoveMultiThreaded(out move, false, 6);
            ////board.MakeMove(move)
            //Console.WriteLine(move);
            //Console.WriteLine(board);
            GameState s = new GameState(Convert.ToBoolean(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), args[3]);
            s.Run();
        }

        public static void CreateMove(Board board, string move)
        {
            int x1 = 0;
            switch(move[1]){
                case 'a':
                    x1 = 0;
                    break;
                    case 'b':
                    x1 = 1;
                    break;
                    case 'c':
                    x1 = 2;
                    break;
                    case 'd':
                    x1 = 3;
                    break;
                    case 'e':
                    x1 = 4;
                    break;
                    case 'f':
                    x1 = 5;
                    break;
                    case 'g':
                    x1 = 6;
                    break;
                    case 'h':
                    x1 = 7;
                    break;
            }
            int x2 = x1;
            switch(move[3]){
                case 'a':
                    x1 = 0;
                    break;
                    case 'b':
                    x1 = 1;
                    break;
                    case 'c':
                    x1 = 2;
                    break;
                    case 'd':
                    x1 = 3;
                    break;
                    case 'e':
                    x1 = 4;
                    break;
                    case 'f':
                    x1 = 5;
                    break;
                    case 'g':
                    x1 = 6;
                    break;
                    case 'h':
                    x1 = 7;
                    break;
            }
            int x3 = x1;
            int y1 = Int32.Parse(Convert.ToString(move[2])) - 1;
            int y2 = Int32.Parse(Convert.ToString(move[4])) - 1;
            Console.WriteLine(x2);
            Console.WriteLine(x3);
            Console.WriteLine(y1);
            Console.WriteLine(y2);
            board.MakeMove(board.CreateMove(x2, y1, x3, y2));
        }
    }

    
}
