using System.Collections.Generic;

namespace ChessAI
{
    /// <summary>
    /// Negamax handles the game tree search for the best possible move.
    /// This representation is actually a NegaScout implementation with Quescence extension and alpha/beta pruning.
    /// </summary>
    class Negamax
    {
        public const int NEGA_SCORE = -999999999;
        public static long pruned = 0;

        /// <summary>
        /// Search for the next best move based on evaluation with alpha beta pruning.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="qs"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int NegaMax(Board state, int depth, int alpha, int beta, bool color, bool qs, int offset)
        {
            pruned++;
            //if you return a score of 10 from white's perspective, 
            // and the last move was a black move, then the score returned should be -10
            if (state.IsTerminal())
            {
                return state.Evaluate(color, 0);
            }

            if(depth == 0 ) //TODO: Checkmate end of game test
            {
                return Quiesce(state, alpha, beta, color, 0);
            }
            List<Move> moves = state.GetAllStates(color, false);
            moves.Sort();
            if (moves.Count == 0)
            {
                return state.Evaluate(color, 0);
            }
            
           
            bool first = true;
            for (int i = 0; i < moves.Count; ++i)
            {
                Board backUp = state.Clone();
                int score = Negamax.NEGA_SCORE;

                if (!first)
                {
                    state.MakeMove(moves[i]);
                    score = -NegaMax(state, depth - 1, -(alpha + 1), -alpha, !color, (qs && moves[i].destinationPiece != 0), offset);
                    if (alpha < score && score < beta)
                    {
                        score = -NegaMax(state, depth - 1, -beta, -score, !color, (qs && moves[i].destinationPiece != 0), offset);
                    }
                    state.UndoMove();
                }
                else
                {
                    state.MakeMove(moves[i]);
                    score = -NegaMax(state, depth - 1, -beta, -alpha, !color, (qs && moves[i].destinationPiece != 0), offset);

                    state.UndoMove();
                    first = false;
                }
                //if (!state.Equals(backUp))
                //{
                //    Console.WriteLine("WARNING: UNDO FAILED ON MOVE");
                //    Console.WriteLine(moves[i].ToString());
                //}
                if (score >= beta)
                {
                        
                    return score;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
            }
            return alpha;
        }

        /// <summary>
        /// Extension to negamax that searches deeper into the game until there are no more capture moves.
        /// Aka "quiet"
        /// </summary>
        /// <param name="state"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static int Quiesce(Board state, int alpha, int beta, bool color, int depth)
        {
            if (state.IsTerminal())
            {
                return state.Evaluate(color, 0);
            }
            int stand_pat = state.Evaluate(color, 0);
            if (stand_pat >= beta)
            {
                return beta;
            }
            if (alpha < stand_pat)
            {
                alpha = stand_pat;
            }

            List<Move> moves = state.GetAllCaptureStates(color);
            if (moves.Count == 0)
            {
                return state.Evaluate(color, 0);
            }
            for (int i = 0; i < moves.Count; ++i)
            {
                state.MakeMove(moves[i]);
                int score = -Quiesce(state, -beta, -alpha, !color, depth + 1);
                state.UndoMove();
                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                }
            }
            return alpha;
        }
    }
}
