using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace ExtendibleHashingFile.DataStructure
{
    public class Table<T>
    {
        public const int GlobalMaxDepth = 30;
        public enum AddResult { NotAdded = 0, Added, AlreadyExists }

        private readonly TableData<T> _data;
        private int[] _directory = new int[1];
        private int _currentFileDepth = 0;
        private int _count = 0;

        public Table(TableData<T> data)
        {
            _data = data;
            var firstBlock = new Block<T>(depth: 0);
            _directory[0] = _data.Blocks.Add(firstBlock);
        }

        // Deserialize constructor
        public Table(BinaryReader reader, TableData<T> data, int blocksCount)
        {
            _data = data;

            _currentFileDepth = reader.ReadInt32();
            if (_currentFileDepth < 0 || _currentFileDepth > GlobalMaxDepth)
            {
                throw new IOException();
            }

            int indicesCount = 1 << _currentFileDepth;

            _count = reader.ReadInt32();
            if (_count < 0 || _count > indicesCount*data.MaxBlockSize)
            {
                throw new IOException();
            }

            _directory = new int[indicesCount];
            for (int i = 0; i < indicesCount; ++i)
            {
                int index = _directory[i] = reader.ReadInt32();
                if (index < 0 || index >= blocksCount)
                {
                    throw new IOException();
                }
            }
        }

        /// <summary>
        /// Uddatuje indexy v Directory zo starych na nove
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        internal void UpdateIndices(int oldIndex, int newIndex)
        {
            System.Diagnostics.Debug.Assert(!_directory.Contains(newIndex));
            for (int i = 0; i < _directory.Length; ++i)
            {
                if (_directory[i] == oldIndex)
                {
                    _directory[i] = newIndex;
                }
            }
        }

        /// <summary>
        /// Zmaze prazdny blok
        /// </summary>
        /// <returns>True ak bol blok zmazany</returns>
        internal bool RemoveBlockIfEmpty()
        {
            if (_count > 0)
            {
                return false;
            }

            int blockIndex = _directory[0];
            _directory = new int[0];
            _data.Blocks.RemoveAt(blockIndex);
            return true;
        }

        internal bool TryGetEqual(int hash, T value, out T existingValue)
        {
            var info = GetRecordInfo(hash, value);
            var block = _data.Blocks.Read(info.BlockIndex);
            return block.TryGetEqual(info.Hash, value, out existingValue);
        }

        public int ConvertHashToAdress(BitArray pole, int depthFile)
        {
            // vrati z hashu prevod na cislo - index v poli directory, 
            //BitArray reversed = Reverse(pole);
            int index = 0;
            for (int i = 0; i < depthFile; i++)
            {
                if (pole[i])
                {
                    index += (int)Math.Pow(2, i);
                }
            }

            return index;
        }

        /// <summary>
        /// Adding new record.
        /// </summary>
        /// <param name="hash">Hash of inserted data</param>
        /// <param name="data">Record data</param>
        /// <param name="updateIfExists">adding or updating flag</param>
        /// <returns>Result from set {AlreadyExists, Added, NotAdded}</returns>
        internal AddResult Add(int hash, T data, bool updateIfExists)
        {
            var info = GetRecordInfo(hash, data);

            var block = _data.Blocks.Read(info.BlockIndex);
            // ak taky zaznam existuje
            if (block.Find(hash, data, updateIfExists))
            {
                if (updateIfExists)
                {
                    _data.Blocks.Write(block, info.BlockIndex);
                }
                return AddResult.AlreadyExists;
            }

            while (true)
            {

                if (block.Records.Count < _data.MaxBlockSize)
                {
                    // je miesto v bloku
                    block.Records.Add(new Record<T>(info.Hash, data));
                    // zapis
                    _data.Blocks.Write(block, info.BlockIndex);
                    ++_count;
                    return AddResult.Added;
                }

                if (block.Depth == _currentFileDepth)
                {
                    if (block.Depth == _data.MaxDepth)
                    {
                        return AddResult.NotAdded;
                    }
                    // treba extendovat
                    Extend();
                }
                // splitovanie bloku
                Split(ref block, ref info);
            }
        }

        public bool Remove(int hash, T data, out T existingValue)
        {
            var info = GetRecordInfo(hash, data);

            var block = _data.Blocks.Read(info.BlockIndex);

            // ak sa nepodarilo zmazat zaznam, koniec
            if (!block.TryRemove(info.Hash, data, out existingValue))
            {
                return false;
            }
            else
            {
                // zniz aktualny pocet zaznamov
                --_count;
                // zapis blok
                _data.Blocks.Write(block, info.BlockIndex);

                // merguj bloky
                while (true)
                {
                    if (!TryMergeBlockSiblings(ref block, info))
                    {
                        break;
                    }
                    else
                    {
                        var maxBlockDepth = _data.Blocks.MaxBlockDepth;
                        while (_currentFileDepth > maxBlockDepth + _data.MaxUnusedDepth)
                        {
                            Reduce(); // zniz hlbku
                        }
                    }
                }
                return true;
            }
        }

        private bool TryMergeBlockSiblings(ref Block<T> block, RecordInfo info)
        {
            // nemam co alebo nemam kde mergovat bloky
            if (block.Depth == 0 || block.Records.Count > _data.MaxSiblingMergeBlockValues)
            {
                return false;
            }

            int blockLocalReferenceIndex = GetBlockIndexIndex(info.Hash, block.Depth);
            bool isFirstSibling = (blockLocalReferenceIndex & 1) == 0;

            // da naspat adresu podla prveho indexu
            uint firstReferenceIndex = (uint)blockLocalReferenceIndex << _currentFileDepth - block.Depth;
            uint blockReferences = 1u << _currentFileDepth - block.Depth;

            uint otherBlockReferenceIndex = firstReferenceIndex;

            if (isFirstSibling)
            {
                otherBlockReferenceIndex += blockReferences;
            }
            else
            {
                otherBlockReferenceIndex -= blockReferences;
            }

            int otherBlockIndex = _directory[otherBlockReferenceIndex];
            if (_directory[otherBlockReferenceIndex + blockReferences/2] != otherBlockIndex)
            {
                return false; // No sibling block
            }

            var otherBlock = _data.Blocks.Read(otherBlockIndex);
            if (block.Records.Count + otherBlock.Records.Count > _data.MaxSiblingMergeBlockValues)
            {
                return false;
            }

            var newBlock = new Block<T>(block.Depth - 1);
            newBlock.Records.AddRange(block.Records.Concat(otherBlock.Records));

            for (uint i = 0; i < blockReferences; ++i)
            {
                _directory[i + otherBlockReferenceIndex] = info.BlockIndex;
            }

            _data.Blocks.Write(newBlock, info.BlockIndex);
            _data.Blocks.RemoveAt(otherBlockIndex);

            block = newBlock;
            return true;
        }

        /// <summary>
        /// Rozdeli mnozinu adries ukazujucich na jeden blok na dve casti
        /// </summary>
        /// <param name="blockToSplit">Blok ktoreho zaznamy sa rozdeluju</param>
        /// <param name="info">Hash a data</param>
        private void Split(ref Block<T> blockToSplit, ref RecordInfo info)
        {
            int newDepth = blockToSplit.Depth + 1;
            Block<T> newBlock1 = new Block<T>(newDepth);
            Block<T> newBlock2 = new Block<T>(newDepth);

            foreach (var record in blockToSplit.Records)
            {
                bool inFirstBlock = (GetBlockIndexIndex(record.Hash, newDepth) & 1) == 0;
                (inFirstBlock ? newBlock1 : newBlock2).Records.Add(record);
            }

            // zapis
            _data.Blocks.Write(newBlock1, info.BlockIndex);
            int newIndex2 = _data.Blocks.Add(newBlock2);

            // uprava adries
            uint firstReferenceIndex = (uint)GetBlockIndexIndex(info.Hash, blockToSplit.Depth) << _currentFileDepth - blockToSplit.Depth;
            uint newBlockReferences = 1u << _currentFileDepth - newDepth; // novy unassigned int :P, count ktory prepise
            for (uint i = 0; i < newBlockReferences; ++i)
            {
                _directory[i + firstReferenceIndex + newBlockReferences] = newIndex2;
            }

            // zapise tam adresy prveho alebo druheho bloku, podla toho, ci novy bit je 0 alebo 1 
            bool valueInFirstBlock = (GetBlockIndexIndex(info.Hash, newDepth) & 1) == 0;
            blockToSplit = valueInFirstBlock ? newBlock1 : newBlock2;
            if (!valueInFirstBlock)
            {
                info.BlockIndex = newIndex2;
            }
        }

        /// <summary>
        /// Doubling the Directory if current depth of file == depth of block
        /// </summary>
        private void Extend()
        {
            var newDirectory = new int[_directory.Length * 2];

            for (int i = 0; i < newDirectory.Length; i++)
            {
                newDirectory[i] = _directory[i/2];
            }

            _directory = newDirectory;
            _currentFileDepth++;
        }

        /// <summary>
        /// Decreasing depth of a file. Opposite of Extending the Directory.
        /// </summary>
        private void Reduce()
        {
            int[] newDirectory = new int[_directory.Length / 2];
            for (int i = 0; i < newDirectory.Length; i++)
            {
                newDirectory[i] = _directory[i*2];
                System.Diagnostics.Debug.Assert(_directory[i * 2 + 1] == newDirectory[i]);
            }
            _directory = newDirectory;
            _currentFileDepth--;
        }

        struct RecordInfo
        {
            internal int Hash, BlockIndex;
        }

        private RecordInfo GetRecordInfo(int hash, T data)
        {
            return new RecordInfo
            {
                Hash = hash,
                BlockIndex = _directory[GetBlockIndexIndex(hash, _currentFileDepth)]
            };
        }

        // index bloku v Directory
        private int GetBlockIndexIndex(int hash, int depth)
        {
            if (depth == 0)
            {
                return 0;
            }

            return (int)((uint)hash >> 32 - depth);
        }

        /// <summary>
        /// Zisti, ci blok obsahuje zaznam s danymi datami
        /// </summary>
        /// <param name="hash">hash hladaneho recordu</param>
        /// <param name="data">Hladany record</param>
        /// <returns>True ak obsahuje take data</returns>
        internal bool Contains(int hash, T data)
        {
            var info = GetRecordInfo(hash, data);
            var block = _data.Blocks.Read(info.BlockIndex);
            return block.Contains(info.Hash, data);
        }

        // hlupe
        private void PrintDirectory()
        {
            Console.WriteLine("Directory depth " + _currentFileDepth);
            for (int i = 0; i < _directory.Length; i++)
            {
                var block = _data.Blocks.Read(_directory[i]);
                var records = block.Records;
                for (int j = 0; j < records.Count; j++)
                {
                    Console.WriteLine(records[j].ToString());
                }
            }
        }

        internal void SerializeTo(BinaryWriter writer)
        {
            writer.Write(_currentFileDepth);
            writer.Write(_count);
            System.Diagnostics.Debug.Assert(_directory.Length == 1 << _currentFileDepth);
            foreach (var index in _directory)
            {
                writer.Write(index);
            }
        }

        private void DebugPrint()
        {
            Console.WriteLine("Table with depth = {0}", _currentFileDepth);
            for (int i = 0; i < _directory.Length; ++i)
            {
                int index = _directory[i];
                var block = _data.Blocks.Read(index);
                int from = i;
                while (i < _directory.Length - 1 && _directory[i + 1] == index)
                    ++i;

                if (i == from)
                    Console.Write("{0}", i);
                else
                    Console.Write("{0}-{1}", from, i);

                if (block.Records.Count == 0)
                    Console.Write(": <empty>");

                bool isFirst = true;
                uint firstHashBits = unchecked((uint)~0);
                foreach (var hashedValue in block.Records)
                {
                    if (!isFirst)
                        Console.Write(" | ");

                    if (block.Depth != 0)
                    {
                        uint hashBits = (uint)hashedValue.Hash >> 32 - block.Depth;
                        if (isFirst)
                        {
                            firstHashBits = hashBits;
                            Console.Write(", hash {0}: ", Convert.ToString(hashBits, 2).PadLeft(block.Depth, '0'));
                        }
                        if (hashBits != firstHashBits)
                            throw new InvalidOperationException("Bug detected");
                    }
                    else
                    {
                        Console.Write(": ");
                    }
                    Console.Write(hashedValue.Data);
                    isFirst = false;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

    }
}
