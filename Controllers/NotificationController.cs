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
    public class NotificationController : ControllerBase
    {
        private INotificationService _notificationService;
        private UserManager<ApplicationUser> _userManager;


        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;

        }

        [HttpGet]
        [Route("NotificationsList")]
        [Authorize]
        public IActionResult GetNotifications([FromQuery] FilterationListViewModel model)
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

                var notificationsList = _notificationService.GetNotificationListByUser(model, currentUserId);
                return Ok(new
                {
                    status = true,
                    message = "Notification List" + ResponseMessages.msgShownSuccess,
                    data = notificationsList,
                    code = StatusCodes.Status200OK
                });
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

        [HttpGet]
        [Route("NotificationsListForDoctor")]
        [Authorize(Roles = "Doctor")]
        public IActionResult GetNotificationsForDoctor([FromQuery] FilterationListViewModel model)
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

                var notificationsList = _notificationService.GetNotificationListForDoctor(model, currentUserId);
                return Ok(new
                {
                    status = true,
                    message = "Notification List" + ResponseMessages.msgShownSuccess,
                    data = notificationsList,
                    code = StatusCodes.Status200OK
                });
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



        [HttpPost]
        [Route("ReadNotification")]
        [Authorize]
        public IActionResult ReadNotification(string NotificationId)
        {
            try
            {
                if (string.IsNullOrEmpty(NotificationId))
                {
                    return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgParametersNotCorrect, data = new { }, code = StatusCodes.Status200OK });
                }
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgTokenExpired, data = new { }, code = StatusCodes.Status200OK });
                }
                bool result = _notificationService.MarkNotificationAsRead(NotificationId, currentUserId);
                if (result)
                {
                    return Ok(new ApiResponseModel { status = true, message = "Notification" + ResponseMessages.msgUpdationSuccess, data = new { } });
                }
                else
                    return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong, data = new { } });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, message = ResponseMessages.msgSomethingWentWrong + ex.Message, data = new { }, code = StatusCodes.Status500InternalServerError });
            }
        }

        [HttpDelete]
        [Route("DeleteNotification")]
        [Authorize]
        public IActionResult DeleteNotification(string NotificationId)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if(string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user =  _userManager.FindByIdAsync(currentUserId);
                if(user != null)
                {
                    bool result;
                    if(string.IsNullOrEmpty(NotificationId))
                    {
                        result = _notificationService.DeleteAllNotifications(currentUserId);
                    }
                    else
                    {
                        result = _notificationService.DeleteNotification(NotificationId);
                    }
                    
                    if(result)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Notification" + ResponseMessages.msgDeletionSuccess,
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

    }
}
