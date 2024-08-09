using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IPaymentService _paymentService;
        private ApplicationDbContext _context;
        public PaymentController(UserManager<ApplicationUser> userManager, IPaymentService paymentService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _paymentService = paymentService;
            _context = context;
        }

        #region PayWithStripe
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [Route("PayWithStripe")]
        public async Task<IActionResult> PayWithStripe(PaymentViewModel model)
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
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (model.Amount > 0)
                {
                    var response = _paymentService.ProcessPayment(model, user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = response,
                        message = "Payment" + response.PaymentStatus,
                        code = StatusCodes.Status200OK,
                    });

                }
                else
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgInvalidAmount,
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
                    code = StatusCodes.Status500InternalServerError,
                });
            }
        }
        #endregion

        #region PayOutWithStripe
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("PayOutWithStripe")]
        public async Task<IActionResult> PayOutWithStripe()
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
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user != null)
                {
                    var response = _paymentService.ProcessPayout(user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = response,
                        message = "Payout done ",
                        code = StatusCodes.Status200OK,
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
                    code = StatusCodes.Status500InternalServerError,
                });
            }

        }
        #endregion

        #region GetPaymentHistory
        [HttpPost]
        [Authorize(Roles = "Patient")]
        [Route("GetPaymentHistory")]
        public async Task<IActionResult> GetPaymentHistory()
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

                    var paymentHistory = _context.Payment
                         .Where(i => i.UserId == CurrentUserId)
                         .ToList();
                    List<PaymentHistoryViewModel> paymentHistoryList = new List<PaymentHistoryViewModel>();
                    foreach (var p in paymentHistory)
                    {
                        var paymnt = new PaymentHistoryViewModel();
                        paymnt.PaymentAmount = p.PaymentAmount;
                        paymnt.PaymentDate = p.PaymentDate;

                        if ((p.ServiceStatus == (int)GlobalVariables.ServiceStatus.Confirmed))
                        {
                            paymnt.ServiceStatus = "Confirmed";
                        }
                        else if ((p.ServiceStatus == (int)GlobalVariables.ServiceStatus.Cancelled))
                        {
                            paymnt.ServiceStatus = "Cancelled";
                        }
                        else if ((p.ServiceStatus == (int)GlobalVariables.ServiceStatus.Pending))
                        {
                            paymnt.ServiceStatus = "Pending";
                        }
                        else if ((p.ServiceStatus == (int)GlobalVariables.ServiceStatus.Rejected))
                        {
                            paymnt.ServiceStatus = "Rejected";
                        }
                        else
                        {
                            paymnt.ServiceStatus = "Completed";
                        }

                        if ((p.ServiceType == (int)GlobalVariables.ServiceType.HomeService))
                        {
                            paymnt.ServiceType = "Home Service";
                        }
                        else if ((p.ServiceType == (int)GlobalVariables.ServiceType.LabService))
                        {
                            paymnt.ServiceType = "Lab Service";
                        }
                        else
                        {
                            paymnt.ServiceType = "TeleConsultancy";
                        }
                        paymentHistoryList.Add(paymnt);
                    }
                    if (paymentHistory.Count > 0)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = paymentHistoryList,
                            message = "Payment history" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = "Payment history is not available",
                            code = StatusCodes.Status200OK
                        });
                    }

                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
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
