﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices; 

namespace ChessAI
{

    class Move
    {
        public int originX;
        public int originY;
        public int destX;
        public int destY;
        public byte destinationPiece;
        public byte originPiece;
        public bool promotion;

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

            sb.Append("x1 " + originX);
            sb.AppendLine();
            sb.Append("y1 "+ originY);
            sb.AppendLine();
            sb.Append("x2 " + destX);
            sb.AppendLine();
            sb.Append("y2 " + destY);
            sb.AppendLine();
            sb.Append("dest " + destinationPiece);
            sb.AppendLine();
            sb.Append("orig " + originPiece);
            sb.AppendLine();
            sb.Append("promote " + promotion);
            return sb.ToString();

        }
    }

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
        private Stack<Move> moves;
        private bool endGame = false;
        private bool blackKingTaken = false;
        private bool whiteKingTaken = false;

        public Board()
        {
            moves = new Stack<Move>();
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

        public Board(byte[,] board, Stack<Move> moves, bool endGame, bool blackKingTaken, bool whiteKingTaken)
        {
            this.board = board;
            this.moves = moves;
            this.endGame = endGame;
            this.blackKingTaken = blackKingTaken;
            this.whiteKingTaken = whiteKingTaken;
        }

        public Board Clone()
        {
            return new Board((byte[,])board.Clone(), new Stack<Move>(), endGame, blackKingTaken, whiteKingTaken);
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
                board[move.originX, move.originY] = move.originPiece;
                board[move.destX, move.destY] = move.destinationPiece;
            }
            else
            {
                if ((move.originPiece - 1) / 6 == 0)
                {
                    board[move.originX, move.originY] = W_PAWN;
                }
                else
                {
                    board[move.originX, move.originY] = B_PAWN;
                }
                board[move.destX, move.destY] = move.destinationPiece;
            }
        }

        public void MakeMove(Move move)
        {
            moves.Push(move);
            if(move.destinationPiece == W_KING)
            {
                this.whiteKingTaken = true;
            }
            else if(move.destinationPiece == B_KING)
            {
                this.blackKingTaken = true;
            }
            // make promotion
            if (!move.promotion)
            {
                board[move.originX, move.originY] = BLANK_PIECE;
                board[move.destX, move.destY] = move.originPiece;
            }
            // make enpassent
            else if (move.originPiece % 6 == W_PAWN && move.originX != move.destX && move.destinationPiece == 0)
            {
                board[move.destX, move.destY] = move.originPiece;
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
            }
        }

        public List<Move> GetAllStates(bool white)
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
                            if ((j < 7) && (board[i, j + 1] == 0 || IsColor(i, j, !white)))
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

        public void sortMoves(List<Move> moves, bool color){
            int[] cache = new int[moves.Count];
            for (int i = 0; i < moves.Count; i++)
            {
                this.MakeMove(moves[i]);
                cache[i] = this.Evaluate(color); // might just be color?
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
            List<Move> boards = GetAllStates(color);
            Console.WriteLine("Moves Available: " + boards.Count);
            int x = rand.Next(boards.Count);
            Board b = this.Clone();
            b.MakeMove(boards[x]);
            move = detectMove(b);
            return b;
        }

        public Board PlayNegaMaxMove(out string move, bool color, int depth)
        {
            Console.WriteLine("suceed");
            List<Move> moves = GetAllStates(color);
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
                    int score = -Negamax.negaMax(this, depth - 1, -beta, -alpha, !color, depth);
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

        public Board PlayNegaMaxMoveMultiThreaded(out string move, bool color, int depth)
        {
            List<Move> moves = GetAllStates(color);
            Console.WriteLine("Moves Available: " + moves.Count);
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
        public int Evaluate(bool color)
        {
            const int pawnVal = 150;
            const int knightVal = 320;
            const int bishopVal = 325;
            const int rookVal = 500;
            const int queenVal = 970;
            const int kingVal = 3000;

            int blackScore = 0;
            int whiteScore = 0;
            short bBishops = 0;
            short wBishops = 0;
            short knights = 0; // TODO use for handling end game
            int totalPieces = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i,j] != BLANK_PIECE)
                    {
                        int scoreToAdd = 0;
                        int bTableScoreToAdd = 0;
                        int wTableScoreToAdd = 0;
                        int pieceToEval = board[i, j] % 6;
                        int tablePosition = j * 8 + i;
                        ++totalPieces;
                        bool isWhitePiece = IsColor(i, j, true);
                        if (pieceToEval == W_PAWN)
                        {
                            scoreToAdd = pawnVal;
                            if (i == 0 || i == 7) // Rook Pawns
                            {
                                scoreToAdd -= 15;
                            }
                            if (isWhitePiece)
                            {
                                if (j == 2) // Whites in 3rd row are bonus
                                {
                                    scoreToAdd += 25;
                                }
                                wTableScoreToAdd = color ? PieceTables.Pawn[63 - tablePosition] : PieceTables.Pawn[tablePosition];
                            }
                            else
                            {
                                if (j == 5) // Blacks in 6th row are bonus
                                {
                                    scoreToAdd += 25;
                                }
                                bTableScoreToAdd = color ? PieceTables.Pawn[63 - tablePosition] : PieceTables.Pawn[tablePosition];
                            }
                        }
                        else if (pieceToEval == W_KNIGHT)
                        {
                            scoreToAdd = knightVal;
                            ++knights;
                            if (isWhitePiece)
                            {
                                wTableScoreToAdd = color ? PieceTables.Knight[63 - tablePosition] : PieceTables.Knight[tablePosition];
                            }
                            else
                            {
                                bTableScoreToAdd = color ? PieceTables.Knight[63 - tablePosition] : PieceTables.Knight[tablePosition];
                            }
                        }
                        else if (pieceToEval == W_ROOK)
                        {
                            scoreToAdd = rookVal;
                        }
                        else if (pieceToEval == W_BISHOP)
                        {
                            scoreToAdd = bishopVal;
                            if (isWhitePiece)
                            {
                                wTableScoreToAdd = color ? PieceTables.Bishop[63 - tablePosition] : PieceTables.Bishop[tablePosition];
                                ++wBishops;
                            }
                            else
                            {
                                bTableScoreToAdd = color ? PieceTables.Bishop[63 - tablePosition] : PieceTables.Bishop[tablePosition];
                                ++bBishops;
                            }
                        }
                        else if (pieceToEval == W_QUEEN)
                        {
                            scoreToAdd = queenVal;
                            if(!endGame)
                            {
                                scoreToAdd -= 10;
                            }
                        }
                        else if (pieceToEval == 0 && board[i, j] != 0) // King
                        {
                            scoreToAdd = kingVal;
                            if (isWhitePiece)
                            {
                                if (endGame)
                                {
                                    wTableScoreToAdd = color ? PieceTables.KingEndGame[63 - tablePosition] : PieceTables.KingEndGame[tablePosition];
                                }
                                else
                                {
                                    wTableScoreToAdd = color ? PieceTables.King[63 - tablePosition] : PieceTables.King[tablePosition];
                                }
                            }
                            else
                            {
                                if (endGame)
                                {
                                    bTableScoreToAdd = color ? PieceTables.KingEndGame[63 - tablePosition] : PieceTables.KingEndGame[tablePosition];
                                }
                                else
                                {
                                    bTableScoreToAdd = color ? PieceTables.King[63 - tablePosition] : PieceTables.King[tablePosition];
                                }
                            }
                        }

                        if (isWhitePiece)
                        {
                            whiteScore += scoreToAdd + wTableScoreToAdd;
                            blackScore += bTableScoreToAdd;
                        }
                        else
                        {
                            blackScore += scoreToAdd + bTableScoreToAdd;
                            whiteScore += wTableScoreToAdd;
                        }
                    }
                }
            }

            if(wBishops >= 2)
            {
                whiteScore += 20;
            }
            if(bBishops >= 2)
            {
                blackScore += 20;
            }

            // Attack boost
            Move lastMove = moves.Peek();
            if (lastMove.destinationPiece != 0 && !lastMove.promotion)
            {
                if (lastMove.destinationPiece >= 1 && lastMove.destinationPiece <= 6)
                {
                    // black took white
                    if (lastMove.originPiece % 6 < lastMove.destinationPiece % 6)
                    {
                        blackScore += 20; // Lesser piece took bigger piece
                    }
                    else
                    {
                        blackScore += 5;
                    }
                }
                else // white took black
                {
                    if (lastMove.originPiece % 6 < lastMove.destinationPiece % 6)
                    {
                        whiteScore += 20; // Lesser piece took bigger piece
                    }
                    else
                    {
                        whiteScore += 5;
                    }
                }
            }

            if(totalPieces < 10)
            {
                this.endGame = true;
            }
            else 
            { 
                this.endGame = false;
            }

            if (color)
            {
                return whiteScore - blackScore;
            }
            else
            {
                return blackScore - whiteScore;
            }
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
