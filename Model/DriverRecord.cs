using System;
using System.IO;
using ExtendibleHashingFile.DataStructure;
using ExtendibleHashingFile.Services;

namespace ExtendibleHashingFile.Model
{
    public class DriverRecord
    {
        public string Name { get; set; } //max 35
        public string Surname { get; set; } // max 35
        public int Id { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool AllowedToDrive { get; set; }
        public int InfractionCount { get; set; }

        public DriverRecord()
        {
            Name = "";
            Surname = "";
            ValidUntil = DateTime.Today;
            AllowedToDrive = true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(DriverRecord))
                return false;

            var other = (DriverRecord)obj;

            return Id == other.Id;
        }

        public override string ToString()
        {
            return Name + " " + Surname + ", valid until: " + ValidUntil + ", allowed to drive: " +
                   AllowedToDrive.ToString();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public sealed class DriverRecordSerializer : SerializationHelper<DriverRecord>
    {
        public override int BlockSize
        {
            get { return 35 + 35 + 4 + 8 + 1 + 4; }
        }

        public override void Serialize(DriverRecord value, BinaryWriter writer)
        {
            writer.Write(SerializeASCIIStringBytes(value.Name, 35));
            writer.Write(SerializeASCIIStringBytes(value.Surname, 35));
            writer.Write(value.Id);
            writer.Write(value.ValidUntil.ToBinary());
            writer.Write(value.AllowedToDrive);
            writer.Write(value.InfractionCount);
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
