using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.DataStructure
{
    public abstract class SerializationHelper<T> : IBlockSerializer<T>
    {
        public abstract int BlockSize { get; }
        public abstract void Serialize(T data, BinaryWriter writer);
        public abstract T Deserialize(BinaryReader reader);

        public byte[] Serialize(T data)
        {
            var array = new byte[BlockSize];
            var stream = new MemoryStream(array);
            var writer = new BinaryWriter(stream);

            Serialize(data, writer);
            System.Diagnostics.Debug.Assert(stream.Position == stream.Length);

            return array;
        }

        public T Deserialize(byte[] array)
        {
            var stream = new MemoryStream(array);
            var reader = new BinaryReader(stream);

            var data = Deserialize(reader);
            System.Diagnostics.Debug.Assert(stream.Position == stream.Length);
            return data;
        }

        public static byte[] SerializeASCIIStringBytes(string str, int byteCount)
        {
            var bytes = Encoding.ASCII.GetBytes(str).Take(byteCount).ToList();
            if (bytes.Count < byteCount)
            {
                bytes.AddRange(Enumerable.Repeat<byte>(0, byteCount - bytes.Count));
            }
            return bytes.ToArray();
        }

        public static string DeserializeASCIIString(BinaryReader reader, int byteCount)
        {
            byte[] bytes = reader.ReadBytes(byteCount);

            int count = 0;
            while (count < bytes.Length && bytes[count] != 0)
            {
                ++count;
            }

            return Encoding.ASCII.GetString(bytes, 0, count);
        }
    }
}
