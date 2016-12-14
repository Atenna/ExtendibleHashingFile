using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public class FileData<T>
    {
        public BlockCollection<T> Blocks { get; private set; }
        public int MaxBlockSize { get; private set; }
        public int MaxSiblingMergeBlockValues { get; private set; }
        public int MaxDepth { get; private set; }
        public int MaxUnusedDepth { get; private set; }

        public FileData(Action<int, int> updateIndices, int maxBucketSize, int maxSiblingMergeBucketValues,
            int maxDepth, int maxUnusedDepth)
        {
            Blocks = new BlockCollection<T>(updateIndices);
            MaxBlockSize = maxBucketSize;
            MaxSiblingMergeBlockValues = maxSiblingMergeBucketValues;
            MaxDepth = maxDepth;
            MaxUnusedDepth = maxUnusedDepth;
        }
    }
}
