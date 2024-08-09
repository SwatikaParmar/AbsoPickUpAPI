using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.Data;
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
    public class DoctorLeaveController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private ApplicationDbContext _context;
        public DoctorLeaveController(ApplicationDbContext context, IDoctorService doctorService, UserManager<ApplicationUser> userManager, IPatientService patientService, IAuthService authService)
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _patientService = patientService;
            _authService = authService;
            _context = context;
        }

        #region RequestDoctorLeave
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("RequestDoctorLeave")]
        public async Task<IActionResult> RequestDoctorLeave(RequestDoctorLeaveViewModel model)
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
                    var addDoctorRequestLeave = new DoctorRequestLeave();
                    addDoctorRequestLeave.DoctorId = CurrentUserId;
                    addDoctorRequestLeave.LeaveReason = model.LeaveReason;
                   DateTime? fromDate = DateTime.ParseExact(model.FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //  DateTime? fromDate = Convert.ToDateTime(model.FromDate);
                    addDoctorRequestLeave.FromDate = fromDate;
                    if (model.ToDate == null || model.ToDate == "")
                    {
                        addDoctorRequestLeave.ToDate = fromDate;
                    }
                    else
                    {
                        DateTime? toDate = DateTime.ParseExact(model.ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        addDoctorRequestLeave.ToDate = toDate;
                    }
                    addDoctorRequestLeave.RequestedDate = DateTime.Now;
                    addDoctorRequestLeave.IsCancelledByUser = false;
                    addDoctorRequestLeave.Status = "Pending";

                    _context.DoctorRequestLeave.Add(addDoctorRequestLeave);
                    await _context.SaveChangesAsync();

                    var responseModel = new ReponseDoctorLeaveViewModel();
                    responseModel.LeaveReason = addDoctorRequestLeave.LeaveReason;
                    responseModel.FromDate = addDoctorRequestLeave.FromDate;
                    responseModel.ToDate = addDoctorRequestLeave.ToDate;
                    responseModel.RequestedDate = addDoctorRequestLeave.RequestedDate;
                    responseModel.DoctorRequestLeaveId = addDoctorRequestLeave.DoctorRequestLeaveId;
                    responseModel.Status = addDoctorRequestLeave.Status;

                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = responseModel,
                        message = "Doctor leave request" + ResponseMessages.msgAdditionSuccess,
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

        #region GetDoctorLeaveRequestList
        [HttpGet]
        [Authorize]
        [Route("GetDoctorLeaveRequestList")]
        public async Task<IActionResult> GetDoctorLeaveRequestList()
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
                    var doctorLeaveRequestList = _context.DoctorRequestLeave
                        .Where(i => i.DoctorId == CurrentUserId)
                        .ToList();

                    List<ReponseDoctorLeaveListViewModel> doctorLeaveList = new List<ReponseDoctorLeaveListViewModel>();
                    foreach (var a in doctorLeaveRequestList)
                    {
                        ReponseDoctorLeaveListViewModel response = new ReponseDoctorLeaveListViewModel();
                        response.LeaveReason = a.LeaveReason;
                        response.DoctorRequestLeaveId = a.DoctorRequestLeaveId;
                        response.FromDate = a.FromDate;
                        response.ToDate = a.ToDate;
                        response.Status = a.Status;
                        response.AdminResponse = a.AdminResponse;
                        response.RequestedDate = a.RequestedDate;
                        response.IsCancelledByUser = a.IsCancelledByUser;

                        doctorLeaveList.Add(response);
                    }

                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = doctorLeaveList,
                        message = "Doctor Leave Request List" + ResponseMessages.msgShownSuccess,
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

        #region CancelDoctorLeaveRequest
        [HttpPost]
        [Authorize]
        [Route("CancelDoctorLeaveRequest")]
        public async Task<IActionResult> CancelDoctorLeaveRequest(int doctorRequestLeaveId)
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
                    var getDoctorLeaveRequest = _context.DoctorRequestLeave
                        .Where(i => i.DoctorRequestLeaveId == doctorRequestLeaveId)
                        .FirstOrDefault();

                    // getDoctorLeaveRequest
                    getDoctorLeaveRequest.IsCancelledByUser = true;

                    _context.DoctorRequestLeave.Update(getDoctorLeaveRequest);
                    _context.SaveChanges();

                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        message = "Doctor Leave Request cancelled successfully",
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
