using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; 

namespace ChessAI
{
    class Move : IComparable<Move>
    {
        public int originX;
        public int originY;
        public int destX;
        public int destY;
        public byte destinationPiece;
        public byte originPiece;
        public bool promotion;

        public static readonly int[] MATERIAL_TABLE = new int[13] { 0, 100, 525, 350, 350, 1000, 10000, 100, 525, 350, 350, 1000, 10000 };

        // Regular move
        // TODO: Special moves aren't considered. Saving only destination piece as opposed to any possible "jumps" like enpassant?
        public Move(int x1, int y1, int x2, int y2, byte[,] board)
        {
            originX = x1;
            originY = y1;
            destX = x2;
            destY = y2;
            originPiece = board[x1, y1];
            destinationPiece = board[x2, y2];
            promotion = false;
        }
        
        // Promotion
        // TODO: Can there be a move where we remove an enemy piece AND promote? Need to save it.
        public Move(int x1, int y1, int x2, int y2, byte[,] board, byte promote)
        {
            originX = x1;
            originY = y1;
            destX = x2;
            destY = y2;
            originPiece = promote;
            destinationPiece = board[x2,y2];
            promotion = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append("x1 " + originX);
            //sb.AppendLine();
            //sb.Append("y1 "+ originY);
            //sb.AppendLine();
            //sb.Append("x2 " + destX);
            //sb.AppendLine();
            //sb.Append("y2 " + destY);
            //sb.AppendLine();
            //sb.Append("dest " + destinationPiece);
            //sb.AppendLine();
            //sb.Append("orig " + originPiece);
            //sb.AppendLine();
            //sb.Append("promote " + promotion);
            sb.Append(originPiece + " ");
            sb.Append(originX);
            sb.Append(originY);
            sb.Append(destX);
            sb.Append(destY);
            sb.Append(" " + destinationPiece);
            if (promotion)
            {
                sb.Append("Q");
            }
            return sb.ToString();

        }

        public bool Equals(Move move)
        {
            return originPiece == move.originPiece && destinationPiece == move.destinationPiece && originX == move.originX && originY == move.originY && destX == move.destX && destY == move.destY;
        }

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

    class Board
    {
        static int ENDGAME = 14;
        private static readonly byte BLANK_PIECE = 0;
        private static readonly byte W_PAWN = 1;
        private static readonly byte W_ROOK = 2;
        private static readonly byte W_KNIGHT = 3;
        private static readonly byte W_BISHOP = 4;
        private static readonly byte W_QUEEN = 5;
        public static readonly byte W_KING = 6;
        private static readonly byte B_PAWN = 7;
        private static readonly byte B_ROOK = 8;
        private static readonly byte B_KNIGHT = 9;
        private static readonly byte B_BISHOP = 10;
        private static readonly byte B_QUEEN = 11;
        public static readonly byte B_KING = 12;

        private static readonly bool WHITE = true;
        private static readonly bool BLACK = false;

        private byte[,] board;
        private byte[] pieceCount;
        private Stack<Move> moves;
        private bool endGame = false;
        private bool blackKingTaken = false;
        private bool whiteKingTaken = false;
        private byte pieces = 32;
        /**
         * Material Values
         * Pawn -  100
         * Bishop - 350
         * Knight - 350
         * Rook - 525
         * Queen - 1000
         * King - 10000
         */
        public static readonly int[] OFFSET_TABLE = new int[13]{ 0, 0, -350, -100, -100, -400, 0, 0, -350, -100, -100, -400, 0 };

        public Board()
        {
            moves = new Stack<Move>();
            board = new byte[8, 8];
            pieceCount = new byte[12];
            for (int i = 0; i < 8; i++) {
                board[i, 1] = W_PAWN;
                board[i, 6] = B_PAWN;
            }

            pieceCount[W_PAWN] = 8;
            pieceCount[W_BISHOP] = 2;
            pieceCount[W_KNIGHT] = 2;
            pieceCount[W_QUEEN] = 1;
            pieceCount[W_ROOK] = 2;
            board[0, 0] = W_ROOK;
            board[1, 0] = W_KNIGHT;
            board[2, 0] = W_BISHOP;
            board[3, 0] = W_QUEEN;
            board[4, 0] = W_KING;
            board[5, 0] = W_BISHOP;
            board[6, 0] = W_KNIGHT;
            board[7, 0] = W_ROOK;

            pieceCount[B_PAWN] = 8;
            pieceCount[B_BISHOP] = 2;
            pieceCount[B_KNIGHT] = 2;
            pieceCount[B_QUEEN] = 1;
            pieceCount[B_ROOK] = 2;
            board[0, 7] = B_ROOK;
            board[1, 7] = B_KNIGHT;
            board[2, 7] = B_BISHOP;
            board[3, 7] = B_QUEEN;
            board[4, 7] = B_KING;
            board[5, 7] = B_BISHOP;
            board[6, 7] = B_KNIGHT;
            board[7, 7] = B_ROOK;
        }

