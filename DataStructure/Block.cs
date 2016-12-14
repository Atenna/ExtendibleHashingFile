using System;
using System.Collections;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;

namespace ExtendibleHashingFile.DataStructure
{
    public class Block<T>
    {
        public int Depth { get; private set; }
        public List<Record<T>> Records;

        internal Block(int depth)
        {
            Depth = depth;
            Records = new List<Record<T>>();
        }

        internal int IndexOf(int hash, T data)
        {
            var comparer = EqualityComparer<T>.Default;
            return Records.FindIndex(p => p.Hash == hash && comparer.Equals(p.Data, data));
        }

        internal bool Contains(int hash, T value)
        {
            return IndexOf(hash, value) != -1;
        }

        internal bool TryRemove(int hash, T value)
        {
            int index = IndexOf(hash, value);
            if (index == -1)
                return false;

            Records.RemoveAt(index);
            return true;
        }

    }

}
