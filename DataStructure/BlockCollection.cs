using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public class BlockCollection<T>
    {
        readonly List<Block<T>> _buckets = new List<Block<T>>();
        readonly int[] _bucketsWithDepths = new int[32 + 1];
        readonly Action<int, int> _updateIndices; //delegatik

        internal Block<T> Read(int index)
        {
            return _buckets[index];
        }

        internal void Write(Block<T> bucket, int index)
        {
            --_bucketsWithDepths[_buckets[index].Depth];
            _buckets[index] = bucket;
            ++_bucketsWithDepths[bucket.Depth];
        }

        internal int Add(Block<T> bucket)
        {
            _buckets.Add(bucket);
            ++_bucketsWithDepths[bucket.Depth];
            return _buckets.Count - 1;
        }

        internal void RemoveAt(int index)
        {
            int lastIndex = _buckets.Count - 1;

            --_bucketsWithDepths[_buckets[index].Depth];
            _buckets[index] = _buckets[lastIndex];
            _buckets.RemoveAt(lastIndex);
            _updateIndices(lastIndex, index);
        }

        internal int MaxBucketDepth
        {
            get
            {
                for (int i = _bucketsWithDepths.Length - 1; i > 0; --i)
                {
                    if (_bucketsWithDepths[i] > 0)
                        return i;
                }

                return 0;
            }
        }

        internal BlockCollection(Action<int, int> updateIndices)
        {
            _updateIndices = updateIndices;
        }
    }
}
