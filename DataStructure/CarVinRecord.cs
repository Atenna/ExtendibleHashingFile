using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ExtendibleHashingFile.DataStructure
{
    class CarVinRecord : IRecord<CarVinRecord>
    {
        public string Ecv; // 7
        public string Vin; // 17
        public int NumOfWheels;
        public int Weight { get; private set; }
        public bool IsStolen { get; private set; }
        public DateTime EndOfStk { get; private set; }
        public DateTime EndOfEk { get; private set; }
        // napr # na doplnenie fix dlzky, get - doplni #, set ich odparsuje

        public CarVinRecord(string ecv, string vin, DateTime endStk, DateTime endEk, int weight, int wheels = 4, bool stolen = false)
        {
            Ecv = CompleteString(ecv, 7);
            Vin = CompleteString(vin, 17);
            EndOfStk = endStk;
            EndOfEk = endEk;
            Weight = weight;
            NumOfWheels = wheels;
            IsStolen = stolen;
        }

        public bool Equal(CarVinRecord data)
        {
            return data.Vin.Equals(Vin);
        }

        public BitArray HashCode()
        {
            return new BitArray(BitConverter.GetBytes(Vin.GetHashCode()));
        }

        public byte[] ToByteArray()
        {
            var ecv = Encoding.UTF8.GetBytes(Ecv);
            var vin = Encoding.UTF8.GetBytes(Vin);
            //var endStk;
            //var endEk;
            var weight = BitConverter.GetBytes(Weight);
            var wheels = BitConverter.GetBytes(NumOfWheels);
            var stolen = BitConverter.GetBytes(IsStolen);

            return ecv.Concat(vin).Concat(weight).Concat(wheels).Concat(stolen).ToArray();
        }

        public void FromByteArray(byte[] arr, int offset)
        {
            // data = reader pola (ktore pole, od indexu, velkost)
            // treba to konvertovat BitConverter.To..(data, 0) 
            throw new NotImplementedException();
        }

        public int Size()
        {
            // string, string, int, int, bool, datum, datum
            return 2*sizeof(int) + sizeof(bool) + 7*sizeof(char) + 17*sizeof(char); // todo datumy
        }

        public string CompleteString(string original, int length)
        {
            if (original.Length == length)
            {
                return original;
            }

            return original.PadRight(length, '#'); 
        }
    }
}
