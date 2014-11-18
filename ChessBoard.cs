/* Representation of a 8x8 chess board
 * Each piece can be numbers 0-12 (enum of Pieces)
 */

namespace ChessAI
{
    class ChessBoard
    {
        public ChessBoard()
        {
            board = new Pieces[8, 8];
        }

        private Pieces[,] board;
    }
}
