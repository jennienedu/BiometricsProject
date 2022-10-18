using AdLoginService;
using BiometricsProject.Contexts.Biometrics;
using BiometricsProject.Entities.Biometrics;
using BiometricsProject.Interface;
using BiometricsProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BiometricsProject.Services
{
    public class BiometricService : IBiometricService
    {
        private readonly IConfiguration _config;
        private readonly BiometricsContext insContext;
        private readonly string fingerPrintPath;
        private readonly AppConfig _appConfig;
        public BiometricService(IConfiguration configuration, BiometricsContext context, IOptions<AppConfig> options)
        {
            insContext = context;
            _config = configuration;
            //fingerPrintPath = _config.GetValue<string>("AppConfig:FilePath");
            _appConfig = options.Value;
            fingerPrintPath = _appConfig.FilePath;
        }

        public async Task<List<StaffBiometricRegistration>> GetAllStaffDetails()
        {
            return await insContext.StaffBiometricRegistrations.ToListAsync();
        }

        public async Task<BiometricsResponse> AddStaffDetails(StaffDetails biometric)
        {
            if (string.IsNullOrEmpty(biometric.firstName) || string.IsNullOrEmpty(biometric.lastName) ||
                string.IsNullOrEmpty(biometric.groupName) || string.IsNullOrEmpty(biometric.username) ||
                string.IsNullOrEmpty(biometric.email) || string.IsNullOrEmpty(biometric.Location) || string.IsNullOrEmpty(biometric.OfficeAllocated))
                return new BiometricsResponse() { response = "Missing fields", IsSuccessful = false };

            StaffBiometricRegistration biometricRegistration = new()
            {
                USERNAME = biometric.username,
                FIRSTNAME = biometric.firstName,
                LASTNAME = biometric.lastName,
                PHONENUMBER = biometric.phoneNumber,
                GROUPNAME = biometric.groupName,
                DATEINSERTED = DateTime.Now,
                EMAIL = biometric.email,
                LOCATION = biometric.Location,
                OFFICEALLOCATED = biometric.OfficeAllocated
            };

            #region SaveFingers
            string filePath = string.Empty;
            foreach (var item in biometric.Fingers)
            {
                filePath += await SaveBase64StringAsFile(item.FingerData, biometric.username + Guid.NewGuid().ToString()) + ",";
            }
            #endregion

            biometricRegistration.FINGER = filePath;
            BiometricsResponse resp = new BiometricsResponse();
            var existingUser = insContext.StaffBiometricRegistrations.Select(x => x.USERNAME).ToList();
            var exist = existingUser.Contains(biometricRegistration.USERNAME);
            if (exist)
            {
                return new BiometricsResponse() { response = "User already exists!!" };
            }
            else
            {
                insContext.StaffBiometricRegistrations.Add(biometricRegistration);
                insContext.SaveChanges();
                if (biometricRegistration.ID > 0)
                {
                    resp.IsSuccessful = true;
                    resp.response = ("User was created successfully");
                }
                else
                {
                    resp.response = ("User was not created successfully");
                }
            }
            return resp;
        }

        public async Task<BiometricsResponse> StaffSignIn(BiometricDailyLog signIn)
        {
            BiometricsResponse resp = new();
            if (string.IsNullOrEmpty(signIn.username))
            {
                resp.response = ("no username");
                return resp;
            }
            else if (string.IsNullOrEmpty(signIn.Location))
            {
                resp.response = ("no location specified");
                return resp;
            }
            var validationResponse = await Validation(signIn.username, true);
            if (validationResponse.IsSuccessful)
            {
                BiometricsLog log = new()
                {
                    USERNAME = signIn.username,
                    TIMEIN = signIn.timeIn,
                    TEMPERATURE = signIn.temperature,
                    LOCATION = signIn.Location
                };
                insContext.BiometricsLogs.Add(log);
                int saved = insContext.SaveChanges();
                if (saved > 0)
                {
                    var userDetails = await insContext.StaffBiometricRegistrations.Where(x => x.USERNAME.ToLower() == signIn.username.ToLower()).FirstOrDefaultAsync();
                    resp.IsSuccessful = true;
                    resp.FullName = userDetails != null ? userDetails.FIRSTNAME + " " + userDetails.LASTNAME : " ";
                    resp.response = (validationResponse.response);
                }
                else
                {
                    resp.response = ("Sign in unsuccessful");
                    resp.FullName = validationResponse.FullName;
                }
            }
            else
            {
                resp.response = validationResponse.response;
                resp.FullName = validationResponse.FullName;
            }
            return resp;
        }

        public async Task<BiometricsResponse> StaffSignOut(BiometricDailyLog signIn)
        {
            BiometricsResponse resp = new();
            if (string.IsNullOrEmpty(signIn.username))
            {
                resp.response = ("no username");
                return resp;
            }
            else if (string.IsNullOrEmpty(signIn.Location))
            {
                resp.response = ("no location specified");
                return resp;
            }
            var validationResponse = await Validation(signIn.username, false);
            if (validationResponse.IsSuccessful)
            {
                BiometricsLog biometricsLog = await insContext.BiometricsLogs.Where(x => x.USERNAME == signIn.username).OrderBy(x => x.TIMEIN).LastOrDefaultAsync();
                if (string.IsNullOrEmpty(biometricsLog.LOCATION))
                {
                    resp.response = "no location specified during sign in";
                    return resp;
                }
                if (biometricsLog == null)
                    resp.response = "Unable to fetch staff sign in record";
                if (biometricsLog.LOCATION.ToLower() != signIn.Location.ToLower())
                    resp.response = "Sign in and sign out location mismatch";
                biometricsLog.TIMEOUT = signIn.timeOut;
                insContext.BiometricsLogs.Update(biometricsLog);
                int updated = insContext.SaveChanges();
                if (updated > 0)
                {
                    var userDetails = await insContext.StaffBiometricRegistrations.Where(x => x.USERNAME.ToLower() == signIn.username.ToLower()).FirstOrDefaultAsync();
                    resp.IsSuccessful = true;
                    resp.FullName = userDetails != null ? userDetails.FIRSTNAME + " " + userDetails.LASTNAME : " ";
                    resp.response = ("Sign out successful");
                }
                else
                {
                    resp.response = ("Sign out unsuccessful");
                    resp.FullName = validationResponse.FullName;
                }
            }
            else
            {
                resp.response = validationResponse.response;
                resp.FullName = validationResponse.FullName;
            };
            return resp;
        }

        public async Task<StaffDetails> Validation(string username, bool isSignIn)
        {
            try
            {
                StaffBiometricRegistration log = await insContext.StaffBiometricRegistrations.Where(x => x.USERNAME == username).FirstOrDefaultAsync();
                if (log == null || string.IsNullOrEmpty(log.USERNAME))
                {
                    return new StaffDetails { response = "User not found" };
                }
                var biometricLog = await insContext.BiometricsLogs.Where(x => x.USERNAME == username).OrderBy(x => x.TIMEIN).LastOrDefaultAsync();
                if (biometricLog != null)
                {
                    if (isSignIn)
                    {
                        if (biometricLog.TIMEOUT == null)
                        {
                            biometricLog.TIMEOUT = DateTime.Now;
                            insContext.BiometricsLogs.Update(biometricLog);
                            int updated = insContext.SaveChanges();
                            if (updated > 0)
                                return new StaffDetails { IsSuccessful = true, response = "user did not sign out previously, automatically signed user out," + "\n" + "user signed in successfully", FullName = log != null ? log.FIRSTNAME + " " + log.LASTNAME : null };
                            else
                                return new StaffDetails { response = "unable to automatically sign user out", FullName = log != null ? log.FIRSTNAME + " " + log.LASTNAME : null };
                        }
                    }
                    else
                    {
                        if (biometricLog.TIMEOUT != null)
                            return new StaffDetails
                            {
                                response = "User already signed out",
                                FullName = log != null ? log.FIRSTNAME + " " + log.LASTNAME : null
                            };
                    }
                }
                else if (isSignIn && biometricLog == null)
                {
                    return new StaffDetails
                    {
                        IsSuccessful = true,
                        response = "Action successful",
                        FullName = log != null ? log.FIRSTNAME + " " + log.LASTNAME : null
                    };
                }
                else
                {
                    return new StaffDetails
                    {
                        response = "User not signed in",
                        FullName = log != null ? log.FIRSTNAME + " " + log.LASTNAME : null
                    };
                }
                return new StaffDetails
                {
                    IsSuccessful = true,
                    response = "Action successful",
                    FullName = log != null ? log.FIRSTNAME + " " + log.LASTNAME : null
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<BiometricsResponse> UpdateStaffDetails(StaffDetails biometric)
        {
            StaffBiometricRegistration biometricRegistration = new StaffBiometricRegistration();
            biometricRegistration.USERNAME = biometric.username;
            biometricRegistration.FIRSTNAME = biometric.firstName;
            biometricRegistration.LASTNAME = biometric.lastName;
            biometricRegistration.PHONENUMBER = biometric.phoneNumber;
            biometricRegistration.GROUPNAME = biometric.groupName;
            biometricRegistration.DATEINSERTED = DateTime.Now;
            BiometricsResponse resp = new BiometricsResponse();
            var result = await insContext.StaffBiometricRegistrations.SingleOrDefaultAsync(model => model.ID == biometricRegistration.ID);
            if (result == null)
            {
                return resp;
            }
            insContext.Entry(result).CurrentValues.SetValues(biometricRegistration);
            insContext.SaveChanges();
            if (result.ID > 0)
            {
                resp.response = ($"User details was updated successfully");
            }
            else
            {
                resp.response = ("User details was not updated successfully");
            }
            return resp;
        }

        public async Task<List<StaffBiometricRegistration>> GetStaffDetails(string groupname)
        {
            return await insContext.StaffBiometricRegistrations.Where(x => x.GROUPNAME == groupname).ToListAsync();
        }

        public async Task<List<BiometricDailyLog>> GetStaffLog(DateTime startdate, DateTime enddate)
        {
            return await insContext.BiometricsLogs.Where(x => x.TIMEIN >= startdate && x.TIMEIN <= enddate).Select(y => new BiometricDailyLog
            {
                temperature = y.TEMPERATURE,
                timeIn = y.TIMEIN,
                timeOut = y.TIMEOUT,
                username = y.USERNAME,
                Location = y.LOCATION
            }).ToListAsync();
        }

        public async Task<List<BiometricDailyLog>> GetDaillyStaffLog(DateTime startdate) //Not working perfectly
        {
            List<BiometricDailyLog> daillyLog = new();
            try
            {
                var dailyLog = await insContext.BiometricsLogs.Where(x => x.TIMEIN.Value.Day == startdate.Day).ToListAsync();
                foreach (var item in dailyLog)
                {
                    BiometricDailyLog biometricDailyLog = new()
                    {
                        temperature = item.TEMPERATURE,
                        timeIn = item.TIMEIN,
                        timeOut = item.TIMEOUT,
                        username = item.USERNAME,
                        Location = item.LOCATION
                    };
                    daillyLog.Add(biometricDailyLog);
                }
            }
            catch (Exception e)
            {
            }
            return daillyLog;
        }

        public async Task<IEnumerable<IGrouping<string, ReportDTO>>> GetLog(DateTime StartDate, DateTime EndDate, string Location)
        {
            var response = insContext.BiometricsLogs.Where(x => x.TIMEIN >= StartDate && x.TIMEIN <= EndDate && x.LOCATION.ToLower() == Location.ToLower()).ToList().OrderByDescending(x => x.TIMEIN);
            if (Location.ToLower().Trim() == "all")
            {
                response = insContext.BiometricsLogs.Where(x => x.TIMEIN >= StartDate && x.TIMEIN <= EndDate).ToList().OrderByDescending(x => x.TIMEIN);
            }
            List<ReportDTO> reportDTOs = new();
            foreach (var item in response)
            {
                var user = await insContext.StaffBiometricRegistrations.Where(x => x.USERNAME == item.USERNAME).FirstOrDefaultAsync();
                ReportDTO reportDTO = new()
                {
                    FULLNAME = user != null ? user.FIRSTNAME + " " + user.LASTNAME : null,
                    Department = user != null ? user.GROUPNAME : null,
                    OfficeAllocated = user != null ? user.OFFICEALLOCATED : null,
                    Location = item.LOCATION,
                    temperature = item.TEMPERATURE,
                    timeIn = item.TIMEIN,
                    timeOut = item.TIMEOUT,
                    username = item.USERNAME
                };
                reportDTOs.Add(reportDTO);
            }
            var Response = reportDTOs.GroupBy(x => x.username);
            return Response;
        }

        public async Task<StaffLoginDTO> GetStaffLogByUsernameAndDate(string username, DateTime StartDate, DateTime EndDate)
        {
            var response = insContext.BiometricsLogs.Where(x => x.TIMEIN >= StartDate && x.TIMEIN <= EndDate && x.USERNAME.ToLower() == username.ToLower()).ToList().OrderByDescending(x => x.TIMEIN);
            var user = await insContext.StaffBiometricRegistrations.Where(x => x.USERNAME == username).FirstOrDefaultAsync();
            if (user != null)
            {
                List<BiometricDailyLog> biometricDailyLogs = new();
                StaffLoginDTO staffLoginDTO = new()
                {
                    FULLNAME = user?.FIRSTNAME + " " + user.LASTNAME,
                    Department = user?.GROUPNAME,
                    OfficeAllocated = user?.OFFICEALLOCATED,
                    Location = user?.LOCATION
                };
                foreach (var item in response)
                {
                    BiometricDailyLog biometricDailyLog = new()
                    {
                        Location = item.LOCATION,
                        temperature = item.TEMPERATURE,
                        timeIn = item.TIMEIN,
                        timeOut = item.TIMEOUT
                    };
                    biometricDailyLogs.Add(biometricDailyLog);
                }
                staffLoginDTO.AttendanceLog = biometricDailyLogs;
                return staffLoginDTO;
            }
            else return new StaffLoginDTO() {FULLNAME = null, AttendanceLog = new List<BiometricDailyLog>(), Department = null, Location = null, OfficeAllocated = null };
        }

        public async Task<string> SaveBase64StringAsFile(string input, string FileName)
        {
            string ReturnedFileName = string.Empty;
            try
            {
                string path = fingerPrintPath;
                string fullPath = path + FileName;
                using (StreamWriter writetext = new StreamWriter(fullPath + ".txt"))
                {
                    writetext.WriteLine(input);
                }
                ReturnedFileName = fullPath;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return ReturnedFileName;
        }

        public async Task<List<Finger>> GetAllFingerprintData()
        {
            List<Finger> fingers = new();
            try
            {
                var fingerPrints = await insContext.StaffBiometricRegistrations.Select(x => x.FINGER).ToListAsync();
                foreach (string item in fingerPrints)
                {
                    string[] userFingerprints = item.Split(',');
                    for (int y = 0; y < userFingerprints.Length - 1; y++)
                    {
                        var testFile = userFingerprints[y].Trim() + ".txt";
                        if (File.Exists(testFile))
                        {
                            Finger finger = new();
                            finger.FingerData = File.ReadAllText(userFingerprints[y] + ".txt");
                            finger.FingerData = finger.FingerData.Trim();
                            fingers.Add(finger);
                        }
                        else
                        {
                            //file cant be found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return fingers;
        }

        public async Task<StaffBiometricRegistration> GeStaffDetailByUsername(string username)
        {
            return await insContext.StaffBiometricRegistrations.Where(x => x.USERNAME == username).FirstOrDefaultAsync();
        }

        public async Task<AdResponse> GetUserDataAsync(EnrollUser user)
        {
            ADUser adUser = new();
            AdResponse resp = new();
            try
            {
                ServiceSoapClient client = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap,
                    _appConfig.AdgatewayUrl, _appConfig.AdgatewayUrl.Contains("https") ?
                    ServiceSoapClient.HttpProtocolType.HttpsProtocol : ServiceSoapClient.HttpProtocolType.HttpProtocol);

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var response = await client.AuthenticateUserAsync(user.username, user.password);
                if (response)
                {
                    adUser = await client.getUserDataAsync(user.username, true);
                    if (adUser != null)
                    {
                        resp.DoesExist = true;
                        resp.ResponseMessage = "Successful Verification";
                        resp.Username = adUser.username;
                        resp.Department = adUser.Department;
                        resp.noOfDirectReport = adUser.NoOfDirectReport;
                        resp.isManagerOfManager = adUser.IsManagerOfManager;
                        resp.isLineManager = adUser.IsLineManager;
                        resp.MobileNo = adUser.MobileNo;
                        resp.noOfDirectReport = adUser.NoOfDirectReport;
                        resp.TelephoneNo = adUser.TelephoneNo;
                        resp.Group = adUser.Group;
                        resp.LineManagerFullname = adUser.LineManagerFullname;
                        resp.LineManagerUsername = adUser.LineManagerUsername;
                        resp.AllSubReports = adUser.AllSubReports;
                        resp.BirthDate = adUser.BirthDate;
                        resp.Designation = adUser.Designation;
                        resp.DirectReports = adUser.DirectReports;
                        resp.Division = adUser.Division;
                        resp.Unit = adUser.Unit;
                        resp.Lastname = adUser.lastname;
                        resp.Firstname = adUser.firstname;
                        resp.Email = adUser.Email;
                        resp.JobLevel = adUser.JobLevel;

                        return resp;
                    }
                    else return new AdResponse() { DoesExist = false, ResponseMessage = "incorrect username or password" };
                }
                else return new AdResponse() { DoesExist = false, ResponseMessage = "incorrect username or password" };
            }
            catch (Exception e)
            {
                throw new FaultException("Fault Exception, Ad Faulty  .....\n" + e.Message + "\n" + e.InnerException + "\n" + e.StackTrace);
            }
            return resp;
        }
    }
}

