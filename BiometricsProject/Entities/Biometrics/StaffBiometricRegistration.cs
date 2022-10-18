using System;
using System.Collections.Generic;

#nullable disable

namespace BiometricsProject.Entities.Biometrics
{
    public partial class StaffBiometricRegistration
    {
        public long ID { get; set; }
        public string USERNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONENUMBER { get; set; }
        public string GROUPNAME { get; set; }
        public DateTime? DATEINSERTED { get; set; }
        public string FINGER { get; set; }
        public string LOCATION { get; set; }
        public string OFFICEALLOCATED { get; set; }
    }
}
