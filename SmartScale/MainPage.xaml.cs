using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SmartScale.Resources;

using SmartScale.SmartScale;

namespace SmartScale
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ScaleManager _scale = ScaleManager.Instance;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            DataContext = _scale;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _scale.LoadCalibrationData();
            _scale.StartAccelerometr();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Zatrzymaj akcelerometr
            _scale.StopAccelerometr();
            // Usuń ustawienia - plik
            SettingsManager.DeleteSettings();
        }

        
        private void tilCalibration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/CalibrationPage.xaml", UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}