using System.IO;
using ExtendibleHashingFile.Services;

namespace ExtendibleHashingFile.Model
{
    public class TestClass
    {
        public int Key { get; set; }
        public int Value { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(TestClass))
            {
                return false;
            }

            var other = (TestClass)obj;

            return Key == other.Key;
        }

        public sealed class TestClassRecordSerializer : BlockSerializer<TestClass>
        {
            public override int BlockSize
            {
                get { return sizeof(int) * 2; }
            }

            public override void Serialize(TestClass value, BinaryWriter writer)
            {
                writer.Write(value.Key);
                writer.Write(value.Value);
            }

            public override TestClass Deserialize(BinaryReader reader)
            {
                return new TestClass
                {
                    Key = reader.ReadInt32(),
                    Value = reader.ReadInt32(),
                };
            }
        }
    }
}
