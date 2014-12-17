using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices; 

namespace ChessAI
{
    /// <summary>
    /// Representation of a chess board.
    /// Holds the bytes of game piece representations, evaluation, move generation, and various flag functions.
    /// White is typically true, black is false
    /// </summary>
    class Board
    {
        public static readonly int[] OFFSET_TABLE = new int[13] { 0, 0, -350, -100, -100, -400, 0, 0, -350, -100, -100, -400, 0 };

        // SETTINGS
        private static readonly byte MIDGAMEPIECES = 19;
        private static readonly byte ENDGAMEPIECES = 14;
        private static readonly byte LATEENDGAMEPIECES = 9;

        // Game state flags
        private static readonly byte BEG_GAME = 0;
        private static readonly byte MID_GAME = 1;
        private static readonly byte END_GAME = 2;
        private static readonly byte LATE_END_GAME = 3;

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

        private Stack<Move> moves; // moves made to the current board
        private byte[,] board;
        private byte[] pieceCount; // specific pieces on the board except kings. Value - 1 % 6 for black
        private int whiteKing;
        private int blackKing;
        private int rightWhiteRook;
        private int leftWhiteRook;
        private int rightBlackRook;
        private int leftBlackRook;
        private bool blackKingTaken = false;
        private bool whiteKingTaken = false;
        private byte gameState = BEG_GAME;
        private byte pieces = 32;

        /// <summary>
        /// Sets up starting chess board
        /// </summary>
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

            whiteKing = 0;
            blackKing = 0;
            rightWhiteRook = 0;
            rightBlackRook = 0;
            leftBlackRook = 0;
            rightWhiteRook = 0;
        }

        /// <summary>
        ///  Copy a board constructor
        /// </summary>
        public Board(byte[,] board, byte[] pieceCount, Stack<Move> moves, byte pieces, byte gameState, bool blackKingTaken, bool whiteKingTaken, int whiteKing,
                    int blackKing, int rightWhiteRook, int rightBlackRook, int leftWhiteRook, int leftBlackRook)
        {
            this.board = board;
            this.pieceCount = pieceCount;
            this.moves = moves;
            this.gameState = gameState;
            this.blackKingTaken = blackKingTaken;
            this.whiteKingTaken = whiteKingTaken;
            this.pieces = pieces;
            this.whiteKing = whiteKing;
            this.blackKing = blackKing;
            this.rightWhiteRook = rightWhiteRook;
            this.leftWhiteRook = leftWhiteRook;
            this.rightBlackRook = rightBlackRook;
            this.leftBlackRook = leftBlackRook;
        }

        /// <summary>
        /// Deep clone board with movestack
        /// </summary>
        /// <returns>Clone of the board</returns>
        public Board Clone()
        {
            Stack<Move> moveStack = new Stack<Move>();
            foreach(Move m in moves.Reverse()){
                moveStack.Push(m);
            }
            return new Board((byte[,])board.Clone(), (byte[])pieceCount.Clone(), 
                moveStack, pieces, gameState, blackKingTaken, whiteKingTaken, whiteKing, blackKing, rightWhiteRook, rightBlackRook, leftWhiteRook, leftBlackRook);
        }

        public bool IsMidGame()
        {
            return gameState == MID_GAME;
        }

        public bool IsEndGame()
        {
            return gameState == END_GAME;
        }

        public bool IsLateEndGame()
        {
            return gameState == LATE_END_GAME;
        }

        public bool IsStartGame()
        {
            return ( whiteKing == 0 && (leftWhiteRook == 0 || rightWhiteRook == 0)) && (blackKing == 0 && (leftBlackRook == 0 || rightBlackRook == 0));
        }

        /// <summary>
        /// Returns if last move applied was a capture
        /// </summary>
        /// <returns>Has piece been taken</returns>
        public bool IsCapture()
        {
            return moves.Peek().destinationPiece != 0;
        }

        /// <summary>
        /// Has either king been taken?
        /// </summary>
        /// <returns>true if game over</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTerminal()
        {
            return blackKingTaken || whiteKingTaken;
        }

        public Move GetLastMove()
        {
            return moves.Peek();
        }

        /// <summary>
        /// Returns all moves applied to board
        /// </summary>
        /// <returns></returns>
        public Stack<Move> GetAllMoves()
        {
            return moves;
        }

