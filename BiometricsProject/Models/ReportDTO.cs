using System;

namespace BiometricsProject.Models
{
    public class ReportDTO
    {
        public string FULLNAME { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public decimal? temperature { get; set; }
        public DateTime? timeIn { get; set; }
        public DateTime? timeOut { get; set; }
        public string username { get; set; }
        public string OfficeAllocated { get; set; }
    }
}
