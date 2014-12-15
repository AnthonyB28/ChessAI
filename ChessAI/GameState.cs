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
        private int lastOurMoveCount;
        private int lastOpponentMoveCount;
        private double lastMoveTime;
        private int lastMoveDepth;

        public GameState(bool color, int gameID, int teamID, string teamKey)
        {
            board = new Board();
            this.color = color;
            turn = -1;
            gameOver = false;
            this.network = new Network(gameID, teamID, teamKey);
            this.lastMoveDepth = 0;
            this.lastMoveTime = 0;
            this.lastOpponentMoveCount = 0;
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
            //List<Move> moves = board.GetAllStates(color, false);
            //moves.Sort();
            //foreach (Move m in moves)
            //{
            //    Console.WriteLine(m);
            //}
            //Console.WriteLine(moves[0].CompareTo(moves[1]));
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
            int depth = 5;
            int ourCurrentBranch = board.GetAllStates(color, true).Count;
            int oppCurrentBranch = board.GetAllStates(!color, false).Count;
            if (turn > 1)
            {
                long nodes = (long)(Math.Pow(lastOurMoveCount, lastMoveDepth / 2.0) * Math.Pow(lastOpponentMoveCount, lastMoveDepth / 2.0));
                long currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                double nodesPerSecond = nodes / lastMoveTime;
                double estimatedTime = (currentNodes / nodesPerSecond);
                Console.WriteLine("Estimated Time: " + estimatedTime);
                if (!board.IsEndGame())
                {
                    while (depth < 10 && estimatedTime < 1500)
                    {
                        depth++;
                        currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                        estimatedTime = (currentNodes / nodesPerSecond);
                    }
                    while (depth > 5 && estimatedTime > 20000)
                    {
                        depth--;
                        currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                        estimatedTime = (currentNodes / nodesPerSecond);
                    }
                }
                else
                {
                    while (depth > 5 && estimatedTime < 10000)
                    {
                        depth++;
                        currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                        estimatedTime = (currentNodes / nodesPerSecond);
                    }
                    while (depth > 5 && (estimatedTime > secondsLeft + 20000 || estimatedTime > 60000))
                    {
                        depth--;
                        currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                        estimatedTime = (currentNodes / nodesPerSecond);
                    }
                }
            }
            if (depth < 5)
            {
                depth = 5;
            }
            if (depth > 10)
            {
                depth = 10;
            }
            //if (turn > 35 && secondsLeft > 200)
            //{
            //    depth = 6;
            //}//else if(secondsLeft > 100 && )
            board = board.PlayNegaMaxMoveMultiThreaded(out move, color, depth);
            t.Stop();
            lastOurMoveCount = ourCurrentBranch;
            lastOpponentMoveCount = oppCurrentBranch;
            lastMoveTime = t.ElapsedMilliseconds;
            lastMoveDepth = depth;
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