        public Board(byte[,] board, byte[] pieceCount, Stack<Move> moves, byte pieces, bool endGame, bool blackKingTaken, bool whiteKingTaken)
        {
            this.board = board;
            this.pieceCount = pieceCount;
            this.moves = moves;
            this.endGame = endGame;
            this.blackKingTaken = blackKingTaken;
            this.whiteKingTaken = whiteKingTaken;
            this.pieces = pieces;
        }

        public Board Clone()
        {
            Stack<Move> moveStack = new Stack<Move>();
            foreach(Move m in moves.Reverse()){
                moveStack.Push(m);
            }
            return new Board((byte[,])board.Clone(), (byte[])pieceCount.Clone(), 
                moveStack, pieces, endGame, blackKingTaken, whiteKingTaken);
        }

        public List<Move> GetAllCaptureStates(bool color)
        {
            List<Move> allMoves = this.GetAllStates(color, false);
            List<Move> capMoves = new List<Move>();
            foreach (Move move in allMoves)
            {
                if (move.destinationPiece != 0)
                {
                    capMoves.Add(move);
                }
            }
            return capMoves;
        }

//         public void MovePiece(int x1, int y1, int x2, int y2)
//         {
//             if (board[x1, y1] % 6 == 0 && Math.Abs(x2 - x1) == 2)
//             {
//                 if (x2 < x1)
//                 {
//                     board[x2 + 1, y1] = board[0, y1];
//                     board[0, y1] = BLANK_PIECE;
//                 }
//                 else
//                 {
//                     board[x2 - 1, y1] = board[7, y1];
//                     board[7, y1] = BLANK_PIECE;
//                 }
//             }
//             else if (board[x1, y1] % 6 == W_PAWN && x1 != x2 && board[x2, y2] == BLANK_PIECE)
//             {
//                 board[x2, y1] = BLANK_PIECE;
//             }
//             board[x2, y2] = board[x1, y1];
//             board[x1, y1] = BLANK_PIECE;
//         }

        public Move CreateMove(int x1, int y1, int x2, int y2)
        {
            // TODO: advanced moves from the old function?
            return new Move(x1, y1, x2, y2, board);
        }

//         public void MovePiece(int x1, int y1, int x2, int y2, byte promote)
//         { 
//             board[x2, y2] = promote;
//             board[x1, y1] = BLANK_PIECE;
//         }

        public Move CreateMove(int x1, int y1, int x2, int y2, byte promote)
        {
            return new Move(x1, y1, x2, y2, board, promote);
        }

        public void UndoMove()
        {
            Move move = moves.Pop();
            if(move.destinationPiece == W_KING)
            {
                this.whiteKingTaken = false;
            }
            else if(move.destinationPiece == B_KING)
            {
                this.blackKingTaken = false;
            }

            if(!move.promotion)
            {
                if (move.destinationPiece != BLANK_PIECE)
                {
                    ++pieces;
                    if (move.destinationPiece != B_KING && move.destinationPiece != W_KING)
                    {
                        ++pieceCount[move.destinationPiece];
                    }
                    if (pieces > ENDGAME)
                    {
                        endGame = false;
                    }
                }
                board[move.originX, move.originY] = move.originPiece;
                board[move.destX, move.destY] = move.destinationPiece;
            }
            else
            {
                if ((move.originPiece - 1) / 6 == 0)
                {
                    board[move.originX, move.originY] = W_PAWN;
                    ++pieceCount[W_PAWN];
                    --pieceCount[W_QUEEN];
                }
                else
                {
                    board[move.originX, move.originY] = B_PAWN;
                    ++pieceCount[B_PAWN];
                    --pieceCount[B_QUEEN];
                }
                if(move.destinationPiece != BLANK_PIECE && move.destinationPiece != B_KING && move.destinationPiece != W_KING)
                {
                    ++pieceCount[move.destinationPiece];
                    ++pieces;
                    if(pieces > ENDGAME)
                    {
                        endGame = false;
                    }
                }
                board[move.destX, move.destY] = move.destinationPiece;
            }
        }

