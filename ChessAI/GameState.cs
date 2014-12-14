using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class GameState
    {
        private Board board;
        private bool color;
        private int turn;
        private bool gameOver;
        private Network network;
        private float secondsLeft;

        public GameState(bool color, int gameID, int teamID, string teamKey)
        {
            Zobrist.InitTable();
            Transposition.InitTable();
            board = new Board();
            this.color = color;
            turn = -1;
            gameOver = false;
//             Console.WriteLine("Default table: " + Zobrist.GetKey(board.board, true).ToString());
//             board.MakeMove(new Move(0, 1, 0, 2, board.board));
//             Console.WriteLine("Make move: " + Zobrist.GetKey(board.board, false).ToString());
//             board.UndoMove();
//             Console.WriteLine("Undo move: " + Zobrist.GetKey(board.board, true).ToString());
//             long key = Zobrist.GetKey(board.board, true);
//             key ^= Zobrist.TABLE[0, 0, 0, 1];
//             key ^= Zobrist.TABLE[0, 0, 0, 2];
//             key ^= Zobrist.SIDE;
//            Console.Write("Make move: " + key.ToString());

//             Console.WriteLine("Default table: " + board.GetKey().ToString());
//             board.MakeMove(new Move(0, 1, 0, 2, board.board));
//             Console.WriteLine("Make move white: " + board.GetKey().ToString());
//             board.MakeMove(new Move(0, 6, 0, 4, board.board));
//             Console.WriteLine("Make move black: " + board.GetKey().ToString());
//             board.UndoMove();
//             Console.WriteLine("Undo move black: " + board.GetKey().ToString());
//             board.UndoMove();
//             Console.WriteLine("Undo move white: " + board.GetKey().ToString());
            this.network = new Network(gameID, teamID, teamKey);
        }

        public void Run()
        {
            while (!gameOver)
            {
                Console.WriteLine("poll");
                PollForTurn();
                Console.WriteLine("make move");
                MakeMove();
            }
            Console.WriteLine("Game Over!");
        }

        public void PollForTurn()
        {
            Network.JSONPollResponse response = network.RequestPoll();
            while ((!response.ready || response.lastmovenumber <= turn) && (response.gameover == null || response.gameover == false))
            {
                System.Threading.Thread.Sleep(5000);
                response = network.RequestPoll();
            }
            if (response.gameover != null && response.gameover == true)
            {
                gameOver = true;
            }
            UpdateBoard(response);
        }

        public void UpdateBoard(Network.JSONPollResponse response)
        {
            secondsLeft = response.secondsleft;
            turn = response.lastmovenumber;
            if (response.lastmove != null && !String.IsNullOrWhiteSpace(response.lastmove))
            {
                string move = response.lastmove;
                int x1 = 0;
                int x2 = 0;
                int y1 = Int32.Parse(Convert.ToString(move[2])) - 1;
                int y2 = Int32.Parse(Convert.ToString(move[4])) - 1;
                switch (move[1])
                {
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
                switch (move[3])
                {
                    case 'a':
                        x2 = 0;
                        break;
                    case 'b':
                        x2 = 1;
                        break;
                    case 'c':
                        x2 = 2;
                        break;
                    case 'd':
                        x2 = 3;
                        break;
                    case 'e':
                        x2 = 4;
                        break;
                    case 'f':
                        x2 = 5;
                        break;
                    case 'g':
                        x2 = 6;
                        break;
                    case 'h':
                        x2 = 7;
                        break;
                }
                if (move.Length > 5)
                {
                    string s = Convert.ToString(move[5]);
                    byte z = 0;
                    switch (move[5])
                    {
                        case 'Q':
                            z = 5;
                            break;
                        case 'B':
                            z = 4;
                            break;
                        case 'R':
                            z = 2;
                            break;
                        case 'N':
                            z = 3;
                            break;
                    }
                    if (color)
                    {
                        z += 6;
                    }
                    board.MakeMove(board.CreateMove(x1, y1, x2, y2, z));
                }
                else
                {
                    board.MakeMove(board.CreateMove(x1, y1, x2, y2));
                }
            }
        }

        public void MakeMove()
        {
            Console.WriteLine("get here");
            String move;
            //Console.WriteLine(board.ToString());
            System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
            //t.Reset();
            //t.Start();
            //board.PlayNegaMaxMove(out move, color);
            //if (turn > 10)
            //{
            //    Diagnostics.singleTime += t.ElapsedMilliseconds;
            //}
            t.Reset();
            t.Start();
            //Console.WriteLine("SingleThreaded Move: " + move);
            int depth = 6;
            if (turn > 35 && secondsLeft > 200)
            {
                depth = 6;
            }//else if(secondsLeft > 100 && )
            board = board.PlayNegaMaxMoveMultiThreaded(out move, color, depth);
            t.Stop();
            Diagnostics.setMaxMulti(t.ElapsedMilliseconds);
            //if)
                Diagnostics.multiTime += t.ElapsedMilliseconds;
                Diagnostics.searches += 1;
                //Console.WriteLine("Single Current Avg: " + Diagnostics.getAvgSingle());
                Console.WriteLine("Multi Current Avg:" + Diagnostics.getAvgMulti());
            //Console.WriteLine("SinglThreaded Move: " + move);
            turn++;
            network.MakeMove(move);
        }
    }
}
