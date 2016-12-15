using System;
using System.Collections;
using System.IO;
using System.Linq;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile.Model
{
    // testing class for implementing IRecord and testing the structure
    public class TestClass : SerializationHelper<TestClass>
    {
        public int Key { get; set; }
        public int Data { get; set; }

        public bool Equal(TestClass record)
        {
            return record.Key == Key;
        }

        public BitArray HashCode()
        {
            return new BitArray(BitConverter.GetBytes(Key.GetHashCode()));
        }
        

        public byte[] ToByteArray()
        {
            var key = BitConverter.GetBytes(Key);
            var data = BitConverter.GetBytes(Data);

            return key.Concat(data).ToArray();
        }

        public void FromByteArray(byte[] arr, int offset)
        {
            Key = BitConverter.ToInt32(arr, offset);
            Data = BitConverter.ToInt32(arr, offset + sizeof(int));
        }

        public int Size()
        {
            return 2*sizeof(int);
        }

        public override string ToString()
        {
            return Key + "," + Data;
        }

        public override int BlockSize
        {
            get { return sizeof(int)*2; }
        }

        public override void Serialize(TestClass data, BinaryWriter writer)
        {
            writer.Write(data.Key);
            writer.Write(data.Data);
        }

        public override TestClass Deserialize(BinaryReader reader)
        {
            return new TestClass()
            {
                Data = reader.ReadInt32(),
                Key = reader.ReadInt32()
            };
        }
    }
}
