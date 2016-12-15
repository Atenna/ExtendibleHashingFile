using System;
using System.Collections.Generic;
using System.IO;

namespace ExtendibleHashingFile.DataStructure
{
    public class Block<T>
    {
        /// <summary>
        /// Hlbka bloku
        /// </summary>
        public int Depth
        {
            get; private set;
            
        }
        /// <summary>
        /// Zaznamy ulozene v bloku
        /// </summary>
        public List<Record<T>> Records
        {
            get; private set; 
        }

        public Block(int depth)
        {
            Depth = depth;
            Records = new List<Record<T>>();
        }

        public int IndexOf(int hash, T data)
        {
            var comparer = EqualityComparer<T>.Default;
            //return Records.FindIndex(p => p.Hash == hash);  ! nie len podla hashu!
            return Records.FindIndex(p => p.Hash == hash && comparer.Equals(p.Data, data));
            // ak nenajde, vrati -1
        }

        public bool Contains(int hash, T data)
        {
            return IndexOf(hash, data) != -1;
        }

        public bool TryRemove(int hash, T data, out T existingValue)
        {
            int index = IndexOf(hash, data);
            if (index == -1)
            {
                existingValue = default(T);
                return false;
            }
            existingValue = Records[index].Data;
            Records.RemoveAt(index);
            return true;
        }

        // !TODO WTF
        public bool TryGetEqual(int hash, T data, out T existingValue)
        {
            int index = IndexOf(hash, data);
            if (index == -1)
            {
                // prazdny objekt daneho typu
                existingValue = default(T);
                return false;
            }

            existingValue = Records[index].Data;
            return true;
        }

        public bool Find(int hash, T data, bool updateIfExists)
        {
            int index = IndexOf(hash, data);
            if (index == -1)
            {
                return false;
            }

            if (updateIfExists)
            {
                Records[index] = new Record<T>(hash, data);
            }

            return true;
        }

        public void SerializeTo(BinaryWriter writer,
            IBlockSerializer<T> valueSerializer,
            int valueBytesCount)
        {
            writer.Write(Depth);
            writer.Write(Records.Count);
            foreach (var hashedValue in Records)
                writer.Write(hashedValue.Hash);
            foreach (var hashedValue in Records)
            {
                var valueBytes = valueSerializer.Serialize(hashedValue.Data);
                if (valueBytes.Length != valueBytesCount)
                    throw new InvalidOperationException("Unexpected Data bytes count.");
                writer.Write(valueBytes);
            }
        }

        // Deserialize constructor
        internal Block(BinaryReader reader,
            IBlockSerializer<T> valueSerializer,
            int valueBytesCount,
            int maxValues)
        {
            Depth = ReadDepth(reader);

            int count = reader.ReadInt32();
            if (count < 0 || count > maxValues)
                throw new IOException();

            int[] hashes = new int[count];
            for (int i = 0; i < count; ++i)
                hashes[i] = reader.ReadInt32();

            Records = new List<Record<T>>(count);
            for (int i = 0; i < count; ++i)
            {
                var value = valueSerializer.Deserialize(reader.ReadBytes(valueBytesCount));
                Records.Add(new Record<T>(hashes[i], value));
            }
        }

        internal static int ReadDepth(BinaryReader reader)
        {
            int depth = reader.ReadInt32();
            if (depth < 0 || depth > ExtendibleHashSet<T>.GlobalMaxDepth)
                throw new IOException();

            return depth;
        }

        internal static int GetSerializedBytesCount(int valueBytesCount, int maxValues)
        {
            return sizeof(int) * (2 + maxValues) + valueBytesCount * maxValues;
        }

    }

}
