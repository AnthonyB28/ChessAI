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
        public static volatile EntryW[] TABLEQ;
        public static readonly int SIZE = 104858300;

        public static void InitTable()
        {
            TABLE = new EntryW[SIZE];
            TABLEQ = new EntryW[SIZE];
            for(int i = 0; i < SIZE; i++)
            {
                TABLE[i] = new EntryW();
                TABLEQ[i] = new EntryW();
            }
            Console.WriteLine("create");
        }

        public static void InsertState(long key, long val)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int)(hashKey % SIZE);
            TABLE[hash].key = key ^ val;
            TABLE[hash].data = val;
        }

        public static bool GetState(long key, out int val)
        {
            val = 0;
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int)(hashKey % SIZE);
            long tableKey = TABLE[hash].key + 0;
            long tableData = TABLE[hash].data + 0;
            if ((tableKey ^ tableData) == key)
            {
                val = (int) tableData;
                return true;
            }

            return false;
        }

        public static void Insert(long key, short depth, byte flag, int eval)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int) (hashKey % SIZE);
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

        public static void InsertQ(long key, short depth, byte flag, int eval)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int)(hashKey % SIZE);
            Entry toSave = new Entry();
            //toSave.key = key;
            toSave.depth = depth;
            toSave.flag = flag;
            toSave.eval = eval;
            //toSave.move = move;
            //toSave.dirty = false;
            long data = toSave.Serialize();
            TABLEQ[hash].key = key ^ data;
            TABLEQ[hash].data = data;
        }

        public static Entry Probe(long key)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int) (hashKey % SIZE);
            
            long tableKey = TABLE[hash].key + 0;
            long tableData = TABLE[hash].data + 0;
            if ((tableKey ^ tableData) == key)
            {
                Entry toTest = Entry.Desserialize(tableData);
                return toTest;
            }
            //else
            //{
            //    if (tableKey != 0)
            //    {
            //        Console.WriteLine("corrupted");
            //    }
            //}
            return null;
        }


        public static Entry ProbeQ(long key)
        {
            long hashKey = key;
            if (key < 0)
            {
                hashKey = -key;
            }
            int hash = (int)(hashKey % SIZE);

            long tableKey = TABLEQ[hash].key + 0;
            long tableData = TABLEQ[hash].data + 0;
            if ((tableKey ^ tableData) == key)
            {
                Entry toTest = Entry.Desserialize(tableData);
                return toTest;
            }
            //else
            //{
            //    if (tableKey != 0)
            //    {
            //        Console.WriteLine("corrupted");
            //    }
            //}
            return null;
        }
    }

    class Entry
    {
        public static readonly byte EXACT = 0;
        public static readonly byte UPPER = 1;
        public static readonly byte LOWER = 2;

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
            result.flag = -1;
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
