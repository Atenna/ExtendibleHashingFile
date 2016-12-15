using System.IO;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile.Model
{
    public struct VinEcvRecord
    {
        public string Vin { get; set; } // max 17
        public string Ecv { get; set; } // max 7

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(VinEcvRecord))
                return false;

            var other = (VinEcvRecord)obj;

            return Vin == other.Vin;
        }

        public override int GetHashCode()
        {
            return Vin.GetHashCode();
        }
    }

    public sealed class VinEcvRecordSerializer : SerializationHelper<VinEcvRecord>
    {
        public override int BlockSize
        {
            get { return 17 + 7; }
        }

        public override void Serialize(VinEcvRecord value, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(value.Vin, 17));
            writer.Write(SerializeASCIIStringBytes(value.Ecv, 7));
        }

        public override VinEcvRecord Deserialize(BinaryReader reader)
        {
            return new VinEcvRecord
            {
                Vin = DeserializeASCIIString(reader, 17),
                Ecv = DeserializeASCIIString(reader, 7),
            };
        }
    }
}
