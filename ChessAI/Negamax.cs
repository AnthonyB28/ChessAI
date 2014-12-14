using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Negamax
    {
        public const int NEGA_SCORE = -999999999;
        public static long pruned = 0;

        public static int negaMax(Board state, int depth, int alpha, int beta, bool color, bool qs)
        {
            pruned++;
            byte type = Entry.ALPHA;
            long key = Zobrist.GetKey(state.board, color);
            int transposeEval = Transposition.Probe(key, depth, alpha, beta);
            if(transposeEval != Int32.MaxValue)
            {
                return transposeEval;
            }

            //if you return a score of 10 from white's perspective, 
            // and the last move was a black move, then the score returned should be -10
            if (state.IsTerminal())
            {
                int eval = state.Evaluate(color);
                Transposition.Insert(key, depth, Entry.EXACT, eval, state.GetLastMove());
                return eval;
            }
            if(depth == 0 ) //TODO: Checkmate end of game test
            {
                if (qs) 
                {
                    depth += 1;
                }
                else
                {
                    int eval = state.Evaluate(color);
                    Transposition.Insert(key, depth, Entry.EXACT, eval, state.GetLastMove());
                    return eval;
                }
            }
            List<Move> moves = state.GetAllStates(color, false);
            if (moves.Count == 0)
            {
                int eval = state.Evaluate(color);
                Transposition.Insert(key, depth, Entry.EXACT, eval, state.GetLastMove());
                return eval;
            }

            bool nextPlayer = color; // Reverse the player role
            
            
                //if (maxDepth - depth <= 1)
                //{
                //    //state.sortMoves(moves, color);
                //}
                for (int i = 0; i < moves.Count; ++i)
                {
                    Board backUp = state.Clone();
                    state.MakeMove(moves[i]);
                    int score = -negaMax(state, depth - 1, -beta, -alpha, !color, (qs && moves[i].destinationPiece != 0));
                    
                    state.UndoMove();
                    //if (!state.Equals(backUp))
                    //{
                    //    Console.WriteLine("WARNING: UNDO FAILED ON MOVE");
                    //    Console.WriteLine(moves[i].ToString());
                    //}
                    if (score >= beta)
                    {
                        Transposition.Insert(key, depth, Entry.BETA, beta, state.GetLastMove());
                        return score;
                    }
                    //if (score < alpha)
                    //{
                    //    //return beta;
                    //}
                    if (score > alpha)
                    {
                        type = Entry.EXACT;
                        alpha = score;
                    }
                }
            //}
            Transposition.Insert(key, depth, type, alpha, state.GetLastMove());
            return alpha;
        }
    }
}
