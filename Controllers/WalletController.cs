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
    public class WalletController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IWalletService _walletService;
        public WalletController(UserManager<ApplicationUser> userManager, IWalletService walletService)
        {
            _userManager = userManager;
            _walletService = walletService;
        }

        #region AddUpdateAppointmentFee
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("AddUpdateAppointmentFee")]
        public async Task<IActionResult> AddUpdateAppointmentFee(UpdateAppointmentFeeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string CurrentUserId = CommonFunctions.getUserId(User);
                    if (string.IsNullOrEmpty(CurrentUserId))
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgTokenExpired, code = StatusCodes.Status200OK });
                    }
                    var user = await _userManager.FindByIdAsync(CurrentUserId);
                    if (user != null && !user.IsDeleted)
                    {
                        var objWalletInfo = new WalletBillingInfo()
                        {
                            UserId = user.Id,
                            Fee = model.Fee,
                            CreatedDate = DateTime.UtcNow
                        };
                        bool returnStatus = _walletService.AddUpdateAppointmentFee(objWalletInfo);
                        if (returnStatus)
                        {
                            return Ok(new ApiResponseModel { status = true, data = new { }, message = "Doctor Fee"+ ResponseMessages.msgUpdationSuccess, code = StatusCodes.Status200OK });
                        }
                        else
                        {
                            return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                        }
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgBlockOrInactiveUserNotPermitted, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgParametersNotCorrect, code = StatusCodes.Status200OK });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong + ex.Message, code = StatusCodes.Status500InternalServerError });
            }
        } 
        #endregion
    }
}
