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
        public static int pruned = 0;

        public static int negaMax(Board state, int depth, int alpha, int beta, bool color)
        {
            //if you return a score of 10 from white's perspective, 
            // and the last move was a black move, then the score returned should be -10
            bool nextPlayer = color ? false : true; // Reverse the player role
            if(depth == 0) //TODO: Checkmate end of game test
            {
                return state.Evaluate(color);
            }
            
            List<Move> moves = state.GetAllStates(nextPlayer);
            for (int i = 0; i < moves.Count; ++i )
            {
                state.MakeMove(moves[i]);
                int score = -negaMax(state, depth - 1, -beta, -alpha, nextPlayer);
                state.UndoMove();
                if(score >= beta)
                {
                    pruned += moves.Count - i;
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
