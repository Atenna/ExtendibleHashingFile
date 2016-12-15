using System.IO;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile.Model
{
    public class VinEcvRecord : SerializationHelper<VinEcvRecord>
    {
        public string Vin { get; private set; }
        public string Ecv { get; private set; }

        public override bool Equals(object another)
        {
            if (another == null || another.GetType() != typeof(VinEcvRecord))
            {
                return false;
            }

            var anotherRecord = (VinEcvRecord) another;
            return Vin == anotherRecord.Vin;
        }

        public override int GetHashCode()
        {
            return Vin.GetHashCode();
        }

        public override int BlockSize
        {
            get { return 7 + 17; }
        }

        public override void Serialize(VinEcvRecord data, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(data.Vin, 17));
            writer.Write(SerializeASCIIStringBytes(data.Ecv, 7));
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
