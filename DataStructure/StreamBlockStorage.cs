﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public class StreamBlockStorage<T> : IBlockStorage<T>
    {
        readonly Stream _stream;
        readonly BinaryReader _reader;
        readonly BinaryWriter _writer;
        readonly int _maxValues, _valueBytes, _bucketBytes;
        readonly IBlockSerializer<T> _valueSerializer;
        readonly Action<int, int> _updateIndices;
        int _blocksCount;

        void SeekStream(int index)
        {
            if (index < 0 || index >= _blocksCount)
                throw new InvalidOperationException();

            _stream.Seek((long)index * _bucketBytes, SeekOrigin.Begin);
        }

        void UpdateStreamLength()
        {
            _stream.SetLength((long)_blocksCount * _bucketBytes);

        }

        public int Count { get { return _blocksCount; } }

        public Block<T> Read(int index)
        {
            SeekStream(index);
            return new Block<T>(_reader, _valueSerializer,
                valueBytesCount: _valueBytes, maxValues: _maxValues);
        }

        public int ReadDepth(int index)
        {
            SeekStream(index);
            return Block<T>.ReadDepth(_reader);
        }

        public void Write(Block<T> block, int index)
        {
            SeekStream(index);
            block.SerializeTo(_writer, _valueSerializer,
                valueBytesCount: _valueBytes);
        }

        public int Add(Block<T> block)
        {
            ++_blocksCount;
            UpdateStreamLength();
            Write(block, _blocksCount - 1);
            return _blocksCount - 1;
        }

        public void RemoveAt(int index)
        {
            int lastIndex = _blocksCount - 1;

            if (index != lastIndex)
            {
                SeekStream(lastIndex);
                // TODO: No need to read free block slots here.
                byte[] bucketBytes = _reader.ReadBytes(_bucketBytes);
                SeekStream(index);
                _writer.Write(bucketBytes);
            }

            --_blocksCount;
            UpdateStreamLength();

            if (index != lastIndex)
                _updateIndices(lastIndex, index);
        }

        internal StreamBlockStorage(Stream stream,
            IBlockSerializer<T> valueSerializer,
            Action<int, int> updateIndices,
            int blocksCount,
            int maxBlockValues)
        {
            _stream = stream;
            _valueSerializer = valueSerializer;
            _updateIndices = updateIndices;
            _blocksCount = blocksCount;
            _maxValues = maxBlockValues;

            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);

            _valueBytes = valueSerializer.BlockSize;
            _bucketBytes = Block<T>.GetSerializedBytesCount(_valueBytes, maxBlockValues);

            if (stream.Length != (long)blocksCount * _bucketBytes)
                throw new IOException();
        }
    }
}
