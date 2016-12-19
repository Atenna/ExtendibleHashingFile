using System;
using System.Diagnostics;
using ExtendibleHashingFile.DataStructure;

namespace ExtendibleHashingFile.Model
{
    public class Database
    {
        const int MaxBlockValues = 2;
        const int MaxSiblingMergeBlockValues = 2;
        const int MaxFileDepth = 3;
        const int MaxUnusedFileDepth = 0;

        public ExtendibleHashSet<CarRecord> CarsByEcv { get; private set; }
        public ExtendibleHashSet<VinEcvRecord> EcvsByVin { get; private set; }
        public ExtendibleHashSet<DriverRecord> Drivers { get; private set; }

        public Database(string pathPrefix)
        {
            string carsPath = pathPrefix + "Car";
            string vinEcvsPath = pathPrefix + "VinEcv";
            string driversPath = pathPrefix + "Driver";

            bool hasCars = ExtendibleHashSet<CarRecord>.ExtendibleHashSetExists(carsPath) &&
                ExtendibleHashSet<VinEcvRecord>.ExtendibleHashSetExists(vinEcvsPath);
            bool hasDrivers = ExtendibleHashSet<DriverRecord>.ExtendibleHashSetExists(driversPath);

            CarsByEcv = hasCars ?
                new ExtendibleHashSet<CarRecord>(carsPath, new CarRecordSerializer()) :
                new ExtendibleHashSet<CarRecord>(
                    carsPath, new CarRecordSerializer(),
                    maxBlockValues: MaxBlockValues,
                    maxSiblingMergeBlockValues: MaxSiblingMergeBlockValues,
                    maxTableDepth: MaxFileDepth,
                    maxUnusedTableDepth: MaxUnusedFileDepth);

            EcvsByVin = hasCars ?
                new ExtendibleHashSet<VinEcvRecord>(vinEcvsPath, new VinEcvRecordSerializer()) :
                new ExtendibleHashSet<VinEcvRecord>(
                    vinEcvsPath, new VinEcvRecordSerializer(),
                    maxBlockValues: MaxBlockValues,
                    maxSiblingMergeBlockValues: MaxSiblingMergeBlockValues,
                    maxTableDepth: MaxFileDepth,
                    maxUnusedTableDepth: MaxUnusedFileDepth);

            Drivers = hasDrivers ?
                new ExtendibleHashSet<DriverRecord>(driversPath, new DriverRecordSerializer()) :
                new ExtendibleHashSet<DriverRecord>(
                    driversPath, new DriverRecordSerializer(),
                    maxBlockValues: MaxBlockValues,
                    maxSiblingMergeBlockValues: MaxSiblingMergeBlockValues,
                    maxTableDepth: MaxFileDepth,
                    maxUnusedTableDepth: MaxUnusedFileDepth);
        }

        public void Dispose()
        {
            CarsByEcv.Dispose();
            EcvsByVin.Dispose();
            Drivers.Dispose();
        }

        // Car

        public CarRecord TryGetCarByVin(string vin)
        {
            VinEcvRecord vinEcv;
            return EcvsByVin.TryGetEqual(new VinEcvRecord { Vin = vin }, out vinEcv) ?
                TryGetCarByEcv(vinEcv.Ecv) : null;
        }

        public CarRecord TryGetCarByEcv(string ecv)
        {
            CarRecord car;
            return CarsByEcv.TryGetEqual(new CarRecord { Ecv = ecv }, out car) ? car : null;
        }

        // Returns true if added new car.
        public bool AddOrUpdateCar(CarRecord selectedCar)
        {
            bool carExists = false;
            var existingCarVin = TryGetCarByEcv(selectedCar.Ecv)?.Vin;
            if (existingCarVin != null)
            {
                selectedCar.Vin = existingCarVin;
                //existingCarVin = selectedCar.Vin;
                carExists = true;
            }
            else
            {
                VinEcvRecord vinEcv;
                if (EcvsByVin.TryGetEqual(new VinEcvRecord { Vin = selectedCar.Vin }, out vinEcv))
                {
                    selectedCar.Ecv = vinEcv.Ecv;
                    carExists = true;
                }
            }

            if (!carExists)
            {
                bool addedVinEcv = EcvsByVin.Add(new VinEcvRecord { Vin = selectedCar.Vin, Ecv = selectedCar.Ecv }, updateIfExists: false);
                //Console.WriteLine("Nenaslo take auto");
                //Debug.Assert(addedVinEcv);
            }

            bool added = CarsByEcv.Add(selectedCar, updateIfExists: true);
            Debug.Assert(added == !carExists);
            Console.WriteLine("Pridane do ECV-VIN tabulky");
            return added;
        }

