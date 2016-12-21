namespace ExtendibleHashingFile.DataStructure
{
    public class TableData<T>
    {
        public BlockCollection<T> Blocks { get; }
        public int MaxBlockSize { get; }
        public int MaxSiblingMergeBlockValues { get; }
        public int MaxDepth { get; }
        public int MaxUnusedDepth { get; }

        public TableData(BlockCollection<T> blocks, int maxBlockValues,
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
