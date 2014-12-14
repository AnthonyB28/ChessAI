using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    // http://web.archive.org/web/20070809015843/http://www.seanet.com/~brucemo/topics/hashing.htm
    // http://sourceforge.net/p/mediocrechess/code/HEAD/tree/branches/evaluationoverhaul/src/main/java/mediocrechess/mediocre/transtable/TranspositionTable.java#l51

    class Entry
    {
        public static readonly byte EXACT = 0;
        public static readonly byte ALPHA = 1;
        public static readonly byte BETA = 2;

        // depth replacement
        public long key;
        public int depth;
        public int eval;
        public byte flag; // EXACT, BETA, ALPHA
        public bool dirty; // old entry
        public Move move;
    }

    class Transposition
    {
        public static Dictionary<int,Entry> TABLE; // Not a real hashmap, but w/e
        public static readonly int SIZE = 1048583;

        public static void InitTable()
        {
            TABLE = new Dictionary<int, Entry>(SIZE);
        }

        public static int GetEval(long key)
        {
            return TABLE[(int)key % SIZE].eval;
        }

        public static void Insert(long key, int depth, byte flag, int eval, Move move)
        {
            int hash = (int) key % SIZE;
            Entry toSave = new Entry();
            toSave.key = key;
            toSave.depth = depth;
            toSave.flag = flag;
            toSave.eval = eval;
            toSave.move = move;
            toSave.dirty = false;
            TABLE[hash] = toSave;
        }

        public static int Probe(long key, int depth, int alpha, int beta)
        {
            int hash = (int) key % SIZE;
            if (TABLE.ContainsKey(hash))
            {
                Entry toTest = TABLE[hash];
                if(toTest.flag == Entry.EXACT)
                {
                    return toTest.eval;
                }
                if(toTest.flag == Entry.ALPHA && toTest.eval <= alpha)
                {
                    return alpha;
                }
                if(toTest.flag == Entry.BETA && toTest.eval >= beta)
                {
                    return beta;
                }
            }
            return Int32.MinValue;
        }


    }
}
