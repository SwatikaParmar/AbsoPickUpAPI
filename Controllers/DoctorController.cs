using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.Repositories;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        public DoctorController(IDoctorService doctorService, UserManager<ApplicationUser> userManager, IPatientService patientService, IAuthService authService)
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _patientService = patientService;
            _authService = authService;
        }

        #region AddDoctorLanguage
        [HttpPost]
        [Authorize]
        [Route("AddLanguage")]
        public async Task<IActionResult> AddDoctorLanguage(DoctorLanguageViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string CurrentUserId = CommonFunctions.getUserId(User);
                    if (string.IsNullOrEmpty(CurrentUserId))
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                    }
                    var user = await _userManager.FindByIdAsync(CurrentUserId);
                    if (user != null && !user.IsDeleted)
                    {

                        ArrayLanguageIdVM request = new ArrayLanguageIdVM();
                        request.languageId = model.languageId;
                        request.UserId = user.Id;

                        bool returnStatus = _doctorService.AddDoctorLanguage(request);
                        if (returnStatus)
                        {
                            return Ok(new ApiResponseModel { status = true, data = new { }, message = ResponseMessages.msgAdditionSuccess, code = StatusCodes.Status200OK });
                        }
                        else
                        {
                            return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                        }
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgBlockOrInactiveUserNotPermitted, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgParametersNotCorrect, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }

        #endregion

        #region UpdateDoctorSpeciality
        [HttpPost]
        [Authorize]
        [Route("UpdateSpeciality")]
        public async Task<IActionResult> UpdateDoctorSpeciality(ArraySpecialityIdVM model)
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
                user.Experience = model.Experience;
                var status = await _userManager.UpdateAsync(user);
                if (status.Succeeded)
                {
                    var doctorSpecialityViewModel = new DoctorSpecialityViewModel()
                    {
                        UserId = user.Id,
                        SpecialityId = model.SpecialityId
                    };
                    var result = _doctorService.UpdateDoctorSpeciality(doctorSpecialityViewModel);
                    if (result)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = ResponseMessages.msgAdditionSuccess,
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
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + " while updating experience.", code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion

        #region AddEducation
        [HttpPost]
        [Authorize]
        [Route("AddEducation")]
        public async Task<IActionResult> AddDoctorEducation(ArrayDoctorEducationalViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string CurrentUserId = CommonFunctions.getUserId(User);
                    if (string.IsNullOrEmpty(CurrentUserId))
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                    }
                    var user = await _userManager.FindByIdAsync(CurrentUserId);
                    var doctorEducationDetails = new ArrayAddDoctorEducationVM()
                    {
                        ArrDoctorEducationalViewModel = model.ArrDoctorEducationalViewModel,
                        UserId = user.Id
                    };
                    bool degreestatus = _doctorService.AddDoctorDegree(doctorEducationDetails);
                    return Ok(new ApiResponseModel { status = degreestatus, data = new { }, message = ResponseMessages.msgAdditionSuccess, code = StatusCodes.Status200OK });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgParametersNotCorrect, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion

        #region UpdateDoctorBasicInfo
        [HttpPost]
        [Authorize]
        [Route("UpdateDoctorBasicInfo")]
        public async Task<IActionResult> UpdateDoctorBasicInfo(UpdateDoctorBasicInfoVM model)
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
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                user.About = model.About;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgUpdationSuccess,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, message = result.Errors.First().Description, data = new { }, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong + ex.Message, data = new { }, code = StatusCodes.Status500InternalServerError });
            }

        }
        #endregion

        #region UpdateDoctorPhoneNumber
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("UpdatePhoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumber(UpdateDoctorPhoneNumberViewModel model)
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
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                user.DialCode = model.DialCode;
                // user.PhoneNumber = model.PhoneNumber;
                if (user.PhoneNumber != model.PhoneNumber)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    user.PhoneNumberConfirmed = false;
                }
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        message = "Phone Number" + ResponseMessages.msgUpdationSuccess,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, message = result.Errors.First().Description, data = new { }, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong + ex.Message, data = new { }, code = StatusCodes.Status500InternalServerError });
            }
        }
        #endregion


        #region GetPersonalInfo
        [HttpGet]
        [Authorize]
        [Route("GetPersonalInfo")]
        public async Task<IActionResult> GetPersonalInfo()
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
                    var doctorPersonalInfoResponse = await _doctorService.GetDoctorPersonalInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = doctorPersonalInfoResponse,
                        message = "Personal Info" + ResponseMessages.msgShownSuccess,
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

        #region GetWorkInfo
        [HttpGet]
        [Authorize]
        [Route("GetDoctorWorkInfo")]
        public async Task<IActionResult> GetDoctorWorkInfo()
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
                    var doctorWorkInfoResponse = await _doctorService.GetDoctorWorkInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = doctorWorkInfoResponse,
                        message = "Work Info" + ResponseMessages.msgShownSuccess,
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

        #region AddBankInfo
        [HttpPost]
        [Authorize]
        [Route("AddBankInfo")]
        public async Task<IActionResult> AddDoctorBankInfo(DoctorBankInfoViewModel model)
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
                var _doctorBankInfo = new DoctorBankInfo()
                {
                    UserId = user.Id,
                    BankName = model.BankName,
                    AccountNumber = model.AccountNumber,
                    RouteNo = model.RouteNo,
                    BranchCode = model.BranchCode,
                    PostCode = model.PostCode,
                    Address = model.Address

                };
                bool result = _doctorService.AddDoctorBankInfo(_doctorBankInfo);
                if (result)
                {
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = new { },
                        message = "Bank Info" + ResponseMessages.msgAdditionSuccess,
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

        #region GetBankInfo
        [HttpGet]
        [Authorize]
        [Route("GetDoctorBankInfo")]
        public async Task<IActionResult> GetDoctorBankInfo()
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
                    var _doctorBankInfo = _doctorService.GetDoctorBankInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _doctorBankInfo,
                        message = "Bank Info" + ResponseMessages.msgShownSuccess,
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

        #region GetDoctorDashboardInfo
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetDoctorDashboardInfo")]
        public async Task<IActionResult> GetDoctorDashboardInfo()
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
                    var _doctorDashboardInfo = await _doctorService.GetDoctorDashboardInfo(user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _doctorDashboardInfo,
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

        #region GetTopDoctors
        [HttpGet]
        [Authorize]
        [Route("GetTopDoctors")]
        public async Task<IActionResult> GetTopDoctors([FromQuery] FilterationListViewModel model)
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
                    var _topDoctorList = _doctorService.GetTopDoctors(model);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _topDoctorList,
                        message = "Top Doctors List" + ResponseMessages.msgShownSuccess,
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

        #region GetAllDoctorList
        [HttpGet]
        [Authorize]
        [Route("GetAllDoctorList")]
        public async Task<IActionResult> GetAllDoctorList([FromQuery] FilterationListViewModel model)
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
                    var _topDoctorList = _doctorService.GetAllDoctors(model);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _topDoctorList,
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

        #region GetDoctorDescription
        [Authorize(Roles = "Patient")]
        [HttpGet]
        [Route("GetDoctorDescription")]
        public async Task<IActionResult> GetDoctorDescription(string DoctorId)
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
                    var _doctorDetails = await _doctorService.GetDoctorDescription(DoctorId);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _doctorDetails,
                        message = "Doctors details" + ResponseMessages.msgShownSuccess,
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

        #region GetPatientInfoById
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetPatientInfoById")]
        public async Task<IActionResult> GetPatientInfoById(string PatientId)
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
                    var PatientInfoResponse = await _patientService.GetPatientInfoById(PatientId);
                    return Ok(new ApiResponseModel
                    {

                        status = true,
                        data = PatientInfoResponse,
                        message = "Patient Info" + ResponseMessages.msgShownSuccess,
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

        #region GetDoctorProfilePercentCompletion
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetDoctorProfilePercentCompletion")]
        public async Task<IActionResult> GetDoctorProfilePercentCompletion()
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
                    var _doctorProfilePercentage = _authService.GetDoctorProfileCompletePersentById(user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _doctorProfilePercentage,
                        message = "Doctor Profile Percent Completion" + ResponseMessages.msgShownSuccess,
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


        
    }
}
