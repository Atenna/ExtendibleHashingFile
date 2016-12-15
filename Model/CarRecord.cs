using System;
using System.IO;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile.Model
{
    public class CarRecord : SerializationHelper<CarRecord>
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

        public override int BlockSize
        {
            get { return 7 + 17 + 4 + 4 + 1 + 8 + 8; }
        }

        public override void Serialize(CarRecord data, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(data.Ecv, 7));
            writer.Write(SerializeASCIIStringBytes(data.Vin, 17));
            writer.Write(data.NumOfWheels);
            writer.Write(data.Weight);
            writer.Write(data.IsStolen);
            writer.Write(data.EndOfStk.ToBinary());
            writer.Write(data.EndOfEk.ToBinary());
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
