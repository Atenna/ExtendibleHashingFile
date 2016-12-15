using System;
using System.Collections.Generic;
using System.IO;

namespace ExtendibleHashingFile.DataStructure
{
    public class BlockCollection<T>
    {
        private readonly IBlockStorage<T> _storage;
        readonly int[] _blocksWithDepths = new int[32 + 1];

        public Block<T> Read(int index)
        {
            return _storage.Read(index);
        }

        public void Write(Block<T> block, int index)
        {
            --_blocksWithDepths[_storage.ReadDepth(index)];
            _storage.Write(block, index);
            ++_blocksWithDepths[block.Depth];
        }

        public int Add(Block<T> block)
        {
            int newIndex = _storage.Add(block);
            ++_blocksWithDepths[block.Depth];
            return newIndex;
        }

        public void RemoveAt(int index)
        {
            --_blocksWithDepths[_storage.ReadDepth(index)];
            _storage.RemoveAt(index);
        }

        public int MaxBlockDepth
        {
            get
            {
                for (int i = _blocksWithDepths.Length - 1; i > 0; --i)
                {
                    if (_blocksWithDepths[i] > 0)
                        return i;
                }

                return 0;
            }
        }

        public BlockCollection(IBlockStorage<T> storage)
        {
            _storage = storage;
        }

        public void SerializeTo(BinaryWriter writer)
        {
            foreach (var count in _blocksWithDepths)
            {
                writer.Write(count);
            }
        }

        // Deserialize constructor
        public BlockCollection(IBlockStorage<T> storage, BinaryReader reader)
        {
            _storage = storage;

            for (int i = 0; i < _blocksWithDepths.Length; ++i)
            {
                int count = _blocksWithDepths[i] = reader.ReadInt32();
                if (count < 0 || count > storage.Count)
                {
                    throw new IOException();
                }
            }
        }
    }
}
