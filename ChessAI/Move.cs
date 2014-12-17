using System;
using System.Text;
using System.Runtime.CompilerServices; 

namespace ChessAI
{
    /// <summary>
    /// Represents a move on the board
    /// </summary>
    class Move : IComparable<Move>
    {
        public static readonly string[] FILE_TABLE = new string[8] { "a", "b", "c", "d", "e", "f", "g", "h" };
        public static readonly string[] PIECE_TABLE = new string[13] { "P", "P", "R", "N", "B", "Q", "K", "P", "R", "N", "B", "Q", "K" };
        public static readonly int[] MATERIAL_TABLE = new int[13] { 0, 100, 525, 350, 350, 1000, 10000, 100, 525, 350, 350, 1000, 10000 };

        public int originX;
        public int originY;
        public int destX;
        public int destY;
        public bool promotion;
        public bool enpassent;
        public byte destinationPiece;
        public byte originPiece;

        /// <summary>
        /// Regular move
        /// </summary>
        /// <param name="x1">origin x</param>
        /// <param name="y1">origin y</param>
        /// <param name="x2">destination x</param>
        /// <param name="y2">destination y</param>
        /// <param name="board">input board</param>
        public Move(int x1, int y1, int x2, int y2, byte[,] board)
        {
            originX = x1;
            originY = y1;
            destX = x2;
            destY = y2;
            originPiece = board[x1, y1];
            destinationPiece = board[x2, y2];
            promotion = false;
            enpassent = false;
        }

        /// <summary>
        /// Handles promotion
        /// </summary>
        /// <param name="x1">origin x</param>
        /// <param name="y1">origin y</param>
        /// <param name="x2">destination x</param>
        /// <param name="y2">destination y</param>
        /// <param name="board">input board</param>
        /// <param name="promote"></param>
        public Move(int x1, int y1, int x2, int y2, byte[,] board, byte promote)
        {
            originX = x1;
            originY = y1;
            destX = x2;
            destY = y2;
            originPiece = promote;
            destinationPiece = board[x2, y2];
            promotion = true;
            enpassent = false;
        }

        /// <summary>
        /// Enpassent
        /// </summary>
        /// <param name="x1">origin x</param>
        /// <param name="y1">origin y</param>
        /// <param name="x2">destination x</param>
        /// <param name="y2">destination y</param>
        /// <param name="board">input board</param>
        /// <param name="passent"></param>
        public Move(int x1, int y1, int x2, int y2, byte[,] board, bool passent)
        {
            originX = x1;
            originY = y1;
            destX = x2;
            destY = y2;
            originPiece = board[x1, y1];
            destinationPiece = board[x2, y1];
            enpassent = true;

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!promotion)
            {
                sb.Append(PIECE_TABLE[originPiece]);
            }
            else
            {
                sb.Append("P");
            }
            sb.Append(FILE_TABLE[originX]);
            sb.Append((originY + 1));
            sb.Append(FILE_TABLE[destX]);
            sb.Append((destY + 1));
            if (promotion)
            {
                sb.Append(PIECE_TABLE[originPiece]);
            }

            return sb.ToString();

        }

        public bool Equals(Move move)
        {
            return originPiece == move.originPiece && destinationPiece == move.destinationPiece && originX == move.originX && originY == move.originY && destX == move.destX && destY == move.destY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Move m)
        {
            //current move is capturing
            if (destinationPiece != 0)
            {
                if (m.destinationPiece != 0)
                {
                    return (MATERIAL_TABLE[m.destinationPiece] - MATERIAL_TABLE[m.originPiece]) - (MATERIAL_TABLE[destinationPiece] - MATERIAL_TABLE[originPiece]);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (m.destinationPiece != 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            //return 0;
        }
    }
}
