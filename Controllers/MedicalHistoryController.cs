using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
    public class MedicalHistoryController : ControllerBase
    {
        private IPatientService _patientService;
        private IContentService _contentService;
        private UserManager<ApplicationUser> _userManager;
        private IMedicalHistoryService _medicalhistoryService;
        private IWebHostEnvironment _hostingEnvironment;
        private IUploadFiles _uploadFiles;

        public MedicalHistoryController(
            IUploadFiles uploadFiles,
            IPatientService patientService,
            UserManager<ApplicationUser> userManager,
            IContentService contentService,
            IMedicalHistoryService medicalHistoryService,
            IWebHostEnvironment hostingEnvironment
        )
        {
            _patientService = patientService;
            _userManager = userManager;
            _contentService = contentService;
            _medicalhistoryService = medicalHistoryService;
            _hostingEnvironment = hostingEnvironment;
            _uploadFiles = uploadFiles;
        }

        #region PatientMedicalHistory
        #region AddPastMedicalHistory
        [HttpPost]
        [Authorize]
        [Route("AddPastMedicalHistory")]
        public async Task<IActionResult> AddPastMedicalHistory(PatientMedicalHistoryViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _patientMedicalHistoryModel = new PatientPastMedicalHistory()
                    {
                        UserId = user.Id,
                        TreatmentName = model.TreatmentName,
                        DoctorName = model.DoctorName,
                        Date = DateTime.ParseExact(
                            model.Date,
                            GlobalVariables.DefaultDateFormat,
                            null
                        ),
                        Description = model.Description
                    };

                    bool result = _medicalhistoryService.AddPatientMedicalHistory(
                        _patientMedicalHistoryModel
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Medical history" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetPastMedicalHistory
        [HttpGet]
        [Authorize]
        [Route("GetPastMedicalHistory")]
        public async Task<IActionResult> GetPastMedicalHistory(string userId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    string userRole = CommonFunctions.getUserRole(User);
                    string patientId = string.Empty;
                    if ((userRole).Equals(GlobalVariables.UserRole.Doctor.ToString()))
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            patientId = userId;
                        }
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    message = ResponseMessages.msgParametersNotCorrect,
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    else
                    {
                        patientId = user.Id;
                    }
                    var List = _medicalhistoryService.GetPatientPastMedicalHistory(patientId);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = List,
                            message = "Medical history" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region DeletePastMedicalHistory
        [HttpDelete]
        [Authorize]
        [Route("DeletePastMedicalHistory")]
        public async Task<IActionResult> DeletePastMedicalHistoryById(int MedicalHistoryId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user == null && MedicalHistoryId == 0)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            message = ResponseMessages.msgParametersNotCorrect,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    var result = await _medicalhistoryService.DeleteMedicalHistoryById(
                        MedicalHistoryId,
                        user.Id
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = result,
                                message = "Medical history " + ResponseMessages.msgDeletionSuccess,
                                data = new { },
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion

        #endregion

        #region PatientSurgicalHistory
        #region AddPatientSurgicalHistory
        [HttpPost]
        [Authorize]
        [Route("AddPatientSurgicalHistory")]
        public async Task<IActionResult> AddPatientSurgicalHistory(
            PatientSurgicalHistoryViewModel model
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _patientSurgicalHistory = new PatientSurgicalHistory()
                    {
                        UserId = user.Id,
                        TreatmentName = model.TreatmentName,
                        DoctorName = model.DoctorName,
                        Date = DateTime.ParseExact(
                            model.Date,
                            GlobalVariables.DefaultDateFormat,
                            null
                        ),
                        Description = model.Description
                    };
                    bool result = _medicalhistoryService.AddPatientSurgicalHistory(
                        _patientSurgicalHistory
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Surgical history" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetPatientSurgicalHistory
        [HttpGet]
        [Authorize]
        [Route("GetPatientSurgicalHistory")]
        public async Task<IActionResult> GetSurgicalHistory(string userId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    string userRole = CommonFunctions.getUserRole(User);
                    string patientId = string.Empty;
                    if ((userRole).Equals(GlobalVariables.UserRole.Doctor.ToString()))
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            patientId = userId;
                        }
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    message = ResponseMessages.msgParametersNotCorrect,
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    else
                    {
                        patientId = user.Id;
                    }
                    var List = _medicalhistoryService.GetPatientSurgicalHistory(patientId);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = List,
                            message = "Surgical history" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region DeleteSurgicalHistoryById
        [HttpDelete]
        [Authorize]
        [Route("DeleteSurgicalHistory")]
        public async Task<IActionResult> DeleteSurgicalHistoryById(int surgicalHistoryId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user == null && surgicalHistoryId == 0)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            message = ResponseMessages.msgParametersNotCorrect,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    var result = await _medicalhistoryService.DeleteSurgicalHistoryById(
                        surgicalHistoryId,
                        user.Id
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = result,
                                message = "Surgical history" + ResponseMessages.msgDeletionSuccess,
                                data = new { },
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }

        #endregion
        #endregion

        #region PatientFamilyHistory
        #region AddPatientFamilyHistory
        [HttpPost]
        [Authorize]
        [Route("AddPatientFamilyHistory")]
        public async Task<IActionResult> AddPatientFamilyHistory(
            PatientFamilyHistoryViewModel model
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _patientFamilyHistory = new PatientFamilyHistory()
                    {
                        UserId = user.Id,
                        DiseaseName = model.DiseaseName,
                        MemberName = model.MemberName,
                        Age = model.Age,
                        Relation = model.Relation,
                        Description = model.Description
                    };
                    bool result = _medicalhistoryService.AddPatientFamilyHistory(
                        _patientFamilyHistory
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Family history" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetPatientFamilyHistory
        [HttpGet]
        [Authorize]
        [Route("GetPatientFamilyHistory")]
        public async Task<IActionResult> GetFamilyHistory(string userId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    string userRole = CommonFunctions.getUserRole(User);
                    string patientId = string.Empty;
                    if ((userRole).Equals(GlobalVariables.UserRole.Doctor.ToString()))
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            patientId = userId;
                        }
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    message = ResponseMessages.msgParametersNotCorrect,
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    else
                    {
                        patientId = user.Id;
                    }
                    var List = _medicalhistoryService.GetPatientFamilyHistory(patientId);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = List,
                            message = "Family history" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region DeleteFamilyHistory
        [HttpDelete]
        [Authorize]
        [Route("DeleteFamilyHistory")]
        public async Task<IActionResult> DeleteFamilyHistoryById(int familyHistoryId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user == null && familyHistoryId == 0)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            message = ResponseMessages.msgParametersNotCorrect,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    var result = await _medicalhistoryService.DeleteFamilyHistoryById(
                        familyHistoryId,
                        user.Id
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = result,
                                message = "Family history" + ResponseMessages.msgDeletionSuccess,
                                data = new { },
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #endregion

        #region PatientPastAllergyHistory
        #region AddPatientAllergyHistory
        [HttpPost]
        [Authorize]
        [Route("AddPatientAllergyHistory")]
        public async Task<IActionResult> AddPatientAllergyHistory(
            PatientPastAllergyHistoryViewModel model
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _patientAllergyHistory = new PatientPastAllergyHistory()
                    {
                        UserId = user.Id,
                        AllergyName = model.AllergyName,
                        Description = model.Description
                    };
                    bool result = _medicalhistoryService.AddPatientAllergyHistory(
                        _patientAllergyHistory
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Allergy history" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetPatientAllergyHistory
        [HttpGet]
        [Authorize]
        [Route("GetPatientAllergyHistory")]
        public async Task<IActionResult> GetPatientAllergyHistory(string userId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    string userRole = CommonFunctions.getUserRole(User);
                    string patientId = string.Empty;
                    if ((userRole).Equals(GlobalVariables.UserRole.Doctor.ToString()))
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            patientId = userId;
                        }
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    message = ResponseMessages.msgParametersNotCorrect,
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    else
                    {
                        patientId = user.Id;
                    }
                    var List = _medicalhistoryService.GetPatientAllergyHistory(patientId);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = List,
                            message = "Allergy history" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region DeletePatientAllergyHistory
        [HttpDelete]
        [Authorize]
        [Route("DeleteAllergyHistory")]
        public async Task<IActionResult> DeleteAllergyHistoryById(int allergyHistoryId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user == null && allergyHistoryId == 0)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            message = ResponseMessages.msgParametersNotCorrect,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    var result = await _medicalhistoryService.DeleteAllergyHistoryById(
                        allergyHistoryId,
                        user.Id
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = result,
                                message = "Allergy history" + ResponseMessages.msgDeletionSuccess,
                                data = new { },
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion



        #endregion

        #region PatientMedicalReportsRecord
        #region AddPatientMedicalReport
        [HttpPost]
        [Authorize]
        [Route("AddPatientMedicalReport")]
        public async Task<IActionResult> AddPatientMedicalReport(
            [FromForm] PatientMedicalReportViewModel model
        )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgParametersNotCorrect,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var documentFile = ContentDispositionHeaderValue
                        .Parse(model.MedicalDocument.ContentDisposition)
                        .FileName.Trim('"');
                    documentFile = CommonFunctions.EnsureCorrectFilename(documentFile);
                    documentFile = CommonFunctions.RenameFileName(documentFile);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(documentFile, GlobalVariables.medicaldocsContainer)))
                    // {
                    //     model.MedicalDocument.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    //Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.MedicalDocument,
                        GlobalVariables.medicaldocsContainer,documentFile
                    );
                    string documentPath = GlobalVariables.medicaldocsContainer + "/" + documentFile;
                    var _patientMedicalReport = new PatientMedicalReport()
                    {
                        UserId = user.Id,
                        ReportName = model.ReportName,
                        Date = DateTime.ParseExact(
                            model.Date,
                            GlobalVariables.DefaultDateFormat,
                            null
                        ),
                        Description = model.Description,
                        MedicalDocumentPath = documentPath
                    };
                    bool result = _medicalhistoryService.AddPatientMedicalReport(
                        _patientMedicalReport
                    );

                    if (result )
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Medical report" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetPatientMedicalReport
        [HttpGet]
        [Authorize]
        [Route("GetPatientMedicalReport")]
        public async Task<IActionResult> GetPatientMedicalReport(string userId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    string userRole = CommonFunctions.getUserRole(User);
                    string patientId = string.Empty;
                    if ((userRole).Equals(GlobalVariables.UserRole.Doctor.ToString()))
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            patientId = userId;
                        }
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    message = ResponseMessages.msgParametersNotCorrect,
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    else
                    {
                        patientId = user.Id;
                    }
                    var List = _medicalhistoryService.GetPatientMedicalReport(patientId);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = List,
                            message = "Medical report" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region DeletePatientMedicalReport
        [HttpDelete]
        [Authorize]
        [Route("DeletePatientMedicalReport")]
        public async Task<IActionResult> DeletePatientMedicalReportById(int medicalReportId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user == null && medicalReportId == 0)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            message = ResponseMessages.msgParametersNotCorrect,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    var result = await _medicalhistoryService.DeletePatientMedicalReport(
                        medicalReportId,
                        user.Id
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = result,
                                message = "Medical report" + ResponseMessages.msgDeletionSuccess,
                                data = new { },
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion
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
