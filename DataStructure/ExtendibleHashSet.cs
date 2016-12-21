using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtendibleHashingFile.DataStructure
{
    public class ExtendibleHashSet<T> : IDisposable
    {
        private const string IndexFilePostfix = ".index.data";
        private const string BlockFilePostfix = ".block.data";

        readonly FileStream _blockFileStream, _indexFileStream;
        readonly IBlockStorage<T> _blockStorage;
        readonly TableData<T> _tableContext;
        readonly List<Table<T>> _tables = new List<Table<T>>();

        public static bool ExtendibleHashSetExists(string dataFilePath)
        {
            return File.Exists(dataFilePath + IndexFilePostfix) &&
                File.Exists(dataFilePath + BlockFilePostfix);
        }

        public ExtendibleHashSet(string dataFilePath, IBlockSerializer<T> valueSerializer, int maxBlockValues,
            int maxSiblingMergeBlockValues, int maxTableDepth, int maxUnusedTableDepth)
        {
            if (maxBlockValues <= 0 ||
                maxSiblingMergeBlockValues < 0 ||
                maxTableDepth <= 0 ||
                maxUnusedTableDepth < 0)
            {
                throw new InvalidOperationException();
            }

            if (maxSiblingMergeBlockValues > maxBlockValues)
                throw new InvalidOperationException();
            // Create index stream now just to lock file.
            _indexFileStream = new FileStream
            (
                dataFilePath + IndexFilePostfix,
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None
            );

            _blockFileStream = new FileStream
            (
                dataFilePath + BlockFilePostfix,
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None
            );

            _blockStorage = new StreamBlockStorage<T>(
                _blockFileStream,
                valueSerializer,
                UpdateIndices,
                blocksCount: 0,
                maxBlockValues: maxBlockValues);

            _tableContext = new TableData<T>
            (
                new BlockCollection<T>(_blockStorage),
                maxBlockValues: maxBlockValues,
                maxSiblingMergeBlockValues: maxSiblingMergeBlockValues,
                maxDepth: maxTableDepth,
                maxUnusedDepth: maxUnusedTableDepth
            );
        }

        /// <summary>
        /// Ulozi do suboru directory index
        /// Velkost blokov, hlbku, "volnu" hlbku
        /// </summary>
        private void SerializeIndexToStream()
        {
            _indexFileStream.Position = 0;

            var writer = new BinaryWriter(_indexFileStream);

            // Table context
            writer.Write(_tableContext.MaxBlockSize);
            writer.Write(_tableContext.MaxSiblingMergeBlockValues);
            writer.Write(_tableContext.MaxDepth);
            writer.Write(_tableContext.MaxUnusedDepth);

            // Block storage info
            writer.Write(_blockStorage.Count);

            // Block collection
            _tableContext.Blocks.SerializeTo(writer);

            // Tables
            writer.Write(_tables.Count);
            foreach (var table in _tables)
            {
                table.SerializeTo(writer);
            }

            _indexFileStream.SetLength(_indexFileStream.Position);
        }

        /// <summary>
        /// Nacitanie dat o tabulkach a indexe zo suborov
        /// Deserializacny konstruktor
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <param name="valueSerializer"></param>
        public ExtendibleHashSet(
            string dataFilePath,
            IBlockSerializer<T> valueSerializer)
        {
            _indexFileStream = new FileStream
            (
                dataFilePath + IndexFilePostfix,
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.None
            );

            _blockFileStream = new FileStream
            (
                dataFilePath + BlockFilePostfix,
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.None
            );

            var indexReader = new BinaryReader(_indexFileStream);

            // Table context

            int maxBucketValues = indexReader.ReadInt32();
            int maxSiblingMergeBucketValues = indexReader.ReadInt32();
            int maxTableDepth = indexReader.ReadInt32();
            int maxUnusedTableDepth = indexReader.ReadInt32();

            if (maxBucketValues <= 0 ||
                maxSiblingMergeBucketValues < 0 ||
                maxTableDepth <= 0 ||
                maxUnusedTableDepth < 0)
            {
                throw new IOException();
            }

            if (maxSiblingMergeBucketValues > maxBucketValues)
            {
                throw new IOException();
            }

            // Block storage info

            int blocksCount = indexReader.ReadInt32();
            if (blocksCount < 0)
            {
                throw new IOException();
            }

            _blockStorage = new StreamBlockStorage<T>(
                _blockFileStream,
                valueSerializer,
                UpdateIndices,
                blocksCount: blocksCount,
                maxBlockValues: maxBucketValues);

            // Block collection

            var blockCollection = new BlockCollection<T>(_blockStorage, indexReader);

            // Tables

            _tableContext = new TableData<T>
            (
                blockCollection,
                maxBlockValues: maxBucketValues,
                maxSiblingMergeBlockValues: maxSiblingMergeBucketValues,
                maxDepth: maxTableDepth,
                maxUnusedDepth: maxUnusedTableDepth
            );

            int tablesCount = indexReader.ReadInt32();
            if (tablesCount < 0)
            {
                throw new IOException();
            }

            if (tablesCount == 0 ? blocksCount > 0 : blocksCount == 0)
            {
                throw new IOException();
            }

            for (int i = 0; i < tablesCount; ++i)
            {
                _tables.Add(new Table<T>(indexReader, _tableContext, blocksCount));
            }
        }

        public void Dispose()
        {
            SerializeIndexToStream();
            _blockFileStream.Close();
            _indexFileStream.Close();
        }

        private void UpdateIndices(int oldIndex, int newIndex)
        {
            _tables.ForEach(table => table.UpdateIndices(oldIndex, newIndex));
        }

        public bool Contains(T value)
        {
            int hash = value.GetHashCode();
            return _tables.Any(f => f.Contains(hash, value));
        }

        public bool TryGetEqual(T value, out T existingValue)
        {
            var hash = value.GetHashCode();

            foreach (var table in _tables)
            {
                if (table.TryGetEqual(hash, value, out existingValue))
                {
                    return true;
                }
            }

            existingValue = default(T);
            return false;
        }

        /// <summary>
        /// Priva data do hlavnej tabulky alebo preplnujuceho bloku
        /// resp. vytvori novy
        /// </summary>
        /// <param name="value"></param>
        /// <param name="updateIfExists"></param>
        /// <returns></returns>
        public bool Add(T value, bool updateIfExists)
        {
            var hash = value.GetHashCode();

            foreach (var table in _tables)
            {
                var res = table.Add(hash, value, updateIfExists);
                if (res != Table<T>.AddResult.NotAdded)
                {
                    // bolo miesto v preplnovacich blokoch alebo hlavnej tabulke
                    return res == Table<T>.AddResult.Added;
                }
            }

            var newTable = new Table<T>(_tableContext);
            _tables.Add(newTable);
            return newTable.Add(hash, value, updateIfExists) == Table<T>.AddResult.Added;
        }

        public void Remove(T value)
        {
            if (!TryRemove(value))
            {
                throw new InvalidOperationException();
            }
        }

        public bool TryRemove(T value)
        {
            T existingValue;
            return TryRemove(value, out existingValue);
        }

        public bool TryRemove(T value, out T existingValue)
        {
            var hash = value.GetHashCode();

            for (int i = 0; i < _tables.Count; ++i)
            {
                if (_tables[i].Remove(hash, value, out existingValue))
                {
                    if (_tables[i].RemoveBlockIfEmpty())
                    {
                        _tables[i] = _tables.Last();
                        _tables.RemoveAt(_tables.Count - 1);
                    }
                    return true;
                }
            }

            existingValue = default(T);
            return false;
        }
    }
}