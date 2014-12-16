using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessAI
{
    class NegaMaxMasterThread
    {
        private Board board;
        private List<Move> moves;
        private int alpha;
        //private int beta;
        private bool color;
        private List<NegaMaxThread> threads;
        private Move moveToMake;
        private int depth;
        private bool loopState;
        private Move loopMove;
        private int loopAlpha;
        //private List<Move> checkMateMoves;
        //private bool checkMate;
        //private Move checkMateMove;

        //private object _lockerCheckMate = new object();
        private object _lockerGet = new object();
        private object _lockerStore = new object();

        public NegaMaxMasterThread(Board board, bool color, int depth)
        {
            this.board = board;
            this.color = color;
            this.moves = this.board.GetAllStates(this.color, true);
            loopState = false;
            loopMove = null;
            loopAlpha = Negamax.NEGA_SCORE;
            //checkMateMoves = new List<Move>();
            //checkMate = false;
            //checkMateMove = null;
            
            Console.WriteLine("Moves available: " + this.moves.Count);
            this.moves.Sort();
            this.threads = new List<NegaMaxThread>();
            alpha = Negamax.NEGA_SCORE;
            this.depth = depth;
            if (this.moves.Count < 16)
            {
                //this.depth += 2;
            }
            if (this.moves.Count < 10)
            {
                //this.depth += 2;
            }
        }

        public Move Run()
        {
            int cpus = Environment.ProcessorCount;
            // Search all moves for a king capture and just take the first one possible, avoids a high depth search in end game
            // that can run out the clock, score is pointless in comparison to a win
            // also avoids letting our king be in check and constantly looping, however it may not be possible due to that a captured king
            // is considered a terminal node in search, and our awesome eval function
            foreach (Move m in moves)
            {
                if (color)
                {
                    if (m.destinationPiece == Board.B_KING)
                    {
                        return m;
                    }
                }
                else
                {
                    if (m.destinationPiece == Board.W_KING)
                    {
                        return m;
                    }
                }
            }
            // find a loop move and save it, so when it's score comes back we compare it to a secondary choice
            // secondary choice is picked if it is within a close margin, this avoids escaping a loop and losing 
            // a big piece
            // consideration, if loop increase depth to try to escape, would be nice to have some kind of var to see how many times
            // we have been looping and continously increase the depth here, perhaps 1 depth every 3 loops
            // if no loop found we reset the counter to 0, this is not multithreaded code, so it should work
            Board bClone = this.board.Clone();
            if (bClone.GetAllMoves().Count >= 6)
            {
                Stack<Move> tempStack = new Stack<Move>();
                foreach (Move m in bClone.GetAllMoves().Reverse())
                {
                    tempStack.Push(m);
                }
                tempStack.Pop();
                Move m1 = tempStack.Pop();
                tempStack.Pop();
                Move m2 = tempStack.Pop();
                tempStack.Pop();
                if (m1.Equals(tempStack.Pop()))
                {
                    loopMove = m2;
                    loopState = true;
                }
            }
            //Console.WriteLine("CPUS: " + cpus);
            // Create threads and fire off
            for (int i = 0; i < cpus; i++)
            {
                NegaMaxThread thread = new NegaMaxThread(board.Clone(), color, this, depth);
                new Thread(thread.Run).Start();
                this.threads.Add(thread);
            }
            //if(board.)
            int x = 0;
            while (threads.Count != 0)
            {
                //Console.WriteLine("x: " + x);
                if (threads[x].isFinished())
                {
                    threads.RemoveAt(x);
                }
                else
                {
                    x++;
                    
                }
                if (x >= threads.Count)
                {
                    x = 0;
                    Thread.Sleep(500);
                    //Console.WriteLine("moves waiting: " + moves.Count);
                }
                
            }
            //do return magic
            if (loopState)
            {
                if (this.alpha < (loopAlpha - 100))
                {
                    moveToMake = loopMove;
                }
                else
                {
                    Console.WriteLine("Avoiding loop move... WARNING!!!");
                }
            }
            //if (checkMate)
            //{
            //    moveToMake = checkMateMove;
            //}
            if (this.alpha < -8000)
            {
                Board b = this.board.Clone();
                b.MakeMove(moveToMake);
                if (b.CheckForKingCheck(0, 0, color)) // TODO fix
                {
                    b.UndoMove();
                    List<Move> nonCheckMoves = new List<Move>();
                    foreach(Move m in this.moves)
                    {
                        b.MakeMove(m);
                        if (!b.CheckForKingCheck(0, 0, color))
                        {
                            nonCheckMoves.Add(m);
                        }
                        b.UndoMove();
                    }
                    if (nonCheckMoves.Count > 0)
                    {
                        Console.WriteLine("Researching....");
                        depth = 5;
                        this.moves = nonCheckMoves;
                        return this.Run();
                    }
                }
            }
            return moveToMake;
        }

        public bool hasMoves(out Move move)
        {
            // double checked locking if there are moves available, if we see none, obviously we don't need to lock
            if (moves.Count > 0)
            {
                lock (_lockerGet)
                {
                    if (moves.Count > 0)
                    {
                        move = moves[0];
                        moves.RemoveAt(0);
                        //return move;
                        return true;
                    }
                }
            }
            move = null;
            return false;
        }

        public void TellMove(Move move, int alpha)
        {
            //Console.WriteLine(alpha);
            //Console.WriteLine("telling");
            bool message = false;
            bool loop = false;
            if (alpha >= this.alpha)
            {
                lock (_lockerStore)
                {
                    if (alpha > this.alpha)
                    {
                        if (loopState && move.Equals(loopMove))
                        {
                            loopAlpha = alpha;
                            loop = true;
                        }
                        else
                        {
                            moveToMake = move;
                            this.alpha = alpha;
                            message = true;// so this IO bound probably fucking murders contention we should consider moving this outside the lock
                        }
                    }
                    else if (alpha == this.alpha)
                    {
                        if (!loopState || !move.Equals(loopMove))
                        {
                            if (move.CompareTo(moveToMake) < 0)
                            {
                                //Console.WriteLine(move);
                                //Console.WriteLine(moveToMake);
                                moveToMake = move;
                                message = true;
                            }
                        }
                    }
                    //if (alpha < this.alpha)
                    //{
                    //    lock (_lockerGet)
                    //    {
                    //        this.moves = new List<Move>();
                    //    }
                    //}
                }
            }
            //if (alpha > 8000)
            //{
            //    if (color)
            //    {
            //        if (move.destinationPiece == Board.B_KING)
            //        {
            //            if (!checkMate)
            //            {
            //                lock (_lockerCheckMate)
            //                {
            //                    checkMateMove = move;
            //                    checkMate = true;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (move.destinationPiece == Board.W_KING)
            //        {
            //            if (!checkMate)
            //            {
            //                lock (_lockerCheckMate)
            //                {
            //                    checkMateMove = move;
            //                    checkMate = true;
            //                }
            //            }
            //        }
            //    }
            //}
            if (message)
            {
                Console.WriteLine("New move:" + alpha + " @depth:" + depth);
            }
            if (loop)
            {
                Console.WriteLine("loop move:" + alpha + " @depth:" + depth);
            }
        }

        public int getCurrentAlpha()
        {
            lock (_lockerStore)
            {
                return this.alpha;
            }
        }

        public Move getNextMove()
        {
            lock (_lockerGet)
            {
                Move move = moves[0];
                moves.RemoveAt(0);
                return move;
            }
        }
    }
}
