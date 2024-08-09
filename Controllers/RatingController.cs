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
    public class RatingController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;
        private readonly IRatingService _ratingService;
        public RatingController(UserManager<ApplicationUser> userManager, IAppointmentService appointmentService, IRatingService ratingService)
        {
            _userManager = userManager;
            _appointmentService = appointmentService;
            _ratingService = ratingService;
        }
        #region "AddUpdateUserRating"
        [HttpPost]
        [Route("AddUpdateUserRating")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> AddUpdateUserRating(UserRatingViewModel model)
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
                var user = await _userManager.FindByIdAsync(currentUserId);
                if(user != null)
                {
                    var _appointmentDetails = _appointmentService.GetAppointmentById(model.AppointmentId);
                    if(_appointmentDetails != null)
                    {
                         UserRating _userRating = new UserRating();
                        _userRating.AppointmentId = model.AppointmentId;
                        _userRating.PatientId = _appointmentDetails.PatientId;
                        _userRating.DoctorId = _appointmentDetails.DoctorId;
                        _userRating.Rating = model.Rating;
                        _userRating.Comments = model.Comments;
                        _userRating.CreatedDate = DateTime.UtcNow;
                        bool result = _ratingService.AddUpdateUserRating(_userRating);
                        if(result)
                        {
                            return Ok(new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "User ratings" + ResponseMessages.msgAdditionSuccess,
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
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ResponseMessages.msgNoAppointment, code = StatusCodes.Status200OK });
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

        #region "GetUserRating"
        [HttpGet]
        [Route("GetUserRating")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetUserRating(string appointmentId)
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
                    var rating = _ratingService.GetUserRating(appointmentId);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = rating,
                        message = "User Ratings" + ResponseMessages.msgShownSuccess,
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

        #region "GetUserReviews"
        [HttpGet]
        [Route("GetUserReviews")]
        [Authorize]
        public async Task<IActionResult> GetUserReviews(string DoctorId)
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
                var user = await _userManager.FindByIdAsync(currentUserId);
                if(user != null)
                {
                    var _reviewsList = _ratingService.GetUserReviews(DoctorId);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = _reviewsList,
                        message = "User Reviews" + ResponseMessages.msgShownSuccess,
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