        /// <summary>
        /// Returns whether the piece matches the color passed in
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="white"></param>
        /// <returns>true if piece matches color</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsColor(int i, int j, bool white)
        {
            return ((board[i, j] - 1) / 6 == 0) == white;
        }

        /// <summary>
        /// Checks board positions and returns false if pieces do not match
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if same chess board</returns>
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

        /// <summary>
        /// Board string
        /// </summary>
        /// <returns>board representation</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 7; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    sb.Append(" " + board[j, i]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns all moves that result in a capture
        /// </summary>
        /// <param name="color"></param>
        /// <returns>list of capture moves</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<Move> GetAllCaptureStates(bool color)
        {
            List<Move> allMoves = this.GetAllStates(color, false);
            List<Move> capMoves = new List<Move>();
            for(int i = 0; i < allMoves.Count; ++i)
            {
                if (allMoves[i].destinationPiece != 0)
                {
                    capMoves.Add(allMoves[i]);
                }
            }
            return capMoves;
        }

        /// <summary>
        /// Creates a move
        /// </summary>
        /// <param name="x1">origin x</param>
        /// <param name="y1">origin y</param>
        /// <param name="x2">dest x</param>
        /// <param name="y2">dest y</param>
        /// <returns></returns>
        public Move CreateMove(int x1, int y1, int x2, int y2)
        {
            return new Move(x1, y1, x2, y2, board);
        }

        /// <summary>
        /// Creates a move
        /// </summary>
        /// <param name="x1">origin x</param>
        /// <param name="y1">origin y</param>
        /// <param name="x2">dest x</param>
        /// <param name="y2">dest y</param>
        /// <param name="promote">is this a promotion</param>
        /// <returns></returns>
        public Move CreateMove(int x1, int y1, int x2, int y2, byte promote)
        {
            return new Move(x1, y1, x2, y2, board, promote);
        }

