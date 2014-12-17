using System;

namespace ChessAI
{
    /// <summary>
    /// A game of chess representation. Only one really exists in the AI at any given time.
    /// </summary>
    class GameState
    {
        private Board board;
        private Network network;
        private double lastMoveTime;
        private float secondsLeft;
        private int lastOurMoveCount;
        private int lastOpponentMoveCount;
        private int turn;
        private int lastMoveDepth;
        private bool isWhite;
        private bool gameOver;

        /// <summary>
        /// Start a game on Carle's server
        /// </summary>
        /// <param name="white">true if white</param>
        /// <param name="gameID">game number</param>
        /// <param name="teamID">team number</param>
        /// <param name="teamKey">secret password</param>
        public GameState(bool white, int gameID, int teamID, string teamKey)
        {
            board = new Board();
            isWhite = white;
            turn = -1;
            gameOver = false;
            network = new Network(gameID, teamID, teamKey);
            lastMoveDepth = 0;
            lastMoveTime = 0;
            lastOpponentMoveCount = 0;
        }

        /// <summary>
        /// Begins the game loop, polling server.
        /// </summary>
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

        /// <summary>
        /// Requests from the server if there is any turn update
        /// </summary>
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

        /// <summary>
        /// Update the board based on the last server update
        /// </summary>
        /// <param name="response"></param>
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
                    if (isWhite)
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

        /// <summary>
        /// Make a move on the board given the last update
        /// </summary>
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
            Board b = board.Clone();
            bool startGame = board.IsStartGame();
            bool midGame = false;
            bool endGame = false;
            bool lateEndGame = false;
            int maxTime = 20000;
            if (!startGame)
            {
                midGame = board.IsMidGame();
                endGame = board.IsEndGame();
                lateEndGame = board.IsLateEndGame();
            }
            else
            {
                maxTime = 8000;
            }

            int minDepth = 4;
            int maxDepth = 8;
            if (midGame)
            {
                minDepth = 5;
                maxDepth = 10;
                
            }
            else if (endGame)
            {
                minDepth = 6;
                maxDepth = 12;
            }
            else if (lateEndGame)
            {
                minDepth = 7;
                maxDepth = 13;
            }
            t.Reset();
            t.Start();
            //Console.WriteLine("SingleThreaded Move: " + move);
            int depth = minDepth;
            int ourCurrentBranch = board.GetAllStates(isWhite, true).Count;
            int oppCurrentBranch = board.GetAllStates(!isWhite, false).Count;
            if (turn > 1)
            {
                long nodes = (long)(Math.Pow(lastOurMoveCount, lastMoveDepth / 2.0) * Math.Pow(lastOpponentMoveCount, lastMoveDepth / 2.0));
                long currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                double nodesPerSecond = nodes / lastMoveTime;
                double estimatedTime = (currentNodes / nodesPerSecond);
                
                if (!lateEndGame)
                {
                    while (depth < maxDepth && estimatedTime < 2500)
                    {
                        depth++;
                        currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                        estimatedTime = (currentNodes / nodesPerSecond);
                    }
                    while (depth > minDepth && estimatedTime > maxTime)
                    {
                        depth--;
                        currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                        estimatedTime = (currentNodes / nodesPerSecond);
                    }
                }
                else
                {
                    if (secondsLeft > 500)
                    {
                        while (depth < maxDepth && estimatedTime < 30000)
                        {
                            depth++;
                            currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                            estimatedTime = (currentNodes / nodesPerSecond);
                        }
                        while (depth > minDepth && (estimatedTime > 85000))
                        {
                            depth--;
                            currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                            estimatedTime = (currentNodes / nodesPerSecond);
                        }
                    }
                    else
                    {
                        while (depth < maxDepth && estimatedTime < 10000)
                        {
                            depth++;
                            currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                            estimatedTime = (currentNodes / nodesPerSecond);
                        }
                        while (depth > minDepth && (estimatedTime > 30000))
                        {
                            depth--;
                            currentNodes = (long)(Math.Pow(ourCurrentBranch, depth / 2.0) * Math.Pow(oppCurrentBranch, depth / 2.0));
                            estimatedTime = (currentNodes / nodesPerSecond);
                        }
                    }
                }
                Console.WriteLine("Estimated Time: " + estimatedTime);
            }
            
            if (depth < minDepth)
            {
                depth = minDepth;
            }
            if (depth > maxDepth)
            {
                depth = maxDepth;
                
            }
            if (secondsLeft < 475 && depth > 12)
            {
                depth = 12;
            }
            //if (turn > 35 && secondsLeft > 200)
            //{
            //    depth = 6;
            //}//else if(secondsLeft > 100 && )
            Move m = board.PlayNegaMaxMoveMultiThreaded(out move, isWhite, depth);
            b.MakeMove(m);
            board = b;
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
