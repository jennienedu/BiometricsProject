using AdLoginService;
using BiometricsProject.Entities.Biometrics;
using BiometricsProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiometricsProject.Interface
{
    public interface IBiometricService
    {
        Task<List<StaffBiometricRegistration>> GetAllStaffDetails();
        Task<BiometricsResponse> AddStaffDetails(StaffDetails biometricRegistration);
        Task<BiometricsResponse> UpdateStaffDetails(StaffDetails staffRegistration);
        Task<List<StaffBiometricRegistration>> GetStaffDetails(string groupname);
        Task<BiometricsResponse> StaffSignIn(BiometricDailyLog signIn);
        Task<BiometricsResponse> StaffSignOut(BiometricDailyLog signIn);
        Task<List<BiometricDailyLog>> GetStaffLog(DateTime startdate, DateTime enddate);
        Task<List<BiometricDailyLog>> GetDaillyStaffLog(DateTime startdate);
        Task<IEnumerable<IGrouping<string, ReportDTO>>> GetLog(DateTime StartDate, DateTime EndDate, string Location);
        //Task<List<List<BiometricDailyLog>>> GetLog(LogRequest logRequest);
        Task<List<Finger>> GetAllFingerprintData();
        Task<StaffBiometricRegistration> GeStaffDetailByUsername(string username);
        Task<AdResponse> GetUserDataAsync(EnrollUser user);
        Task<StaffLoginDTO> GetStaffLogByUsernameAndDate(string username, DateTime StartDate, DateTime EndDate);
    }
}
