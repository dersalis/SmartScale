using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Devices.Sensors;
using System.Windows;

namespace SmartScale.SmartScale
{
    public static class Scale
    {

        //
        // Uruchamia ważenie i akcelerometr
        //
        public static bool StartScale(Accelerometer accelerometer, bool isActive)
        {
            // Sprawdz czy akcelerometr jest obsługiwany
            if (Accelerometer.IsSupported)
            {
                // Wykryto akcelerometr
                try
                {
                    // Uruchom akcelerometr
                    accelerometer.Start();
                    // Uruchomiono akcelerometr
                    isActive = true;
                }
                catch (AccelerometerFailedException)
                {
                    // Nie uruchomiono akcelerometru
                    isActive = false;
                    MessageBox.Show("Nie można uruchomić akcelerometru.");
                }
            }
            else
            {
                // Nie wykryto akcelerometru
                string title = "Brak akcelerometra";
                string message = "Aby program mógł działać wymagany jest akcelerometr!";
                MessageBox.Show(message, title, MessageBoxButton.OK);
            }

            return isActive;
        }


        //
        // Zatrzymuje ważenie i akcelerometr
        //
        public static bool StopScale(Accelerometer accelerometer, bool isActive)
        {
            try
            {
                // Zatrzymaj akcelerometr
                accelerometer.Stop();
                // Zatrzymano akcelerometr
                isActive = false;
            }
            catch (AccelerometerFailedException)
            {
                // Nie zatrzymano akcelerometru
                isActive = true;
                MessageBox.Show("Nie można wyłączyć akcelerometru.");
            }

            return isActive;
        }


        //
        // Oblicza wagę
        //
        public static float CalculateWeight(float currentAngle, float knownWeight = 0, float knownAngle = 0, float correctionAngle = 0)
        {
            /*
             * CEL:
             *  Oblicza wagę na podstawie odczytanego kąta oraz pozostałych zmiennych
             * PARAMETRY:
             *  currentAngle:float - wartość konta odczytana z akcelerometru;
             *  knownWeight:float - waga przedmiotu użytego do kalibracji - przedmiot wzorcowy;
             *  knownAngle:float - kąt wskazany przy ważeniu przedmiotu wzorcowego;
             *  correctionAngle:float - kąt korygujący wskazany podczas zerowania
             * 
             * WARTOŚĆ ZWRACANA:
             *  weight:float - waga;
             */

            // Oblicz wagę
            //float weight = (knownWeight * Math.Abs(currentAngle - correctionAngle)) / Math.Abs(knownAngle);
            float weight = (knownWeight * (Math.Abs(currentAngle) - Math.Abs(correctionAngle))) / (Math.Abs(knownAngle) - Math.Abs(correctionAngle));
            //float weight = (knownWeight * Math.Abs(currentAngle)) / Math.Abs(knownAngle);
            // Zwróć
            return weight;
        }
    }
}
