using System;
using System.Collections;
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
    public class ScheduleController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IScheduleService _scheduleService;

        public ScheduleController(UserManager<ApplicationUser> userManager, IScheduleService scheduleService)
        {
            _userManager = userManager;
            _scheduleService = scheduleService;
        }

        #region "Doctor API"

        #region "Insert Doctor Schedule"
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("InsertDoctorSchedule")]
        public async Task<IActionResult> InsertDoctorSchedule(ScheduleViewModel model)
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
                if (user != null)
                {
                    var _schedule = new DocScheduleVM()
                    {
                        UserId = user.Id,
                        IsAvailable = true,
                        schedule = model.schedule

                    };
                    int result = _scheduleService.InsertDoctorSchedule(_schedule);
                    // return 1 -> schedule created successfully
                    if (result > 0)
                    {
                        return Ok(new ApiResponseModel { status = true, data = new { }, message = "Doctor schedule" + ResponseMessages.msgAdditionSuccess, code = StatusCodes.Status200OK });
                    }
                    else if (result == -1)  // return -1 -> if invalid date & time
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgInvalidTime, code = StatusCodes.Status200OK });
                    }
                    else if (result == -2)  // return -2 -> if schedule not added
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + "schedule not added.", code = StatusCodes.Status200OK });
                    }
                    else  // return 0 -> schedule already exists
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgShiftAlreadyExists, code = StatusCodes.Status200OK });
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

        #region GetAvailableDates
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetAvailableDates")]
        public async Task<IActionResult> GetAvailableDates(string Date)
        {
            try
            {
                string CurrentUserId = CommonFunctions.getUserId(User);
                if(string.IsNullOrEmpty(CurrentUserId))
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
                if(user != null)
                {
                    var _dateList = _scheduleService.GetAvailableDates(user.Id, Date);
                    ArrayList arrList = new ArrayList();
                    if (_dateList != null && _dateList.Count > 0)
                    { 
                        foreach (var item in _dateList)
                        {
                            arrList.Add(item);
                        }
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = "Available Dates" + ResponseMessages.msgShownSuccess,
                            data = arrList,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = ResponseMessages.msgNoavailabledates,
                            data = arrList,
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

        #region GetDoctorTimeSlotByDate
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetDoctorTimeSlotByDate")]
        public async Task<IActionResult> GetDoctorTimeSlotByDate(string selectedDate)
        {
            try
            {
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
                if(user != null)
                {
                    var _list = _scheduleService.GetDoctorTimeSlotByDate(user.Id, selectedDate);
                    if(_list != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = _list,
                            message = "Time slots"+ ResponseMessages.msgShownSuccess,
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
                            code = StatusCodes.Status500InternalServerError
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


        #region GetDoctorDateWiseTimeShift
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetDoctorDateWiseTimeShift")]
        public async Task<IActionResult> GetDoctorDateWiseTimeShift(string Date)
        {
            try
            {
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
                if (user != null)
                {
                    var _orderedtimeShift = _scheduleService.GetAvailableTimeShift(user.Id, Date);
                    
                    if (_orderedtimeShift != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = "Available Time Shifts" + ResponseMessages.msgShownSuccess,
                            data = _orderedtimeShift,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = ResponseMessages.msgNoShiftAvailabe,
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

        #region DeleteDoctorSchedule
        [HttpDelete]
        [Authorize(Roles = "Doctor")]
        [Route("DeleteDoctorSchedule")]
        public async Task<IActionResult> DeleteDoctorSchedule(int Id)
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
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgTokenExpired,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    bool result = _scheduleService.DeleteSchedule(Id, user.Id);
                    if (result == true)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Doctor Schedule" + ResponseMessages.msgDeletionSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgInvalidScheduleForDeletion, code = StatusCodes.Status200OK });
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

        #region EditDoctorSchedule
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("EditDoctorSchedule")]
        public async Task<IActionResult> EditDoctorSchedule(UpdateScheduleViewModel model)
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
                    bool result = _scheduleService.UpdateDoctorSchedule(model, user.Id);
                    if (result == true)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Doctor Schedule" + ResponseMessages.msgUpdationSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgInvalidScheduleForEdit,
                            code = StatusCodes.Status200OK
                        });
                    }
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

        #endregion

        #region "Patient APIs" 
         
        #region GetAvailableDatesForPatient
        [HttpGet]
        [Authorize(Roles = "Patient")]
        [Route("GetAvailableDatesForPatient")]
        public async Task<IActionResult> GetAvailableDatesForPatient(string Date, string DoctorId)
        {
            try
            {
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
                if (user != null)
                {
                    var _dateList = _scheduleService.GetAvailableDates(DoctorId, Date);
                    if (_dateList != null)
                    {
                        ArrayList arrList = new ArrayList();
                        foreach (var item in _dateList)
                        {
                            arrList.Add(item);
                        }
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            message = "Available dates for Patient" + ResponseMessages.msgShownSuccess,
                            data = arrList,
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


        #region GetTimeSlotsForPatient
        [HttpGet]
        [Authorize(Roles = "Patient")]
        [Route("GetTimeSlotsForPatient")]
        public async Task<IActionResult> GetTimeSlotsForPatient(string DoctorId, string selectedDate)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgTokenExpired,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var _availableTimeSlots = _scheduleService.GetDoctorTimeSlotByDate(DoctorId, selectedDate);
                   // _availableTimeSlots = _availableTimeSlots.Where(x => x.IsSlotAvailable == true).ToList();
                    if (_availableTimeSlots != null)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = _availableTimeSlots,
                            message = "Available Time slots" + ResponseMessages.msgShownSuccess,
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
                            code = StatusCodes.Status500InternalServerError
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
        #endregion

    }
}
