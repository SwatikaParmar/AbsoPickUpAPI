using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.Repositories;
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
    public class AdminDoctorController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private IContentService _contentService;
        private ApplicationDbContext _context;
        private IWebHostEnvironment _hostingEnvironment;
        private IUploadFiles _uploadFiles;

        public AdminDoctorController(
            ApplicationDbContext context,
            IWebHostEnvironment hostingEnvironment,
            IDoctorService doctorService,
            IEmailSender emailSender,
            IContentService contentService,
            UserManager<ApplicationUser> userManager,
            IPatientService patientService,
            IAuthService authService,
            IUploadFiles uploadFiles
        )
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _patientService = patientService;
            _authService = authService;
            _emailSender = emailSender;
            _contentService = contentService;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uploadFiles = uploadFiles;
        }

        #region AddDoctor
        [HttpPost]
        [Authorize]
        [Route("AddDoctor")]
        public async Task<IActionResult> AddDoctor(AddDoctorViewModel model)
        {
            ApplicationUser _appuser = new ApplicationUser();
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
                _appuser.UserName = model.Email;
                _appuser.FirstName = model.FirstName;
                _appuser.LastName = model.LastName;
                _appuser.GenderId = model.GenderId;
                _appuser.Email = model.Email;
                _appuser.DateOfBirth = DateTime.ParseExact(
                    model.DateOfBirth,
                    GlobalVariables.DefaultDateFormat,
                    null
                );
                // _appuser.DeviceType = model.DeviceType.ToLower();
                // _appuser.DeviceToken = model.DeviceToken;
                _appuser.DialCode = model.DialCode;
                _appuser.PhoneNumber = model.PhoneNumber;
                _appuser.IsSocialUser = false;
                _appuser.LastScreenId = (int)GlobalVariables.LastScreenInfo.PersonalInfo;
                _appuser.Experience = model.Experience;
                _appuser.CreatedDate = DateTime.UtcNow;
                _appuser.MedicalRegistrationNumber = model.MedicalRegistrationNumber;
                _appuser.About = model.About;

                // generate random password
                StringBuilder builder = new StringBuilder();
                builder.Append(CommonFunctions.RandomString(4, true));
                builder.Append(CommonFunctions.RandomNumber(1000, 9999));
                builder.Append(CommonFunctions.RandomString(2, false));
                string password = builder.ToString();
                var result = await _userManager.CreateAsync(_appuser, password);
                if (result.Succeeded)
                {
                    var objWalletInfo = new WalletBillingInfo()
                    {
                        UserId = _appuser.Id,
                        Fee = 1000,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.WalletBillingInfo.Add(objWalletInfo);
                    _context.SaveChanges();
                    await _userManager.AddToRoleAsync(
                        _appuser,
                        GlobalVariables.UserRole.Doctor.ToString()
                    );

                    //send email here
                    var msg = EmailMessages.GetUserRegistrationSendPasswordMsg(password);
                    await _emailSender.SendEmailAsync(
                        email: _appuser.Email,
                        subject: EmailMessages.confirmationEmailSubject,
                        message: msg
                    );

                    // add doctor education
                    string doctorEducationDegree = model.DegreeId;
                    String[] splitDoctorEducationDegree = doctorEducationDegree.Split(",");
                    foreach (var l in splitDoctorEducationDegree)
                    {
                        var doctorEducationInfo = new DoctorEducationInfo();
                        var degree = _context.DegreeMaster.Find(Convert.ToInt32(l));
                        doctorEducationInfo.Degree = degree.Name;
                        doctorEducationInfo.DegreeId = Convert.ToInt32(l);
                        doctorEducationInfo.UserId = _appuser.Id;

                        _context.Add(doctorEducationInfo);
                        _context.SaveChanges();
                    }
                    // add doctor language
                    string doctorLanguageString = model.DoctorLanguage;
                    String[] splitDoctorLanguage = doctorLanguageString.Split(",");
                    foreach (var l in splitDoctorLanguage)
                    {
                        var languageKnown = new DoctorLanguageInfo();
                        languageKnown.LanguageId = Convert.ToInt32(l);
                        languageKnown.UserId = _appuser.Id;

                        _context.Add(languageKnown);
                        _context.SaveChanges();
                    }
                    string doctorSpecializationString = model.DoctorSpecialization;
                    String[] splitDoctorSpecialization = doctorSpecializationString.Split(",");
                    // add doctor specialization
                    foreach (var s in splitDoctorSpecialization)
                    {
                        var specialization = new DoctorSpecialityInfo();
                        specialization.SpecialityId = Convert.ToInt32(s);
                        specialization.UserId = _appuser.Id;

                        _context.Add(specialization);
                        _context.SaveChanges();
                    }

                    // add user address
                    var _userAddress = new UserAddress()
                    {
                        UserId = _appuser.Id,
                        CountryId = model.CountryId,
                        StateId = model.StateId,
                        // NationalityId = model.NationalityId,
                        City = model.City,
                        CurrentAddress = model.Address
                    };

                    _context.Add(_userAddress);
                    _context.SaveChanges();

                    // string accessToken = CommonFunctions.GenerateAccessToken(_appuser.Id, GlobalVariables.UserRole.Doctor.ToString(), _appuser.DeviceToken);
                    var _userConfigurations = new UserConfigurations()
                    {
                        UserId = _appuser.Id,
                        EmailNotificationStatus = true,
                        PushNotificationsStatus = true,
                        SMSNotificationStatus = true
                    };
                    _contentService.AddUpdateUserConfigurations(_userConfigurations);

                    // var doctorLoginResponse = await _authService.GetDoctorLoginResponse(_appuser.Id, accessToken);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = _appuser.Id,
                            message = "Doctor" + ResponseMessages.msgCreationSuccess,
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
                            message = result.Errors.First().Description,
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
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region UploadDoctorImage
        [HttpPost]
        [Authorize]
        [Route("UploadDoctorImage")]
        public async Task<IActionResult> UploadDoctorImage([FromForm] UploadDoctorImage model)
        {
            //ApplicationUser _appuser = new ApplicationUser();
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
                        .Parse(model.DoctorImage.ContentDisposition)
                        .FileName.Trim('"');
                    filename = CommonFunctions.EnsureCorrectFilename(filename);
                    filename = CommonFunctions.RenameFileName(filename);

                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.DoctorImage,
                        GlobalVariables.DoctorProfilePicContainer,
                        filename
                    );

                    var doctor = await _userManager.FindByIdAsync(model.Id);

                    if (doctor != null && doctor.IsDeleted != true)
                    {
                        doctor.ProfilePic =
                            GlobalVariables.DoctorProfilePicContainer + "/" + filename;

                        await _userManager.UpdateAsync(doctor);

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = "Doctor Image" + ResponseMessages.msgAdditionSuccess,
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
                                message = ResponseMessages.msgUserNotFound,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgUserNotFound,
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

        #region GetDoctorDetails
        [HttpGet]
        [Authorize]
        [Route("GetDoctorDetails")]
        public async Task<IActionResult> GetDoctorDetails(string Id)
        {
            //ApplicationUser _appuser = new ApplicationUser();
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
                    var doctorObj = await _userManager.FindByIdAsync(Id);

                    if (doctorObj != null && doctorObj.IsDeleted != true)
                    {
                        var doctorDetail = new GetDoctorDetailsViewModels();

                        doctorDetail.Email = doctorObj.Email;
                        doctorDetail.FirstName = doctorObj.FirstName;
                        doctorDetail.LastName = doctorObj.LastName;
                        doctorDetail.GenderId = doctorObj.GenderId;
                        doctorDetail.GenderName = _context.GenderMaster.Where(a=>a.Id == doctorObj.GenderId).Select(a=>a.Name).FirstOrDefault();
                        //doctorDetail.DateOfBirth = doctorObj.DateOfBirth.ToString();
                        DateTime date = Convert.ToDateTime(doctorObj.DateOfBirth);
                        doctorDetail.DateOfBirth = date.ToString("dd/MM/yyyy");
                        doctorDetail.DialCode = doctorObj.DialCode;
                        doctorDetail.PhoneNumber = doctorObj.PhoneNumber;
                        doctorDetail.Experience = doctorObj.Experience;
                        doctorDetail.MedicalRegistrationNumber =
                            doctorObj.MedicalRegistrationNumber;
                        doctorDetail.ProfilePic = doctorObj.ProfilePic;
                        doctorDetail.About = doctorObj.About;
                        var doctorEducationInfo = _context.DoctorEducationInfo
                            .Where(a => a.UserId == Id)
                            .ToList();

                        // Get Degree
                        if (doctorEducationInfo != null)
                        {
                            string degree = string.Empty;
                            string degreeName = string.Empty;

                            foreach (var item in doctorEducationInfo)
                            {
                                degree = degree + item.DegreeId + ",";
                                degreeName = degreeName + _context.DegreeMaster.Where(a => a.Id == item.DegreeId).Select(a => a.Name).FirstOrDefault() + ",";
                            }
                            if (degree.Length > 0)
                            {
                                degree = degree.Remove(degree.Length - 1);
                                degreeName = degreeName.Remove(degreeName.Length - 1);

                                doctorDetail.DegreeId = degree;
                                doctorDetail.DegreeName = degreeName;
                            }
                        }

                        // Get Language
                        var doctorLanguageInfo = _context.DoctorLanguageInfo
                            .Where(a => a.UserId == Id)
                            .ToList();

                        if (doctorLanguageInfo != null)
                        {
                            string language = string.Empty;
                            string languageName = string.Empty;

                            foreach (var item in doctorLanguageInfo)
                            {
                                language = language + item.LanguageId + ",";
                                languageName = languageName + _context.LanguageMaster.Where(a => a.Id == item.LanguageId).Select(a => a.Name).FirstOrDefault() + ",";
                            }
                            if (language.Length > 0)
                            {
                                language = language.Remove(language.Length - 1);
                                languageName = languageName.Remove(languageName.Length - 1);

                                doctorDetail.DoctorLanguage = language;
                                doctorDetail.DoctorLanguageName = languageName;
                            }
                        }

                        // Get Speciality
                        var doctorSpecialityInfo = _context.DoctorSpecialityInfo
                            .Where(a => a.UserId == Id)
                            .ToList();

                        if (doctorSpecialityInfo != null)
                        {
                            string speciality = string.Empty;
                            string specialityName = string.Empty;

                            foreach (var item in doctorSpecialityInfo)
                            {
                                speciality = speciality + item.SpecialityId + ",";
                                specialityName = specialityName + _context.SpecialityMaster.Where(a => a.Id == item.SpecialityId).Select(a => a.Name).FirstOrDefault() + ",";
                            }
                            if (speciality.Length > 0)
                            {
                                speciality = speciality.Remove(speciality.Length - 1);
                                specialityName = specialityName.Remove(specialityName.Length - 1);

                                doctorDetail.DoctorSpecialization = speciality;
                                doctorDetail.DoctorSpecializationName = specialityName;
                            }
                        }

                        var doctorUserAddress = _context.UserAddress
                            .Where(a => a.UserId == Id)
                            .ToList();
                        foreach (var item in doctorUserAddress)
                        {
                            doctorDetail.CountryId = item.CountryId;
                            doctorDetail.StateId = item.StateId;
                            doctorDetail.StateName = _context.StateMaster.Where(a => a.Id == item.StateId).Select(a => a.Name).FirstOrDefault();
                            doctorDetail.City = item.City;
                            doctorDetail.Address = item.CurrentAddress;
                        }

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = "Doctor detail" + ResponseMessages.msgShownSuccess,
                                code = StatusCodes.Status200OK,
                                data = doctorDetail
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
                                message = ResponseMessages.msgUserNotFound,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgUserNotFound,
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

        #region UpdateDoctorDetails
        [HttpPost]
        [Authorize]
        [Route("UpdateDoctorDetails")]
        public async Task<IActionResult> UpdateDoctorDetails(
            [FromBody] UpdateDoctorDetailsViewModels model
        )
        {
            //ApplicationUser _appuser = new ApplicationUser();
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
                    var doctorObj = await _userManager.FindByIdAsync(model.Id);

                    if (doctorObj != null && doctorObj.IsDeleted != true)
                    {
                        var doctorDetail = new UpdateDoctorDetailsViewModels();

                        doctorObj.Email = model.Email;
                        doctorObj.FirstName = model.FirstName;
                        doctorObj.LastName = model.LastName;
                        doctorObj.GenderId = model.GenderId;
                        doctorObj.DateOfBirth.ToString();
                        doctorObj.DialCode = model.DialCode;
                        doctorObj.PhoneNumber = model.PhoneNumber;
                        doctorObj.Experience = model.Experience;
                        doctorObj.MedicalRegistrationNumber = model.MedicalRegistrationNumber;
                        //doctorObj.ProfilePic = model.ProfilePic;
                        doctorObj.About = model.About;
                        await _userManager.UpdateAsync(doctorObj);

                        var doctorEducationInfo = _context.DoctorEducationInfo
                            .Where(a => a.UserId == model.Id)
                            .ToList();
                        foreach (var item in doctorEducationInfo)
                        {
                            _context.DoctorEducationInfo.Remove(item);
                            _context.SaveChanges();
                        }

                        string doctorEducationDegree = model.DegreeId;
                        String[] splitDoctorEducationDegree = doctorEducationDegree.Split(",");
                        foreach (var l in splitDoctorEducationDegree)
                        {
                            var educationInfo = new DoctorEducationInfo();
                            educationInfo.Degree = _context.DegreeMaster
                                .Find(Convert.ToInt32(l))
                                .ToString();
                            educationInfo.DegreeId = Convert.ToInt32(l);
                            educationInfo.UserId = model.Id;

                            _context.DoctorEducationInfo.Add(educationInfo);
                            _context.SaveChanges();
                        }

                        // Get Language
                        var doctorLanguageInfo = _context.DoctorLanguageInfo
                            .Where(a => a.UserId == model.Id)
                            .ToList();
                        foreach (var item in doctorLanguageInfo)
                        {
                            _context.DoctorLanguageInfo.Remove(item);
                            _context.SaveChanges();
                        }

                        string doctorLanguageString = model.DoctorLanguage;
                        String[] splitDoctorLanguage = doctorLanguageString.Split(",");
                        foreach (var l in splitDoctorLanguage)
                        {
                            var languageKnown = new DoctorLanguageInfo();
                            languageKnown.LanguageId = Convert.ToInt32(l);
                            languageKnown.UserId = model.Id;

                            _context.DoctorLanguageInfo.Add(languageKnown);
                            _context.SaveChanges();
                        }

                        // Get specialization
                        var doctorSpecialityInfo = _context.DoctorSpecialityInfo
                            .Where(a => a.UserId == model.Id)
                            .ToList();

                        foreach (var item in doctorSpecialityInfo)
                        {
                            _context.DoctorSpecialityInfo.Remove(item);
                            _context.SaveChanges();
                        }

                        string doctorSpecializationString = model.DoctorSpecialization;
                        String[] splitDoctorSpecialization = doctorSpecializationString.Split(",");
                        // add doctor specialization
                        foreach (var s in splitDoctorSpecialization)
                        {
                            var specialization = new DoctorSpecialityInfo();
                            specialization.SpecialityId = Convert.ToInt32(s);
                            specialization.UserId = model.Id;

                            _context.DoctorSpecialityInfo.Add(specialization);
                            _context.SaveChanges();
                        }

                        var doctorUserAddress = _context.UserAddress
                            .Where(a => a.UserId == model.Id)
                            .FirstOrDefault();

                        doctorUserAddress.CountryId = model.CountryId;
                        doctorUserAddress.StateId = model.StateId;
                        doctorUserAddress.City = model.City;
                        doctorUserAddress.CurrentAddress = model.Address;

                        _context.UserAddress.Update(doctorUserAddress);
                        _context.SaveChanges();

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = "Doctor detail" + ResponseMessages.msgUpdationSuccess,
                                code = StatusCodes.Status200OK,
                                //data = model
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
                                message = ResponseMessages.msgUserNotFound,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgUserNotFound,
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

        #region AssignDoctor
        [HttpPost]
        [Authorize]
        [Route("AssignDoctor")]
        public async Task<IActionResult> AssignDoctor([FromBody] AssignDoctorViewModel model)
        {
            //ApplicationUser _appuser = new ApplicationUser();
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
                    var doctorObj = await _userManager.FindByIdAsync(model.DoctorId);

                    if (
                        model.ServiceTypeValue
                        == Convert.ToInt32(GlobalVariables.ServiceType.HomeService)
                    )
                    {
                        if (doctorObj != null && doctorObj.IsDeleted != true)
                        {
                            var bookAppointment = _context.BookHomeService.Find(model.BookingId);
                            if (bookAppointment.IsAssignedDoctor == false)
                            {
                                bookAppointment.IsAssignedDoctor = true;
                                bookAppointment.DoctorId = model.DoctorId;
                                //bookAppointment.Status = "Assigned";

                                _context.BookHomeService.Update(bookAppointment);
                                _context.SaveChanges();

                                return Ok(
                                    new ApiResponseModel
                                    {
                                        status = true,
                                        message = ResponseMessages.msgSuccessfully,
                                        code = StatusCodes.Status200OK,
                                        //data = model
                                    }
                                );
                            }
                            else
                            {
                                return Ok(
                                    new ApiResponseModel
                                    {
                                        status = true,
                                        message =
                                            ResponseMessages.msgNotApplicableForBookAppointment,
                                        code = StatusCodes.Status200OK,
                                        //data = model
                                    }
                                );
                            }
                        }
                        else
                        {
                            if (doctorObj != null && doctorObj.IsDeleted != true)
                            {
                                var bookAppointment = _context.BookLabService.Find(model.BookingId);
                                if (
                                    bookAppointment.Status
                                    == (int)GlobalVariables.AppointmentStatus.Pending
                                )
                                {
                                    bookAppointment.IsAssignedDoctor = true;
                                    bookAppointment.DoctorId = model.DoctorId;
                                    //bookAppointment.Status = "Assigned";

                                    _context.BookLabService.Update(bookAppointment);
                                    _context.SaveChanges();

                                    return Ok(
                                        new ApiResponseModel
                                        {
                                            status = true,
                                            message = ResponseMessages.msgSuccessfully,
                                            code = StatusCodes.Status200OK,
                                            //data = model
                                        }
                                    );
                                }
                                else
                                {
                                    return Ok(
                                        new ApiResponseModel
                                        {
                                            status = true,
                                            message =
                                                ResponseMessages.msgNotApplicableForBookAppointment,
                                            code = StatusCodes.Status200OK,
                                            //data = model
                                        }
                                    );
                                }
                            }
                        }
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgUserNotFound,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgUserNotFound,
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

        #region DeleteDoctorDetails
        [HttpDelete]
        [Authorize]
        [Route("DeleteDoctorDetails")]
        public async Task<IActionResult> DeleteDoctorDetails(string Id)
        {
            //ApplicationUser _appuser = new ApplicationUser();
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
                    var doctorObj = await _userManager.FindByIdAsync(Id);

                    if (doctorObj.IsDeleted != true)
                    {
                        doctorObj.IsDeleted = true;
                        await _userManager.UpdateAsync(doctorObj);
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgDeletionSuccess,
                                code = StatusCodes.Status200OK,
                                //data = model
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgUserNotFound,
                                code = StatusCodes.Status200OK,
                                //data = model
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
                            code = StatusCodes.Status500InternalServerError
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

        #region GetAvailableDoctorList
        [HttpGet]
        [Authorize]
        [Route("GetAvailableDoctorList")]
        public IActionResult GetAvailableDoctorList(int bookingId, int ServiceTypeValue)
        {
            try
            {
                if (ServiceTypeValue == Convert.ToInt32(GlobalVariables.ServiceType.HomeService))
                {
                    var bookingDoctor = _context.BookHomeService
                        .Where(a => a.BookHomeServiceId == bookingId)
                        .FirstOrDefault();
                    if (bookingDoctor != null)
                    {
                        var source = (
                            from u in _context.ApplicationUser
                            join ur in _context.UserRoles on u.Id equals ur.UserId
                            join r in _context.Roles on ur.RoleId equals r.Id
                            //join w in _context.WalletBillingInfo on u.Id equals w.UserId
                            where
                                r.Name.ToLower()
                                    == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                                && (u.IsDeleted == false)
                            select new GetTopDoctorsViewModel { Id = u.Id }
                        ).ToList();

                        List<string> doctorList = new List<string>();
                        foreach (var item in source)
                        {
                            int docObj = 1;
                            string id = item.Id;
                            var allDoctor = _context.BookHomeService
                                .Where(a => a.DoctorId == id)
                                .FirstOrDefault();
                            if (allDoctor != null)
                            {
                                var bookDoctor = _context.BookHomeService
                                    .Where(a => a.DoctorId == id)
                                    .ToList();

                                // var bookingDoctor = _context.BookHomeService
                                //     .Where(a => a.BookHomeServiceId == bookingId)
                                //     .FirstOrDefault();
                                foreach (var data in bookDoctor)
                                {
                                    if (
                                        data.FromDate == bookingDoctor.FromDate
                                        && data.ToDate == bookingDoctor.ToDate
                                        && data.FromTime == bookingDoctor.FromTime
                                        && data.ToTime == bookingDoctor.ToTime
                                        && data.IsAssignedDoctor == true
                                    )
                                    {
                                        docObj = 0;
                                        System.Console.WriteLine(true);
                                    }
                                }
                            }
                            if (docObj == 1)
                            {
                                doctorList.Add(item.Id);
                            }
                        }

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = doctorList,
                                message = ResponseMessages.msgShownSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgNotFound + "booking for this id",
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    var bookingDoctor = _context.BookLabService
                        .Where(a => a.BookLabServiceId == bookingId)
                        .FirstOrDefault();
                    if (bookingDoctor != null)
                    {
                        var source = (
                            from u in _context.ApplicationUser
                            join ur in _context.UserRoles on u.Id equals ur.UserId
                            join r in _context.Roles on ur.RoleId equals r.Id
                            //join w in _context.WalletBillingInfo on u.Id equals w.UserId
                            where
                                r.Name.ToLower()
                                    == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                                && (u.IsDeleted == false)
                            select new GetTopDoctorsViewModel { Id = u.Id }
                        ).ToList();

                        List<string> doctorList = new List<string>();
                        foreach (var item in source)
                        {
                            int docObj = 1;
                            string id = item.Id;
                            var allDoctor = _context.BookLabService
                                .Where(a => a.DoctorId == id)
                                .FirstOrDefault();
                            if (allDoctor != null)
                            {
                                var bookDoctor = _context.BookLabService
                                    .Where(a => a.DoctorId == id)
                                    .ToList();

                                // var bookingDoctor = _context.BookHomeService
                                //     .Where(a => a.BookHomeServiceId == bookingId)
                                //     .FirstOrDefault();
                                foreach (var data in bookDoctor)
                                {
                                    if (
                                        data.FromDate == bookingDoctor.FromDate
                                        && data.ToDate == bookingDoctor.ToDate
                                        && data.FromTime == bookingDoctor.FromTime
                                        && data.ToTime == bookingDoctor.ToTime
                                        && data.IsAssignedDoctor == true
                                    )
                                    {
                                        docObj = 0;
                                        System.Console.WriteLine(true);
                                    }
                                }
                            }
                            if (docObj == 1)
                            {
                                doctorList.Add(item.Id);
                            }
                        }

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = doctorList,
                                message = ResponseMessages.msgShownSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgNotFound + "booking for this id",
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
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion


        #region Private Methods
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
