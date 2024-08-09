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
    public class RehabilationServicesController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private ApplicationDbContext _context;

        public RehabilationServicesController(
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

        // #region GetRehabilationServiceDetail
        // [HttpGet]
        // //[Authorize(Roles = "Patient")]
        // [Route("GetRehabilationServiceDetail")]
        // public async Task<IActionResult> GetRehabilationServiceDetail()
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

        //         // Get Rehabilation service Detail
        //         var getRehabilationServiceDetail = _context.RehabilationServices
        //             .Include(i => i.RehabilationServicesFeature)
        //             .FirstOrDefault();

        //         var responseModel = new ResponseRehabilationServicesViewModel();
        //         if (getRehabilationServiceDetail != null)
        //         {

        //             responseModel.RehabilationServicesId = getRehabilationServiceDetail.RehabilationServicesId;
        //             responseModel.Description = getRehabilationServiceDetail.Description;
        //             responseModel.ImagePath = getRehabilationServiceDetail.ImagePath;
        //             responseModel.ServiceType = getRehabilationServiceDetail.ServiceType;
        //             responseModel.Status = getRehabilationServiceDetail.Status;
        //             responseModel.ContactName = getRehabilationServiceDetail.ContactName;
        //             responseModel.ContactDescription = getRehabilationServiceDetail.ContactDescription;
        //             responseModel.ContactImagePath = getRehabilationServiceDetail.ContactImagePath;
        //             responseModel.RehabilationServiceAddedDate = getRehabilationServiceDetail.RehabilationServiceAddedDate;
        //             responseModel.RehabilationServiceFeatures = new List<ResponseRehabilationServicesFeatureViewModel>();
        //             if (getRehabilationServiceDetail.RehabilationServicesFeature.Count > 0)
        //             {
        //                 foreach (var i in getRehabilationServiceDetail.RehabilationServicesFeature)
        //                 {
        //                     var feature = new ResponseRehabilationServicesFeatureViewModel();
        //                     feature.FeatureDescription = i.FeatureDescription;
        //                     responseModel.RehabilationServiceFeatures.Add(feature);
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

        // #region UpdateRehabilationService
        // [HttpPost]
        // [Authorize]
        // [Route("UpdateRehabilationService")]
        // public async Task<IActionResult> UpdateRehabilationService([FromBody] RequestUpdateRehabilationServicesViewModel model)
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

        //             var RehabilationServices = _context.RehabilationServices.Find(model.RehabilationServicesId);
        //             if (RehabilationServices != null)
        //             {   RehabilationServices.RehabilationServicesId = model.RehabilationServicesId;
        //                 RehabilationServices.Description = model.Description;
        //                 RehabilationServices.ContactName=model.ContactName;
        //                 RehabilationServices.ContactDescription = model.ContactDescription;
        //                 _context.Update(RehabilationServices);
        //                 _context.SaveChanges();
        //             }
        //             else { }
        //             if (model.RehabilationServiceFeatures.Count > 0)
        //             {
        //                 foreach (var i in model.RehabilationServiceFeatures)
        //                 {
        //                     var feature = _context.RehabilationServicesFeature.Find(i.RehabilationServicesFeatureId);
        //                     if (feature != null)
        //                     {
        //                         feature.FeatureDescription = i.FeatureDescription;
        //                         _context.RehabilationServicesFeature.Update(feature);
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


        #region GetRehabilationServicesPlanList
        [HttpGet]
        //[Authorize]
        [Route("GetRehabilationServicesPlanList")]
        public IActionResult GetRehabilationServicesPlanList(string searchString)
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

                // Get Rehabilation service Detail
                var getRehabilationServicePlans = _context.RehabilationServicesPlans.ToList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    var RehabilationServicePlans = getRehabilationServicePlans
                        .Where(
                            s =>
                                s.SpecialtyName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || s.ServiceName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                    List<ResponseRehabilationServicesPlanViewModel> responseModel =
                        new List<ResponseRehabilationServicesPlanViewModel>();
                    if (RehabilationServicePlans.Count > 0)
                    {
                        foreach (var i in RehabilationServicePlans)
                        {
                            var plan = new ResponseRehabilationServicesPlanViewModel();
                            plan.RehabilationServicesPlanId = i.RehabilationServicesPlanId;
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
                    List<ResponseRehabilationServicesPlanViewModel> responseModel =
                        new List<ResponseRehabilationServicesPlanViewModel>();
                    if (getRehabilationServicePlans.Count > 0)
                    {
                        foreach (var i in getRehabilationServicePlans)
                        {
                            var plan = new ResponseRehabilationServicesPlanViewModel();
                            plan.RehabilationServicesPlanId = i.RehabilationServicesPlanId;
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

        #region BookRehabilationService
        [HttpPost]
        [Authorize] //(Roles = "Patient")]
        [Route("BookRehabilationService")]
        public IActionResult BookRehabilationServices(
            RequestBookRehabilationServiceViewModel model
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

                var bookRehabilationService = new BookRehabilationService();

                bookRehabilationService.RehabilationServicePlanId = model.RehabilationServicePlanId;
                bookRehabilationService.BookingDate = model.BookingDate;
                bookRehabilationService.BookingTime = model.BookingTime;
                bookRehabilationService.Reason = model.Reason;
                bookRehabilationService.PatientId = CurrentUserId;
                bookRehabilationService.BookingStatus = 0;

                _context.BookRehabilationService.Add(bookRehabilationService);
                _context.SaveChanges();

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        // data = responseModel,
                        message = "Rehabilation service booked successfully",
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

        #region GetBookedRehabilationServicesList
        [HttpGet]
        [Authorize]
        [Route("GetBookedRehabilationServicesList")]
        public IActionResult GetBookedRehabilationServicesList()
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

                // Get Rehabilation service Detail
                var getBookedRehabilationServices = _context.BookRehabilationService.ToList();

                List<ResponseBookRehabilationServiceViewModel> responseModel =
                    new List<ResponseBookRehabilationServiceViewModel>();
                if (getBookedRehabilationServices.Count > 0)
                {
                    foreach (var i in getBookedRehabilationServices)
                    {
                        var bookedRehabilationService = new ResponseBookRehabilationServiceViewModel();
                        bookedRehabilationService.RehabilationServicePlanId = i.RehabilationServicePlanId;
                        bookedRehabilationService.BookingDate = i.BookingDate;
                        bookedRehabilationService.BookingTime = i.BookingTime;
                        bookedRehabilationService.Reason = i.Reason;
                        responseModel.Add(bookedRehabilationService);
                    }
                }

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        data = responseModel,
                        message = "Booked Rehabilation services" + ResponseMessages.msgShownSuccess,
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

        #region AddRehabilationServicePlan
        [HttpPost]
        [Authorize]
        [Route("AddRehabilationServicePlan")]
        public async Task<IActionResult> AddRehabilationServicePlan(
            [FromBody] RequestRehabilationServicesPlanViewModel model
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
                    var RehabilationServicesPlans = new RehabilationServicesPlans()
                    {
                        ServiceName = model.ServiceName,
                        SpecialtyName = model.SpecialtyName,
                        ServiceCode = model.ServiceCode,
                        Value = model.Value,
                    };
                    _context.Add(RehabilationServicesPlans);
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

        #region DeleteRehabilationServicePlan
        [HttpDelete]
        [Authorize]
        [Route("DeleteRehabilationServicePlan")]
        public async Task<IActionResult> DeleteRehabilationServicePlan(int RehabilationServicePlanId)
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
                if (user == null && string.IsNullOrEmpty(RehabilationServicePlanId.ToString()))
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
                    var DeletePackage = await _context.RehabilationServicesPlans
                        .Where(x => x.RehabilationServicesPlanId == RehabilationServicePlanId)
                        .SingleOrDefaultAsync();

                    var result = _context.RehabilationServicesPlans.Remove(DeletePackage);
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

        #region UpdateRehabilationServicePlan
        [HttpPost]
        [Authorize]
        [Route("UpdateRehabilationServicePlan")]
        public async Task<IActionResult> UpdateRehabilationServicePlan(
            [FromBody] RequestUpdateRehabilationServicesPlanViewModel model
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
                    var RehabilationServicesPlans = _context.RehabilationServicesPlans.Find(
                        model.RehabilationServicesPlanId
                    );
                    if (RehabilationServicesPlans != null)
                    {
                        RehabilationServicesPlans.ServiceName = model.ServiceName;
                        RehabilationServicesPlans.SpecialtyName = model.SpecialtyName;
                        RehabilationServicesPlans.ServiceCode = model.ServiceCode;
                        RehabilationServicesPlans.Value = model.Value;
                        _context.Update(RehabilationServicesPlans);
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
