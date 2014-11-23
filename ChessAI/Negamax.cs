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

        public static int negaMax(Board state, int depth, bool color)
        {
            bool nextPlayer = color ? false : true; // Reverse the player role
            if(depth == 0)
            {
                return state.Evaluate(nextPlayer);
            }
            int max = NEGA_SCORE;
            List<Move> moves = state.GetAllStates(nextPlayer);
            for (int i = 0; i < moves.Count; ++i )
            {
                state.MakeMove(moves[i]);
                int score = -negaMax(state, depth - 1, nextPlayer);
                if (score > max)
                {
                    max = score;
                }
                state.UndoMove();
            }
            return max;
        }
    }
}