        public void MakeMove(Move move)
        {
            moves.Push(move);
            if(move.destinationPiece == W_KING)
            {
                whiteKingTaken = true;
            }
            else if(move.destinationPiece == B_KING)
            {
                blackKingTaken = true;
            }
            // make promotion
            if (move.promotion)
            {
                board[move.originX, move.originY] = BLANK_PIECE;
                board[move.destX, move.destY] = move.originPiece;
                
                // Increment queen count, decrement pawn count, decrement destination if capture
                ++pieceCount[move.originPiece];
                if ((move.originPiece - 1) / 6 == 0)
                {
                    --pieceCount[W_PAWN];
                }
                else
                {
                    --pieceCount[B_PAWN];
                }
                if (move.destinationPiece != BLANK_PIECE)
                {
                    --pieces;
                    if(move.destinationPiece != B_KING && move.destinationPiece != W_KING)
                    {
                        --pieceCount[move.destinationPiece];
                    }
                    if (pieces <= ENDGAME)
                    {
                        endGame = true;
                    }
                }
            }
            // make enpassent
            else if (move.originPiece % 6 == W_PAWN && move.originX != move.destX && move.destinationPiece == 0)
            {
                board[move.destX, move.destY] = move.originPiece;
                board[move.originX, move.destY] = BLANK_PIECE;
                --pieces;
                if (pieces <= ENDGAME)
                {
                    this.endGame = true;
                }
                board[move.destX, move.originY] = BLANK_PIECE;
            }
            // make castle
            else if(move.originPiece % 6 == 0 && move.destX - move.originX == 2)
            {
                board[move.originX, move.originY] = BLANK_PIECE;
                board[move.destX, move.destY] = move.originPiece;
                int y = move.originY;
                int x = 0;
                int x2 = move.destX + 1;
                if (move.destX > move.originX)
                {
                    x = 7;
                    x2 = move.destX - 1;
                }
                board[x2, y] = board[x, y];
                board[x, y] = BLANK_PIECE;

            }
            else
            {
                // make regular move
                board[move.originX, move.originY] = BLANK_PIECE;
                board[move.destX, move.destY] = move.originPiece;
                if(move.destinationPiece != BLANK_PIECE) //decrement pieces count if capture
                {
                    --pieces;
                    if(pieces <= ENDGAME)
                    {
                        endGame = true;
                    }
                    if(move.destinationPiece != W_KING && move.destinationPiece != B_KING)
                    {
                        --pieceCount[move.destinationPiece];
                    }
                }
            }
        }

