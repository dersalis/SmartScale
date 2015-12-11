using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartScale.Model;
using System.ComponentModel;
using Microsoft.Devices.Sensors;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;

namespace SmartScale.SmartScale
{
    public class ScaleManager : INotifyPropertyChanged
    {
        // Akcelerometr
        Accelerometer _accelerometer;
        // Określa czy akcelerometr jest aktywny
        public bool IsActive { get; private set; }
        // Waga
        public float Weight { get; private set; }
        // Waga odczytana z kalibracji
        private float _knownWeight; // gramy
        // Kąt odczytany z kalibracji
        private float _knownAngle; // 
        // Kąt poprawka określony podczas kalibracji
        private float _correctionAngle;
        // Określa czy tykonano kalibrację
        public bool IsCalibrated { get; set; }
        // Czas pomiędzy odczytami akcelerometru
        private const int ACCELEROMETER_TIME_INTERVAL = 1000;

        //public double X { get; set; }
        //public double Y { get; set; }
        //public double Z { get; set; }


        // 
        // Konstruktor
        //
        public ScaleManager()
        { 
            // Inicjuj akcelerometr
            _accelerometer = new Accelerometer();
            // Ustaw czas pomiędzy odczytami
            _accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1000);
            // Czytanie 
            _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(_accelerometer_ReadingChanged);

            // Ustaw wartości domyślne
            IsCalibrated = false;
        }

        private void _accelerometer_ReadingChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }

        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            //X = accelerometerReading.Acceleration.X;
            //Y = accelerometerReading.Acceleration.Y;
            //Z = accelerometerReading.Acceleration.Z;

            //RaisePropertyChanged("X");
            //RaisePropertyChanged("Y");
            //RaisePropertyChanged("Z");

            // Odczytaj współrzędną Y
            float y = (float)Math.Round(accelerometerReading.Acceleration.Y, 3);

            // Jeśli kąt jest mniejszy od 0
            if (y < 0)
            {
                // Oblicz wagę
                Weight = Scale.CalculateWeight((float)y, _knownWeight, _knownAngle, _correctionAngle);
            }
            else
            { 
                // Waga jest równa 0
                Weight = 0;
            }
            RaisePropertyChanged("Weight");
        }


        //
        // Uruchamia akcelerometr
        //
        public void StartAccelerometr()
        {
            IsActive = Scale.StartScale(_accelerometer, IsActive);
        }


        //
        // Zatrzymuje akcelerometr
        //
        public void StopAccelerometr()
        {
            IsActive = Scale.StopScale(_accelerometer, IsActive);
        }


        //
        // Odczytuje dane z kalibracji
        //
        public void LoadCalibrationData()
        {
            // Odczytane ustawienia
            ScaleSettings settings = SettingsManager.Load();
            if (settings != null)
            {
                _knownWeight = settings.KnownWeight;
                _knownAngle = settings.KnownAngle;
                _correctionAngle = settings.CorrectionAngle;
                IsCalibrated = settings.IsCalibrated;
                RaisePropertyChanged("IsCalibrated");
            }
        }


        #region Singleton
        // Singleton
        private static ScaleManager _instance;
        public static ScaleManager Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = new ScaleManager();
                return _instance;
            }
        }

        #endregion


        #region INotifyPropertyChanged

        // Deklaracja ReisePropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }
}