        /// <summary>
        /// Applies a move to the board and records any flags in the process as well as pieces removed/added
        /// </summary>
        /// <param name="move">move to apply to board</param>
        public void MakeMove(Move move)
        {
            moves.Push(move);
            if (move.destinationPiece == W_KING)
            {
                whiteKingTaken = true;
            }
            else if (move.destinationPiece == B_KING)
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
                    if (move.destinationPiece != B_KING && move.destinationPiece != W_KING)
                    {
                        --pieceCount[move.destinationPiece];
                    }
                }
                UpdateGameState(true);
            }
            // make enpassent
            else if (move.originPiece % 6 == W_PAWN && move.originX != move.destX && move.destinationPiece == 0)
            {
                board[move.destX, move.destY] = move.originPiece;
                board[move.destX, move.originY] = BLANK_PIECE;
                --pieces;
                UpdateGameState(true);
                board[move.originX, move.originY] = BLANK_PIECE;
            }
            // make castle
            else if (move.originPiece % 6 == 0 && (move.destX - move.originX == 2 || move.originX - move.destX == 2))
            {
                board[move.originX, move.originY] = BLANK_PIECE;
                board[move.destX, move.destY] = move.originPiece;
                bool colorPiece = (move.originPiece - 1) / 6 == 0;
                if (colorPiece)
                {
                    whiteKing = this.moves.Count;
                }
                else
                {
                    blackKing = this.moves.Count;
                }
                int y = move.originY;
                int x = 0;
                int x2 = move.destX + 1;
                if (move.destX > move.originX)
                {
                    x = 7;
                    x2 = move.destX - 1;
                    if (colorPiece)
                    {
                        rightWhiteRook = this.moves.Count;
                    }
                    else
                    {
                        rightBlackRook = this.moves.Count;
                    }
                }
                else
                {
                    if (colorPiece)
                    {
                        leftWhiteRook = this.moves.Count;
                    }
                    else
                    {
                        leftBlackRook = this.moves.Count;
                    }
                }
                board[x2, y] = board[x, y];
                board[x, y] = BLANK_PIECE;

            }
            else
            {
                // make regular move
                board[move.originX, move.originY] = BLANK_PIECE;
                board[move.destX, move.destY] = move.originPiece;
                if (move.destinationPiece != BLANK_PIECE) //decrement pieces count if capture
                {
                    --pieces;
                    UpdateGameState(true);
                    if (move.destinationPiece != W_KING && move.destinationPiece != B_KING)
                    {
                        --pieceCount[move.destinationPiece];
                    }
                }
                if (move.originPiece % 6 == 0 && move.originX == 4)
                {
                    if (whiteKing == 0 && move.originPiece == W_KING && move.originY == 0)
                    {
                        whiteKing = this.moves.Count;
                    }
                    else if (blackKing == 0 && move.originPiece == B_KING && move.originY == 7)
                    {
                        blackKing = this.moves.Count;
                    }
                }
                else if (move.originPiece % 6 == W_ROOK)
                {
                    if (move.originX == 0)
                    {
                        if (leftWhiteRook == 0 && move.originPiece == W_ROOK && move.originY == 0)
                        {
                            leftWhiteRook = this.moves.Count;
                        }
                        else if (leftBlackRook == 0 && move.originPiece == B_ROOK && move.originY == 7)
                        {
                            leftBlackRook = this.moves.Count;
                        }
                    }
                    else if (move.originX == 7)
                    {
                        if (rightWhiteRook == 0 && move.originPiece == W_ROOK && move.originY == 0)
                        {
                            rightWhiteRook = this.moves.Count;
                        }
                        else if (rightBlackRook == 0 && move.originPiece == B_ROOK && move.originY == 7)
                        {
                            rightBlackRook = this.moves.Count;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Undos the last applied move (pop stack) and undoes any flags or pieces removed/added
        /// </summary>
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

            if(move.promotion)
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
                }
                UpdateGameState(false);
                board[move.destX, move.destY] = move.destinationPiece;
            }
            else if(move.originPiece % 6 == 0 && (move.destX - move.originX == 2 || move.originX - move.destX == 2))
            {
                int y = move.originY;
                int x = move.destX + 1;
                int x2 = 0;
                bool colorPiece = move.originPiece == W_KING;
                if (move.destX > move.originX)
                {
                    x = move.destX - 1;
                    x2 = 7;
                    if (colorPiece)
                    {
                        whiteKing = 0;
                        rightWhiteRook = 0;
                    }
                    else
                    {
                        blackKing = 0;
                        rightBlackRook = 0;
                    }
                }
                else 
                {
                    if (colorPiece)
                    {
                        whiteKing = 0;
                        leftWhiteRook = 0;
                    }
                    else
                    {
                        blackKing = 0;
                        leftBlackRook = 0;
                    }
                    //x2 
                }

                board[move.originX, move.originY] = move.originPiece;
                board[move.destX, move.destY] = BLANK_PIECE;
                board[x2, y] = board[x, y];
                board[x, y] = BLANK_PIECE;
            }
            else if(move.enpassent)
            {
                ++pieces;
                ++pieceCount[move.destinationPiece];
                UpdateGameState(false);
                board[move.originX, move.originY] = move.originPiece;
                board[move.destX, move.destY] = BLANK_PIECE;
                board[move.destX, move.originY] = move.destinationPiece;
                //Console.WriteLine("En Passent");
            }
            else
            {
                if (move.destinationPiece != BLANK_PIECE)
                {
                    ++pieces;
                    if (move.destinationPiece != B_KING && move.destinationPiece != W_KING)
                    {
                        ++pieceCount[move.destinationPiece];
                    }
                    UpdateGameState(false);
                }
                board[move.originX, move.originY] = move.originPiece;
                board[move.destX, move.destY] = move.destinationPiece;
                if (move.originPiece % 6 == 0 && move.destX == 4)
                {
                    if (move.originPiece == W_KING && move.destY == 0 && this.moves.Count < whiteKing)
                    {
                        whiteKing = 0;
                    }
                    else if (move.originPiece == B_KING && move.destY == 7 && this.moves.Count < blackKing)
                    {
                        blackKing = 0;
                    }
                }
                else if (move.originPiece % 6 == W_ROOK)
                {
                    if (move.originPiece == W_ROOK)
                    {
                        if (move.destX == 0 && move.destY == 0 && this.moves.Count < leftWhiteRook)
                        {
                            leftWhiteRook = 0;
                        }
                        else if (move.destX == 7 && move.destY == 0 && this.moves.Count < rightWhiteRook)
                        {
                            rightWhiteRook = 0;
                        }
                    }
                    else if (move.originPiece == B_ROOK)
                    {
                        if (move.destX == 0 && move.destY == 7 && this.moves.Count < leftBlackRook)
                        {
                            leftBlackRook = 0;
                        }
                        else if (move.destX == 7 && move.destY == 7 && this.moves.Count < rightBlackRook)
                        {
                            rightBlackRook = 0;
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Updates the state of the game based on pieces
        /// </summary>
        /// <param name="makeMove">true if making a move, or false of undomove</param>
        private void UpdateGameState(bool makeMove)
        {
            if(makeMove)
            {
                if (pieces <= LATEENDGAMEPIECES)
                {
                    gameState = LATE_END_GAME;
                }
                else if (pieces <= ENDGAMEPIECES)
                {
                    gameState = END_GAME;
                }
                else if (pieces <= MIDGAMEPIECES)
                {
                    gameState = MID_GAME;
                }
            }
            else
            {
                if (pieces > MIDGAMEPIECES)
                {
                    gameState = BEG_GAME;
                }
                else if (pieces > ENDGAMEPIECES)
                {
                    gameState = MID_GAME;
                }
                else if (pieces > LATEENDGAMEPIECES)
                {
                    gameState = END_GAME;
                }
            }
        }

        /// <summary>
        /// Sorts a list of moves based on evaluation
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SortMoves(List<Move> moves, bool color){
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

        /// <summary>
        /// Plays random move from available moves
        /// </summary>
        /// <param name="move"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public Board PlayRandomMove(out string move, bool color)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            List<Move> boards = GetAllStates(color, true);
            Console.WriteLine("Moves Available: " + boards.Count);
            int x = rand.Next(boards.Count);
            Board b = this.Clone();
            b.MakeMove(boards[x]);
            move = DetectMove(b);
            return b;
        }

        /// <summary>
        /// Plays a single negamax move and returns the resultant board and string move in single thread
        /// </summary>
        /// <param name="move"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Board PlayNegaMaxMove(out string move, bool color, int depth)
        {
            Console.WriteLine("suceed");
            List<Move> moves = GetAllStates(color, true);
            this.SortMoves(moves, color);
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
                    int score = -Negamax.NegaMax(this, depth - 1, -beta, -alpha, !color, moves[i].destinationPiece != 0, 0);
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
            move = DetectMove(b);
            return b;
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
                    int score = -Negamax.NegaMax(this, depth - 1, -beta, -alpha, !color, moves[i].destinationPiece != 0, 0);
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
        }

        /// <summary>
        /// Plays a single negamax move and returns the resultant board and string move in mulithread
        /// </summary>
        /// <param name="move">output from function with chess move syntax</param>
        /// <param name="color"> color to move</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Move PlayNegaMaxMoveMultiThreaded(out string move, bool color, int depth)
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
            //Board b = this.Clone();

            if (moveToMake == null)
            {
                Console.WriteLine("No move to make");
                move = "";
                return null;
            }
            //Console.WriteLine(b.ToString());
            //b.MakeMove(moveToMake);
            //Console.WriteLine(moveToMake.ToString());
            //Console.WriteLine(b.ToString());
            move = moveToMake.ToString();
            return moveToMake;
        }

        /// <summary>
        /// Detects the last played move based on the board difference and returns Chess syntax
        /// </summary>
        /// <param name="b"> board to detect change in</param>
        /// <returns>String that represents the last move played</returns>
        public string DetectMove(Board b)
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

        /// <summary>
        /// Evaluates the board state and returns the score based on the side to move
        /// </summary>
        /// <param name="white"></param>
        /// <param name="offset"></param>
        /// <returns>score of the turn to move</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Evaluate(bool white, int offset)
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
                            if (IsEndGame())
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
                            if(IsEndGame())
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
                            if (!IsEndGame() && ((j != 0 || j != 8) || i != 3) )
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
                                if (IsEndGame())
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
                                if (IsEndGame())
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

            if(IsEndGame())
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
                if (!IsEndGame())
                {
                    whiteScore += 20;
                }
            }
            if (pieceCount[B_BISHOP] >= 2)
            {
                if(!IsEndGame())
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

            if (white)
            {
                return (whiteScore - blackScore) + offset;
            }
            else
            {
                return (blackScore - whiteScore) + offset;
            }
        }

        /// <summary>
        /// Returns all moves available for a color
        /// </summary>
        /// <param name="white"></param>
        /// <param name="first"></param>
        /// <returns>List of all possible moves for color to play</returns>
        public List<Move> GetAllStates(bool white, bool first)
        {
            List<Move> moves = new List<Move>();
            //generate all moves
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((board[i, j] != 0) && IsColor(i, j, white))
                    {
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
                                        moves.Add(CreateMove(i, j, i, j + 1, W_KNIGHT));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i, j + 1));
                                    }
                                }
                                if (j == 1 && board[i, j + 1] == 0 && board[i, j + 2] == 0)
                                {

                                    moves.Add(CreateMove(i, j, i, j + 2));
                                }
                                if ((i < 7) && board[i + 1, j + 1] != 0 && (IsColor(i + 1, j + 1, !white)))
                                {

                                    if (j == 6)
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j + 1, W_QUEEN));
                                        moves.Add(CreateMove(i, j, i + 1, j + 1, W_KNIGHT));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j + 1));
                                    }
                                }
                                if ((i > 0) && board[i - 1, j + 1] != 0 && (IsColor(i - 1, j + 1, !white)))
                                {

                                    if (j == 6)
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j + 1, W_QUEEN));
                                        moves.Add(CreateMove(i, j, i - 1, j + 1, W_KNIGHT));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j + 1));
                                    }
                                }
                                // en passent
                                if (j == 4)
                                {
                                    if (i > 0 && board[i - 1, j] == B_PAWN)
                                    {
                                        if (this.moves.Count > 0)
                                        {
                                            Move m = this.moves.Peek();
                                            if (m.originPiece == B_PAWN && m.originX == (i - 1) && m.originY == 6 && m.destX == (i - 1) && m.destY == 4)
                                            {
                                                moves.Add(new Move(i, j, i - 1, j + 1, this.board, true));
                                            }
                                        }
                                    }
                                    if (i < 7 && board[i + 1, j] == B_PAWN)
                                    {
                                        if (this.moves.Count > 0)
                                        {
                                            Move m = this.moves.Peek();
                                            if (m.originPiece == B_PAWN && m.originX == (i + 1) && m.originY == 6 && m.destX == (i + 1) && m.destY == 4)
                                            {
                                                moves.Add(new Move(i, j, i + 1, j + 1, this.board, true));
                                            }
                                        }
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
                                        moves.Add(CreateMove(i, j, i, j - 1, B_KNIGHT));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i, j - 1));
                                    }
                                }
                                if (j == 6 && board[i, j - 1] == 0 && board[i, j - 2] == 0)
                                {

                                    moves.Add(CreateMove(i, j, i, j - 2));
                                }
                                if ((i < 7) && board[i + 1, j - 1] != 0 && (IsColor(i + 1, j - 1, !white)))
                                {

                                    if (j == 1)
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j - 1, B_QUEEN));
                                        moves.Add(CreateMove(i, j, i + 1, j - 1, B_KNIGHT));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i + 1, j - 1));
                                    }
                                }
                                if ((i > 0) && board[i - 1, j - 1] != 0 && (IsColor(i - 1, j - 1, !white)))
                                {

                                    if (j == 1)
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j - 1, B_QUEEN));
                                        moves.Add(CreateMove(i, j, i - 1, j - 1, B_KNIGHT));
                                    }
                                    else
                                    {
                                        moves.Add(CreateMove(i, j, i - 1, j - 1));
                                    }

                                }
                                // en passent
                                if (j == 3)
                                {
                                    if (i > 0 && board[i - 1, j] == W_PAWN)
                                    {
                                        if (this.moves.Count > 0)
                                        {
                                            Move m = this.moves.Peek();
                                            if (m.originPiece == W_PAWN && m.originX == (i - 1) && m.originY == 1 && m.destX == (i - 1) && m.destY == j)
                                            {
                                                moves.Add(new Move(i, j, i - 1, j - 1, this.board, true));
                                            }
                                        }
                                    }
                                    if (i < 7 && board[i + 1, j] == W_PAWN)
                                    {
                                        if (this.moves.Count > 0)
                                        {
                                            Move m = this.moves.Peek();
                                            if (m.originPiece == W_PAWN && m.originX == (i + 1) && m.originY == 1 && m.destX == (i + 1) && m.destY == j)
                                            {
                                                moves.Add(new Move(i, j, i + 1, j - 1, this.board, true));
                                            }
                                        }
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
                            if (i == 4 && j == 0 && board[i, j] == W_KING && whiteKing == 0)
                            {
                                if (rightWhiteRook == 0)
                                {
                                    // kingside white castle
                                    if (board[5, 0] == 0 && board[6, 0] == 0)
                                    {
                                        Board b = this.Clone();
                                        if (!b.CheckForKingCheck(true))
                                        {
                                            b.MakeMove(b.CreateMove(4, 0, 5, 0));
                                            if (!b.CheckForKingCheck(true))
                                            {
                                                //b.UndoMove();
                                                b.MakeMove(b.CreateMove(5, 0, 6, 0));
                                                if (!b.CheckForKingCheck(true))
                                                {
                                                    moves.Add(CreateMove(4, 0, 6, 0));
                                                }
                                                b.UndoMove();
                                            }
                                            b.UndoMove();
                                        }
                                    }
                                }
                                if (leftWhiteRook == 0)
                                {
                                    if (board[3, 0] == 0 && board[2, 0] == 0 && board[1, 0] == 0)
                                    {
                                        Board b = this.Clone();
                                        if (!b.CheckForKingCheck(true))
                                        {
                                            b.MakeMove(b.CreateMove(4, 0, 3, 0));
                                            if (!b.CheckForKingCheck(true))
                                            {
                                                //b.UndoMove();
                                                b.MakeMove(b.CreateMove(3, 0, 2, 0));
                                                if (!b.CheckForKingCheck(true))
                                                {
                                                    moves.Add(CreateMove(4, 0, 2, 0));
                                                }
                                                b.UndoMove();
                                            }
                                            b.UndoMove();
                                        }
                                    }
                                }
                            }
                            else if (j == 7 && i == 4 && board[i, j] == B_KING && blackKing == 0)
                            {
                                if (rightBlackRook == 0)
                                {
                                    if (board[5, 7] == 0 && board[6, 7] == 0)
                                    {
                                        Board b = this.Clone();
                                        if (!b.CheckForKingCheck(false))
                                        {
                                            b.MakeMove(b.CreateMove(4, 7, 5, 7));
                                            if (!b.CheckForKingCheck(false))
                                            {
                                                //b.UndoMove();
                                                b.MakeMove(b.CreateMove(5, 7, 6, 7));
                                                if (!b.CheckForKingCheck(false))
                                                {
                                                    moves.Add(CreateMove(4, 7, 6, 7));
                                                }
                                                b.UndoMove();
                                            }
                                            b.UndoMove();
                                        }
                                    }
                                }
                                if (leftBlackRook == 0)
                                {
                                    if (board[2, 7] == 0 && board[3, 7] == 0 && board[1, 7] == 0)
                                    {
                                        Board b = this.Clone();
                                        if (!b.CheckForKingCheck(false))
                                        {
                                            b.MakeMove(b.CreateMove(4, 7, 3, 7));
                                            if (!b.CheckForKingCheck(false))
                                            {
                                                //b.UndoMove();
                                                b.MakeMove(b.CreateMove(3, 7, 2, 7));
                                                if (!b.CheckForKingCheck(false))
                                                {
                                                    moves.Add(CreateMove(4, 7, 2, 7));
                                                }
                                                b.UndoMove();
                                            }
                                            b.UndoMove();
                                        }
                                    }
                                }
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

        /// <summary>
        /// Check if a king is in check based on their color. Will search board for king location.
        /// </summary>
        /// <param name="white"></param>
        /// <returns>true if king is in check</returns>
        public bool CheckForKingCheck(bool white)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(board[i,j] == W_KING && white)
                    {
                        return CheckForKingCheck(i, j, white);
                    }
                    else if(board[i,j] == B_KING && !white)
                    {
                        return CheckForKingCheck(i, j, white);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a king is in check based on their color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns>true if king is in check</returns>
        public bool CheckForKingCheck(int x, int y, bool color)
        {
            int i = x+1;
            for (; i < 8; ++i) // CHECKING VERTICLE
            {
                if (board[i, y] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, y] == B_ROOK || board[i, y] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, y] == W_ROOK || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x-1;
            for (; i >= 0; --i)
            {
                if (board[i, y] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, y] == B_ROOK || board[i, y] == B_QUEEN)
                        {
                            return true;
                        } 
                        break;
                    }
                    else
                    {
                        if (board[i, y] == W_ROOK || board[i, y] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            int j = y+1;
            for (; j < 8; ++j)
            {
                if (board[x, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[x, j] == B_ROOK || board[x, j] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[x, j] == W_ROOK || board[x, j] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            j = y-1;
            for (; j >= 0; --j)
            {
                if (board[x, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[x, j] == B_ROOK || board[x, j] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[x, j] == W_ROOK || board[x, j] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }

            // CHECKING DIAGONAL
            i = x+1;
            j = y+1;
            for (; j < 8 && i < 8; ++j, ++i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_BISHOP || board[i, j] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_BISHOP || board[i, j] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x-1;
            j = y+1;
            for (; j < 8 && i >= 0; ++j, --i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_BISHOP || board[i, j] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_BISHOP || board[i, j] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x+1;
            j = y-1;
            for (; j >= 0 && i < 8; --j, ++i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_BISHOP || board[i, j] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_BISHOP || board[i, j] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }
            i = x-1;
            j = y-1;
            for (; j >= 0 && i >= 0; --j, --i)
            {
                if (board[i, j] != BLANK_PIECE)
                {
                    if (color)
                    {
                        if (board[i, j] == B_BISHOP || board[i, j] == B_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                    else
                    {
                        if (board[i, j] == W_BISHOP || board[i, j] == W_QUEEN)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }

            // Check for Kings
            if(y + 1 < 8)
            {
                // UP KING
                if(color && board[x,y+1] == B_KING)
                {
                    return true;
                }
                else if(!color && board[x,y+1] == W_KING)
                {
                    return true;
                }

                // DIAG
                if(x + 1 < 8)
                {
                    if (color && board[x+1, y + 1] == B_KING)
                    {
                        return true;
                    }
                    else if (!color && board[x+1, y + 1] == W_KING)
                    {
                        return true;
                    }
                }
                if (x - 1 >= 0)
                {
                    if (color && board[x - 1, y + 1] == B_KING)
                    {
                        return true;
                    }
                    else if (!color && board[x - 1, y + 1] == W_KING)
                    {
                        return true;
                    }
                }
            }
            if(y - 1 >= 0)
            {
                // DOWN KING
                if (color && board[x, y - 1] == B_KING)
                {
                    return true;
                }
                else if (!color && board[x, y - 1] == W_KING)
                {
                    return true;
                }
                // DIAG KING
                if (x + 1 < 8)
                {
                    if (color && board[x + 1, y - 1] == B_KING)
                    {
                        return true;
                    }
                    else if (!color && board[x + 1, y - 1] == W_KING)
                    {
                        return true;
                    }
                }
                if (x - 1 >= 0)
                {
                    if (color && board[x - 1, y - 1] == B_KING)
                    {
                        return true;
                    }
                    else if (!color && board[x - 1, y - 1] == W_KING)
                    {
                        return true;
                    }
                }
            }
            // LEFT/RIGHT KING
            if(x + 1 < 8)
            {
                if (color && board[x+1, y] == B_KING)
                {
                    return true;
                }
                else if (!color && board[x+1, y] == W_KING)
                {
                    return true;
                }
            }
            if (x - 1 >= 0)
            {
                if (color && board[x-1, y] == B_KING)
                {
                    return true;
                }
                else if (!color && board[x-1, y] == W_KING)
                {
                    return true;
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
                    //if (color && board[x + 1, y + 1] == B_PAWN)
                    //{
                    //    return true;
                    //}
                    if (!color && board[x + 1, y - 1] == W_PAWN)
                    {
                        return true;
                    }
                }
                if (y - 1 >= 0)
                {
                    if (color && board[x + 1, y + 1] == B_PAWN)
                    {
                        return true;
                    }
                    //else if (!color && board[x + 1, y - 1] == W_PAWN)
                    //{
                    //    return true;
                    //}
                }
            }
            if (x - 1 >= 0)
            {
                if (y + 1 < 8)
                {
                    //if (color && board[x - 1, y + 1] == B_PAWN)
                    //{
                    //    return true;
                    //}
                    if (!color && board[x - 1, y - 1] == W_PAWN)
                    {
                        return true;
                    }
                }
                if (y - 1 >= 0)
                {
                    if (color && board[x - 1, y + 1] == B_PAWN)
                    {
                        return true;
                    }
                    //else if (!color && board[x - 1, y - 1] == W_PAWN)
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }

        /// <summary>
        ///  Returns the file string letter
        /// </summary>
        /// <param name="i"></param>
        /// <returns>letter of file</returns>
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
    }
}
