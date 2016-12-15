using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendibleHashingFile.Model
{
    public class CarRecord
    {
        public string Ecv { get; private set; } // 7
        public string Vin { get; private set; }
        public int NumOfWheels { get; private set; }
        public int Weight { get; private set; }

        public bool IsStolen { get; private set; }
        public DateTime EndOfStk { get; private set; }

        public DateTime EndOfEk { get; private set; }

        public CarRecord()
        {
            Ecv = "";
            Vin = "";
            NumOfWheels = 4;
            EndOfStk = DateTime.Today;
            EndOfEk = DateTime.Today;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(CarRecord))
                return false;

            var other = (CarRecord)obj;

            return Ecv == other.Ecv;
        }

        public override int GetHashCode()
        {
            return Ecv.GetHashCode();
        }
    }

    public sealed class CarRecordSerializer : SerializationHelper<CarRecord>
    {
        public override int BlockSize
        {
            get { return 7 + 17 + 4 + 4 + 1 + 8 + 8; }
        }

        protected override void Serialize(CarRecord value, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(value.Ecv, 7));
            writer.Write(SerializeASCIIStringBytes(value.Vin, 17));
            writer.Write(value.NumOfWheels);
            writer.Write(value.Weight);
            writer.Write(value.IsStolen);
            writer.Write(value.EndOfStk.ToBinary());
            writer.Write(value.EndOfEk.ToBinary());
        }

        protected override CarRecord Deserialize(BinaryReader reader)
        {
            return new CarRecord
            {
                Ecv = DeserializeASCIIString(reader, 7),
                Vin = DeserializeASCIIString(reader, 17),
                NumOfWheels = reader.ReadInt32(),
                Weight = reader.ReadInt32(),
                IsStolen = reader.ReadBoolean(),
                EndOfStk = DateTime.FromBinary(reader.ReadInt64()),
                EndOfEk = DateTime.FromBinary(reader.ReadInt64()),
            };
        }
    }
}
