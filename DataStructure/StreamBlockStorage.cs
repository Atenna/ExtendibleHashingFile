using System;
using System.IO;

namespace ExtendibleHashingFile.DataStructure
{
    public class StreamBlockStorage<T> : IBlockStorage<T>
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;
        private readonly int _maxValues, _valueBytes, _blockBytes;
        private readonly IBlockSerializer<T> _valueSerializer;
        private readonly Action<int, int> _updateIndices; // delegat
        private int _blocksCount;
        public int Count { get { return _blocksCount; } }

        // Stream pre reader alebo writer - najde a nastavi poziciu zaciatku bloku
        void SeekStream(int index)
        {
            if (index < 0 || index >= _blocksCount)
            {
                throw new InvalidOperationException();
            }

            _stream.Seek((long)index * _blockBytes, SeekOrigin.Begin);
        }

        // podla poctu blokov
        void UpdateStreamLength()
        {
            _stream.SetLength((long)_blocksCount * _blockBytes);
        }

        // cita pole bajtov, vracia novy blok
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
            block.SerializeTo(_writer, _valueSerializer, valueBytesCount: _valueBytes);
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

            // pokial nemaze posledny
            if (index != lastIndex)
            {
                SeekStream(lastIndex);
                byte[] blockBytes = _reader.ReadBytes(_blockBytes);
                // miesto mazaneho prepise poslednym
                SeekStream(index);
                _writer.Write(blockBytes);
            }

            --_blocksCount;
            UpdateStreamLength();

            if (index != lastIndex)
            {
                _updateIndices(lastIndex, index);
            }
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
            _blockBytes = Block<T>.GetSerializedBytesCount(_valueBytes, maxBlockValues);

            if (stream.Length != (long) blocksCount*_blockBytes)
            {
                throw new IOException();
            }
        }
    }
}
