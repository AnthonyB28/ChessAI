﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Program
    {
        static void Main(string[] args)
        {
            Board b = new Board();
            Console.WriteLine(b.GetAllStates(true).Count);
            Console.WriteLine(b.PlayRandomMove());
            Network.JSONPollResponse response = Network.MakePoll();
            Network.MakeMove("Pd2d3");
            while(true)
            {

            }
        }
    }
}