        // toto >:(
        public bool TryDeleteCarByVin(string vin)
        {
            VinEcvRecord vinEcv;
            if (EcvsByVin.TryRemove(new VinEcvRecord { Vin = vin }, out vinEcv))
            {
                var r = new CarRecord();
                r.Ecv = vinEcv.Ecv;
                bool removed = CarsByEcv.TryRemove(r);
                Debug.Assert(removed);
                return true;
            }
            return false;
        }

        public bool TryDeleteCarByEcv(string ecv)
        {
            CarRecord car;
            if (CarsByEcv.TryRemove(new CarRecord { Ecv = ecv }, out car))
            {
                bool removed = EcvsByVin.TryRemove(new VinEcvRecord { Vin = car.Vin });
                Debug.Assert(removed);
                return true;
            }
            return false;
        }

        // Driver

        public DriverRecord TryGetDriver(int id)
        {
            DriverRecord driver;
            return Drivers.TryGetEqual(new DriverRecord { Id = id }, out driver) ? driver : null;
        }

        // Returns true if added new driver.
        public bool AddOrUpdateDriver(DriverRecord driver)
        {
            return Drivers.Add(driver, updateIfExists: true);
        }

        public bool TryDeleteDriver(int id)
        {
            return Drivers.TryRemove(new DriverRecord { Id = id });
        }

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

        public void GenerateDriverData()
        {
            Console.WriteLine("__________DRIVER DATA___________");
            Random rnd = new Random();
            Array names = Enum.GetValues(typeof(Names));
            Array surnames = Enum.GetValues(typeof(Surnames));
            for (int i = 0; i < 100; i++)
            {
                // name, surname, id, valid until, allowed to drive, count
                var name = names.GetValue(rnd.Next(20));
                var surname = surnames.GetValue(rnd.Next(20));
                var r = new DriverRecord();
                r.Id = i;
                r.Name = name.ToString();
                r.Surname = surname.ToString();
                r.ValidUntil = DateTime.Today.AddDays(rnd.Next(300));
                r.AllowedToDrive = true;
                r.InfractionCount = rnd.Next(2);
                Console.WriteLine(r.ToString());
                AddOrUpdateDriver(r);
            }
        }

        private enum Prefixes //14
        {
            ZA, PD, PO, BB, TN, TR, KE, NR, NO, TT, BA, KS, VT, MI
        }

        private enum Postfexes //14
        {
            AB, BH, DS, KL, WE, RT, XS, OL, PL, FE, IL, HG, AC, AD
        }

        public void GenerateCarData()
        {
            Console.WriteLine("__________CAR DATA___________");
            Random rnd = new Random();
            Array prefixes = Enum.GetValues(typeof(Prefixes));
            Array postfixes = Enum.GetValues(typeof(Postfexes));
            for (int i = 0; i < 100; i++)
            {
                // 
                var prefix = prefixes.GetValue(rnd.Next(13));
                var postfix = postfixes.GetValue(rnd.Next(13));
                var r = new CarRecord();
                r.Ecv = prefix + rnd.Next(100, 999).ToString() + postfix;
                r.Vin = prefix + rnd.Next(100, 999).ToString() + postfix;
                r.NumOfWheels = 4;
                r.IsStolen = false;
                r.EndOfEk = DateTime.Today.AddDays(rnd.Next(300));
                r.EndOfStk = DateTime.Today.AddDays(rnd.Next(300));

                Console.WriteLine(r.ToString());
                AddOrUpdateCar(r);
            }
        }

        public void GenerateData()
        {
            GenerateCarData();
            GenerateDriverData();
        }
    }
}
