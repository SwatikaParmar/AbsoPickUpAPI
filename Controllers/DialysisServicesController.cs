using System;
using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DialysisServicesController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private ApplicationDbContext _context;

        public DialysisServicesController(
            ApplicationDbContext context,
            IDoctorService doctorService,
            UserManager<ApplicationUser> userManager,
            IPatientService patientService,
            IAuthService authService
        )
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _patientService = patientService;
            _authService = authService;
            _context = context;
        }

        // #region GetDialysisServiceDetail
        // [HttpGet]
        // //[Authorize(Roles = "Patient")]
        // [Route("GetDialysisServiceDetail")]
        // public async Task<IActionResult> GetDialysisServiceDetail()
        // {
        //     try
        //     {
        //         string CurrentUserId = CommonFunctions.getUserId(User);
        //         if (string.IsNullOrEmpty(CurrentUserId))
        //         {
        //             return Ok(new ApiResponseModel
        //             {
        //                 status = false,
        //                 data = new { },
        //                 message = ResponseMessages.msgTokenExpired,
        //                 code = StatusCodes.Status200OK
        //             });
        //         }

        //         // Get Dialysis service Detail
        //         var getDialysisServiceDetail = _context.DialysisServices
        //             .Include(i => i.DialysisServicesFeature)
        //             .FirstOrDefault();

        //         var responseModel = new ResponseDialysisServicesViewModel();
        //         if (getDialysisServiceDetail != null)
        //         {

        //             responseModel.DialysisServicesId = getDialysisServiceDetail.DialysisServicesId;
        //             responseModel.Description = getDialysisServiceDetail.Description;
        //             responseModel.ImagePath = getDialysisServiceDetail.ImagePath;
        //             responseModel.ServiceType = getDialysisServiceDetail.ServiceType;
        //             responseModel.Status = getDialysisServiceDetail.Status;
        //             responseModel.ContactName = getDialysisServiceDetail.ContactName;
        //             responseModel.ContactDescription = getDialysisServiceDetail.ContactDescription;
        //             responseModel.ContactImagePath = getDialysisServiceDetail.ContactImagePath;
        //             responseModel.DialysisServiceAddedDate = getDialysisServiceDetail.DialysisServiceAddedDate;
        //             responseModel.DialysisServiceFeatures = new List<ResponseDialysisServicesFeatureViewModel>();
        //             if (getDialysisServiceDetail.DialysisServicesFeature.Count > 0)
        //             {
        //                 foreach (var i in getDialysisServiceDetail.DialysisServicesFeature)
        //                 {
        //                     var feature = new ResponseDialysisServicesFeatureViewModel();
        //                     feature.FeatureDescription = i.FeatureDescription;
        //                     responseModel.DialysisServiceFeatures.Add(feature);
        //                 }
        //             }
        //         }

        //         return Ok(new ApiResponseModel
        //         {
        //             status = true,
        //             data = responseModel,
        //             message = "" + ResponseMessages.msgShownSuccess,
        //             code = StatusCodes.Status200OK
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(new ApiResponseModel
        //         {
        //             status = false,
        //             data = new { },
        //             message = ResponseMessages.msgSomethingWentWrong + ex.Message,
        //             code = StatusCodes.Status500InternalServerError
        //         });
        //     }
        // }
        // #endregion

        // #region UpdateDialysisService
        // [HttpPost]
        // [Authorize]
        // [Route("UpdateDialysisService")]
        // public async Task<IActionResult> UpdateDialysisService([FromBody] RequestUpdateDialysisServicesViewModel model)
        // {
        //     try
        //     {
        //         if (!ModelState.IsValid)
        //         {
        //             return Ok(new ApiResponseModel
        //             {
        //                 status = false,
        //                 data = new { },
        //                 message = ResponseMessages.msgParametersNotCorrect,
        //                 code = StatusCodes.Status200OK
        //             });
        //         }

        //         string CurrentUserId = CommonFunctions.getUserId(User);
        //         if (string.IsNullOrEmpty(CurrentUserId))
        //         {
        //             return Ok(new ApiResponseModel
        //             {
        //                 status = false,
        //                 data = new { },
        //                 message = ResponseMessages.msgTokenExpired,
        //                 code = StatusCodes.Status200OK
        //             });
        //         }
        //         var user = await _userManager.FindByIdAsync(CurrentUserId);
        //         if (user != null)
        //         {

        //             var DialysisServices = _context.DialysisServices.Find(model.DialysisServicesId);
        //             if (DialysisServices != null)
        //             {   DialysisServices.DialysisServicesId = model.DialysisServicesId;
        //                 DialysisServices.Description = model.Description;
        //                 DialysisServices.ContactName=model.ContactName;
        //                 DialysisServices.ContactDescription = model.ContactDescription;
        //                 _context.Update(DialysisServices);
        //                 _context.SaveChanges();
        //             }
        //             else { }
        //             if (model.DialysisServiceFeatures.Count > 0)
        //             {
        //                 foreach (var i in model.DialysisServiceFeatures)
        //                 {
        //                     var feature = _context.DialysisServicesFeature.Find(i.DialysisServicesFeatureId);
        //                     if (feature != null)
        //                     {
        //                         feature.FeatureDescription = i.FeatureDescription;
        //                         _context.DialysisServicesFeature.Update(feature);
        //                         _context.SaveChanges();
        //                     }
        //                     else { }
        //                 }
        //             }

        //             return Ok(new ApiResponseModel
        //             {
        //                 status = true,
        //                 data = new { },
        //                 message = "Service" + ResponseMessages.msgUpdationSuccess,
        //                 code = StatusCodes.Status200OK
        //             });

        //         }
        //         else
        //         {
        //             return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(new ApiResponseModel
        //         {
        //             status = false,
        //             data = new { },
        //             message = ResponseMessages.msgSomethingWentWrong + ex.Message,
        //             code = StatusCodes.Status500InternalServerError
        //         });
        //     }
        // }
        // #endregion


        #region GetDialysisServicesPlanList
        [HttpGet]
        //[Authorize]
        [Route("GetDialysisServicesPlanList")]
        public IActionResult GetDialysisServicesPlanList(string searchString)
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
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                // Get Dialysis service Detail
                var getDialysisServicePlans = _context.DialysisServicesPlans.ToList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    var DialysisServicePlans = getDialysisServicePlans
                        .Where(
                            s =>
                                s.SpecialtyName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || s.ServiceName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                    List<ResponseDialysisServicesPlanViewModel> responseModel =
                        new List<ResponseDialysisServicesPlanViewModel>();
                    if (DialysisServicePlans.Count > 0)
                    {
                        foreach (var i in DialysisServicePlans)
                        {
                            var plan = new ResponseDialysisServicesPlanViewModel();
                            plan.DialysisServicesPlanId = i.DialysisServicesPlanId;
                            plan.Value = i.Value;
                            plan.SpecialtyName = i.SpecialtyName;
                            plan.ServiceName = i.ServiceName;
                            plan.ServiceCode = i.ServiceCode;

                            responseModel.Add(plan);
                        }
                    }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Plan list " + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    List<ResponseDialysisServicesPlanViewModel> responseModel =
                        new List<ResponseDialysisServicesPlanViewModel>();
                    if (getDialysisServicePlans.Count > 0)
                    {
                        foreach (var i in getDialysisServicePlans)
                        {
                            var plan = new ResponseDialysisServicesPlanViewModel();
                            plan.DialysisServicesPlanId = i.DialysisServicesPlanId;
                            plan.Value = i.Value;
                            plan.SpecialtyName = i.SpecialtyName;
                            plan.ServiceName = i.ServiceName;
                            plan.ServiceCode = i.ServiceCode;

                            responseModel.Add(plan);
                        }
                    }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Plan list " + ResponseMessages.msgShownSuccess,
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

        #region BookDialysisService
        [HttpPost]
        [Authorize] //(Roles = "Patient")]
        [Route("BookDialysisService")]
        public IActionResult BookDialysisServices(
            RequestBookDialysisServiceViewModel model
        )
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
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                var bookDialysisService = new BookDialysisService();

                bookDialysisService.DialysisServicePlanId = model.DialysisServicePlanId;
                bookDialysisService.BookingDate = model.BookingDate;
                bookDialysisService.BookingTime = model.BookingTime;
                bookDialysisService.Reason = model.Reason;
                bookDialysisService.PatientId = CurrentUserId;
                bookDialysisService.BookingStatus = 0;

                _context.BookDialysisService.Add(bookDialysisService);
                _context.SaveChanges();

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        // data = responseModel,
                        message = "Dialysis service booked successfully",
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

        #region GetBookedDialysisServicesList
        [HttpGet]
        [Authorize]
        [Route("GetBookedDialysisServicesList")]
        public IActionResult GetBookedDialysisServicesList()
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
                            code = StatusCodes.Status200OK
                        }
                    );
                }

                // Get Dialysis service Detail
                var getBookedDialysisServices = _context.BookDialysisService.ToList();

                List<ResponseBookDialysisServiceViewModel> responseModel =
                    new List<ResponseBookDialysisServiceViewModel>();
                if (getBookedDialysisServices.Count > 0)
                {
                    foreach (var i in getBookedDialysisServices)
                    {
                        var bookedDialysisService = new ResponseBookDialysisServiceViewModel();
                        bookedDialysisService.DialysisServicePlanId = i.DialysisServicePlanId;
                        bookedDialysisService.BookingDate = i.BookingDate;
                        bookedDialysisService.BookingTime = i.BookingTime;
                        bookedDialysisService.Reason = i.Reason;
                        responseModel.Add(bookedDialysisService);
                    }
                }

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        data = responseModel,
                        message = "Booked Dialysis services" + ResponseMessages.msgShownSuccess,
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

        #region AddDialysisServicePlan
        [HttpPost]
        [Authorize]
        [Route("AddDialysisServicePlan")]
        public async Task<IActionResult> AddDialysisServicePlan(
            [FromBody] RequestDialysisServicesPlanViewModel model
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
                if (user != null)
                {
                    var DialysisServicesPlans = new DialysisServicesPlans()
                    {
                        ServiceName = model.ServiceName,
                        SpecialtyName = model.SpecialtyName,
                        ServiceCode = model.ServiceCode,
                        Value = model.Value,
                    };
                    _context.Add(DialysisServicesPlans);
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Service plan" + ResponseMessages.msgAdditionSuccess,
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

        #region DeleteDialysisServicePlan
        [HttpDelete]
        [Authorize]
        [Route("DeleteDialysisServicePlan")]
        public async Task<IActionResult> DeleteDialysisServicePlan(int DialysisServicePlanId)
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
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(CurrentUserId);
                if (user == null && string.IsNullOrEmpty(DialysisServicePlanId.ToString()))
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
                else
                {
                    var DeletePackage = await _context.DialysisServicesPlans
                        .Where(x => x.DialysisServicesPlanId == DialysisServicePlanId)
                        .SingleOrDefaultAsync();

                    var result = _context.DialysisServicesPlans.Remove(DeletePackage);
                    await _context.SaveChangesAsync();
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Service Plan" + ResponseMessages.msgDeletionSuccess,
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
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong + ex.Message,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region UpdateDialysisServicePlan
        [HttpPost]
        [Authorize]
        [Route("UpdateDialysisServicePlan")]
        public async Task<IActionResult> UpdateDialysisServicePlan(
            [FromBody] RequestUpdateDialysisServicesPlanViewModel model
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
                if (user != null)
                {
                    var DialysisServicesPlans = _context.DialysisServicesPlans.Find(
                        model.DialysisServicesPlanId
                    );
                    if (DialysisServicesPlans != null)
                    {
                        DialysisServicesPlans.ServiceName = model.ServiceName;
                        DialysisServicesPlans.SpecialtyName = model.SpecialtyName;
                        DialysisServicesPlans.ServiceCode = model.ServiceCode;
                        DialysisServicesPlans.Value = model.Value;
                        _context.Update(DialysisServicesPlans);
                        _context.SaveChanges();
                    }
                    else { }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "Service Plan" + ResponseMessages.msgUpdationSuccess,
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
    }
}
