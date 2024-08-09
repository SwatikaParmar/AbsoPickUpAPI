using AnfasAPI.Common;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;

using System.Net;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IWebHostEnvironment _hostingEnvironment;
        private IAuthService _authService;
        private IContentService _contentService;
        private IPatientService _patientService;
        private IDoctorService _doctorService;
        private ITwilioManager _twilioManager;
        private IUploadFiles _uploadFiles;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            IWebHostEnvironment hostingEnvironment,
            IAuthService authService,
            IContentService contentService,
            IPatientService patientService,
            IDoctorService doctorService,
            ITwilioManager twilioManager,
            IUploadFiles uploadFiles
        )
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
            _authService = authService;
            _contentService = contentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _twilioManager = twilioManager;
            _uploadFiles = uploadFiles;
        }

        #region "SuperAdminLogin"
        [HttpPost]
        [Route("AdminLogin")]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if ((roles[0]).Equals(GlobalVariables.UserRole.SuperAdmin.ToString()))
                    {
                        user.DeviceToken = model.DeviceToken;
                        user.DeviceType = model.DeviceType;
                        // change the security stamp only on correct username/password
                        await _userManager.UpdateSecurityStampAsync(user);
                        string accessToken = CommonFunctions.GenerateAccessToken(
                            user.Id,
                            roles.FirstOrDefault(),
                            user.DeviceToken
                        );
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new
                                {
                                    email = user.Email,
                                    emailConfirmed = user.EmailConfirmed,
                                    roles = roles[0],
                                    accessToken,
                                    pic = user.ProfilePic
                                },
                                message = ResponseMessages.msgUserLoginSuccess,
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
                                message = ResponseMessages.msgProfileforSuperAdmin,
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
                            message = ResponseMessages.msgInvalidCredentials,
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

        #region "Register Patient"
        [HttpPost]
        [Route("PatientRegister")]
        public async Task<IActionResult> PatientRegister(AddPatientViewModel model)
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
                int otpcode = CommonFunctions.getFourDigitCode();
                _appuser.UserName = model.Email;
                _appuser.Email = model.Email;
                _appuser.DialCode = model.DialCode;
                _appuser.FirstName = model.FirstName;
                _appuser.LastName = model.LastName;
                _appuser.PhoneNumber = model.PhoneNumber;
                _appuser.DeviceType = model.DeviceType.ToLower();
                _appuser.DeviceToken = model.DeviceToken;
                _appuser.IsSocialUser = false;
                _appuser.CreatedDate = DateTime.UtcNow;
                _appuser.Otp = otpcode;
                _appuser.EmailConfirmed = true;
                _appuser.PhoneNumberConfirmed = true;
                var result = await _userManager.CreateAsync(_appuser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(
                        _appuser,
                        GlobalVariables.UserRole.Patient.ToString()
                    );

                    // //send email otp here
                    // var msg = EmailMessages.GetUserRegistrationEmailConfirmationMsg(_appuser.Otp.Value);
                    // await _emailSender.SendEmailAsync(email: _appuser.Email, subject: EmailMessages.confirmationEmailSubject, message: msg);

                    // // send phone otp here
                    // int phoneOtpCode = CommonFunctions.getFourDigitCode();

                    // WebClient client = new WebClient();
                    // string baseurl = "http://mshastra.com/sendurlcomma.aspx?user=20096121&pwd=B15n0de1*&senderid=ANFAS CARE&CountryCode=" + model.DialCode + "&mobileno=" + model.PhoneNumber + "&msgtext=Your confirmation code to verify your phone is " + phoneOtpCode;
                    // Stream data = client.OpenRead(baseurl);
                    // StreamReader reader = new StreamReader(data);
                    // string s = reader.ReadToEnd();
                    // data.Close();
                    // reader.Close();

                    string accessToken = CommonFunctions.GenerateAccessToken(
                        _appuser.Id,
                        GlobalVariables.UserRole.Patient.ToString(),
                        _appuser.DeviceToken
                    );
                    TokenResponseViewModel objtoken = new TokenResponseViewModel();
                    objtoken.accessToken = accessToken;
                    var _userConfigurations = new UserConfigurations()
                    {
                        UserId = _appuser.Id,
                        EmailNotificationStatus = true,
                        PushNotificationsStatus = true,
                        SMSNotificationStatus = true
                    };
                    _contentService.AddUpdateUserConfigurations(_userConfigurations);

                    var addedPatient = new PatientResponseModel();
                    addedPatient.FirstName = _appuser.FirstName;
                    addedPatient.LastName = _appuser.LastName;
                    addedPatient.Email = _appuser.Email;
                    addedPatient.DialCode = _appuser.DialCode;
                    addedPatient.PhoneNumber = _appuser.PhoneNumber;
                    addedPatient.AccessToken = objtoken.accessToken;
                    // addedPatient.EmailOtp = otpcode;
                    // addedPatient.PhoneOtp = phoneOtpCode;

                    return Ok(
                        new
                        {
                            status = true,
                            data = addedPatient,
                            message = ResponseMessages.msgUserRegisterSuccess,
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

        #region "Update Patient Profile"
        [HttpPost]
        [Route("UpdatePatientProfile")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdatePatientProfile(PatientRegistrationViewModel model)
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
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
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
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.GenderId = model.GenderId;
                    user.DateOfBirth = DateTime.ParseExact(
                        model.DateOfBirth,
                        GlobalVariables.DefaultDateFormat,
                        null
                    );
                    user.DialCode = model.DialCode;
                    user.PhoneNumber = model.PhoneNumber;
                    user.CreatedDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    var _patientHealthInfo = new PatientHealthInfo()
                    {
                        UserId = user.Id,
                        Height = 0,
                        BloodGroup = string.Empty,
                        Weight = 0,
                        IsVegetarian = false,
                        UseAlcohol = false,
                        UseDrug = false,
                        UseSmoke = false
                    };
                    _patientService.AddUpdatePatientHealthInfo(_patientHealthInfo);
                    var patientProfileResponse = await _authService.GetPatientProfileResponse(
                        user.Id
                    );
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { patientProfileResponse },
                            message = "Patient Profile" + ResponseMessages.msgUpdationSuccess,
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
                            data = { },
                            message = "Could not find associated user",
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

        //#region Register Patient
        //[HttpPost]
        //[Route("PatientRegister")]
        //public async Task<IActionResult> PatientRegister(PatientRegistrationViewModel model)
        //{
        //    ApplicationUser _appuser = new ApplicationUser();
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgParametersNotCorrect, code = StatusCodes.Status200OK });
        //        }

        //        _appuser.UserName = model.Email;
        //        _appuser.FirstName = model.FirstName;
        //        _appuser.LastName = model.LastName;
        //        _appuser.GenderId = model.GenderId;
        //        _appuser.Email = model.Email;
        //        _appuser.DateOfBirth = DateTime.ParseExact(model.DateOfBirth, GlobalVariables.DefaultDateFormat, null);
        //        _appuser.DeviceType = model.DeviceType.ToLower();
        //        _appuser.DeviceToken = model.DeviceToken;
        //        _appuser.DialCode = model.DialCode;
        //        _appuser.PhoneNumber = model.PhoneNumber;
        //        _appuser.IsSocialUser = false;
        //        _appuser.CreatedDate = DateTime.UtcNow;
        //        var result = await _userManager.CreateAsync(_appuser, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(_appuser, GlobalVariables.UserRole.Patient.ToString());
        //            string accessToken = CommonFunctions.GenerateAccessToken(_appuser.Id, GlobalVariables.UserRole.Patient.ToString(), _appuser.DeviceToken);
        //            var _userConfigurations = new UserConfigurations()
        //            {
        //                UserId = _appuser.Id,
        //                EmailNotificationStatus = true,
        //                PushNotificationsStatus = true,
        //                SMSNotificationStatus = true
        //            };
        //            _contentService.AddUpdateUserConfigurations(_userConfigurations);

        //            var _patientHealthInfo = new PatientHealthInfo()
        //            {
        //                UserId = _appuser.Id,
        //                Height = 0,
        //                BloodGroup = string.Empty,
        //                Weight = 0,
        //                IsVegetarian = false,
        //                UseAlcohol = false,
        //                UseDrug = false,
        //                UseSmoke = false
        //            };
        //            _patientService.AddUpdatePatientHealthInfo(_patientHealthInfo);

        //            var patientLoginResponse = await _authService.GetPatientLoginResponse(_appuser.Id, accessToken);
        //            return Ok(new ApiResponseModel
        //            {
        //                status = true,
        //                data = patientLoginResponse,
        //                message = "Patient" + ResponseMessages.msgCreationSuccess,
        //                code = StatusCodes.Status200OK
        //            });
        //        }
        //        else
        //        {
        //            return Ok(new ApiResponseModel { status = false, data = new { }, message = result.Errors.First().Description, code = StatusCodes.Status200OK });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong + ex.Message, data = new { }, code = StatusCodes.Status500InternalServerError });
        //    }
        //}
        //#endregion

        #region Patient Login
        [HttpPost]
        [Route("PatientLogin")]
        public async Task<IActionResult> PatientLogin(LoginViewModel model)
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
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await _userManager.CheckPasswordAsync(user, model.Password);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    user.DeviceToken = model.DeviceToken;
                    user.DeviceType = model.DeviceType.ToLower();
                    // change security stamp only on current username/password
                    await _userManager.UpdateSecurityStampAsync(user);
                    // fetching roles of user
                    var roles = await _userManager.GetRolesAsync(user);
                    // Generating access Token
                    string accessToken = CommonFunctions.GenerateAccessToken(
                        user.Id,
                        roles.FirstOrDefault(),
                        user.DeviceToken
                    );
                    if (roles[0].ToLower() == GlobalVariables.UserRole.Patient.ToString().ToLower())
                    {
                        var patientLoginResponse = await _authService.GetPatientLoginResponse(
                            user.Id,
                            accessToken
                        );
                        // if (!user.EmailConfirmed)
                        // {
                        //     return Ok(new ApiResponseModel
                        //     {
                        //         status = false,
                        //         message = ResponseMessages.msgEmailNotConfirmed,
                        //         data = patientLoginResponse,
                        //         code = StatusCodes.Status200OK
                        //     });
                        // }
                        // else if (!user.PhoneNumberConfirmed)
                        // {
                        //     return Ok(new ApiResponseModel
                        //     {
                        //         status = false,
                        //         message = ResponseMessages.msgPhoneNotConfirmed,
                        //         data = patientLoginResponse,
                        //         code = StatusCodes.Status200OK
                        //     });
                        // }
                        // else
                        // {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = patientLoginResponse,
                                message = ResponseMessages.msgUserLoginSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                        // }
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgProfileforPatient,
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
                            message = ResponseMessages.msgInvalidCredentials,
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

        #region "Patient Social Login"
        [HttpPost]
        [Route("PatientSocialLogin")]
        public async Task<IActionResult> PatientSocialLogin(SocialLoginViewModel model)
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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ApplicationUser _appuser = new ApplicationUser();
                    _appuser.FirstName = model.FirstName;
                    _appuser.LastName = model.LastName;
                    if (
                        model.LoginType.ToLower()
                        == GlobalVariables.LoginType.Facebook.ToString().ToLower()
                    )
                    {
                        _appuser.FacebookId = model.SocialId;
                    }
                    else if (
                        model.LoginType.ToLower()
                        == GlobalVariables.LoginType.Google.ToString().ToLower()
                    )
                    {
                        _appuser.GoogleId = model.SocialId;
                    }
                    else
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
                    _appuser.Email = model.Email;
                    _appuser.EmailConfirmed = true;
                    _appuser.DeviceType = model.DeviceType.ToLower();
                    _appuser.DeviceToken = model.DeviceToken;
                    _appuser.IsSocialUser = true;
                    _appuser.CreatedDate = DateTime.UtcNow;
                    var result = await _userManager.CreateAsync(_appuser);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(
                            _appuser,
                            GlobalVariables.UserRole.Patient.ToString()
                        );
                        string accessToken = CommonFunctions.GenerateAccessToken(
                            _appuser.Id,
                            GlobalVariables.UserRole.Patient.ToString(),
                            _appuser.DeviceToken
                        );
                        var _userConfigurations = new UserConfigurations()
                        {
                            UserId = _appuser.Id,
                            EmailNotificationStatus = true,
                            PushNotificationsStatus = true,
                            SMSNotificationStatus = true
                        };
                        _contentService.AddUpdateUserConfigurations(_userConfigurations);

                        var _patientHealthInfo = new PatientHealthInfo()
                        {
                            UserId = _appuser.Id,
                            Height = 0,
                            BloodGroup = string.Empty,
                            Weight = 0,
                            IsVegetarian = false,
                            UseAlcohol = false,
                            UseDrug = false,
                            UseSmoke = false
                        };
                        _patientService.AddUpdatePatientHealthInfo(_patientHealthInfo);

                        var patientLoginResponse = await _authService.GetPatientLoginResponse(
                            _appuser.Id,
                            accessToken
                        );
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = patientLoginResponse,
                                message = "Patient" + ResponseMessages.msgCreationSuccess,
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
                else
                {
                    if (string.IsNullOrEmpty(user.FirstName))
                    {
                        user.FirstName = model.FirstName;
                    }

                    if (string.IsNullOrEmpty(user.LastName))
                    {
                        user.LastName = model.LastName;
                    }
                    user.DeviceToken = model.DeviceToken;
                    user.DeviceType = model.DeviceType.ToLower();
                    // change security stamp only on current username/password
                    await _userManager.UpdateSecurityStampAsync(user);
                    // fetching roles of user
                    var roles = await _userManager.GetRolesAsync(user);
                    // Generating access Token
                    string accessToken = CommonFunctions.GenerateAccessToken(
                        user.Id,
                        roles.FirstOrDefault(),
                        user.DeviceToken
                    );
                    if (roles[0].ToLower() == GlobalVariables.UserRole.Patient.ToString().ToLower())
                    {
                        var patientLoginResponse = await _authService.GetPatientLoginResponse(
                            user.Id,
                            accessToken
                        );

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = patientLoginResponse,
                                message = ResponseMessages.msgUserLoginSuccess,
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
                                message = ResponseMessages.msgProfileforPatient,
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

        #region Register Doctor
        [HttpPost]
        [Route("DoctorRegister")]
        public async Task<IActionResult> DoctorRegister(DoctorRegistrationViewModel model)
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
                _appuser.DeviceType = model.DeviceType.ToLower();
                _appuser.DeviceToken = model.DeviceToken;
                _appuser.DialCode = model.DialCode;
                _appuser.PhoneNumber = model.PhoneNumber;
                _appuser.IsSocialUser = false;
                _appuser.LastScreenId = (int)GlobalVariables.LastScreenInfo.PersonalInfo;
                _appuser.Otp = CommonFunctions.getFourDigitCode();
                _appuser.CreatedDate = DateTime.UtcNow;
                var result = await _userManager.CreateAsync(_appuser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(
                        _appuser,
                        GlobalVariables.UserRole.Doctor.ToString()
                    );

                    //send email here
                    var msg = EmailMessages.GetUserRegistrationEmailConfirmationMsg(
                        _appuser.Otp.Value
                    );
                    await _emailSender.SendEmailAsync(
                        email: _appuser.Email,
                        subject: EmailMessages.confirmationEmailSubject,
                        message: msg
                    );

                    string accessToken = CommonFunctions.GenerateAccessToken(
                        _appuser.Id,
                        GlobalVariables.UserRole.Doctor.ToString(),
                        _appuser.DeviceToken
                    );
                    var _userConfigurations = new UserConfigurations()
                    {
                        UserId = _appuser.Id,
                        EmailNotificationStatus = true,
                        PushNotificationsStatus = true,
                        SMSNotificationStatus = true
                    };
                    _contentService.AddUpdateUserConfigurations(_userConfigurations);

                    var doctorLoginResponse = await _authService.GetDoctorLoginResponse(
                        _appuser.Id,
                        accessToken
                    );
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = doctorLoginResponse,
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

        #region Doctor Login
        [HttpPost]
        [Route("DoctorLogin")]
        public async Task<IActionResult> DoctorLogin(LoginViewModel model)
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
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await _userManager.CheckPasswordAsync(user, model.Password);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    user.DeviceToken = model.DeviceToken;
                    user.DeviceType = model.DeviceType.ToLower();
                    user.IsSocialUser = false;
                    // change security stamp only on current username/password
                    await _userManager.UpdateSecurityStampAsync(user);
                    // fetching roles of user
                    var roles = await _userManager.GetRolesAsync(user);
                    // Generating access Token
                    string accessToken = CommonFunctions.GenerateAccessToken(
                        user.Id,
                        roles.FirstOrDefault(),
                        user.DeviceToken
                    );
                    if (roles[0].ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower())
                    {
                        var doctorLoginResponse = await _authService.GetDoctorLoginResponse(
                            user.Id,
                            accessToken
                        );
                        if (!user.EmailConfirmed)
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = true,
                                    message = ResponseMessages.msgGotSuccess,
                                    data = doctorLoginResponse,
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                        else if (
                            user.ApplicationStatus
                            == (int)GlobalVariables.DoctorApplicationStatus.Rejected
                        )
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    data = new { },
                                    message = ResponseMessages.msgUserBlockedByAdmin,
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                        else if (
                            user.ApplicationStatus
                            == (int)GlobalVariables.DoctorApplicationStatus.Pending
                        )
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    data = new { },
                                    message = ResponseMessages.msgUserStatusPendingForApproval,
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
                                    data = doctorLoginResponse,
                                    message = ResponseMessages.msgUserLoginSuccess,
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
                                message = ResponseMessages.msgProfileforDoctor,
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
                            message = ResponseMessages.msgInvalidCredentials,
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

        #region "Doctor Social Login"
        [HttpPost]
        [Route("DoctorSocialLogin")]
        public async Task<IActionResult> DoctorSocialLogin(SocialLoginViewModel model)
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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { isUserExist = false },
                            message = ResponseMessages.msgCouldNotFoundAssociatedUser,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    if (string.IsNullOrEmpty(user.FirstName))
                    {
                        user.FirstName = model.FirstName;
                    }

                    if (string.IsNullOrEmpty(user.LastName))
                    {
                        user.LastName = model.LastName;
                    }

                    if (
                        model.LoginType.ToLower()
                        == GlobalVariables.LoginType.Facebook.ToString().ToLower()
                    )
                    {
                        user.FacebookId = model.SocialId;
                    }
                    else if (
                        model.LoginType.ToLower()
                        == GlobalVariables.LoginType.Google.ToString().ToLower()
                    )
                    {
                        user.GoogleId = model.SocialId;
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgParametersNotCorrect,
                                code = StatusCodes.Status406NotAcceptable
                            }
                        );
                    }

                    user.DeviceToken = model.DeviceToken;
                    user.DeviceType = model.DeviceType.ToLower();
                    user.IsSocialUser = true;
                    // change security stamp only on current username/password
                    await _userManager.UpdateSecurityStampAsync(user);
                    // fetching roles of user
                    var roles = await _userManager.GetRolesAsync(user);
                    // Generating access Token
                    string accessToken = CommonFunctions.GenerateAccessToken(
                        user.Id,
                        roles.FirstOrDefault(),
                        user.DeviceToken
                    );
                    if (roles[0].ToLower() == GlobalVariables.UserRole.Doctor.ToString().ToLower())
                    {
                        var doctorLoginResponse = await _authService.GetDoctorLoginResponse(
                            user.Id,
                            accessToken
                        );
                        if (
                            user.ApplicationStatus
                            == (int)GlobalVariables.DoctorApplicationStatus.Rejected
                        )
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    data = new { },
                                    message = ResponseMessages.msgUserBlockedByAdmin,
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                        else if (
                            user.ApplicationStatus
                            == (int)GlobalVariables.DoctorApplicationStatus.Pending
                        )
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = false,
                                    data = new { },
                                    message = ResponseMessages.msgUserStatusPendingForApproval,
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
                                    data = doctorLoginResponse,
                                    message = ResponseMessages.msgUserLoginSuccess,
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
                                message = ResponseMessages.msgProfileforDoctor,
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

        #region "Doctor Register Login"
        [HttpPost]
        [Route("DoctorSocialRegister")]
        public async Task<IActionResult> DoctorSocialRegister(DoctorSocialLoginViewModel model)
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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ApplicationUser _appuser = new ApplicationUser();
                    _appuser.FirstName = model.FirstName;
                    _appuser.LastName = model.LastName;
                    _appuser.GenderId = model.GenderId;
                    _appuser.DateOfBirth = DateTime.ParseExact(
                        model.DateOfBirth,
                        GlobalVariables.DefaultDateFormat,
                        null
                    );
                    _appuser.DialCode = model.DialCode;
                    _appuser.PhoneNumber = model.PhoneNumber;
                    if (
                        model.LoginType.ToLower()
                        == GlobalVariables.LoginType.Facebook.ToString().ToLower()
                    )
                    {
                        _appuser.FacebookId = model.SocialId;
                    }
                    else if (
                        model.LoginType.ToLower()
                        == GlobalVariables.LoginType.Google.ToString().ToLower()
                    )
                    {
                        _appuser.GoogleId = model.SocialId;
                    }
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgParametersNotCorrect,
                                code = StatusCodes.Status406NotAcceptable
                            }
                        );
                    }

                    _appuser.UserName = model.Email;
                    _appuser.Email = model.Email;
                    _appuser.EmailConfirmed = true;
                    _appuser.DeviceType = model.DeviceType.ToLower();
                    _appuser.DeviceToken = model.DeviceToken;
                    _appuser.IsSocialUser = true;
                    _appuser.LastScreenId = (int)GlobalVariables.LastScreenInfo.PersonalInfo;
                    _appuser.CreatedDate = DateTime.UtcNow;
                    var result = await _userManager.CreateAsync(_appuser);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(
                            _appuser,
                            GlobalVariables.UserRole.Doctor.ToString()
                        );
                        string accessToken = CommonFunctions.GenerateAccessToken(
                            _appuser.Id,
                            GlobalVariables.UserRole.Doctor.ToString(),
                            _appuser.DeviceToken
                        );
                        var _userConfigurations = new UserConfigurations()
                        {
                            UserId = _appuser.Id,
                            EmailNotificationStatus = true,
                            PushNotificationsStatus = true,
                            SMSNotificationStatus = true
                        };
                        _contentService.AddUpdateUserConfigurations(_userConfigurations);

                        var doctorLoginResponse = await _authService.GetDoctorLoginResponse(
                            _appuser.Id,
                            accessToken
                        );
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = doctorLoginResponse,
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
                else
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { isUserExist = true },
                            message = "User" + ResponseMessages.msgAlreadyExists,
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

        #region ForgotPassword
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(EmailModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Email))
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
                var user = await _userManager.FindByEmailAsync(model.Email);
                int OtpCode = CommonFunctions.getFourDigitCode();
                if (user != null)
                {
                    user.Otp = OtpCode;
                    await _userManager.UpdateAsync(user);
                    var msg = EmailMessages.GetUserForgotPasswordMsg(user.FirstName, OtpCode);
                    await _emailSender.SendEmailAsync(
                        email: user.Email,
                        subject: EmailMessages.resetPasswordSubject,
                        message: msg
                    );
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { OtpCode },
                            message = ResponseMessages.msgOTPSentSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgCouldNotFoundAssociatedUser,
                        data = new { },
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
                        message = ResponseMessages.msgSomethingWentWrong + " " + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region ResetPassword
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
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
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.Otp == Convert.ToInt32(model.otp))
                    {
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        IdentityResult result = await _userManager.ResetPasswordAsync(
                            user,
                            code,
                            model.newPassword
                        );
                        if (result.Succeeded)
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = true,
                                    data = new { },
                                    message = ResponseMessages.msgPasswordChangeSuccess,
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
                    else
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgInvalidOTP,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgCouldNotFoundAssociatedUser,
                        data = new { },
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
                        message = ResponseMessages.msgSomethingWentWrong + " " + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
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
                if (model.OldPassword == model.NewPassword)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSamePasswords,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var CurrentUserId = CommonFunctions.getUserId(User);
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
                    IdentityResult result = await _userManager.ChangePasswordAsync(
                        user,
                        model.OldPassword,
                        model.NewPassword
                    );
                    if (result.Succeeded)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = ResponseMessages.msgPasswordChangeSuccess,
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
                else
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
            catch (Exception ex)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region VerifyEmail
        [HttpPost]
        [Route("VerifyEmail")]
        [Authorize]
        public async Task<IActionResult> VerifyEmail(VerifyUserModel model)
        {
            try
            {
                var CurrentUserId = CommonFunctions.getUserId(User);
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
                    var roles = await _userManager.GetRolesAsync(user);
                    // Check whether otp matches
                    if (user.Otp == Convert.ToInt32(model.otp))
                    {
                        user.EmailConfirmed = true;
                        if (
                            roles[0].ToLower()
                            == GlobalVariables.UserRole.Doctor.ToString().ToLower()
                        )
                        {
                            user.LastScreenId = (int)GlobalVariables.LastScreenInfo.Verification;
                        }
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = true,
                                    data = new { },
                                    message = ResponseMessages.msgVerifiedUser,
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
                                message = ResponseMessages.msgInvalidOTP,
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
                            message = ResponseMessages.msgCouldNotFoundAssociatedUser,
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
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region ResendEmailCode
        [HttpPost]
        [Route("ResendEmailCode")]
        public async Task<IActionResult> ResendEmailCode(EmailModel model)
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
                var user = await _userManager.FindByEmailAsync(model.Email);
                int otpcode = CommonFunctions.getFourDigitCode();

                if (model.IsUserExisting == true)
                {
                    if (user == null)
                    {
                        await _emailSender.SendEmailAsync(
                            email: model.Email,
                            subject: EmailMessages.confirmationEmailSubject,
                            message: "Your ANFAS OTP is " + otpcode.ToString()
                        );
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { otpcode },
                                message = "OTP" + ResponseMessages.msgSentSuccess,
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
                                // data = new { otpcode },
                                message = "Email" + ResponseMessages.msgAlreadyExists,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                else
                {
                    if (user == null)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                message = ResponseMessages.msgCouldNotFoundAssociatedUser,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        user.Otp = otpcode;
                        await _userManager.UpdateAsync(user);
                        var msg = EmailMessages.GetUserRegistrationResendEmailConfirmationMsg(
                            user.FirstName,
                            Convert.ToInt32(user.Otp)
                        );

                        await _emailSender.SendEmailAsync(
                            email: model.Email,
                            subject: EmailMessages.confirmationEmailSubject,
                            message: msg
                        );
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { otpcode },
                                message = ResponseMessages.msgOTPSentSuccess,
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

        #region VerifyPhone
        [HttpPost]
        [Route("VerifyPhone")]
        [Authorize]
        public async Task<IActionResult> VerifyPhone(VerifyPhoneModel model)
        {
            try
            {
                var CurrentUserId = CommonFunctions.getUserId(User);
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
                    string userPhoneNo = "+" + user.DialCode + user.PhoneNumber;

                    var verificationResult = await _twilioManager.CheckVerificationAsync(
                        userPhoneNo,
                        model.code
                    );
                    if (verificationResult.IsValid)
                    {
                        user.PhoneNumberConfirmed = true;
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = true,
                                    data = new { },
                                    message = ResponseMessages.msgPhoneNoVerifiedSuccess,
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
                                message = verificationResult.Errors.FirstOrDefault().ToString(),
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
                            message = ResponseMessages.msgCouldNotFoundAssociatedUser,
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
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        data = new { },
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region ResendPhoneOtp
        [HttpPost]
        [Route("ResendPhoneOtp")]
        [Authorize]
        public async Task<IActionResult> ResendPhoneOtp(PhoneModel model)
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

                var CurrentUserId = CommonFunctions.getUserId(User);
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
                    user.DialCode = model.DialCode;
                    user.PhoneNumber = model.PhoneNo;
                    await _userManager.UpdateAsync(user);

                    string userPhoneNo = "+" + user.DialCode + user.PhoneNumber;
                    var verificationResult = await _twilioManager.StartVerificationAsync(
                        userPhoneNo,
                        GlobalVariables.TwilioChannelTypes.Sms.ToString().ToLower()
                    );
                    if (verificationResult.IsValid)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = ResponseMessages.msgOTPSentOnMobileSuccess,
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
                                message = verificationResult.Errors.FirstOrDefault().ToString(),
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                }
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgCouldNotFoundAssociatedUser,
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

        #region UpdateUserProfilePic
        [HttpPost]
        [Authorize]
        [Route("UpdateProfilePic")]
        public async Task<IActionResult> UpdateProfilePic([FromForm] ImageFileUploadModel model)
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
                        .Parse(model.imgFile.ContentDisposition)
                        .FileName.Trim('"');
                    filename = CommonFunctions.EnsureCorrectFilename(filename);
                    filename = CommonFunctions.RenameFileName(filename);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(filename, GlobalVariables.profilePictureContainer)))
                    // {
                    //     model.imgFile.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.imgFile,
                        GlobalVariables.profilePictureContainer, filename
                    );

                    user.ProfilePic = GlobalVariables.profilePictureContainer + "/" + filename;
                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { profilepicurl = user.ProfilePic },
                                message = ResponseMessages.msgProfilePicUpdated,
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

        #region Logout
        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
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
                            code = StatusCodes.Status401Unauthorized
                        }
                    );
                }

                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    user.DeviceType = string.Empty;
                    user.DeviceToken = string.Empty;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = ResponseMessages.msgLogoutSuccess,
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
                                message = result.Errors.FirstOrDefault().Description,
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

        #region Add Doctor Professional Info
        [HttpPost]
        [Authorize]
        [Route("AddDoctorSpecialityInfo")]
        public async Task<IActionResult> AddDoctorSpecialityInfo(
            DoctorProfessionalInfoViewModel model
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

                var doctorSpecialityViewModel = new DoctorSpecialityViewModel()
                {
                    UserId = user.Id,
                    SpecialityId = model.SpecialityId
                };
                _doctorService.UpdateDoctorSpeciality(doctorSpecialityViewModel);

                var _useraddress = new UserAddress()
                {
                    UserId = user.Id,
                    NationalityId = model.NationalityId,
                    CountryId = model.CountryId,
                    StateId = model.StateId,
                    City = model.City,
                    CurrentAddress = model.Address
                };
                _contentService.AddUpdateAddressInfo(_useraddress);

                user.RegNo = model.RegNo;
                user.LastScreenId = (int)GlobalVariables.LastScreenInfo.ProfessionalInfo;
                user.ApplicationStatus = (int)GlobalVariables.DoctorApplicationStatus.Pending;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = ResponseMessages.msgDoctorApplicationSubmitForApproval,
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

        #region ResendEmailCode
        [HttpPost]
        [Route("sendPhoneOtp")]
        public IActionResult sendPhoneOtp(PhoneModel model)
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

                int otpcode = CommonFunctions.getFourDigitCode();

                WebClient client = new WebClient();
                // string baseurl = "http://mshastra.com/sendurlcomma.aspx?user=20096121&pwd=B15n0de1*&senderid=ANFAS CARE&CountryCode=" + model.DialCode + "&mobileno=" + model.PhoneNo + "&msgtext=Your confirmation code to verify your phone is " + otpcode;
                // Stream data = client.OpenRead(baseurl);
                // StreamReader reader = new StreamReader(data);
                // string s = reader.ReadToEnd();
                // data.Close();
                // reader.Close();

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        data = new { otpcode },
                        message = "Otp sent successfully.",
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
