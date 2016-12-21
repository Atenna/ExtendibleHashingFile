using System;
using System.IO;
using ExtendibleHashingFile.DataStructure;
using ExtendibleHashingFile.Services;

namespace ExtendibleHashingFile.Model
{
    public class CarRecord
    {
        public string Ecv { get; set; } // 7
        public string Vin { get; set; }
        public int NumOfWheels { get; set; }
        public int Weight { get; set; }

        public bool IsStolen { get; set; }
        public DateTime EndOfStk { get; set; }

        public DateTime EndOfEk { get; set; }

        public CarRecord()
        {
            Ecv = "";
            Vin = "";
            NumOfWheels = 4;
            IsStolen = false;
            Weight = 30000;
            EndOfStk = DateTime.Today;
            EndOfEk = DateTime.Today;
        }

        public override String ToString()
        {
            return "Ecv: " + Ecv + ", Vin: " + Vin + ", End of stk: " + EndOfStk.ToString("dd-MM-yyyy") + ", End of ek: " +
                   EndOfEk.ToString("dd-MM-yyyy");
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

    public sealed class CarRecordSerializer : BlockSerializer<CarRecord>
    {
        public override int BlockSize
        {
            get { return 7 + 17 + 4 + 4 + 1 + 8 + 8; }
        }

        public override void Serialize(CarRecord value, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(value.Ecv, 7));
            writer.Write(SerializeASCIIStringBytes(value.Vin, 17));
            writer.Write(value.NumOfWheels);
            writer.Write(value.Weight);
            writer.Write(value.IsStolen);
            writer.Write(value.EndOfStk.ToBinary());
            writer.Write(value.EndOfEk.ToBinary());
        }

        public override CarRecord Deserialize(BinaryReader reader)
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
