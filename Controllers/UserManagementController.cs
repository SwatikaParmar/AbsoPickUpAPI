using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IWebHostEnvironment _hostingEnvironment;
        private IAuthService _authService; 
        private IPatientService _patientService;
        private IDoctorService _doctorService;
        public UserManagementController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IWebHostEnvironment hostingEnvironment, IAuthService authService, IPatientService patientService, IDoctorService doctorService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
            _authService = authService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        #region GetAdminDoctorsList
        [HttpPost]
        [Authorize]
        [Route("GetAdminDoctorsList")]
        public async Task<IActionResult> GetAdminDoctorsList(FilterationListViewModel model)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var doctorListResponse = _doctorService.GetAdminDoctorsList(model);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = doctorListResponse,
                        message = "Doctors List" + ResponseMessages.msgShownSuccess,
                        code = StatusCodes.Status200OK 
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion

        #region GetDoctor
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("GetDoctor")]
        public async Task<IActionResult> GetDoctor(string DoctorId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if(user != null)
                {
                    var doctorDetailedInfoResponse = await _doctorService.GetDoctorDetailInfo(DoctorId);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = doctorDetailedInfoResponse,
                        message = "Doctor Detail Info" + ResponseMessages.msgShownSuccess,
                        code = StatusCodes.Status200OK

                    });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                }
            }
            catch(Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion

        #region UpdateDoctorApplicationStatus
        [HttpPost]
        [Authorize]
        [Route("UpdateDoctorApplicationStatus")]
        public async Task<IActionResult> UpdateDoctorApplicationStatus(UpdateDoctorApplicationStatus model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgParametersNotCorrect,
                        code = StatusCodes.Status200OK
                    });
                }

                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    bool result = _doctorService.UpdateDoctorApplicationStatus(model);
                    if (result)
                    {
                        var doctor = await _userManager.FindByIdAsync(model.UserId);
                        string doctorName = doctor.FirstName + " " + doctor.LastName;
                        if (model.ApplicationStatus == (int)GlobalVariables.DoctorApplicationStatus.Accepted)
                        {
                            var message = EmailMessages.GetDoctorApprovedByAdminMsg(doctorName);
                            await _emailSender.SendEmailAsync(doctor.Email, EmailMessages.adminApprovedDoctorEmailSubject, message);
                        }
                        if(model.ApplicationStatus == (int)GlobalVariables.DoctorApplicationStatus.Rejected)
                        {
                            var message = EmailMessages.GetDoctorRejectedByAdminMsg(doctorName);
                            await _emailSender.SendEmailAsync(doctor.Email, EmailMessages.adminRejectedDoctorEmailSubject, message);
                        }

                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Doctor Status" + ResponseMessages.msgUpdationSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion

        #region GetAdminPatientList
        [HttpPost]
        [Authorize]
        [Route("GetAdminPatientList")]
        public async Task<IActionResult> GetAdminPatientList(FilterationListViewModel model)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var patientListResponse = _patientService.GetAdminPatientsList(model);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = patientListResponse,
                        message = "Patient List" + ResponseMessages.msgShownSuccess,
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion
    }
}
