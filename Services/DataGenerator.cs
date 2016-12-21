using System;
using System.IO;
using ExtendibleHashingFile.Model;

namespace ExtendibleHashingFile.Services
{
    public static class DataGenerator
    {
        private enum Names
        {
            Andreas, Peter, Raphael, Michael, Thobias, Sebastian, Bello, Anton, Ricco, Marek,
            Martina, Dominika, Ema, Dana, Petra, Klara, Denisa, Laura, Veronika, Nikoletta, Slavka
        }

        private enum Surnames
        {
            Benz, Gasiak, Hiesgen, Mruskovic, Szandor, Thun, Belica, Lieskovsky, Greksak, Franko,
            Bolibruch, Ivanis, Gaspar, Zidzik, Michalko, Harcek, Funtik, Tamaiero, Klapita, Ivanec, Kysela
        }

        private enum Prefixes //14
        {
            ZA, PD, PO, BB, TN, TR, KE, NR, NO, TT, BA, KS, VT, MI
        }

        private enum Postfixes //14
        {
            AB, BH, DS, KL, WE, RT, XS, OL, PL, FE, IL, HG, AC, AD
        }

        static string NextEnumValueString<T>(this Random rnd)
        {
            var values = Enum.GetValues(typeof(T));
            return values.GetValue(rnd.Next(values.Length - 1)).ToString();
        }

        public static void GenerateDriverData(Database db, int count, string path)
        {
            var rnd = new Random();
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                for (int i = 0; i < count; ++i)
                {
                    var r = new DriverRecord();

                    r.Id = i;
                    r.Name = rnd.NextEnumValueString<Names>();
                    r.Surname = rnd.NextEnumValueString<Surnames>();
                    r.ValidUntil = DateTime.Today.AddDays(rnd.Next(300));
                    r.AllowedToDrive = true;
                    r.InfractionCount = rnd.Next(2);

                    sw.WriteLine(r.ToString());

                    //Console.WriteLine(r.ToString());

                    db.AddOrUpdateDriver(r);
                }
            }
        }

        public static void GenerateCarData(Database db, int count, string path)
        {
            var rnd = new Random();
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                for (int i = 0; i < count; ++i)
                {
                    var r = new CarRecord();

                    var prefix = rnd.NextEnumValueString<Prefixes>();
                    var postfix = rnd.NextEnumValueString<Postfixes>();
                    r.Ecv = prefix + rnd.Next(100, 999) + postfix;
                    r.Vin = prefix + rnd.Next(100, 999) + postfix;
                    r.NumOfWheels = 4;
                    r.IsStolen = false;
                    r.EndOfEk = DateTime.Today.AddDays(rnd.Next(300));
                    r.EndOfStk = DateTime.Today.AddDays(rnd.Next(300));

                    sw.WriteLine(r.ToString());
                    //Console.WriteLine(r.ToString());
                    db.AddOrUpdateCar(r);
                }
            }
        }
    }
}
