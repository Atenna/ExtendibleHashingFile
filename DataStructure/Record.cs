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
        public int Hash { get; private set; }
        public T Data { get; private set; }
    }
}
