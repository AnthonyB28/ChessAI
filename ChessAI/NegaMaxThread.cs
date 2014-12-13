using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class NegaMaxThread
    {
        private Board board;
        private Move move;
        private int alpha;
        private int beta;
        private bool color;
        private NegaMaxMasterThread master;
        private bool finished;
        private int depth;

        public NegaMaxThread(Board board, bool color, NegaMaxMasterThread master, int depth)
        {
            this.board = board;
            this.color = color;
            this.master = master;
            this.finished = false;
            this.depth = depth;
            alpha = Negamax.NEGA_SCORE;
            beta = -Negamax.NEGA_SCORE;
        }

        public void Run()
        {
            
            while (master.hasMoves(out move))
            {
                //move = master.get
                //while loop here to do multiple depths
                //t.Reset();
                //t.Start();
                Move moveToMake = null;
                Negamax.pruned = 0;
                //for (int i = 0; i < moves.Count; ++i)
                //{
                board.MakeMove(move);
                int score = -Negamax.negaMax(board, depth - 1, -beta, -alpha, !color, move.destinationPiece != 0);
                board.UndoMove();
                if (score > alpha)
                {
                    alpha = score;
                    moveToMake = move;
                    
                    master.TellMove(move, alpha);
                    //}
                }
                alpha = master.getCurrentAlpha();
            }
            finished = true;
        }

        public bool isFinished()
        {
            return this.finished;
        }
    }
}
