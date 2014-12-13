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

        private object _lockerGet = new object();
        private object _lockerStore = new object();

        public NegaMaxMasterThread(Board board, bool color, int depth)
        {
            this.board = board;
            this.color = color;
            this.moves = this.board.GetAllStates(this.color, true);
            Console.WriteLine("Moves available: " + this.moves.Count);
            this.board.sortMoves(this.moves, this.color);
            this.threads = new List<NegaMaxThread>();
            alpha = Negamax.NEGA_SCORE;
            this.depth = depth;
        }

        public Move Run()
        {
            int cpus = Environment.ProcessorCount;
            //Console.WriteLine("CPUS: " + cpus);
            // Create threads and fire off
            for (int i = 0; i < cpus; i++)
            {
                NegaMaxThread thread = new NegaMaxThread(board.Clone(), color, this, depth);
                new Thread(thread.Run).Start();
                this.threads.Add(thread);
            }
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
            //if (alpha > this.alpha)
            //{
                lock (_lockerStore)
                {
                    if (alpha > this.alpha)
                    {
                        moveToMake = move;
                        this.alpha = alpha;
                        Console.WriteLine("New move:" + alpha + " @depth:" + depth);
                    }
                    if (alpha < this.alpha)
                    {
                        lock (_lockerGet)
                        {
                            this.moves = new List<Move>();
                        }
                    }
                }
            //}
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
