using System;

namespace ExtendibleHashingFile.DataStructure
{
    public class FileData<T>
    {
        public BlockCollection<T> Blocks { get; private set; }
        public int MaxBlockSize { get; private set; }
        public int MaxSiblingMergeBlockValues { get; private set; }
        public int MaxDepth { get; private set; }
        public int MaxUnusedDepth { get; private set; }

        public FileData(BlockCollection<T> blocks, int maxBlockValues,
            int maxSiblingMergeBlockValues, int maxDepth, int maxUnusedDepth)
        {
            Blocks = blocks;
            MaxBlockSize = maxBlockValues;
            MaxSiblingMergeBlockValues = maxSiblingMergeBlockValues;
            MaxDepth = maxDepth;
            MaxUnusedDepth = maxUnusedDepth;
        }
    }
}
