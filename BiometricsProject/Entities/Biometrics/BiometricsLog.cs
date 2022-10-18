using System;
using System.Collections.Generic;

#nullable disable

namespace BiometricsProject.Entities.Biometrics
{
    public partial class BiometricsLog
    {
        public long ID { get; set; }
        public string USERNAME { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public DateTime? TIMEIN { get; set; }
        public DateTime? TIMEOUT { get; set; }
        public string LOCATION { get; set; }
    }
}
