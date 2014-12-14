using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ChessAI
{
    class Zobrist
    {
        public static long[, , ,] TABLE;
        public static long SIDE;

        public static void InitTable()
        {
            TABLE = new long[6, 2, 8, 8];
            byte[] bytes;
            var rng = new RNGCryptoServiceProvider();
            for (int x = 0; x < 6; ++x)
            {
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 0; j < 8; ++j)
                    {
                        bytes = new byte[8];
                        rng = new RNGCryptoServiceProvider();
                        rng.GetBytes(bytes);
                        TABLE[x, 0, i, j] = BitConverter.ToInt64(bytes, 0);
                        bytes = new byte[8];
                        rng = new RNGCryptoServiceProvider();
                        rng.GetBytes(bytes);
                        TABLE[x, 1, i, j] = BitConverter.ToInt64(bytes, 0);
                    }
                }
            }

            bytes = new byte[8];
            rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            SIDE = BitConverter.ToInt64(bytes, 0);
        }

        public static long GetKey(byte[,] board, bool color)
        {
            long zobristKey = 0;
            for(int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    byte pieceToEval = board[i, j];
                    if(pieceToEval != 0)
                    {

                        bool isWhitePiece = (pieceToEval - 1) / 6 == 0;
                        if (isWhitePiece)
                        {
                            zobristKey ^= Zobrist.TABLE[pieceToEval - 1, 0, i, j];
                        }
                        else
                        {
                            zobristKey ^= Zobrist.TABLE[(pieceToEval-1)%6, 1, i, j];
                        }
                    }
                }
            }
            // if side to move
            if(!color)
            {
                zobristKey ^= Zobrist.SIDE;
            }
            return zobristKey;
        }

    }
}
