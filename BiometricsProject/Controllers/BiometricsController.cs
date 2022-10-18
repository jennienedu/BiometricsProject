using BiometricsProject.Entities.Biometrics;
using BiometricsProject.Interface;
using BiometricsProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiometricsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiometricsController : ControllerBase
    {
        private readonly IBiometricService biometrics;

        private readonly ILogger<BiometricsController> _logger;

        public BiometricsController(ILogger<BiometricsController> logger, IBiometricService biometricService)
        {
            _logger = logger;
            biometrics = biometricService;
        }

        [HttpPost]
        [Route("add-new-staff")]
        public async Task<IActionResult> RegisterNewStaff(StaffDetails staff)
        {
            try
            {
                var response = await biometrics.AddStaffDetails(staff);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }

        }

        [HttpGet]
        [Route("get-all-staff")]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                var response = await biometrics.GetAllStaffDetails();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-staff-details")]
        public async Task<IActionResult> GetStaff(string groupname)
        {
            try
            {
                var response = await biometrics.GetStaffDetails(groupname);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpPost]
        [Route("update-staff-details")]
        public async Task<IActionResult> UpdateStaffDetails(StaffDetails staff)
        {
            try
            {
                var response = await biometrics.UpdateStaffDetails(staff);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpPost]
        [Route("staff-sign-in")]
        public async Task<IActionResult> StaffSignIn(BiometricDailyLog signIn)
        {
            try
            {
                var response = await biometrics.StaffSignIn(signIn);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpPost]
        [Route("staff-sign-out")]
        public async Task<IActionResult> StaffSignOut(BiometricDailyLog signOut)
        {
            try
            {
                var response = await biometrics.StaffSignOut(signOut);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-All-FingerData")]
        public async Task<IActionResult> GetAllFingerprintData()
        {
            try
            {
                var response = await biometrics.GetAllFingerprintData();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-staff-log")]
        public async Task<IActionResult> GetStaffLog(DateTime startdate, DateTime enddate)
        {
            try
            {
                var response = await biometrics.GetStaffLog(startdate, enddate);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-daily-staff-log")]
        public async Task<IActionResult> GetDailyStaffLog(DateTime startdate)
        {
            try
            {
                var response = await biometrics.GetDaillyStaffLog(startdate);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-Staff-details-by-username")]
        public async Task<IActionResult> GetStaffDetailsByUsername(string username)
        {
            try
            {
                var response = await biometrics.GeStaffDetailByUsername(username);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpPost]
        [Route("get-Staff-details-from-Ad")]
        public async Task<IActionResult> GetUserDataAsync(EnrollUser user)
        {
            try
            {
                var response = await biometrics.GetUserDataAsync(user);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-log")]
        public async Task<IActionResult> GetLog(DateTime StartDate, DateTime EndDate, string Location)
        {
            try
            {
                var response = await biometrics.GetLog(StartDate, EndDate, Location);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }

        [HttpGet]
        [Route("get-Staff-Log-By-Username-And-Date")]
        public async Task<IActionResult> GetStaffLogByUsernameAndDate(string username, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var response = await biometrics.GetStaffLogByUsernameAndDate(username, StartDate, EndDate);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest("Error occured while processing your request: " + e.Message + " : " + e.ToString());
            }
        }
    }
}
