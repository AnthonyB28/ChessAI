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
            //if you return a score of 10 from white's perspective, 
            // and the last move was a black move, then the score returned should be -10
            if (state.IsTerminal())
            {
                return state.Evaluate(color);
            }
            if(depth == 0 ) //TODO: Checkmate end of game test
            {
                if (qs) 
                {
                    depth += 1;
                }
                else
                {
                    return state.Evaluate(color);
                }
            }
            List<Move> moves = state.GetAllStates(color, false);
            if (moves.Count == 0)
            {
                return state.Evaluate(color);
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
                        
                        return score;
                    }
                    //if (score < alpha)
                    //{
                    //    //return beta;
                    //}
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            //}
            return alpha;
        }
    }
}
