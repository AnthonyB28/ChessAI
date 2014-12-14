using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace ChessAI
{
    // http://web.archive.org/web/20070809015843/http://www.seanet.com/~brucemo/topics/hashing.htm
    // http://sourceforge.net/p/mediocrechess/code/HEAD/tree/branches/evaluationoverhaul/src/main/java/mediocrechess/mediocre/transtable/TranspositionTable.java#l51

    class Transposition
    {
        public static volatile EntryW[] TABLE; // Not a real hashmap, but w/e
        public static readonly int SIZE = 1048583;

        public static void InitTable()
        {
            TABLE = new EntryW[SIZE];
            for(int i = 0; i < SIZE; i++)
            {
                TABLE[i] = new EntryW();
            }
            Console.WriteLine("create");
        }

        public static void Insert(long key, byte depth, byte flag, int eval, Move move)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int) (hashKey % SIZE);
            //Console.WriteLine("hash: "+ hash);
            Entry toSave = new Entry();
            //toSave.key = key;
            toSave.depth = depth;
            toSave.flag = flag;
            toSave.eval = eval;
            //toSave.move = move;
            //toSave.dirty = false;
            long data = toSave.Serialize();
            TABLE[hash].key = key ^ data;
            TABLE[hash].data = data;
        }

        public static int Probe(long key, int depth, int alpha, int beta)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int) (hashKey % SIZE);
            //Console.WriteLine("hash: " + key);
            
            long tableKey = TABLE[hash].key;
            long tableData = TABLE[hash].data;
            if ((tableKey ^ tableData) == key)
            {
                Entry toTest = Entry.Desserialize(tableData);
                if (toTest.flag == Entry.EXACT)
                {
                    return toTest.eval;
                }
                if (toTest.flag == Entry.ALPHA && toTest.eval <= alpha)
                {
                    return alpha;
                }
                if (toTest.flag == Entry.BETA && toTest.eval >= beta)
                {
                    return beta;
                }
            }
            return Int32.MinValue;
        }
    }

    class Entry
    {
        public static readonly byte EXACT = 0;
        public static readonly byte ALPHA = 1;
        public static readonly byte BETA = 2;

        //public long key;
        public int eval;
        public short depth;
        public short flag; // EXACT, BETA, ALPHA

        public long Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(eval);
                    writer.Write(depth);
                    writer.Write(flag);
                }
                byte[] arr = m.ToArray();
                return BitConverter.ToInt64(arr, 0);
            }
        }

        public static Entry Desserialize(long data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            Entry result = new Entry();
            using (MemoryStream m = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    result.eval = reader.ReadInt32();
                    result.depth = reader.ReadInt16();
                    result.flag = reader.ReadInt16();
                }
            }
            return result;
        }
    }

    struct EntryW
    {
        public long key;
        public long data;
    }
}
