using System;
using System.IO;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile.Model
{
    public class DriverRecord : SerializationHelper<DriverRecord>
    {
        public string Name { get; private set; } //max 35
        public string Surname { get; private set; } // max 35
        public int Id { get; private set; }
        public DateTime ValidUntil { get; private set; }
        public bool AllowedToDrive { get; private set; }
        public int InfractionCount { get; private set; }

        public DriverRecord()
        {
            Name = "";
            Surname = "";
            ValidUntil = DateTime.Today;
            AllowedToDrive = true;
            InfractionCount = 0;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object another)
        {
            if (another == null || another.GetType() != typeof(DriverRecord))
            {
                return false;
            }

            var driverRecord = (DriverRecord) another;
            return Id == driverRecord.Id;
        }

        public override int BlockSize
        {
            get { return 35 + 35 + 4 + 8 + 1 + 4; }
        }
        public override void Serialize(DriverRecord data, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(data.Name, 35));
            writer.Write(SerializeASCIIStringBytes(data.Surname, 35));
            writer.Write(data.Id);
            writer.Write(data.ValidUntil.ToBinary());
            writer.Write(data.AllowedToDrive);
            writer.Write(data.InfractionCount);
        }

        public override DriverRecord Deserialize(BinaryReader reader)
        {
            return new DriverRecord
            {
                Name = DeserializeASCIIString(reader, 35),
                Surname = DeserializeASCIIString(reader, 35),
                Id = reader.ReadInt32(),
                ValidUntil = DateTime.FromBinary(reader.ReadInt64()),
                AllowedToDrive = reader.ReadBoolean(),
                InfractionCount = reader.ReadInt32(),
            };
        }
    }
}
