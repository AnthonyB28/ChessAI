using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class GameState
    {
        private Board board;
        private bool color;
        private int turn;
        private bool gameOver;
        private Network network;

        public GameState(bool color, int gameID, int teamID, string teamKey)
        {
            board = new Board();
            this.color = color;
            turn = -1;
            gameOver = false;
            this.network = new Network(gameID, teamID, teamKey);
        }

        public void Run()
        {
            while (!gameOver)
            {
                PollForTurn();
                MakeMove();
            }
        }

        public void PollForTurn()
        {
            Network.JSONPollResponse response = network.RequestPoll();
            while (!response.ready || response.lastMoveNumber <= turn)
            {
                System.Threading.Thread.Sleep(5000);
                response = network.RequestPoll();
            }
            UpdateBoard(response);
        }

        public void UpdateBoard(Network.JSONPollResponse response)
        {
            turn = response.lastMoveNumber;
            if (response.lastMove != null)
            {
                string move = response.lastMove;
                int x1 = 0;
                int x2 = 0;
                int y1 = Convert.ToInt32(move[2]) - 1;
                int y2 = Convert.ToInt32(move[4]) - 1;
                switch (move[1])
                {
                    case 'a':
                        x1 = 0;
                        break;
                    case 'b':
                        x1 = 1;
                        break;
                    case 'c':
                        x1 = 2;
                        break;
                    case 'd':
                        x1 = 3;
                        break;
                    case 'e':
                        x1 = 4;
                        break;
                    case 'f':
                        x1 = 5;
                        break;
                    case 'g':
                        x1 = 6;
                        break;
                    case 'h':
                        x1 = 7;
                        break;
                }
                switch (move[3])
                {
                    case 'a':
                        x2 = 0;
                        break;
                    case 'b':
                        x2 = 1;
                        break;
                    case 'c':
                        x2 = 2;
                        break;
                    case 'd':
                        x2 = 3;
                        break;
                    case 'e':
                        x2 = 4;
                        break;
                    case 'f':
                        x2 = 5;
                        break;
                    case 'g':
                        x2 = 6;
                        break;
                    case 'h':
                        x2 = 7;
                        break;
                }
                board.MovePiece(x1, y1, x2, y2);
            }
        }

        public void MakeMove()
        {
            String move;
            board = board.PlayRandomMove(out move, color);
            turn++;
            network.MakeMove(move);
        }
    }
}
