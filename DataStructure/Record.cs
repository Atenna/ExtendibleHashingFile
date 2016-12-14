using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public class Record<T>
    {
        // hashed value
        public int Hash { get; set; }
        public T Data { get; set; }

        public Record(int hash, T data)
        {
            Hash = hash;
            Data = data;
        }

        public override string ToString()
        {
            return Hash + ", " + Data.ToString();
        }
    }
}
