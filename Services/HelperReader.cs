using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.Services
{
    class HelperReader
    {
        public string PathName { get; private set; }
        public HelperReader(string pathName)
        {
            PathName = pathName;
        }
        public byte[] ReadBlock(int offset, int blockSizeInBytes)
        {
            var res = new byte[blockSizeInBytes];
            FileStream fs = new FileStream(PathName, FileMode.Open);
            fs.Position = offset;
            fs.Read(res, 0, blockSizeInBytes);
            fs.Close();
            return res;
        }

        public byte[] ReadFromRecord(byte[] array, int fromIndex, int length)
        {
            return SubArray(array, fromIndex, length);
        }

        public static T[] SubArray<T>( T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
