using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
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
using System.Drawing.Design;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Transactions;
using AnfasAPI.Data;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private IContentService _contentService;
        private IEmailSender _emailSender;
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _hostingEnvironment;
        private ApplicationDbContext _context;
        private IUploadFiles _uploadFiles;

        public ContentController(
            ApplicationDbContext context,
            IContentService service,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostingEnvironment,
            IUploadFiles uploadFiles
        )
        {
            _contentService = service;
            _emailSender = emailSender;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _uploadFiles = uploadFiles;
        }

        #region GetCountries
        [HttpGet]
        [Route("GetCountries")]
        public ActionResult GetCountries()
        {
            try
            {
                var list = _contentService.GetCountries();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region Nationalities
        [HttpGet]
        [Route("GetNationalities")]
        public ActionResult GetNationalities()
        {
            try
            {
                var list = _contentService.GetNationalities();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetStates
        [HttpGet]
        [Route("GetStates")]
        public ActionResult GetStatesByCountryId(int CountryId)
        {
            try
            {
                var list = _contentService.GetStatesByCountryId(CountryId);
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetAllStates
        [HttpGet]
        [Route("GetAllStates")]
        public ActionResult GetAllStates()
        {
            try
            {
                var list = _contentService.GetStates();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region GetLanguages
        [HttpGet]
        [Route("GetLanguages")]
        public ActionResult GetLanguages()
        {
            try
            {
                var list = _contentService.GetLanguages();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region AddLanguageMaster
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("AddLanguageMaster")]
        public async Task<IActionResult> AddLanguageMaster(LanguageMasterViewModel model)
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
                            message = ResponseMessages.msgTokenExpired,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _languagemaster = _context.LanguageMaster.Find(model.Id);
                    _languagemaster.Id = model.Id;
                    _languagemaster.Name = model.Name;
                    _languagemaster.IsActive = model.IsActive;

                    bool result = _contentService.AddUpdateLanguageMaster(_languagemaster);

                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Language Info" + ResponseMessages.msgUpdationSuccess,
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

        #region GetSpeciality
        [HttpGet]
        [Route("GetSpeciality")]
        public ActionResult GetSpeciality()
        {
            try
            {
                var list = _contentService.GetSpecialities();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region AddSpecialityMaster
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("AddUpdateSpecialityMaster")]
        public async Task<IActionResult> AddUpdateSpecialityMaster(
            [FromForm] SpecialityMasterViewModel model
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

                if (model.Id == 0 && model.Image == null)
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
                            message = ResponseMessages.msgTokenExpired,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var imgFile =
                        model.Image != null
                            ? ContentDispositionHeaderValue
                                .Parse(model.Image.ContentDisposition)
                                .FileName.Trim('"')
                            : string.Empty;
                    if (model.Image != null)
                    {
                        imgFile = CommonFunctions.EnsureCorrectFilename(imgFile);
                        imgFile = CommonFunctions.RenameFileName(imgFile);
                        string ext = System.IO.Path.GetExtension(imgFile);
                        var image = Image.FromStream(model.Image.OpenReadStream());
                        if (ext == ".png")
                        {
                            if (image.Height == 200 && image.Width == 200)
                            {
                                // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(imgFile, GlobalVariables.specialityImagesContainer)))
                                // {
                                //     model.Image.CopyTo(fs);
                                //     fs.Flush();
                                // }
                                // Upload Image To Server
                                bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                                    model.Image,
                                    GlobalVariables.specialityImagesContainer,imgFile
                                );
                            }
                            else
                            {
                                return Ok(
                                    new ApiResponseModel
                                    {
                                        status = false,
                                        data = new { },
                                        message = ResponseMessages.msgInvalidImageSize,
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
                                    message = ResponseMessages.msgInvalidImage,
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    string imgFilePath = string.Empty;
                    if (!string.IsNullOrEmpty(imgFile))
                    {
                        imgFilePath = GlobalVariables.specialityImagesContainer + "/" + imgFile;
                    }

                    var _specialitymaster = new SpecialityMaster()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        ImagePath = imgFilePath,
                        IsActive = model.IsActive
                    };
                    bool result = _contentService.AddUpdateSpecialityMaster(_specialitymaster);
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Speciality Info" + ResponseMessages.msgUpdationSuccess,
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

        #region GetDegrees
        [HttpGet]
        [Route("GetDegrees")]
        public ActionResult GetDegrees()
        {
            try
            {
                var list = _contentService.GetDegree();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = new { list },
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region AddDegreeMaster
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("AddDegreeMaster")]
        public async Task<IActionResult> AddDegreeMaster(DegreeMasterViewModel model)
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
                            message = ResponseMessages.msgTokenExpired,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _degreemaster = new DegreeMaster()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive
                    };
                    bool result = _contentService.AddUpdateDegreeMaster(_degreemaster);
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Degree Info" + ResponseMessages.msgUpdationSuccess,
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

        #region ContactUs
        [HttpPost]
        [Route("ContactUs")]
        public async Task<IActionResult> ContactUs(ContactInfoViewModel model)
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
                else
                {
                    string subject = "Enquiry By " + model.Name;
                    ;
                    string message =
                        $"<b>Name:</b> "
                        + model.Name
                        + "<br/>"
                        + "<b>Email Id:</b> "
                        + model.Email
                        + "<br/>"
                        + "<b>Mobile No:</b> "
                        + model.MobileNo
                        + "<br/>"
                        + "<b>Message:</b> "
                        + model.Message
                        + "<br/><br/>Thanks.";
                    await _emailSender.SendEmailAsync(
                        "loveneet.dotnet@outlook.com",
                        subject,
                        message
                    );
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = ResponseMessages.msgEnquirySentSuccess,
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

        #region GetAppSettings
        [HttpGet]
        [Route("GetAppSettings")]
        public async Task<IActionResult> GetAppSettings()
        {
            try
            {
                var _appsettings = await _contentService.GetAppSettings();
                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        data = _appsettings,
                        message = "Settings Info" + ResponseMessages.msgShownSuccess,
                        code = StatusCodes.Status200OK
                    }
                );
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

        #region AddUpdateAppSettings
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("AddUpdateAppSettings")]
        public async Task<IActionResult> AddUpdateAppSettings(AppSettingsViewModel model)
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
                            message = ResponseMessages.msgTokenExpired,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var _appSettings = new AppSettings()
                    {
                        Id = model.Id,
                        AboutUs = model.AboutUs,
                        TermsConditions = model.TermsConditions,
                        PrivacyPolicy = model.PrivacyPolicy
                    };
                    bool result = _contentService.AddUpdateAppSettings(_appSettings);
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "App Settings Info" + ResponseMessages.msgUpdationSuccess,
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

        #region UpdateNotificationStatus
        [HttpPost]
        [Authorize(Roles = "Doctor, Patient")]
        [Route("UpdateNotificationStatus")]
        public async Task<IActionResult> UpdateNotificationStatus(NotificatonStatusViewModel model)
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
                            message = ResponseMessages.msgTokenExpired,
                            data = new { },
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    bool result = _contentService.UpdateNotificationStatus(user.Id, model);
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message =
                                    "Notification Status Info"
                                    + ResponseMessages.msgUpdationSuccess,
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

        #region UpdateUserAddress
        [HttpPost]
        [Authorize]
        [Route("UpdateUserAddress")]
        public async Task<IActionResult> UpdateUserAddress(UserAddressViewModel model)
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
                    var _userAddress = new UserAddress()
                    {
                        UserId = user.Id,
                        CountryId = model.CountryId,
                        StateId = model.StateId,
                        NationalityId = model.NationalityId,
                        City = model.City,
                        CurrentAddress = model.CurrentAddress
                    };
                    bool result = _contentService.UpdateUserAddress(_userAddress);
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "User address" + ResponseMessages.msgUpdationSuccess,
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

        #region GetFounderInfo
        [HttpGet]
        [Route("GetFounderInfo")]
        public IActionResult GetFounderInfo()
        {
            try
            {
                var getFounderInfo = _context.FounderInfo.FirstOrDefault();
                var model = new ResponseFounderInfoViewModel();
                if (getFounderInfo != null)
                {
                    var getFounderPersonalAchievement = _context.FounderPersonalAchievement
                        .Where(i => i.FounderInfoId == getFounderInfo.FounderInfoId)
                        .ToList();
                    var getFounderAchievement = _context.FounderAchievement
                        .Where(i => i.FounderInfoId == getFounderInfo.FounderInfoId)
                        .ToList();
                    var getFounderEducation = _context.FounderEducation
                        .Where(i => i.FounderInfoId == getFounderInfo.FounderInfoId)
                        .ToList();
                    var getFounderAdditionalInfo = _context.FounderAdditionalInformation
                        .Where(i => i.FounderInfoId == getFounderInfo.FounderInfoId)
                        .ToList();

                    model.FounderInfoId = getFounderInfo.FounderInfoId;
                    model.FounderName = getFounderInfo.FounderName;
                    model.FounderDesignation = getFounderInfo.FounderDesignation;
                    model.FounderImage = getFounderInfo.FounderImage;
                    model.FounderAchievementList = new List<ResponseFounderAchievementViewModel>();
                    foreach (var a in getFounderAchievement)
                    {
                        var m = new ResponseFounderAchievementViewModel();
                        m.AchievementDescription = a.AchievementDescription;
                        m.FounderAchievementId = a.FounderAchievementId;
                        model.FounderAchievementList.Add(m);
                    }

                    model.FounderPersonalAchievementList =
                        new List<ResponseFounderPersonalAchievementViewModel>();
                    foreach (var i in getFounderPersonalAchievement)
                    {
                        var m = new ResponseFounderPersonalAchievementViewModel();
                        m.Description = i.Description;
                        m.FounderPersonalAchievementId = i.FounderPersonalAchievementId;
                        model.FounderPersonalAchievementList.Add(m);
                    }

                    model.FounderEducationList = new List<ResponseFounderEducationViewModel>();
                    foreach (var f in getFounderEducation)
                    {
                        var m = new ResponseFounderEducationViewModel();
                        m.EducationFrom = f.EducationFrom;
                        m.EducationType = f.EducationType;
                        m.FounderEducationId = f.FounderEducationId;
                        model.FounderEducationList.Add(m);
                    }

                    model.FounderAdditionalInformationList =
                        new List<ResponseFounderAdditionalInformationViewModel>();
                    foreach (var f in getFounderAdditionalInfo)
                    {
                        var m = new ResponseFounderAdditionalInformationViewModel();
                        m.Description = f.Description;
                        m.FounderAdditionalInformationId = f.FounderAdditionalInformationId;
                        model.FounderAdditionalInformationList.Add(m);
                    }
                }

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        message = ResponseMessages.msgGotSuccess,
                        data = model,
                        code = StatusCodes.Status200OK
                    }
                );
            }
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region UpdateFounderInfo
        [HttpPost]
        [Authorize]
        [Route("UpdateFounderInfo")]
        public async Task<IActionResult> UpdateFounderInfo(
            [FromBody] RequestupdateUpdateFounderInfoViewModel model
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
                    var founderInfo = _context.FounderInfo.Find(model.FounderInfoId);
                    if (founderInfo == null)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = "Founder Info" + ResponseMessages.msgNotFound,
                                code = StatusCodes.Status404NotFound
                            }
                        );
                    }
                    else
                    {
                        var UpdateModel = new RequestupdateUpdateFounderInfoViewModel();
                        //founderInfo.FounderInfoId = UpdateModel.FounderInfoId;
                        //founderInfo.FounderImage = model.FounderImage;
                        founderInfo.FounderName = model.FounderName;
                        founderInfo.FounderDesignation = model.FounderDesignation;
                        //founderInfo.Createdate = model.Createdate;
                        founderInfo.ModifyDate = model.ModifyDate;
                        _context.Update(founderInfo);
                        _context.SaveChanges();

                        if (model.FounderAchievementList.Count > 0)
                        {
                            foreach (var i in model.FounderAchievementList)
                            {
                                if (i.FounderAchievementId < 1)
                                {
                                    var newObj = new FounderAchievement();
                                    newObj.AchievementDescription = i.AchievementDescription;
                                    newObj.FounderInfoId = model.FounderInfoId;
                                    _context.FounderAchievement.Add(newObj);
                                    _context.SaveChanges();
                                }
                                var obj = _context.FounderAchievement.Find(i.FounderAchievementId);
                                if (obj != null)
                                {
                                    obj.AchievementDescription = i.AchievementDescription;
                                    //obj.FounderInfoId = model.FounderInfoId;
                                    _context.FounderAchievement.Update(obj);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        if (model.FounderEducationList.Count > 0)
                        {
                            foreach (var i in model.FounderEducationList)
                            {
                                if (i.FounderEducationId < 1)
                                {
                                    var newObj = new FounderEducation();
                                    newObj.EducationFrom = i.EducationFrom;
                                    newObj.EducationType = i.EducationType;
                                    newObj.FounderInfoId = model.FounderInfoId;
                                    _context.FounderEducation.Add(newObj);
                                    _context.SaveChanges();
                                }
                                var obj = _context.FounderEducation.Find(i.FounderEducationId);
                                if (obj != null)
                                {
                                    obj.EducationFrom = i.EducationFrom;
                                    obj.EducationType = i.EducationType;
                                    //obj.FounderInfoId= i.FounderInfoId;
                                    _context.FounderEducation.Update(obj);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        if (model.FounderPersonalAchievementList.Count > 0)
                        {
                            foreach (var i in model.FounderPersonalAchievementList)
                            {
                                if (i.FounderPersonalAchievementId < 1)
                                {
                                    var newObj = new FounderPersonalAchievement();
                                    newObj.Description = i.Description;
                                    newObj.FounderInfoId = model.FounderInfoId;
                                    //newObj.FounderPersonalAchievementId= i.FounderPersonalAchievementId;
                                    _context.FounderPersonalAchievement.Add(newObj);
                                    _context.SaveChanges();
                                }
                                var obj = _context.FounderPersonalAchievement.Find(
                                    i.FounderPersonalAchievementId
                                );
                                if (obj != null)
                                {
                                    obj.Description = i.Description;
                                    _context.FounderPersonalAchievement.Update(obj);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        if (model.FounderAdditionalInformationList.Count > 0)
                        {
                            foreach (var i in model.FounderAdditionalInformationList)
                            {
                                if (i.FounderAdditionalInformationId < 1)
                                {
                                    var newObj = new FounderAdditionalInformation();
                                    newObj.Description = i.Description;
                                    newObj.FounderInfoId = model.FounderInfoId;
                                    //newObj.FounderPersonalAchievementId= i.FounderPersonalAchievementId;
                                    _context.FounderAdditionalInformation.Add(newObj);
                                    _context.SaveChanges();
                                }
                                var obj = _context.FounderAdditionalInformation.Find(
                                    i.FounderAdditionalInformationId
                                );
                                if (obj != null)
                                {
                                    obj.Description = i.Description;
                                    _context.FounderAdditionalInformation.Update(obj);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Founder Info" + ResponseMessages.msgUpdationSuccess,
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

        #region UpdateFounderPic
        [HttpPost]
        [Authorize]
        [Route("UpdateFounderPic")]
        public async Task<IActionResult> UpdateProfilePic([FromForm] ImageUploadModel model)
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
                string currentuserid = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentuserid))
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
                var user = await _userManager.FindByIdAsync(currentuserid);
                if (user != null)
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(model.ImgFile.ContentDisposition)
                        .FileName.Trim('"');
                    filename = CommonFunctions.EnsureCorrectFilename(filename);
                    filename = CommonFunctions.RenameFileName(filename);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(filename, GlobalVariables.FounderPictureContainer)))
                    // {
                    //     model.ImgFile.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    //Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.ImgFile,
                        GlobalVariables.FounderPictureContainer,filename
                    );

                    var founder = _context.FounderInfo.Find(model.FounderInfoId);
                    founder.FounderImage = GlobalVariables.FounderPictureContainer + "/" + filename;
                    var result = _context.FounderInfo.Update(founder);
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            // data = new
                            // {
                            //     profilepicurl = user.ProfilePic
                            // },
                            message = ResponseMessages.msgUpdationSuccess,
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
                            message = ResponseMessages.msgCouldNotFoundAssociatedUser,
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

        #region DeleteFounderEducation
        [HttpDelete]
        [Authorize]
        [Route("DeleteFounderEducation")]
        public async Task<IActionResult> DeleteFounderEducation(int founderEducationId)
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
                if (user == null && string.IsNullOrEmpty(founderEducationId.ToString()))
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
                    var deleteFounderEducation = _context.FounderEducation
                        .Where(x => x.FounderEducationId == founderEducationId)
                        .FirstOrDefault();
                    var result = _context.FounderEducation.Remove(deleteFounderEducation);
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Founder Education" + ResponseMessages.msgDeletionSuccess,
                            data = new { },
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

        #region DeleteFounderAdditionalInfo
        [HttpDelete]
        [Authorize]
        [Route("DeleteFounderAdditionalInfo")]
        public async Task<IActionResult> DeleteFounderAdditionalInfo(
            int founderAdditionalInformationId
        )
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
                if (user == null && string.IsNullOrEmpty(founderAdditionalInformationId.ToString()))
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
                    var deleteFounderAdditionalInfo = _context.FounderAdditionalInformation
                        .Where(
                            x => x.FounderAdditionalInformationId == founderAdditionalInformationId
                        )
                        .FirstOrDefault();
                    var result = _context.FounderAdditionalInformation.Remove(
                        deleteFounderAdditionalInfo
                    );
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Information" + ResponseMessages.msgDeletionSuccess,
                            data = new { },
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

        #region DeleteFounderAchievement
        [HttpDelete]
        [Authorize]
        [Route("DeleteFounderAchievement")]
        public async Task<IActionResult> DeleteFounderAchievement(int founderAchievementId)
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
                if (user == null && string.IsNullOrEmpty(founderAchievementId.ToString()))
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
                    var deleteFounderAchievement = _context.FounderAchievement
                        .Where(x => x.FounderAchievementId == founderAchievementId)
                        .FirstOrDefault();
                    var result = _context.FounderAchievement.Remove(deleteFounderAchievement);
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Founder Achievement" + ResponseMessages.msgDeletionSuccess,
                            data = new { },
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

         #region DeleteFounderPersonalAchievement
        [HttpDelete]
        [Authorize]
        [Route("DeleteFounderPersonalAchievement")]
        public async Task<IActionResult> DeleteFounderPersonalAchievement(
            int founderPersonalAchievementId
        )
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
                if (user == null && string.IsNullOrEmpty(founderPersonalAchievementId.ToString()))
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
                    var deleteFounderPersonalAchievement = _context.FounderPersonalAchievement
                        .Where(x => x.FounderPersonalAchievementId == founderPersonalAchievementId)
                        .FirstOrDefault();
                    var result = _context.FounderPersonalAchievement.Remove(
                        deleteFounderPersonalAchievement
                    );
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message =
                                "Founder Personal Achievement"
                                + ResponseMessages.msgDeletionSuccess,
                            data = new { },
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
