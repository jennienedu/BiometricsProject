using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiometricsProject.Models
{
    public class StaffDetails
    {
        public bool IsSuccessful { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string groupName { get; set; }
        public string response { get; set; }
        public string Location { get; set; }
        public string OfficeAllocated { get; set; }
        public string FullName { get; set; }
        public List<Finger> Fingers { get; set; }
    }

    public class Finger
    {
        public string FingerData { get; set; }
    }

}
