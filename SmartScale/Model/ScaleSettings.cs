using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartScale.Model
{
    public class ScaleSettings
    {
        // Kąt korekcji określony podczas zerowania
        public float CorrectionAngle { get; set; }
        // Kąt podczas kalibracji przy wadze KnownWeight
        public float KnownAngle { get; set; }
        // Waga przedmiotu użytego podczas kalibracji
        public float KnownWeight { get; set; }
        // Określa czy wykonano kalibrację
        public bool IsCalibrated { get; set; }
    }
}
