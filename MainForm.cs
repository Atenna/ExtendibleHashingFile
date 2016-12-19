using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendibleHashingFile.Model;

namespace PoliceSystem
{
    public partial class MainForm : Form
    {
        const string dataBaseFileName = "PoliceSystemDataBase";

        Database database;
        CarRecord selectedCar;
        DriverRecord selectedDriver;
        bool deleteByVin;

        public MainForm()
        {
            InitializeComponent();
            database = new Database(Path.Combine(Application.StartupPath, dataBaseFileName));

            database.GenerateData();

            pgdCar.SelectedObject = selectedCar = new CarRecord();
            pgdDriver.SelectedObject = selectedDriver = new DriverRecord();
        }

        // Form events

        void MainFormClosed(object sender, FormClosedEventArgs e)
        {
            database.Dispose();
            database = null;
        }

        // Button click events

        void OnSearchCarByVinButtonClick(object sender, EventArgs e)
        {
            UpdateCar(database.TryGetCarByVin(tbxCarVin.Text), byVin: true);
        }

        void OnSearchCarByEcvButtonClick(object sender, EventArgs e)
        {
            UpdateCar(database.TryGetCarByEcv(tbxCarEcv.Text), byVin: false);
        }

        void OnUpdateOrInsertCarButtonClick(object sender, EventArgs e)
        {
            string vin = selectedCar.Vin;
            string ecv = selectedCar.Ecv;
            bool addedNew = database.AddOrUpdateCar(selectedCar);
            bool vinChanged = vin != selectedCar.Vin;
            if (vinChanged || ecv != selectedCar.Ecv)
            {
                pgdCar.Refresh();

                ShowWarning(string.Format("{0} set to '{1}', car information updated.",
                    vinChanged ? "Vin" : "Ecv",
                    vinChanged ? selectedCar.Vin : selectedCar.Ecv));
            }
            else
            {
                ShowInfo(addedNew ? "New car added." : "Car information updated.");
            }
        }

        void OnDeleteCarByVinButtonClick(object sender, EventArgs e)
        {
            if (!database.TryDeleteCarByVin(tbxCarVin.Text))
                ShowError("Car not found by Vin.");
        }

        void OnDeleteCarByEcvButtonClick(object sender, EventArgs e)
        {
            if (!database.TryDeleteCarByEcv(tbxCarEcv.Text))
                ShowError("Car not found by Ecv.");
        }

        void OnSearchDriverButtonClick(object sender, EventArgs e)
        {
            int driverId;
            if (!int.TryParse(tbxDriverId.Text, out driverId))
            {
                ShowError("Invalid driver ID.");
                return;
            }

            UpdateDriver(database.TryGetDriver(driverId));
        }

        void OnUpdateOrInsertDriverButtonClick(object sender, EventArgs e)
        {
            bool addedNew = database.AddOrUpdateDriver(selectedDriver);
            ShowInfo(addedNew ? "New driver added." : "Driver information updated.");
        }

        void OnDeleteDriverButtonClick(object sender, EventArgs e)
        {
            int driverId;
            if (!int.TryParse(tbxDriverId.Text, out driverId))
            {
                ShowError("Invalid driver ID.");
                return;
            }

            if (!database.TryDeleteDriver(driverId))
                ShowError("Driver not found.");
        }

        // Update

        void UpdateCar(CarRecord car, bool byVin)
        {
            if (car == null)
            {
                ShowError("Car not found.");
                return;
            }

            btnDeleteCar.Enabled = car != null;
            btnDeleteCar.Text = car == null ? "Delete" :
                byVin ? "Delete by Vin" : "Delete by Ecv";

            selectedCar = car;
            pgdCar.SelectedObject = selectedCar;

            deleteByVin = byVin;
        }

        void UpdateDriver(DriverRecord driver)
        {
            if (driver == null)
            {
                ShowError("Driver not found.");
                return;
            }

            selectedDriver = driver;
            pgdDriver.SelectedObject = selectedDriver;
        }

        // Messages

        void ShowInfo(string text)
        {
            MessageBox.Show(text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void ShowWarning(string text)
        {
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        void ShowError(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
