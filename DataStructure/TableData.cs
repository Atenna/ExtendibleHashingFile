using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public class TableData<T>
    {
        public BlockCollection<T> Blocks { get; private set; }
        public int MaxBlockValues { get; private set; }
        public int MaxSiblingMergeBlockValues { get; private set; }
        public int MaxDepth { get; private set; }
        public int MaxUnusedDepth { get; private set; }

        public TableData(Action<int, int> updateIndices, int maxBucketValues, int maxSiblingMergeBucketValues,
            int maxDepth, int maxUnusedDepth)
        {
            Blocks = new BlockCollection<T>(updateIndices);
            MaxBlockValues = maxBucketValues;
            MaxSiblingMergeBlockValues = maxSiblingMergeBucketValues;
            MaxDepth = maxDepth;
            MaxUnusedDepth = maxUnusedDepth;
        }
    }
}
