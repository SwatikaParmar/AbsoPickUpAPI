using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
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
using Microsoft.AspNetCore.Routing;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private IPatientService _patientService;
        private IContentService _contentService;
        private UserManager<ApplicationUser> _userManager;
        private IMedicalHistoryService _medicalhistoryService;
        private IWebHostEnvironment _hostingEnvironment;
        public PatientController(IPatientService patientService, UserManager<ApplicationUser> userManager, IContentService contentService, IMedicalHistoryService medicalHistoryService, IWebHostEnvironment hostingEnvironment)
        {
            _patientService = patientService;
            _userManager = userManager;
            _contentService = contentService;
            _medicalhistoryService = medicalHistoryService;
            _hostingEnvironment = hostingEnvironment;
        }

        #region GetPatientBasicInfo
        [HttpGet]
        [Authorize]
        [Route("GetPatientBasicInfo")]
        public async Task<IActionResult> GetPatientBasicInfo()
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
                if (user != null)
                {
                    var PatientBasicInfoResponse = await _patientService.GetPatientBasicInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = PatientBasicInfoResponse,
                        message = "Basic Info" + ResponseMessages.msgShownSuccess,
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

        #region GetPatientAdditionalInfo
        [HttpGet]
        [Authorize]
        [Route("GetPatientAdditionalInfo")]
        public async Task<IActionResult> GetPatientAdditionalInfo()
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
                if (user != null)
                {
                    var patientAdditionalInfoResponse = await _patientService.GetPatientAdditionalInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = patientAdditionalInfoResponse,
                        message = "Additional Info" + ResponseMessages.msgShownSuccess,
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

        #region AddPatientHealthInfo
        [HttpPost]
        [Authorize]
        [Route("AddPatientHealthInfo")]
        public async Task<IActionResult> AddPatientHealthInfo(PatientAdditionalInfoVM model)
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
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgTokenExpired,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }

                var user = await _userManager.FindByIdAsync(CurrentUserId);
                var _patientHealthInfo = new PatientHealthInfo()
                {
                    UserId = user.Id,
                    Height = model.Height,
                    BloodGroup = model.BloodGroup,
                    Weight = model.Weight,
                    IsVegetarian = model.IsVegetarian,
                    UseAlcohol = model.UseAlcohol,
                    UseDrug = model.UseDrug,
                    UseSmoke = model.UseSmoke
                };
                bool result = _patientService.AddUpdatePatientHealthInfo(_patientHealthInfo);
                if (result)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = new { },
                        message = "Additional Info" + ResponseMessages.msgUpdationSuccess,
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong,
                        code = StatusCodes.Status200OK
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion

        #region UpdatePatientBasicInfo
        [HttpPost]
        [Authorize]
        [Route("UpdatePatientBasicInfo")]
        public async Task<IActionResult> UpdatePatientBasicInfo(UpdatePatientBasicInfoVM model)
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

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.GenderId = model.GenderId; 
                if(user.PhoneNumber != model.PhoneNumber)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.PhoneNumberConfirmed = false;
                }
                    
                user.DialCode = model.DialCode;
                user.DateOfBirth = DateTime.ParseExact(model.DateOfBirth, GlobalVariables.DefaultDateFormat, null);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    
                    var _Useraddress = new UserAddress()
                    {
                        UserId = user.Id,
                        CountryId = model.CountryId,
                        StateId = model.StateId,
                        City = model.City,
                        CurrentAddress = model.CurrentAddress 
                    };
                    bool status = _contentService.AddUpdateAddressInfo(_Useraddress);
                    if (status)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Basic Info" + ResponseMessages.msgUpdationSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        });
                    } 
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                } 
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion 

        #region GetPatientDashboardInfo
        [HttpGet]
        [Authorize(Roles = "Patient")]
        [Route("GetPatientDashboardInfo")]
        public async Task<IActionResult> GetPatientDashboardInfo()
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
                if (user != null)
                {
                    var _patientDashboardInfo = await _patientService.GetPatientDashboardInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _patientDashboardInfo,
                        message = "Dashboard Info" + ResponseMessages.msgShownSuccess,
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
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status500InternalServerError
                });
            }
        }
        #endregion

        #region "private Methods"
        private string GetPathAndFilename(string filename, string foldername)
        {
            string path = _hostingEnvironment.WebRootPath + "//" + foldername + "//";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path + filename;
        }
        #endregion

    }
}
