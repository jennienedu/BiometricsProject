using System.Collections.Generic;

namespace BiometricsProject.Models
{
    public class StaffLoginDTO
    {
        public string FULLNAME { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string OfficeAllocated { get; set; }
        public List<BiometricDailyLog> AttendanceLog { get; set; }
    }
}
