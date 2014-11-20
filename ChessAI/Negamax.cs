using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Negamax
    {
        public int negaMax(Board state, int depth, bool color)
        {
            bool nextPlayer = color ? false : true; // Reverse the player role
            if(depth == 0)
            {
                return state.Evaluate(nextPlayer);
            }
            int max = -999999999;
            List<Board> moves = state.GetAllStates(nextPlayer);
            for (int i = 0; i < moves.Count; ++i )
            {
                Board newBoard = moves[i].Clone(); // This is depth first search, we should be doing negamax with a move, and then undo later.
                int score = -negaMax(newBoard, depth - 1, nextPlayer);
                if (score > max)
                {
                    max = score;
                }
                //TODO: Undo move on state
            }
            return max;
        }
    }
}
