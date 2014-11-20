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
            board[1, 0] = W_KNIGHT;
            board[2, 0] = W_BISHOP;
            board[3, 0] = W_QUEEN;
            board[4, 0] = W_KING;
            board[5, 0] = W_BISHOP;
            board[6, 0] = W_KNIGHT;
            board[7, 0] = W_ROOK;

            board[0, 7] = B_ROOK;
            board[1, 7] = B_KNIGHT;
            board[2, 7] = B_BISHOP;
            board[3, 7] = B_QUEEN;
            board[4, 7] = B_KING;
            board[5, 7] = B_BISHOP;
            board[6, 7] = B_KNIGHT;
            board[7, 7] = B_ROOK;
        }

        public Board(byte[,] board)
        {
            this.board = board;
        }

        public Board Clone()
        {
            return new Board((byte[,])board.Clone());
        }

        public void MovePiece(int x1, int y1, int x2, int y2)
        {
            board[x2, y2] = board[x1, y1];
            board[x1, y1] = BLANK_PIECE;
        }

        public void MovePiece(int x1, int y1, int x2, int y2, byte promote)
        {
            board[x2, y2] = promote;
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
                        if (board[i, j] % 6 == W_PAWN)
                        {
                            if (white)
                            {
                                if (board[i, j + 1] == 0)
                                {
                                    Board b = this.Clone();
                                    
                                    if (j == 6) 
                                    {
                                        b.MovePiece(i, j, i, j + 1, W_QUEEN);
                                        moves.Add(b);
                                    }
                                    else
                                    {
                                        b.MovePiece(i, j, i, j + 1);
                                        moves.Add(b);
                                    }
                                }
                                if (j == 1 && board[i,j+1] == 0 && board[i, j + 2] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, j + 2);
                                    moves.Add(b);
                                }
                                if ((i < 7) && board[i+1,j+1] != 0 && (IsColor(i + 1, j + 1, !white)))
                                {
                                    Board b = this.Clone();
                                    if (j == 6)
                                    {
                                        b.MovePiece(i, j, i + 1, j + 1, W_QUEEN);
                                        moves.Add(b);
                                    }
                                    else
                                    {
                                        b.MovePiece(i, j, i + 1, j + 1);
                                        moves.Add(b);
                                    }
                                }
                                if ((i > 0) && board[i-1,j+1] != 0 && (IsColor(i - 1, j + 1, !white)))
                                {
                                    Board b = this.Clone();
                                    if (j == 6)
                                    {
                                        b.MovePiece(i, j, i - 1, j + 1, W_QUEEN);
                                        moves.Add(b);
                                    }
                                    else
                                    {
                                        b.MovePiece(i, j, i - 1, j + 1);
                                        moves.Add(b);
                                    }
                                }
                            }
                            else
                            {
                                if (board[i, j - 1] == 0)
                                {
                                    Board b = this.Clone();
                                    if (j == 1)
                                    {
                                        b.MovePiece(i, j, i, j - 1, B_QUEEN);
                                        moves.Add(b);
                                    }
                                    else
                                    {
                                        b.MovePiece(i, j, i, j - 1);
                                        moves.Add(b);
                                    }
                                }
                                if (j == 6 && board[i,j-1] == 0 && board[i, j - 2] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, j - 2);
                                    moves.Add(b);
                                }
                                if ((i < 7) && board[i+1,j-1] != 0 && (IsColor(i + 1, j - 1, !white)))
                                {
                                    Board b = this.Clone();
                                    if (j == 1)
                                    {
                                        b.MovePiece(i, j, i + 1, j - 1, B_QUEEN);
                                        moves.Add(b);
                                    }
                                    else
                                    {
                                        b.MovePiece(i, j, i + 1, j - 1);
                                        moves.Add(b);
                                    }
                                }
                                if ((i > 0) && board[i-1,j-1] !=0 && (IsColor(i - 1, j - 1, !white)))
                                {
                                    Board b = this.Clone();
                                    if (j == 1)
                                    {
                                        b.MovePiece(i, j, i - 1, j - 1, B_QUEEN);
                                        moves.Add(b);
                                    }
                                    else
                                    {
                                        b.MovePiece(i, j, i - 1, j - 1);
                                        moves.Add(b);
                                    }

                                }
                            }
                        }
                        //ROOK
                        else if (board[i, j] % 6 == W_ROOK)
                        {
                            for (int x = i + 1; x < 8; x++)
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
                            for (int y = j + 1; y < 8; y++)
                            {
                                if (board[i, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, i, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int y = j - 1; y >= 0; y--)
                            {
                                if (board[i, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, i, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                        }
                        else if (board[i, j] % 6 == W_BISHOP)
                        {
                            for (int x = i + 1, y = j + 1; x < 8 && y < 8; x++, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j - 1; x >= 0 && y >= 0; x--, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i + 1, y = j - 1; x < 8 && y >= 0; x++, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j + 1; x >= 0 && y < 8; x--, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                        }
                        else if (board[i, j] % 6 == W_QUEEN)
                        {
                            for (int x = i + 1; x < 8; x++)
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
                            for (int y = j + 1; y < 8; y++)
                            {
                                if (board[i, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, i, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int y = j - 1; y >= 0; y--)
                            {
                                if (board[i, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, i, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, i, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i + 1, y = j + 1; x < 8 && y < 8; x++, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j - 1; x >= 0 && y >= 0; x--, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i + 1, y = j - 1; x < 8 && y >= 0; x++, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j + 1; x >= 0 && y < 8; x--, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    Board b = this.Clone();
                                    b.MovePiece(i, j, x, y);
                                    moves.Add(b);
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        Board b = this.Clone();
                                        b.MovePiece(i, j, x, y);
                                        moves.Add(b);
                                    }
                                    break;
                                }
                            }
                        }
                        else if (board[i, j] % 6 == 0)
                        {
                            if ((i < 7) && (board[i + 1, j] == 0 || IsColor(i + 1, j, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 1, j);
                                moves.Add(b);
                            }
                            if ((i < 7) && (j > 0) && (board[i + 1, j - 1] == 0 || IsColor(i + 1, j - 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 1, j - 1);
                                moves.Add(b);
                            }
                            if ((i < 7) && (j < 7) && (board[i + 1, j + 1] == 0 || IsColor(i + 1, j + 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 1, j + 1);
                                moves.Add(b);
                            }
                            if ((j < 7) && (board[i, j + 1] == 0 || IsColor(i, j, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i, j + 1);
                                moves.Add(b);
                            }
                            if ((j > 0) && (board[i, j - 1] == 0 || IsColor(i, j - 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i, j - 1);
                                moves.Add(b);
                            }
                            if ((i > 0) && (board[i - 1, j] == 0 || IsColor(i - 1, j, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 1, j);
                                moves.Add(b);
                            }
                            if ((i > 0) && (j < 7) && (board[i - 1, j + 1] == 0 || IsColor(i - 1, j + 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 1, j + 1);
                                moves.Add(b);
                            }
                            if ((i > 0) && (j > 0) && (board[i - 1, j - 1] == 0 || IsColor(i - 1, j - 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 1, j - 1);
                                moves.Add(b);
                            }
                        }
                        else if (board[i, j] % 6 == W_KNIGHT)
                        {
                            if ((i < 6) && (j < 7) && (board[i + 2, j + 1] == 0 || IsColor(i + 2, j + 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 2, j + 1);
                                moves.Add(b);
                            }
                            if ((i < 6) && (j > 0) && (board[i + 2, j - 1] == 0 || IsColor(i + 2, j - 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 2, j - 1);
                                moves.Add(b);
                            }
                            if ((i < 7) && (j < 6) && (board[i + 1, j + 2] == 0 || IsColor(i + 1, j + 2, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 1, j + 2);
                                moves.Add(b);
                            }
                            if ((i < 7) && (j > 1) && (board[i + 1, j - 2] == 0 || IsColor(i + 1, j - 2, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i + 1, j - 2);
                                moves.Add(b);
                            }
                            if ((i > 0) && (j < 6) && (board[i - 1, j + 2] == 0 || IsColor(i - 1, j + 2, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 1, j + 2);
                                moves.Add(b);
                            }
                            if ((i > 0) && (j > 1) && (board[i - 1, j - 2] == 0 || IsColor(i - 1, j - 2, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 1, j - 2);
                                moves.Add(b);
                            }
                            if ((i > 1) && (j < 7) && (board[i - 2, j + 1] == 0 || IsColor(i - 2, j + 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 2, j + 1);
                                moves.Add(b);
                            }
                            if ((i > 1) && (j > 0) && (board[i - 2, j - 1] == 0 || IsColor(i - 2, j - 1, !white)))
                            {
                                Board b = this.Clone();
                                b.MovePiece(i, j, i - 2, j - 1);
                                moves.Add(b);
                            }
                        }
                    }
                }
            }

            return moves;
        }

        public Board PlayRandomMove(out string move, bool color)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            List<Board> boards = GetAllStates(color);
            Console.WriteLine("Moves Available: " + boards.Count);
            int x = rand.Next(boards.Count);
            Board b = boards[x];
            move = detectMove(b);
            return b;
        }

        public string detectMove(Board b)
        {
            int startPieceI = 0;
            int startPieceJ = 0;
            int endPieceI = 0;
            int endPieceJ = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != b.board[i, j])
                    {
                        if (b.board[i, j] == 0)
                        {
                            startPieceI = i;
                            startPieceJ = j;
                        }
                        else
                        {
                            endPieceI = i;
                            endPieceJ = j;
                        }
                    }
                }
            }
            string piece = "";
            switch (board[startPieceI, startPieceJ] % 6)
            {
                case 0:
                    piece += "K";
                    break;
                case 1:
                    piece += "P";
                    break;
                case 2:
                    piece += "R";
                    break;
                case 3:
                    piece += "N";
                    break;
                case 4:
                    piece += "B";
                    break;
                case 5:
                    piece += "Q";
                    break;
            }
            piece += GetFile(startPieceI);
            piece += (startPieceJ + 1);
            piece += GetFile(endPieceI);
            piece += (endPieceJ + 1);
            if (board[startPieceI, startPieceJ] != b.board[endPieceI, endPieceJ])
            {
                switch (b.board[endPieceI, endPieceJ] % 6)
                {
                    case 0:
                        piece += "K";
                        break;
                    case 1:
                        piece += "P";
                        break;
                    case 2:
                        piece += "R";
                        break;
                    case 3:
                        piece += "N";
                        break;
                    case 4:
                        piece += "B";
                        break;
                    case 5:
                        piece += "Q";
                        break;
                }
            }
            return piece;
        }

        public string GetFile(int i)
        {
            switch (i)
            {
                case 0:
                    return "a";
                case 1:
                    return "b";
                case 2:
                    return "c";
                case 3:
                    return "d";
                case 4:
                    return "e";
                case 5:
                    return "f";
                case 6:
                    return "g";
                case 7:
                    return "h";
            }
            return "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsColor(int i, int j, bool white)
        {
            return ((board[i, j] - 1) / 6 == 0) == white;
        }
    }
}
