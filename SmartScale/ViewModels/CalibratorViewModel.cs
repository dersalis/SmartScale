using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmartScale.SmartScale;
using SmartScale.Model;
using System.ComponentModel;
using Microsoft.Devices.Sensors;
using System.Windows;

namespace SmartScale.ViewModels
{
    public class CalibratorViewModel : INotifyPropertyChanged
    {
        // Kąt korekcji określony podczas zerowania
        private float _correctionAngle;
        // Kąt podczas kalibracji przy wadze KnownWeight
        private float _knownAngle;
        // Waga przedmiotu użytego podczas kalibracji
        public float KnownWeight { get; set; }
        // Wskazywana waga
        public float Weight { get; private set; }
        // Określa czy jest przeprowadzana kalibracja
        private bool _isCalibration;
        // Określa czy jest przeprowadzane zerowanie
        private bool _isReset;
        // Określa czy została wykonana poprawnie kalibracja
        private bool _isCalibrated;
        // Akcelerometr
        Accelerometer _accelerometer;
        // Określa czy akcelerometr jest aktywny
        public bool IsActive { get; private set; }
        // Określa czy reset jest aktywny
        public bool IsResetActive { get; private set; }
        // Określa czy kalibracja jest aktywna
        public bool IsCalibrationActive { get; private set; }
        // Wiadomość
        public string Message { get; private set; }
        // Czas pomiędzy odczytami akcelerometru
        private const int ACCELEROMETER_TIME_INTERVAL = 1000;
        // Licznik regetu - 5 cykli
        private int _resetCount;
        // Licznik kalibracji - 5 cykli
        private int _calibrationCount;
        // Wartości tymczasowe
        private float _tempCorrectionAngle;
        private float _tempCalibrationAngle;
        // Stałe liczniki 
        private const int CALIBRATION_COUNT = 5;
        private const int RESET_COUNT = 5;

        //
        // Konstruktor
        //
        CalibratorViewModel()
        { 
            // Ustaw wartości domyślne
            _correctionAngle = 0.001f;
            _knownAngle = 0.001f;
            KnownWeight = 1;
            _isCalibration = false;
            _isReset = false;
            IsResetActive = true;
            IsCalibrationActive = false;
            _resetCount = RESET_COUNT;
            _calibrationCount = CALIBRATION_COUNT;
            _tempCalibrationAngle = 0;
            _tempCorrectionAngle = 0;
            _isCalibrated = false;

            // Inicjuj akcelerometr
            _accelerometer = new Accelerometer();
            // Ustaw czas pomiędzy odczytami
            _accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(ACCELEROMETER_TIME_INTERVAL);
            // Czytanie 
            _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(_accelerometer_ReadingChanged);
        }


        //
        // Odczytuje zmiany akcelerometra
        //
        private void _accelerometer_ReadingChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }


        //
        // Uaktualnia zmiany akcelerometra
        //
        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            // Odczytaj współrzędną Y
            float y = (float)Math.Round(accelerometerReading.Acceleration.Y, 3);

            // Jeśli kąt jest mniejszy od 0
            if (y < 0)
            {
                // Jeśli ustawiono zerowanie
                if (_isReset)
                {
                    IsResetActive = false;
                    RaisePropertyChanged("IsResetActive");
                    IsCalibrationActive = false;
                    RaisePropertyChanged("IsCalibrationActive");
                    // Ustaw wiadomość
                    Message = "Trwa poziomowanie...";
                    RaisePropertyChanged("Message");
                    // Jeśli licznik resetu jest większy od 0
                    if (_resetCount > 0)
                    {
                        // Zmniejsz wartość licznika
                        _resetCount--;
                        // Określ kąt korekcji
                        _tempCorrectionAngle += y;
                    }
                    else
                    {
                        // Jeśli licznik resetu jest mniejszy lub równy 0
                        _resetCount = RESET_COUNT;
                        _correctionAngle = (float)Math.Round((_tempCorrectionAngle / _resetCount), 3);
                        _tempCorrectionAngle = 0;

                        _isReset = false;
                        IsResetActive = true;
                        RaisePropertyChanged("IsResetActive");
                        IsCalibrationActive = true;
                        RaisePropertyChanged("IsCalibrationActive");
                        Message = "";
                        RaisePropertyChanged("Message");
                    }
                }

                // Jeśli ustawiono kalibrację
                if (_isCalibration)
                {
                    IsResetActive = false;
                    RaisePropertyChanged("IsResetActive");
                    IsCalibrationActive = false;
                    RaisePropertyChanged("IsCalibrationActive");
                    // Ustaw wiadomość
                    Message = "Trwa kalibrowanie...";
                    RaisePropertyChanged("Message");
                    // Jeśli licznik kalibracji jest większy od 0
                    if (_calibrationCount > 0)
                    {
                        // Zmniejsz wartość licznika
                        _calibrationCount--;
                        // Określ kont kalibracji
                        _tempCalibrationAngle += y;
                    }
                    else
                    {
                        // Jeśli licznik kalibracji jest mniejszy lub równy 0
                        _calibrationCount = CALIBRATION_COUNT;
                        _knownAngle = (float)Math.Round((_tempCalibrationAngle / _calibrationCount), 3);
                        _tempCalibrationAngle = 0;

                        _isCalibrated = true;
                        _isCalibration = false;
                        IsResetActive = true;
                        RaisePropertyChanged("IsResetActive");
                        IsCalibrationActive = true;
                        RaisePropertyChanged("IsCalibrationActive");
                        Message = "";
                        RaisePropertyChanged("Message");
                    }
                }
                // Oblicz wagę
                Weight = Scale.CalculateWeight((float)y, KnownWeight, _knownAngle, _correctionAngle);
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
        // Resetuje wagę
        //
        public void Reset()
        {
            // Ustaw możliwość resetowania
            _isReset = true;
            // Ukryj
            IsResetActive = false;
            RaisePropertyChanged("IsResetActive");
        }


        //
        // Kalibruje wagę
        //
        public void Calibration()
        {
            // Uaktywnij kalibrowanie
            _isCalibration = true;
            // Ukryj
            IsCalibrationActive = false;
            RaisePropertyChanged("IsCalibrationActive");
            IsResetActive = false;
            RaisePropertyChanged("IsResetActive");
        }


        // 
        // Zapisuje ustawienia
        //
        public void SaveCalibrationData()
        {
            // Utwórz obiekt ustawień
            ScaleSettings settings = new ScaleSettings() { KnownWeight = KnownWeight, KnownAngle = _knownAngle, CorrectionAngle = _correctionAngle, IsCalibrated = _isCalibrated };
            // Zapisz ustawienia
            SettingsManager.Save(settings);
        }


        #region Singleton

        // Singleton
        private static CalibratorViewModel _instance;
        public static CalibratorViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CalibratorViewModel();
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