        public List<Move> GetAllStates(bool white, bool first)
        {
            List<Move> moves = new List<Move>();
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
                                    if (j == 6) 
                                    {
                                        moves.Add(CreateMove(i, j, i, j + 1, W_QUEEN));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i, j + 1));
                                    }
                                }
                                if (j == 1 && board[i,j+1] == 0 && board[i, j + 2] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, i, j + 2));
                                }
                                if ((i < 7) && board[i+1,j+1] != 0 && (IsColor(i + 1, j + 1, !white)))
                                {
                                    
                                    if (j == 6)
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j + 1, W_QUEEN));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j + 1));
                                    }
                                }
                                if ((i > 0) && board[i-1,j+1] != 0 && (IsColor(i - 1, j + 1, !white)))
                                {
                                    
                                    if (j == 6)
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j + 1, W_QUEEN));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j + 1));
                                    }
                                }
                            }
                            else
                            {
                                if (board[i, j - 1] == 0)
                                {
                                    
                                    if (j == 1)
                                    {
                                        moves.Add(CreateMove(i, j, i, j - 1, B_QUEEN));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i, j - 1));
                                    }
                                }
                                if (j == 6 && board[i,j-1] == 0 && board[i, j - 2] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, i, j - 2));
                                }
                                if ((i < 7) && board[i+1,j-1] != 0 && (IsColor(i + 1, j - 1, !white)))
                                {
                                    
                                    if (j == 1)
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j - 1, B_QUEEN));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j - 1));
                                    }
                                }
                                if ((i > 0) && board[i-1,j-1] !=0 && (IsColor(i - 1, j - 1, !white)))
                                {
                                    
                                    if (j == 1)
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j - 1, B_QUEEN));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j - 1));
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
                                    
                                    moves.Add(CreateMove(i, j, x, j));
                                }
                                else
                                {
                                    if (IsColor(x, j, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, j));
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1; x >= 0; x--)
                            {
                                if (board[x, j] == 0)
                                {
                                    moves.Add(CreateMove(i, j, x, j));
                                }
                                else
                                {
                                    if (IsColor(x, j, !white))
                                    {
                                        moves.Add(CreateMove(i, j, x, j));
                                    }
                                    break;
                                }
                            }
                            for (int y = j + 1; y < 8; y++)
                            {
                                if (board[i, y] == 0)
                                {
                                    moves.Add(CreateMove(i, j, i, y));
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        moves.Add(CreateMove(i, j, i, y));
                                    }
                                    break;
                                }
                            }
                            for (int y = j - 1; y >= 0; y--)
                            {
                                if (board[i, y] == 0)
                                {   
                                    moves.Add(CreateMove(i, j, i, y));
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        moves.Add(CreateMove(i, j, i, y));
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
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j - 1; x >= 0 && y >= 0; x--, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i + 1, y = j - 1; x < 8 && y >= 0; x++, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j + 1; x >= 0 && y < 8; x--, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
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
                                    
                                    moves.Add(CreateMove(i, j, x, j));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, j, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, j));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1; x >= 0; x--)
                            {
                                if (board[x, j] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, j));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, j, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, j));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int y = j + 1; y < 8; y++)
                            {
                                if (board[i, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, i, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, i, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int y = j - 1; y >= 0; y--)
                            {
                                if (board[i, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, i, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(i, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, i, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i + 1, y = j + 1; x < 8 && y < 8; x++, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j - 1; x >= 0 && y >= 0; x--, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i + 1, y = j - 1; x < 8 && y >= 0; x++, y--)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                            for (int x = i - 1, y = j + 1; x >= 0 && y < 8; x--, y++)
                            {
                                if (board[x, y] == 0)
                                {
                                    
                                    moves.Add(CreateMove(i, j, x, y));
                                   
                                }
                                else
                                {
                                    if (IsColor(x, y, !white))
                                    {
                                        
                                        moves.Add(CreateMove(i, j, x, y));
                                       
                                    }
                                    break;
                                }
                            }
                        }
                        else if (board[i, j] % 6 == 0)
                        {
                            if ((i < 7) && (board[i + 1, j] == 0 || IsColor(i + 1, j, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 1, j));
                            }
                            if ((i < 7) && (j > 0) && (board[i + 1, j - 1] == 0 || IsColor(i + 1, j - 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 1, j - 1));
                            }
                            if ((i < 7) && (j < 7) && (board[i + 1, j + 1] == 0 || IsColor(i + 1, j + 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 1, j + 1));
                            }
                            if ((j < 7) && (board[i, j + 1] == 0 || IsColor(i, j + 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i, j + 1));
                            }
                            if ((j > 0) && (board[i, j - 1] == 0 || IsColor(i, j - 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i, j - 1));
                            }
                            if ((i > 0) && (board[i - 1, j] == 0 || IsColor(i - 1, j, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 1, j));
                            }
                            if ((i > 0) && (j < 7) && (board[i - 1, j + 1] == 0 || IsColor(i - 1, j + 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 1, j + 1));
                            }
                            if ((i > 0) && (j > 0) && (board[i - 1, j - 1] == 0 || IsColor(i - 1, j - 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 1, j - 1));
                            }
                        }
                        else if (board[i, j] % 6 == W_KNIGHT)
                        {
                            if ((i < 6) && (j < 7) && (board[i + 2, j + 1] == 0 || IsColor(i + 2, j + 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 2, j + 1));
                            }
                            if ((i < 6) && (j > 0) && (board[i + 2, j - 1] == 0 || IsColor(i + 2, j - 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 2, j - 1));
                            }
                            if ((i < 7) && (j < 6) && (board[i + 1, j + 2] == 0 || IsColor(i + 1, j + 2, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 1, j + 2));
                            }
                            if ((i < 7) && (j > 1) && (board[i + 1, j - 2] == 0 || IsColor(i + 1, j - 2, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i + 1, j - 2));
                            }
                            if ((i > 0) && (j < 6) && (board[i - 1, j + 2] == 0 || IsColor(i - 1, j + 2, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 1, j + 2));
                            }
                            if ((i > 0) && (j > 1) && (board[i - 1, j - 2] == 0 || IsColor(i - 1, j - 2, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 1, j - 2));
                            }
                            if ((i > 1) && (j < 7) && (board[i - 2, j + 1] == 0 || IsColor(i - 2, j + 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 2, j + 1));
                            }
                            if ((i > 1) && (j > 0) && (board[i - 2, j - 1] == 0 || IsColor(i - 2, j - 1, !white)))
                            {
                                
                                moves.Add(CreateMove(i, j, i - 2, j - 1));
                            }
                        }
                    }
                }
            }
            return moves;
        }

        public Stack<Move> GetAllMoves()
        {
            return this.moves;
        }

        public bool IsEndGame()
        {
            return this.endGame;
        }
        public bool isCapture()
        {
            return this.moves.Peek().destinationPiece != 0;
        }

        public void sortMoves(List<Move> moves, bool color){
            int[] cache = new int[moves.Count];
            for (int i = 0; i < moves.Count; i++)
            {
                this.MakeMove(moves[i]);
                cache[i] = this.Evaluate(color, 0);// this.PlayNegaMaxMoveVal(!color, 1); // might just be color?
                this.UndoMove();
            }
            for (int i = 0; i < moves.Count; i++)
            {
                int max = -999999999;
                int loc = 0;
                for (int j = i; j < moves.Count; j++)
                {
                    if (cache[j] > max)
                    {
                        loc = j;
                        max = cache[j];
                    }
                }
                Move moveTemp = moves[i];
                int cacheTemp = cache[i];
                moves[i] = moves[loc];
                cache[i] = cache[loc];
                moves[loc] = moveTemp;
                cache[loc] = cacheTemp;
            }
        }

        public Board PlayRandomMove(out string move, bool color)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            List<Move> boards = GetAllStates(color, true);
            Console.WriteLine("Moves Available: " + boards.Count);
            int x = rand.Next(boards.Count);
            Board b = this.Clone();
            b.MakeMove(boards[x]);
            move = detectMove(b);
            return b;
        }

        public bool isTerminal()
        {
            return blackKingTaken || whiteKingTaken;
        }

        public Board PlayNegaMaxMove(out string move, bool color, int depth)
        {
            Console.WriteLine("suceed");
            List<Move> moves = GetAllStates(color, true);
            this.sortMoves(moves, color);
            Console.WriteLine("Moves Available: " + moves.Count);
            System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
            Move moveToMake = null;
            if (moves.Count > 0)
            {
                //int depth = 6;
                int alpha = Negamax.NEGA_SCORE;
                int beta = -Negamax.NEGA_SCORE;
                //while loop here to do multiple depths
                t.Reset();
                t.Start();
                Negamax.pruned = 0;
                for (int i = 0; i < moves.Count; ++i)
                {
                    MakeMove(moves[i]);
                    int score = -Negamax.negaMax(this, depth - 1, -beta, -alpha, !color, moves[i].destinationPiece != 0, 0);
                    Console.WriteLine(score);
                    UndoMove();
                    if (score > alpha)
                    {
                        alpha = score;
                        moveToMake = moves[i];
                        Console.WriteLine("New move:" + score + " @depth:" + depth);
                    }
                    else if (score < alpha)
                    {
                        break;
                    }
                }
                Console.WriteLine("Searched: " + Negamax.pruned);
                Console.WriteLine("time at depth: " + depth +" = " + t.ElapsedMilliseconds);
                //Diagnostics.singleTime += t.ElapsedMilliseconds;
                //++depth; Use while loop to do multiple depths
                t.Stop();
            }


            
            Board b = this.Clone();

            if(moveToMake == null)
            {
                Console.WriteLine("No move to make");
                move = "";
                return b;
            }
            //Console.WriteLine(b.ToString());
            b.MakeMove(moveToMake);
            //Console.WriteLine(moveToMake.ToString());
            //Console.WriteLine(b.ToString());
            move = detectMove(b);
            return b;
        }

        public Move LastMove()
        {
            return this.moves.Peek();
        }

        public int PlayNegaMaxMoveVal(bool color, int depth)
        {
            //Console.WriteLine("suceed");
            List<Move> moves = GetAllStates(color, true);
            //this.sortMoves(moves, color);
            //Console.WriteLine("Moves Available: " + moves.Count);
            //System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
            //Move moveToMake = null;
            if (moves.Count > 0)
            {
                //int depth = 6;
                int alpha = Negamax.NEGA_SCORE;
                int beta = -Negamax.NEGA_SCORE;
                //while loop here to do multiple depths
                //t.Reset();
                //t.Start();
                Negamax.pruned = 0;
                for (int i = 0; i < moves.Count; ++i)
                {
                    MakeMove(moves[i]);
                    int score = -Negamax.negaMax(this, depth - 1, -beta, -alpha, !color, moves[i].destinationPiece != 0, 0);
                    //Console.WriteLine(score);
                    UndoMove();
                    if (score > alpha)
                    {
                        alpha = score;
                        //moveToMake = moves[i];
                        //Console.WriteLine("New move:" + score + " @depth:" + depth);
                    }
                    else if (score < alpha)
                    {
                        break;
                    }
                }
                //Console.WriteLine("Searched: " + Negamax.pruned);
                //Console.WriteLine("time at depth: " + depth + " = " + t.ElapsedMilliseconds);
                ////Diagnostics.singleTime += t.ElapsedMilliseconds;
                ////++depth; Use while loop to do multiple depths
                //t.Stop();
                return alpha;
            }
            return this.Evaluate(color, 0);


            //Board b = this.Clone();

            //if (moveToMake == null)
            //{
            //    Console.WriteLine("No move to make");
            //    move = "";
            //    return b;
            //}
            ////Console.WriteLine(b.ToString());
            //b.MakeMove(moveToMake);
            ////Console.WriteLine(moveToMake.ToString());
            ////Console.WriteLine(b.ToString());
            //move = detectMove(b);
            //return b;
        }

        public Board PlayNegaMaxMoveMultiThreaded(out string move, bool color, int depth)
        {
            //List<Move> moves = GetAllStates(color);
            //Console.WriteLine("Moves Available: " + moves.Count);
            System.Diagnostics.Stopwatch t = new System.Diagnostics.Stopwatch();
            t.Reset();
            t.Start();
            NegaMaxMasterThread negaThread = new NegaMaxMasterThread(this, color, depth);
            Move moveToMake = negaThread.Run();
            t.Stop();
            Console.WriteLine("MultiThread Time: " + t.ElapsedMilliseconds);
            //Diagnostics.multiTime += t.ElapsedMilliseconds;
            Board b = this.Clone();

            if (moveToMake == null)
            {
                Console.WriteLine("No move to make");
                move = "";
                return b;
            }
            //Console.WriteLine(b.ToString());
            b.MakeMove(moveToMake);
            //Console.WriteLine(moveToMake.ToString());
            //Console.WriteLine(b.ToString());
            move = detectMove(b);
            return b;
        }

        public bool Equals(Board obj)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != obj.board[i, j])
                    {
                        return false;
                    }
                }
            }
                return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    sb.Append(" " + board[j,i]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
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

        // TODO: Speed this fucker up. He's the 80%
        public int Evaluate(bool color, int offset)
        {
            const int pawnVal = 100;
            const int knightVal = 350;
            const int bishopVal = 350;
            const int rookVal = 525;
            const int queenVal = 1000;
            const int kingVal = 10000;

            int blackScore = 0;
            int whiteScore = 0;
            byte[] whitePawnFiles = new byte[8];
            byte[] blackPawnFiles = new byte[8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != BLANK_PIECE)
                    {
                        int scoreToAdd = 0;
                        int bTableScoreToAdd = 0;
                        int wTableScoreToAdd = 0;
                        int pieceToEval = board[i, j] % 6;
                        int tablePosition = j * 8 + i;
                        bool isWhitePiece = IsColor(i, j, true);
                        if(isWhitePiece)
                        {
                            tablePosition = 63 - tablePosition;
                        }
                        if (pieceToEval == W_PAWN)
                        {
                            scoreToAdd = pawnVal;
                            if (i == 0 || i == 7) // Rook Pawns
                            {
                                scoreToAdd -= 15;
                            }
                            if (isWhitePiece)
                            {
                                ++whitePawnFiles[i];
                                wTableScoreToAdd = PieceTables.Pawn[tablePosition];
                            }
                            else
                            {
                                ++blackPawnFiles[i];
                                bTableScoreToAdd = PieceTables.Pawn[tablePosition];
                            }
                        }
                        else if (pieceToEval == W_KNIGHT)
                        {
                            scoreToAdd = knightVal;
                            if (endGame)
                            {
                                scoreToAdd -= 10;
                            }
                            if (isWhitePiece)
                            {
                                wTableScoreToAdd = PieceTables.Knight[tablePosition];
                            }
                            else
                            {
                                bTableScoreToAdd = PieceTables.Knight[tablePosition];
                            }
                        }
                        else if (pieceToEval == W_ROOK)
                        {
                            scoreToAdd = rookVal;
                        }
                        else if (pieceToEval == W_BISHOP)
                        {
                            scoreToAdd = bishopVal;
                            if(endGame)
                            {
                                scoreToAdd += 10;
                            }
                            if (isWhitePiece)
                            {
                                wTableScoreToAdd = PieceTables.Bishop[tablePosition];
                            }
                            else
                            {
                                bTableScoreToAdd = PieceTables.Bishop[tablePosition];
                            }
                        }
                        else if (pieceToEval == W_QUEEN)
                        {
                            scoreToAdd = queenVal;
                            if (!endGame && ((j != 0 || j != 8) || i != 3) )
                            {
                                scoreToAdd -= 10;
                            }
                        }
                        else if (pieceToEval == 0 && board[i, j] != 0) // King
                        {
                            scoreToAdd = kingVal;
                            byte kingMoves = 0;
                            bool moveUp = j + 1 < 8;
                            bool moveDown = j - 1 >= 0;
                            if(moveUp)
                            {
                                if(board[i, j+1] == BLANK_PIECE)
                                {
                                    ++kingMoves;
                                }
                            }
                            if(moveDown)
                            {
                                if (board[i, j - 1] == BLANK_PIECE)
                                {
                                    ++kingMoves;
                                }
                            }
                            if(i+1 < 8)
                            {
                                if (board[i + 1, j] == BLANK_PIECE)
                                {
                                    ++kingMoves;
                                }
                                if(moveUp)
                                {
                                    if(board[i+1,j+1] == BLANK_PIECE)
                                    {
                                        ++kingMoves;
                                    }
                                }
                                if(moveDown)
                                {
                                    if(board[i+1,j-1] == BLANK_PIECE)
                                    {
                                        ++kingMoves;
                                    }
                                }
                            }
                            if(i-1 >= 0)
                            {
                                if (board[i - 1, j] == BLANK_PIECE)
                                {
                                    ++kingMoves;
                                }
                                if (moveUp)
                                {
                                    if (board[i - 1, j + 1] == BLANK_PIECE)
                                    {
                                        ++kingMoves;
                                    }
                                }
                                if (moveDown)
                                {
                                    if (board[i - 1, j - 1] == BLANK_PIECE)
                                    {
                                        ++kingMoves;
                                    }
                                }
                            }
                            if(kingMoves < 2)
                            {
                                scoreToAdd -= 5;
                            }
                            if (isWhitePiece)
                            { 
//                                 if(CheckForKingCheck(i, j, true))
//                                 {
//                                     whiteScore -= 100;
//                                 }
                                if (endGame)
                                {
                                    wTableScoreToAdd = PieceTables.KingEndGame[tablePosition];
                                }
                                else
                                {
                                    wTableScoreToAdd = PieceTables.King[tablePosition];
                                }
                            }
                            else
                            {
//                                 if(CheckForKingCheck(i, j, false))
//                                 {
//                                     blackScore -= 100;
//                                 }
                                if (endGame)
                                {
                                    bTableScoreToAdd = PieceTables.KingEndGame[tablePosition];
                                }
                                else
                                {
                                    bTableScoreToAdd = PieceTables.King[tablePosition];
                                }
                            }
                        }

                        if (isWhitePiece)
                        {
                            whiteScore += scoreToAdd + wTableScoreToAdd;
                        }
                        else
                        {
                            blackScore += scoreToAdd + bTableScoreToAdd;
                        }
                    }
                }
            }

            if(pieceCount[W_ROOK] == 0 && pieceCount[W_QUEEN] == 0)
            {
                whiteScore -= 500;
            }
            if(pieceCount[B_ROOK] == 0 && pieceCount[B_QUEEN] == 0)
            {
                blackScore -= 500;
            }

            if(endGame)
            {
                if(pieceCount[W_ROOK] >= 1)
                {
                    whiteScore += 15;
                }
                if(pieceCount[B_ROOK] >= 1)
                {
                    blackScore += 15;
                }
            }

            if (pieceCount[W_BISHOP] >= 2)
            {
                if (!endGame)
                {
                    whiteScore += 20;
                }
            }
            if (pieceCount[B_BISHOP] >= 2)
            {
                if(!endGame)
                {
                    blackScore += 20;
                }
            }

            //Pawn bonuses
            for (int x = 0; x < 8; ++x )
            {
                byte blackPawnsInFile = blackPawnFiles[x];
                byte whitePawnsInFile = whitePawnFiles[x];
                if (blackPawnsInFile >= 1 && whitePawnsInFile == 0)
                {
                    whiteScore -= blackPawnsInFile;
                }
                if (blackPawnsInFile >= 1 && whitePawnsInFile == 0)
                {
                    blackScore -= whitePawnsInFile;
                }
                // Isolated Pawns
                if(x == 0)
                {
                    if (blackPawnsInFile >= 1 && blackPawnFiles[x + 1] == 0)
                    {
                        blackScore -= 12;
                    }
                    if (whitePawnsInFile >= 1 && whitePawnFiles[x + 1] == 0)
                    {
                        whiteScore -= 12;
                    }
                }
                else if(x == 7)
                {
                    if (blackPawnsInFile >= 1 && blackPawnFiles[x - 1] == 0)
                    {
                        blackScore -= 12;
                    }
                    if (blackPawnsInFile >= 1 && whitePawnFiles[x - 1] == 0)
                    {
                        whiteScore -= 12;
                    }
                }
                else
                {
                    if (blackPawnsInFile >= 1 && blackPawnFiles[x - 1] == 0 && blackPawnFiles[x + 1] == 0)
                    {
                        blackScore -= 15;
                    }
                    if (whitePawnsInFile >= 1 && whitePawnFiles[x - 1] == 0 && whitePawnFiles[x + 1] == 0)
                    {
                        whiteScore -= 15;
                    }
                }
            }
                
            

            if (color)
            {
                return (whiteScore - blackScore) + offset;
            }
            else
            {
                return (blackScore - whiteScore) + offset;
            }
        }

        public bool CheckForKingCheck(bool color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(board[i,j] == W_KING && color)
                    {
                        return CheckForKingCheck(i, j, color);
                    }
                    else if(board[i,j] == B_KING && !color)
                    {
                        return CheckForKingCheck(i, j, color);
                    }
                }
            }

            return false;
        }

        public bool CheckForKingCheck(int x, int y, bool color)
        {
            int i = x;
            for (; i < 8; ++i) // CHECKING VERTICLE
            {
                if (board[i, y] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, y] == B_KING || board[i, y] == B_ROOK || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, y] == W_KING || board[i, y] == W_ROOK || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x;
            for (; i >= 0; --i)
            {
                if (board[i, y] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, y] == B_KING || board[i, y] == B_ROOK || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, y] == W_KING || board[i, y] == W_ROOK || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            int j = y;
            for (; j < 8; ++j)
            {
                if (board[x, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[x, j] == B_KING || board[x, y] == B_ROOK || board[x, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[x, j] == W_KING || board[x, y] == W_ROOK || board[x, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            j = y;
            for (; j >= 0; --j)
            {
                if (board[x, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[x, j] == B_KING || board[x, y] == B_ROOK || board[x, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[x, j] == W_KING || board[x, y] == W_ROOK || board[x, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }

            // CHECKING DIAGONAL
            i = x;
            j = y;
            for (; j < 8 && i < 8; ++j, ++i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_KING || board[i, y] == B_BISHOP || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_KING || board[i, y] == W_BISHOP || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x;
            j = y;
            for (; j < 8 && i >= 0; ++j, --i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_KING || board[i, y] == B_BISHOP || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_KING || board[i, y] == W_BISHOP || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x;
            j = y;
            for (; j >= 0 && i < 8; --j, ++i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_KING || board[i, y] == B_BISHOP || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_KING || board[i, y] == W_BISHOP || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x;
            j = y;
            for (; j >= 0 && i >= 0; --j, --i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_KING || board[i, y] == B_BISHOP || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_KING || board[i, y] == W_BISHOP || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            
            // CHECK FOR KNIGHTS
            if (y - 2 >= 0) //over 2 up 1
            {
                if (x + 1 < 8)
                {
                    if (color && board[x + 1, y - 2] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x + 1, y - 2] == W_KNIGHT)
                    {
                        return true;
                    }
                }
                if (x - 1 >= 0)
                {
                    if (color && board[x - 1, y - 2] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x - 1, y - 2] == W_KNIGHT)
                    {
                        return true;
                    }
                }
            }
            if (y + 2 < 8) //over 2 up 1
            {
                if (x + 1 < 8)
                {
                    if (color && board[x + 1, y + 2] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x + 1, y + 2] == W_KNIGHT)
                    {
                        return true;
                    }
                }
                if (x - 1 >= 0)
                {
                    if (color && board[x - 1, y + 2] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x - 1, y + 2] == W_KNIGHT)
                    {
                        return true;
                    }
                }
            }

            if (x + 2 < 8) //up 2 over 1
            {
                if (y + 1 < 8)
                {
                    if (color && board[x + 2, y + 1] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x + 2, y + 1] == W_KNIGHT)
                    {
                        return true;
                    }
                }
                if (y - 1 >= 0)
                {
                    if (color && board[x + 2, y - 1] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x + 2, y - 1] == W_KNIGHT)
                    {
                        return true;
                    }
                }
            }
            if (x - 2 >= 0)
            {
                if (y + 1 < 8)
                {
                    if (color && board[x - 2, y + 1] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x - 2, y + 1] == W_KING)
                    {
                        return true;
                    }
                }
                if (y - 1 >= 0)
                {
                    if (color && board[x - 2, y - 1] == B_KNIGHT)
                    {
                        return true;
                    }
                    else if (!color && board[x - 2, y - 1] == W_KNIGHT)
                    {
                        return true;
                    }
                }
            }
            
            // PAWN CHECK
            if (x + 1 < 8)
            {
                if (y + 1 < 8)
                {
                    if (color && board[x + 1, y + 1] == B_PAWN)
                    {
                        return true;
                    }
                    else if (!color && board[x + 1, y + 1] == W_PAWN)
                    {
                        return true;
                    }
                }
                if (y - 1 >= 0)
                {
                    if (color && board[x + 1, y - 1] == B_PAWN)
                    {
                        return true;
                    }
                    else if (!color && board[x + 1, y - 1] == W_PAWN)
                    {
                        return true;
                    }
                }
            }
            if (x - 1 >= 0)
            {
                if (y + 1 < 8)
                {
                    if (color && board[x - 1, y + 1] == B_PAWN)
                    {
                        return true;
                    }
                    else if (!color && board[x - 1, y + 1] == W_PAWN)
                    {
                        return true;
                    }
                }
                if (y - 1 >= 0)
                {
                    if (color && board[x - 1, y - 1] == B_PAWN)
                    {
                        return true;
                    }
                    else if (!color && board[x - 1, y - 1] == W_PAWN)
                    {
                        return true;
                    }
                }
            }

            return false;
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
