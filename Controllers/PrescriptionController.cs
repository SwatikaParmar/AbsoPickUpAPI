using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IPrescriptionService _prescriptionService;
        private readonly INotificationService _notificationService;
        private readonly IContentService _contentService;
        private readonly IEmailSender _emailSenderService;

        public PrescriptionController(UserManager<ApplicationUser> userManager, IPrescriptionService prescriptionService, INotificationService notificationService, IContentService contentService, IEmailSender emailSenderService)
        {
            _userManager = userManager;
            _prescriptionService = prescriptionService;
            _notificationService = notificationService;
            _contentService = contentService;
            _emailSenderService = emailSenderService;
        }

        #region AddPrescription
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("AddPrescription")]
        public async Task<IActionResult> AddPrescription(PrescriptionsViewModel model)
        {
            try
            {
                if(!ModelState.IsValid)
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
                if(string.IsNullOrEmpty(CurrentUserId))
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
                    var _Prescriptions = new Prescriptions()
                    {
                        DoctorId = user.Id,
                        PatientId = model.PatientId,
                        AppointmentId = model.AppointmentId,
                        MedicareNumber = model.MedicareNumber,
                        ReferenceNumber = model.ReferenceNumber,
                        EntitlementNumber = model.EntitlementNumber,
                        ISPBSSafetyEntitlementCardHolder = model.ISPBSSafetyEntitlementCardHolder,
                        IsPBSSafetyConcessionCardHolder = model.IsPBSSafetyConcessionCardHolder,
                        IsPBSPrescriptionFromStateManager = model.IsPBSPrescriptionFromStateManager,
                        IsRBPSPrescription = model.IsRBPSPrescription,
                        IsBrandNotPermitted = model.IsBrandNotPermitted,
                        CreatedDate = DateTime.UtcNow
                    };
                    var result = _prescriptionService.AddPrescriptions(_Prescriptions);
                    if(result.Status)
                    {
                        foreach (var item in model.PatientMedicines)
                        {
                            var _medicines = new Medications();
                            _medicines.PrescriptionId = result.ReturnId;
                            _medicines.MedicineName = item.MedicineName;
                            _medicines.DosageDirections = item.DosageDirections;
                            _medicines.Quantity = item.Quantity;
                            _medicines.NumberOfRepeats = item.NumberOfRepeats;
                            _medicines.Date = DateTime.ParseExact(item.Date, GlobalVariables.DefaultDateFormat, null);
                           _prescriptionService.AddPatientMedications(_medicines);

                        }
                        //// create notification
                        //Notifications objNotifications = new Notifications();
                        //objNotifications.FromUser = CurrentUserId;
                        //objNotifications.ToUser = model.PatientId;
                        //objNotifications.Type = (int)GlobalVariables.NotificationTypes.DOCTOR_PRESCRIPTION;
                        //objNotifications.Text = NotificationMessages.GetPrescriptionNotificationMsg();
                        //objNotifications.IsRead = false;
                        //objNotifications.CreatedOn = DateTime.UtcNow;
                        //objNotifications.Purpose = (int)GlobalVariables.PurposeTypes.Prescription;
                        //objNotifications.PurposeId = result.ReturnId;
                        //_notificationService.CreateNotification(objNotifications);

                        //// sending push notification
                        //var _userConfiguration = _contentService.GetUserConfigurations(model.PatientId);
                        //var _patient = await _userManager.FindByIdAsync(model.PatientId);
                        //var _patientName = _patient.FirstName + " " + _patient.LastName;
                        //var _doctor = await _userManager.FindByIdAsync(CurrentUserId);
                        //var _doctorName = _doctor.FirstName + " " + _doctor.LastName;

                        //if(_userConfiguration.PushNotificationStatus)
                        //{
                        //    string[] tokens = new string[] { _patient.DeviceToken };
                        //    var body = PushNotificationMessages.GetPatientPrescriptionByDoctorMsg(_doctorName, model.AppointmentId);
                        //    await _notificationService.SendPushNotification(tokens, PushNotificationMessages.doctorPrescriptionTitle, body);
                        //}
                        ////sending email to patient
                        //if (_userConfiguration.EmailNotificationStatus)
                        //{
                        //    var _emailMsg = EmailMessages.GetPatientPrescriptionEmailNotificationMsg(_patientName, _doctorName, model.AppointmentId);
                        //    await _emailSenderService.SendEmailAsync(email: _patient.Email, subject: EmailMessages.GetPatientPrescriptionEmailSubject(_doctorName), message: _emailMsg);

                        //}

                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Prescription info" + ResponseMessages.msgAdditionSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else if(result.Status ==  false)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "No appointment exists",
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
            catch(Exception ex)
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

        #region GetPrescriptionList
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("GetPrescriptionList")]
        public async Task<IActionResult> GetPrescriptionList(PrescriptionListViewModel model)
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
                    var _prescriptionList = _prescriptionService.GetPrescriptionList(user.Id, model);
                    if (_prescriptionList != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = _prescriptionList,
                            message = "Patient Prescription List" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = false,
                            message = ResponseMessages.msgNotFound,
                            data = new { },
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

        #region UpdatePrescription
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("UpdatePrescription")]
        public async Task<IActionResult> UpdatePrescription(UpdatePrescriptionViewModel model)
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
                bool result = _prescriptionService.UpdatePrescriptionDetails(user.Id, model);
                if(result)
                {
                    return Ok(new { status=result, data=new { }, message="Prescription" + ResponseMessages.msgUpdationSuccess, code=StatusCodes.Status200OK });
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
            catch(Exception ex)
            {
                return Ok(new ApiResponseModel
                {
                    status = false,
                    data = new { },
                    message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                    code = StatusCodes.Status200OK
                });
            }
        }
        #endregion

        #region DeleteMedicine
        [HttpDelete]
        [Route("DeleteMedication")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteMedication(string MedicineId)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    return Ok(new { status = false, message = ResponseMessages.msgTokenExpired, data = new { }, code = StatusCodes.Status200OK });
                }
                if(string.IsNullOrEmpty(MedicineId))
                {
                    return Ok(new { status = false, message = ResponseMessages.msgParametersNotCorrect, data = new { }, code = StatusCodes.Status200OK });
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if(user != null)
                {
                    var result = await _prescriptionService.DeleteMedicationById(MedicineId);
                    if (result)
                    {
                        return Ok(new { status = result, message = "Medicine" + ResponseMessages.msgDeletionSuccess, data = new { }, code = StatusCodes.Status200OK });
                    }
                    else
                    {
                        return Ok(new { status = false, message = ResponseMessages.msgSomethingWentWrong, data = new { }, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    return Ok(new { status = false, message = ResponseMessages.msgCouldNotFoundAssociatedUser, data = new { }, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ResponseMessages.msgSomethingWentWrong + ex.Message, data = new { }, code = StatusCodes.Status200OK });
            }
        }
        #endregion

    }
}
