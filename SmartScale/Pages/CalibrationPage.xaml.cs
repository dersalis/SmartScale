using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using SmartScale.ViewModels;

namespace SmartScale.Pages
{
    public partial class CalibrationPage : PhoneApplicationPage
    {
        // Kalibracja
        CalibratorViewModel _calibration = CalibratorViewModel.Instance;

        public CalibrationPage()
        {
            InitializeComponent();

            DataContext = _calibration;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Uruchom akcelerometr
            _calibration.StartAccelerometr();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Zapisz ustawienia
            _calibration.SaveCalibrationData();
            // Wyłącz akcelerometr
            _calibration.StopAccelerometr();
        }

        private void tilReset_Click(object sender, RoutedEventArgs e)
        {
            _calibration.Reset();
        }

        private void tilCalibration_Click(object sender, RoutedEventArgs e)
        {
            _calibration.Calibration();
        }
    }
}