using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiometricsProject.Models
{
    public class BiometricDailyLog
    {
        public string username { get; set; }
        public string fingerPrint { get; set; }
        public decimal? temperature { get; set; }
        public DateTime? timeIn { get; set; }
        public DateTime? timeOut { get; set; }
        public string Location { get; set; }
    }

    public class LogRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
    }
}
