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
                Debug.Assert(addedVinEcv);
            }

            bool added = CarsByEcv.Add(selectedCar, updateIfExists: true);
            Debug.Assert(added == !carExists);
            return added;
        }

        public bool TryDeleteCarByVin(string vin)
        {
            VinEcvRecord vinEcv;
            if (EcvsByVin.TryRemove(new VinEcvRecord { Vin = vin }, out vinEcv))
            {
                bool removed = CarsByEcv.TryRemove(new CarRecord { Ecv = vinEcv.Ecv });
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
    }
}
