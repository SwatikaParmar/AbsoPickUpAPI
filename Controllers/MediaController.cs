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
    public class MediaController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IMediaService _mediaService;
        private readonly IAppointmentService _appointmentService;
        public MediaController(UserManager<ApplicationUser> userManager, IMediaService mediaService, IAppointmentService appointmentService)
        {

            _userManager = userManager;
            _mediaService = mediaService;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Authorize]
        [Route("GetVideoCallToken")]
        public async Task<IActionResult> GetVideoCallToken(string AppointmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(AppointmentId))
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
                var appointmentDetails = _appointmentService.GetAppointmentById(AppointmentId);
                if (appointmentDetails != null)
                {
                    // Replace with meaningful metadata for the connection.
                    string connectionMetadata = "userId=" + user.Id + ",userName=" + user.FirstName + " " + user.LastName + ",email=" + user.Email;
                    bool status = _mediaService.CheckAppointmentCallSession(appointmentDetails);
                    if (status)
                    {
                        AppointmentMediaSession _sessionMedia = new AppointmentMediaSession();
                        _sessionMedia.AppointmentId = appointmentDetails.AppointmentId;
                        _sessionMedia.DoctorId = appointmentDetails.DoctorId;
                        _sessionMedia.PatientId = appointmentDetails.PatientId;
                        _sessionMedia.SessionId = OpentokMediaManager.GetOpentokSession();
                        _sessionMedia.connectionId = Guid.NewGuid().ToString();
                        _sessionMedia.ExpireTime = Convert.ToDecimal(OpentokMediaManager.GetExpirationTime());
                        _sessionMedia.Token = OpentokMediaManager.GetOpentokVideoCallToken(_sessionMedia.SessionId, Convert.ToDouble(_sessionMedia.ExpireTime), connectionMetadata);
                        _sessionMedia.CreatedBy = currentUserId;
                        _sessionMedia.CreatedDate = DateTime.UtcNow;
                        _sessionMedia.IsCompleted = false;
                        var mediaValue = _mediaService.CheckCreateAppointmentCallSession(_sessionMedia);
                        if (mediaValue != null)
                        {
                            return Ok(new ApiResponseModel
                            {
                                status = true,
                                message = ResponseMessages.msgCallShouldInitializeSoon,
                                data = new
                                {
                                    mediaValue,
                                    openTokApiKey = GlobalVariables.OpenTokApiKey
                                },
                                code = StatusCodes.Status200OK
                            });
                            
                        }
                        else
                        {
                            return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong, data = new { }, code = StatusCodes.Status200OK });
                        }
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = true, message = ResponseMessages.msgTimeSlotNotAvailableForCall, data = new { }, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = true, message = ResponseMessages.msgNotFound + "appointment", data = new { }, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong + ex.Message, data = new { }, code = StatusCodes.Status200OK });
            }
        }
    }
}
