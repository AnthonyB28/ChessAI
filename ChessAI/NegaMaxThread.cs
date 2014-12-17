
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

        /// <summary>
        /// Creates a negamax thread
        /// </summary>
        /// <param name="board">Clone of the board to search on</param>
        /// <param name="color">Team side to search for</param>
        /// <param name="master">Reference to master thread to communicate</param>
        /// <param name="depth">Depth to search to</param>
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

        /// <summary>
        /// Runs the search thread
        /// </summary>
        public void Run()
        {
            bool first = true;
            while (master.HasMoves(out move))
            {
                //move = master.get
                //while loop here to do multiple depths
                //t.Reset();
                //t.Start();
                Move moveToMake = null;
                Negamax.pruned = 0;
                //for (int i = 0; i < moves.Count; ++i)
                //{
                int score = Negamax.NEGA_SCORE;
                int offset = 0;
                //if (board.LastMove().destinationPiece == 0)
                //{
                //    if (move.destinationPiece != 0 && Board.OFFSET_TABLE[move.originPiece] == Board.OFFSET_TABLE[move.destinationPiece])
                //    {
                //        offset = Board.OFFSET_TABLE[move.originPiece];
                //    }
                //}
                if (!first) 
                {
                    board.MakeMove(move);
                    score = -Negamax.NegaMax(board, depth - 1, -(alpha + 1), -alpha, !color, move.destinationPiece != 0, offset);
                    if (alpha < score && score < beta)
                    {
                        score = -Negamax.NegaMax(board, depth - 1, -beta, -score, !color, move.destinationPiece != 0, offset);
                    }
                    board.UndoMove();
                }
                else
                {
                    board.MakeMove(move);
                    score = -Negamax.NegaMax(board, depth - 1, -beta, -alpha, !color, move.destinationPiece != 0, offset);
                    board.UndoMove();
                    first = false;
                }
                if (score > alpha)
                {
                    alpha = score;
                    moveToMake = move;
                    
                    master.TellMove(move, alpha);
                    //}
                }
                alpha = master.GetAlpha();
            }
            finished = true;
        }

        /// <summary>
        /// Let's the master thread know if it's finished
        /// </summary>
        /// <returns>true if finished, false otherwise</returns>
        public bool isFinished()
        {
            return this.finished;
        }
    }
}
