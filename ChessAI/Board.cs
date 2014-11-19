using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; 

namespace ChessAI
{
    class Board
    {
        private static readonly byte BLANK_PIECE = 0;
        private static readonly byte W_PAWN = 1;
        private static readonly byte W_ROOK = 2;
        private static readonly byte W_KNIGHT = 3;
        private static readonly byte W_BISHOP = 4;
        private static readonly byte W_QUEEN = 5;
        private static readonly byte W_KING = 6;
        private static readonly byte B_PAWN = 7;
        private static readonly byte B_ROOK = 8;
        private static readonly byte B_KNIGHT = 9;
        private static readonly byte B_BISHOP = 10;
        private static readonly byte B_QUEEN = 11;
        private static readonly byte B_KING = 12;

        private static readonly bool WHITE = true;
        private static readonly bool BLACK = false;

        private byte[,] board;

        public Board()
        {
            board = new byte[8, 8];
            for (int i = 0; i < 8; i++) {
                board[i, 1] = W_PAWN;
                board[i, 6] = B_PAWN;
            }

            board[0, 0] = W_ROOK;
            board[0, 1] = W_KNIGHT;
            board[0, 2] = W_BISHOP;
            board[0, 3] = W_QUEEN;
            board[0, 4] = W_KING;
            board[0, 5] = W_BISHOP;
            board[0, 6] = W_KNIGHT;
            board[0, 7] = W_ROOK;

            board[7, 0] = B_ROOK;
            board[7, 1] = B_KNIGHT;
            board[7, 2] = B_BISHOP;
            board[7, 3] = B_QUEEN;
            board[7, 4] = B_KING;
            board[7, 5] = B_BISHOP;
            board[7, 6] = B_KNIGHT;
            board[7, 7] = B_ROOK;
        }

        public Board(byte[,] board)
        {
            this.board = board;
        }

        public override Board Clone()
        {
            return new Board((byte[,])board.Clone());
        }

        public void MovePiece(int x1, int y1, int x2, int y2)
        {
            board[x2, y2] = board[x1, y1];
            board[x1, y1] = BLANK_PIECE;
        }

        public List<Board> GetAllStates(bool white)
        {
            List<Board> moves = new List<Board>();
            //generate all moves
            for(int i = 0; i < 8; i++){
                for(int j = 0; j < 8; j++){
                    if((board[i,j] != 0) && IsColor(i, j, white) ){
                        //PAWN
                        if(board[i,j] % 6 == W_PAWN){
                            if(white){
                                if(board[i, j+1] == 0){
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, j+1);
                                    moves.Add(b);
                                }
                                if(j==1 && board[i,j+2] == 0){
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, j+2);
                                    moves.Add(b);
                                }
                                if((i < 7) && (IsColor(i+1, j+1, !white))){
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i+1, j+1);
                                    moves.Add(b);
                                }
                                if((i > 0) && (IsColor(i-1, j+1, !white))){
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i-1, j+1);
                                    moves.Add(b);
                                }
                            }else{
                                if (board[i, j - 1] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, j - 1);
                                    moves.Add(b);
                                }
                                if (j == 6 && board[i, j - 2] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, j - 2);
                                    moves.Add(b);
                                }
                                if ((i < 7) && (IsColor(i + 1, j - 1, !white)))
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i + 1, j - 1);
                                    moves.Add(b);
                                }
                                if ((i > 0) && (IsColor(i - 1, j - 1, !white)))
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i - 1, j - 1);
                                    moves.Add(b);

                                }
                            }
                        }
                        //ROOK
                        else if(board[i,j] % 6 == W_ROOK){
                            for (int x = i+1; x < 8; x++)
                            {
                                if (board[x, j] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, j);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, j, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, j);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1; x >= 0; x--)
                            {
                                if (board[x, j] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, j);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, j, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, j);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int y = j + 1; y < 7; y++)
                            {

                            }
                        }
                    }
                }
            }

            return moves;
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsColor(int i, int j, bool white)
        {
            return ((board[i, j] - 1) / 6 == 0) == white;
        }
    }
}
