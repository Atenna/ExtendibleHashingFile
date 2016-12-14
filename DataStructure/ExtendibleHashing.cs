using System;
using System.Collections;
using ExtendibleHashingFile.Services;

namespace ExtendibleHashingFile.DataStructure
{
    public class ExtendibleHashing<T>
    {
        //public int RecordCount { get; private set; }
        //public string FileName { get; private set; }
        private HelperReader reader;
        private HelperWriter writer;
        public int CurrentFileDepth { get; private set; }
        //public int BlockCount { get; private set; }
        public int[] Directory;
        internal enum AddResult { NotAdded = 0, Added, AlreadyExists }

        private TableData<T> _data;

        public ExtendibleHashing(string fileName, TableData<T> data)
        {
            _data = data;
            
            Directory = new int[1];
            CurrentFileDepth = 0;
            Block<T> b1 = new Block<T>(CurrentFileDepth);
            Directory[0] = _data.Blocks.Add(b1);
            
            //writer.Write(0, b1.ToByteArray());
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

        public bool Add(T record)
        {
            //WriteDirectory();
            bool isInserted = false;
            BitArray hash = record.HashCode();

            while (!isInserted)
            {

                int index = ConvertHashToAdress(hash, CurrentFileDepth); // index bloku, na ktory chcem vkladat
                //Console.WriteLine("Vkladam zaznam " + record.ToString() + " na index " + index);
                byte[] data = reader.ReadBlock(Directory[index], BlockSizeInBytes); // nacitanie dat bloku
                Block<T> b = new Block<T>(MaxBlockSize, CurrentFileDepth); // do tohto bloku chcem vkladat
                b.FromByteArray(data);

                if (b.MaxRecordCount == b.Count)
                {

                    if (CurrentFileDepth == b.Depth)
                    {
                        if (CurrentFileDepth == MaxFileDepth)
                        {
                            return false;
                        }
                        else
                        {
                            //Console.WriteLine("Extending directory");
                            Extend();
                            //WriteDirectory();
                            // prepocitat adresy<<
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Splitting directory");
                        Split(b, index);
                        //WriteDirectory();
                        //WriteBlock(b, index);
                    }
                    
                }
                else
                {
                    b.Add(record);
                    
                    //Console.WriteLine("After insert");
                    //WriteBlock(b, index);

                    writer.Write(Directory[index], b.ToByteArray());
                    RecordCount++;
                    isInserted = true;

                    Console.WriteLine("___________________________");
                    PrintDirectory();
                }
            }
            return isInserted;
        }

        private void WriteDirectory()
        {
            string print = "";
            foreach (var address in Directory)
            {
                print += "[" + address + "]";
            }
            Console.WriteLine(print);
        }

        private void Split(Block<T> blockToSplit, int index)
        {
            int address = Directory[index];

            Block<T> newBlock = new Block<T>(MaxBlockSize, ++blockToSplit.Depth);

            OrderRecords(blockToSplit, newBlock);

            int newAddress = BlockCount*BlockSizeInBytes;
            BlockCount++;

            int blocksToReindex = (int)Math.Pow(2, CurrentFileDepth - (newBlock.Depth-1));

            int i = 0;
            for (i = index; (i >= 0) && (Directory[i] == address); i--){}
            int first = i+1;
            for (i = index; (i < Directory.Length) && (Directory[i] == address); i++) { }
            int last = i-1;

            int mid = (last - first)/2;
            if (last - first == 1 || last - first == 0)
            {
                Directory[last] = newAddress;
            }

            else
            {
                for (i = mid + 1; i <= last; i++)
                {
                    Directory[i] = newAddress;
                }
            }

            writer.Write(newAddress, newBlock.ToByteArray());
            writer.Write(address, blockToSplit.ToByteArray());
        }

        private void OrderRecords(Block<T> blockToSplit, Block<T> newBlock)
        {
            // zaznamy s novym bitom 0 zostanu v povodnom bloku
            T[] records = new T[blockToSplit.Records.Length];
            for (int i = 0; i < records.Length; i++)
            {
                records[i] = new T();
            }
            int it = 0;

            blockToSplit.Count = 0;
            newBlock.Count = 0;

            for (int i = 0; i < blockToSplit.Records.Length; i++)
            {
                var record = blockToSplit.Records[i];
                bool bit = record.HashCode()[blockToSplit.Depth-1];
                if (bit)
                {
                    newBlock.Add(record);
                }
                else
                {
                    records[it] = record;
                    it++;
                }
            }

            blockToSplit.Records = records;
            blockToSplit.Count = it;
        }

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

        public void WriteBlock(Block<T> block, int index)
        {
            Console.WriteLine("Block at [{0}]", index);
            var records = block.Records;
            foreach (var r in records)
            {
                Console.WriteLine("[{0}] ", r.ToString());
            }
            Console.WriteLine("");
        }

        public string SearchKey(T record)
        {
            string ret = "";
            for (int i = 0; i < Directory.Length; i++)
            {
                byte[] data = reader.ReadBlock(Directory[i], BlockSizeInBytes); // nacitanie dat bloku
                Block<T> b = new Block<T>(MaxBlockSize, CurrentFileDepth); // do tohto bloku chcem vkladat
                b.FromByteArray(data);
                var records = b.Records;
                for (int j = 0; j < records.Length; j++)
                {
                    if (record.Equal(records[j]))
                    {
                        ret = records[j].ToString() + " | Found at " + Directory[i];
                    }
                }
            }
            return ret;
        }

        public bool Contains(T value)
        {
            // todo
            return false;
        }

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

        public void PrintDirectory()
        {
            Console.WriteLine("Directory depth " + CurrentFileDepth);
            for (int i = 0; i < Directory.Length; i++)
            {
                byte[] data = reader.ReadBlock(Directory[i], BlockSizeInBytes); // nacitanie dat bloku
                Block<T> b = new Block<T>(MaxBlockSize, CurrentFileDepth); // do tohto bloku chcem vkladat
                b.FromByteArray(data);
                var records = b.Records;
                Console.WriteLine("Block depth " + b.Depth);
                Console.WriteLine("index " + i + " address " + Directory[i]);
                for (int j = 0; j < records.Length; j++)
                {
                    Console.Write(records[j].ToString() + " | ");
                }
                Console.WriteLine();
            }
        }
    }
}
