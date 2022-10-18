using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiometricsProject.Models
{
    public class BiometricsResponse
    {
        public bool IsSuccessful { get; set; }
        public string response { get; set; }
        public string FullName { get; set; }
    }
}
