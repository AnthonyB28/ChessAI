using System;

namespace ChessAI
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState s = new GameState(Convert.ToBoolean(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), args[3]);
            s.Run();
        }
    }
}
