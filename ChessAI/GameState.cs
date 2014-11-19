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
            turn = 0;
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
            Network.JSONPollResponse response = network.MakePoll();
            while (!response.ready)
            {
                System.Threading.Thread.Sleep(5000);
                response = network.MakePoll();
            }
            UpdateBoard(response);
        }

        public void UpdateBoard(Network.JSONPollResponse response)
        {
            turn = response.lastMoveNumber;
            string move = response.lastMove;
            int x1 = 0;
            int x2 = 0;
            int y1 = Convert.ToInt32(move[2]) - 1;
            int y2 = Convert.ToInt32(move[4]) - 1;
            switch (move[1])
            {
                case 'A':
                    x1 = 0;
                    break;
                case 'B':
                    x1 = 1;
                    break;
                case 'C':
                    x1 = 2;
                    break;
                case 'D':
                    x1 = 3;
                    break;
                case 'E':
                    x1 = 4;
                    break;
                case 'F':
                    x1 = 5;
                    break;
                case 'G':
                    x1 = 6;
                    break;
                case 'H':
                    x1 = 7;
                    break;
            }
            switch (move[3])
            {
                case 'A':
                    x2 = 0;
                    break;
                case 'B':
                    x2 = 1;
                    break;
                case 'C':
                    x2 = 2;
                    break;
                case 'D':
                    x2 = 3;
                    break;
                case 'E':
                    x2 = 4;
                    break;
                case 'F':
                    x2 = 5;
                    break;
                case 'G':
                    x2 = 6;
                    break;
                case 'H':
                    x2 = 7;
                    break;
            }
            board.MovePiece(x1, y1, x2, y2);
        }

        public void MakeMove()
        {
            
        }
    }
}
