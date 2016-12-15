using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace ExtendibleHashingFile.DataStructure
{
    public class File<T>
    {
        //public int RecordCount { get; private set; }
        //public string FileName { get; private set; }
        //private HelperReader reader;
        //private HelperWriter writer;
        public int CurrentFileDepth { get; private set; }
        //public int BlockCount { get; private set; }
        public int[] Directory;

        public enum AddResult { NotAdded = 0, Added, AlreadyExists }
        public int Count { get; private set; }
        private readonly FileData<T> _data;

        public File(FileData<T> data)
        {
            _data = data;
            
            Directory = new int[1];
            CurrentFileDepth = 0;
            Block<T> b1 = new Block<T>(CurrentFileDepth);
            Directory[0] = _data.Blocks.Add(b1);
            
            //writer.Write(0, b1.ToByteArray());
        }

        public File(BinaryReader reader, FileData<T> data, int bucketsCount)
        {
            _data = data;

            CurrentFileDepth = reader.ReadInt32();
            if (CurrentFileDepth < 0 || CurrentFileDepth > ExtendibleHashSet<T>.GlobalMaxDepth)
            {
                throw new IOException();
            }

            int indicesCount = 1 << CurrentFileDepth;

            Count = reader.ReadInt32();
            if (Count < 0 || Count > indicesCount*data.MaxBlockSize)
            {
                throw new IOException();
            }

            Directory = new int[indicesCount];
            for (int i = 0; i < indicesCount; ++i)
            {
                int index = Directory[i] = reader.ReadInt32();
                if (index < 0 || index >= bucketsCount)
                {
                    throw new IOException();
                }
            }
        }

        public bool TryGetEqual(int hash, T value, out T existingValue)
        {
            var info = GetRecordInfo(value);
            var bucket = _data.Blocks.Read(info.BlockIndex);
            return bucket.TryGetEqual(info.Hash, value, out existingValue);
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
        /// <param name="data">Record data</param>
        /// <returns>Result from set {AlreadyExists, Added, NotAdded}</returns>
        public AddResult Add(int hash, T data, bool updateIfExists)
        {
            //WriteDirectory();
            var info = GetRecordInfo(data);
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
                    Record<T> record = new Record<T>(info.Hash, data);
                    block.Records.Add(record);
                    // zapis
                    _data.Blocks.Write(block, info.BlockIndex);
                    ++Count;
                    return AddResult.Added;
                }

                if (block.Depth == CurrentFileDepth)
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
            var info = GetRecordInfo(data);
            var block = _data.Blocks.Read(info.BlockIndex);

            if (block.TryRemove(info.Hash, data, out existingValue))
            {
                return false;
            }
            else
            {
                // zniz aktualny pocet zaznamov
                --Count;
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
                        int maxBlockDepth = _data.Blocks.MaxBlockDepth;
                        while (CurrentFileDepth > maxBlockDepth + _data.MaxUnusedDepth)
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
            if (block.Depth == 0 || block.Records.Count > _data.MaxSiblingMergeBlockValues)
            {
                return false;
            }

            int blockLocalReferenceIndex = GetBlockIndexIndex(info.Hash, block.Depth);
            bool isFirstSibling = (blockLocalReferenceIndex & 1) == 0;

            // da naspat adresu podla prveho indexu
            uint firstReferenceIndex = (uint)blockLocalReferenceIndex << CurrentFileDepth - block.Depth;
            uint blockReferences = 1u << CurrentFileDepth - block.Depth;

            uint otherBlockReferenceIndex = firstReferenceIndex;

            if (isFirstSibling)
            {
                otherBlockReferenceIndex += blockReferences;
            }
            else
            {
                otherBlockReferenceIndex -= blockReferences;
            }

            int otherBlockIndex = Directory[otherBlockReferenceIndex];
            if (Directory[otherBlockReferenceIndex + blockReferences/2] != otherBlockIndex)
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
                Directory[i + otherBlockReferenceIndex] = info.BlockIndex;
            }

            _data.Blocks.Write(newBlock, info.BlockIndex);
            _data.Blocks.RemoveAt(otherBlockIndex);

            block = newBlock;
            return true;
        }

        /// <summary>
        /// Debug method. Printing the Directory - only for depth check
        /// </summary>
        private void WriteDirectory()
        {
            string print = "";
            foreach (var address in Directory)
            {
                print += "[" + address + "]";
            }
            Console.WriteLine(print);
        }

        private void Split(ref Block<T> blockToSplit, ref RecordInfo info)
        {
            int address = Directory[info.BlockIndex];
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
            uint firstReferenceIndex = (uint)GetBlockIndexIndex(info.Hash, blockToSplit.Depth) << CurrentFileDepth - blockToSplit.Depth;
            uint newBlockReferences = 1u << CurrentFileDepth - newDepth; // novy unassigned int :P, count ktory prepise
            for (uint i = 0; i < newBlockReferences; ++i)
            {
                Directory[i + firstReferenceIndex + newBlockReferences] = newIndex2;
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
        /// Reversing BitArray with hashed key for computing the correct index in Directory.
        /// </summary>
        /// <param name="array">Hash as BitArray</param>
        /// <returns>Reversed hash</returns>
        public BitArray Reverse(BitArray array)
        {
           
            int length = array.Length;
            int mid = (length / 2);
            BitArray reversed = new BitArray(array.Length);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                reversed[i] = array[length - i - 1];
                reversed[length - i - 1] = bit;
            }

            return reversed;
        }

        /// <summary>
        /// Doubling the Directory if current depth of file == depth of block
        /// </summary>
        public void Extend()
        {
            int[] newDirectory = new int[Directory.Length * 2];

            for (int i = 0; i < newDirectory.Length; i++)
            {
                newDirectory[i] = Directory[i/2];
            }

            Directory = newDirectory;
            CurrentFileDepth++;
        }

        /// <summary>
        /// Decreasing depth of a file. Opposite of Extending the Directory.
        /// </summary>
        public void Reduce()
        {
            int[] newDirectory = new int[Directory.Length / 2];
            for (int i = 0; i < newDirectory.Length; i++)
            {
                newDirectory[i] = Directory[i*2];
            }
            Directory = newDirectory;
            CurrentFileDepth--;
        }

        public string SearchKey(T record)
        {
            if (Contains(record))
            {
                var info = GetRecordInfo(record);
                var block = _data.Blocks.Read(info.BlockIndex);
                var found = block.Records[block.IndexOf(info.Hash, record)];
                return found.ToString();
            }
            return "Record not found";
        }

        struct RecordInfo
        {
            internal int Hash, BlockIndex;
        }

        RecordInfo GetRecordInfo(T data)
        {
            int hash = data.GetHashCode();
            return new RecordInfo
            {
                Hash = hash,
                BlockIndex = Directory[GetBlockIndexIndex(hash,
                CurrentFileDepth)]
            };
        }

        // index bloku v Directory
        int GetBlockIndexIndex(int hash, int depth)
        {
            if (depth == 0)
            {
                return 0;
            }

            return (int)((uint)hash >> 32 - depth);
        }

        public bool Contains(T data)
        {
            var info = GetRecordInfo(data);
            var block = _data.Blocks.Read(info.BlockIndex);
            return block.Contains(info.Hash, data);
        }

        // updatne indexy na nove a stare, ak tam uz novy nie je
        internal void UpdateIndices(int oldIndex, int newIndex)
        {
            bool containsIndex = false;
            for (int i = 0; i < Directory.Length; i++)
            {
                if (Directory[i] == newIndex)
                {
                    containsIndex = true;
                    break;
                }
            }
            if (!containsIndex)
            {
                for (int i = 0; i < Directory.Length; ++i)
                {
                    if (Directory[i] == oldIndex)
                        Directory[i] = newIndex;
                }
            }
        }

        public bool RemoveBlockIfEmpty()
        {
            if (Count > 0)
            {
                return false;
            }

            int blockIndex = Directory[0];
            Directory = new int[0];
            _data.Blocks.RemoveAt(blockIndex);
            return true;
        }

        // hlupe
        public void PrintDirectory()
        {
            Console.WriteLine("Directory depth " + CurrentFileDepth);
            for (int i = 0; i < Directory.Length; i++)
            {
                var block = _data.Blocks.Read(Directory[i]);
                var records = block.Records;
                for (int j = 0; j < records.Count; j++)
                {
                    Console.WriteLine(records[j].ToString());
                }
            }
        }

        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(CurrentFileDepth);
            writer.Write(Count);
            System.Diagnostics.Debug.Assert(Directory.Length == 1 << CurrentFileDepth);
            foreach (var index in Directory)
                writer.Write(index);
        }

    }
}
