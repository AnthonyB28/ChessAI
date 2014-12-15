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

        public static int Quiesce(Board state, int alpha, int beta, bool color, int depth)
        {
            //List<Move> moves = state.GetAllCaptureStates(color);
            //if (moves.Count == 0)
            //{
            //    return state.Evaluate(color, 0);
            //}
            //foreach (Move move in moves)
            //{
            //    state.MakeMove(move);
            //    int score = -Quiesce(state, -beta, -alpha, !color);
            //    state.UndoMove();

            //    if (score >= beta)
            //    {

            //        return score;
            //    }
            //    //if (score < alpha)
            //    //{
            //    //    //return beta;
            //    //}
            //    if (score > alpha)
            //    {
            //        alpha = score;
            //    }
            //}

            //return alpha;
            if (state.isTerminal())
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
            foreach (Move move in moves)
            {
                state.MakeMove(move);
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

        public static int negaMax(Board state, int depth, int alpha, int beta, bool color, bool qs, int offset)
        {
            pruned++;
            byte type = Entry.ALPHA;
            long key = state.GetKey(); // Use Zobrist.GetKey(state.board, color) instead if there appears to be issues, slower as its linear
            Entry transposeEval = Transposition.Probe(key);
            if (transposeEval != null)
            {
                if (transposeEval.depth >= depth)
                {
                    if (transposeEval.flag == Entry.EXACT)
                    {
                        return transposeEval.eval;
                    }
                    if (transposeEval.flag == Entry.ALPHA)
                    {
                        if(transposeEval.eval > alpha)
                        {
                            alpha = transposeEval.eval;
                        }
                    }
                    if (transposeEval.flag == Entry.BETA)
                    {
                        if(transposeEval.eval < beta)
                        {
                            beta = transposeEval.eval;
                        }
                    }
                    if (alpha >= beta)
                    {
                        return transposeEval.eval;
                    }
                }
            }
            //if you return a score of 10 from white's perspective, 
            // and the last move was a black move, then the score returned should be -10
            if (state.isTerminal())
            {
                int eval = state.Evaluate(color, 0);
                Transposition.Insert(key, (short)depth, Entry.EXACT, eval);
                return eval;
            }
            if(depth == 0 ) //TODO: Checkmate end of game test
            {
                //
                return Quiesce(state, alpha, beta, color, 0);
            }
            List<Move> moves = state.GetAllStates(color, false);
            if (moves.Count == 0)
            {
                int eval = state.Evaluate(color, 0);
                Transposition.Insert(key, (short)depth, Entry.EXACT, eval);
                return eval;
            }

            bool nextPlayer = color; // Reverse the player role
            
            
                //if (maxDepth - depth <= 1)
                //{
                //    //state.sortMoves(moves, color);
                //}
            bool first = true;
                for (int i = 0; i < moves.Count; ++i)
                {
                    Board backUp = state.Clone();
                    int score = Negamax.NEGA_SCORE;

                    if (!first)
                    {
                        state.MakeMove(moves[i]);
                        score = -negaMax(state, depth - 1, -(alpha + 1), -alpha, !color, (qs && moves[i].destinationPiece != 0), offset);
                        if (alpha < score && score < beta)
                        {
                            score = -negaMax(state, depth - 1, -beta, -score, !color, (qs && moves[i].destinationPiece != 0), offset);
                        }
                        state.UndoMove();
                    }
                    else
                    {
                        state.MakeMove(moves[i]);
                        score = -negaMax(state, depth - 1, -beta, -alpha, !color, (qs && moves[i].destinationPiece != 0), offset);

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
                        Transposition.Insert(key, (short)depth, Entry.BETA, beta);
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
                Transposition.Insert(key, (short)depth, type, alpha);
            return alpha;
        }
    }
}
