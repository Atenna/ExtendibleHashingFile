using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public interface IBlockSerializer<T>
    {
        int BlockSize { get; }
        byte[] Serialize(T data);
        T Deserialize(byte[] array);
    }
}
