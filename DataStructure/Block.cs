using System;
using System.Collections;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;

namespace ExtendibleHashingFile.DataStructure
{
    public class Block<T> where T : class, IRecord<T>, new()
    {
        public T[] Records;

        public int MaxRecordCount; // max pocet zaznamov v bloku
        public int Depth { get; set; } // aktualna hlbka bloku = podla kolkych bitov sa tu identifikuje adresa
        public int Count { get; set; } // aktualny pocet zaznamov v bloku

        public Block(int maxRecordCount, int depth)
        {
            MaxRecordCount = maxRecordCount;
            Depth = depth;
            Count = 0;
            //Records = Enumerable.Repeat(new T(), MaxRecordCount).ToArray();
            Records = new T[MaxRecordCount];
            for (int i = 0; i < MaxRecordCount; i++)
            {
                Records[i] = new T();
            }
        }

        public int Size()
        {
            return sizeof(int)*2 + Records[0].Size()*MaxRecordCount;
        }

        // na ukladanie info o bloku - hlbku a pocet zaznamov a velkost 1 recordu a vsetky recordy
        public byte[] ToByteArray()
        {
            var depth = BitConverter.GetBytes(Depth).ToArray();
            var count = BitConverter.GetBytes(Count).ToArray();

            var result = depth.Concat(count);
            for (int i = 0; i < Records.Length; i++)
            {
                result = result.Concat(Records[i].ToByteArray());
            }

            
            return result.ToArray();
        }

        public void Update(T data)
        {
            for (int i = 0; i < Records.Length; i++)
            {
                if (Records[i].Equal(data))
                {
                    Records[i] = data;
                }
            }
        }

    public void FromByteArray(byte[] arr)
        {
            Depth = BitConverter.ToInt32(arr, 0);
            Count = BitConverter.ToInt32(arr, sizeof(int));
            int readFrom = sizeof(int)*2;
            for (int i = 0; i < Records.Length; i++)
            {
                Records[i].FromByteArray(arr, readFrom);
                readFrom += Records[i].Size();
            }
        }

        public bool Add(T r)
        {
            if (IsFull)
            {
                return false;
            }

            Records[Count++] = r;

            // sortovanie?

            return true;
        }

        public bool IsFull
        {
            get { return Count >= MaxRecordCount; }
        }

    }

    internal class RecordNotFoundException : Exception
    {
    }
}
