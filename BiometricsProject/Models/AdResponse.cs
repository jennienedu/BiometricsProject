namespace BiometricsProject.Models
{
    public class AdResponse
    {
        public bool DoesExist {get; set; }
        public string ResponseMessage {get; set; }
        public string Username {get; set; }

        public string Fullname {get; set; }

        public string Firstname {get; set; }

        public string Lastname {get; set; }

        public string TelephoneNo {get; set; }

        public string MobileNo {get; set; }

        public string Department {get; set; }

        public string Email {get; set; }

        public bool isLineManager {get; set; }

        public bool isManagerOfManager {get; set; }

        public int noOfDirectReport {get; set; }

        public string[] DirectReports {get; set; }

        public string LineManagerFullname {get; set; }

        public string LineManagerUsername {get; set; }

        public string JobLevel {get; set; }

        public string Division {get; set; }

        public string Group {get; set; }

        public string Unit {get; set; }

        public string BirthDate {get; set; }

        public string Designation {get; set; }

        public string[] AllSubReports {get; set; }
    }
}
