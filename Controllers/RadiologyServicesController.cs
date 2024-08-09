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
    public class RadiologyServicesController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private ApplicationDbContext _context;

        public RadiologyServicesController(
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

        // #region GetRadiologyServiceDetail
        // [HttpGet]
        // //[Authorize(Roles = "Patient")]
        // [Route("GetRadiologyServiceDetail")]
        // public async Task<IActionResult> GetRadiologyServiceDetail()
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

        //         // Get Radiology service Detail
        //         var getRadiologyServiceDetail = _context.RadiologyServices
        //             .Include(i => i.RadiologyServicesFeature)
        //             .FirstOrDefault();

        //         var responseModel = new ResponseRadiologyServicesViewModel();
        //         if (getRadiologyServiceDetail != null)
        //         {

        //             responseModel.RadiologyServicesId = getRadiologyServiceDetail.RadiologyServicesId;
        //             responseModel.Description = getRadiologyServiceDetail.Description;
        //             responseModel.ImagePath = getRadiologyServiceDetail.ImagePath;
        //             responseModel.ServiceType = getRadiologyServiceDetail.ServiceType;
        //             responseModel.Status = getRadiologyServiceDetail.Status;
        //             responseModel.ContactName = getRadiologyServiceDetail.ContactName;
        //             responseModel.ContactDescription = getRadiologyServiceDetail.ContactDescription;
        //             responseModel.ContactImagePath = getRadiologyServiceDetail.ContactImagePath;
        //             responseModel.RadiologyServiceAddedDate = getRadiologyServiceDetail.RadiologyServiceAddedDate;
        //             responseModel.RadiologyServiceFeatures = new List<ResponseRadiologyServicesFeatureViewModel>();
        //             if (getRadiologyServiceDetail.RadiologyServicesFeature.Count > 0)
        //             {
        //                 foreach (var i in getRadiologyServiceDetail.RadiologyServicesFeature)
        //                 {
        //                     var feature = new ResponseRadiologyServicesFeatureViewModel();
        //                     feature.FeatureDescription = i.FeatureDescription;
        //                     responseModel.RadiologyServiceFeatures.Add(feature);
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

        // #region UpdateRadiologyService
        // [HttpPost]
        // [Authorize]
        // [Route("UpdateRadiologyService")]
        // public async Task<IActionResult> UpdateRadiologyService([FromBody] RequestUpdateRadiologyServicesViewModel model)
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

        //             var radiologyServices = _context.RadiologyServices.Find(model.RadiologyServicesId);
        //             if (radiologyServices != null)
        //             {   radiologyServices.RadiologyServicesId = model.RadiologyServicesId;
        //                 radiologyServices.Description = model.Description;
        //                 radiologyServices.ContactName=model.ContactName;
        //                 radiologyServices.ContactDescription = model.ContactDescription;
        //                 _context.Update(radiologyServices);
        //                 _context.SaveChanges();
        //             }
        //             else { }
        //             if (model.RadiologyServiceFeatures.Count > 0)
        //             {
        //                 foreach (var i in model.RadiologyServiceFeatures)
        //                 {
        //                     var feature = _context.RadiologyServicesFeature.Find(i.RadiologyServicesFeatureId);
        //                     if (feature != null)
        //                     {
        //                         feature.FeatureDescription = i.FeatureDescription;
        //                         _context.RadiologyServicesFeature.Update(feature);
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


        #region GetRadiologyServicesPlanList
        [HttpGet]
        //[Authorize]
        [Route("GetRadiologyServicesPlanList")]
        public IActionResult GetRadiologyServicesPlanList(string searchString)
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

                // Get Radiology service Detail
                var getRadiologyServicePlans = _context.RadiologyServicesPlans.ToList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    var radiologyServicePlans = getRadiologyServicePlans
                        .Where(
                            s =>
                                s.SpecialtyName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                || s.ServiceName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        )
                        .ToList();
                    List<ResponseRadiologyServicesPlanViewModel> responseModel =
                        new List<ResponseRadiologyServicesPlanViewModel>();
                    if (radiologyServicePlans.Count > 0)
                    {
                        foreach (var i in radiologyServicePlans)
                        {
                            var plan = new ResponseRadiologyServicesPlanViewModel();
                            plan.RadiologyServicesPlanId = i.RadiologyServicesPlanId;
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
                    List<ResponseRadiologyServicesPlanViewModel> responseModel =
                        new List<ResponseRadiologyServicesPlanViewModel>();
                    if (getRadiologyServicePlans.Count > 0)
                    {
                        foreach (var i in getRadiologyServicePlans)
                        {
                            var plan = new ResponseRadiologyServicesPlanViewModel();
                            plan.RadiologyServicesPlanId = i.RadiologyServicesPlanId;
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

        #region BookRadiologyService
        [HttpPost]
        [Authorize] //(Roles = "Patient")]
        [Route("BookRadiologyService")]
        public IActionResult BookRadiologyServices(
            RequestBookRadiologyServiceViewModel model
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

                var bookRadiologyService = new BookRadiologyService();

                bookRadiologyService.RadiologyServicePlanId = model.RadiologyServicePlanId;
                bookRadiologyService.BookingDate = model.BookingDate;
                bookRadiologyService.BookingTime = model.BookingTime;
                bookRadiologyService.Reason = model.Reason;
                bookRadiologyService.PatientId = CurrentUserId;
                bookRadiologyService.BookingStatus = 0;

                _context.BookRadiologyService.Add(bookRadiologyService);
                _context.SaveChanges();

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        // data = responseModel,
                        message = "Radiology service booked successfully",
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

        #region GetBookedRadiologyServicesList
        [HttpGet]
        [Authorize]
        [Route("GetBookedRadiologyServicesList")]
        public IActionResult GetBookedRadiologyServicesList()
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

                // Get Radiology service Detail
                var getBookedRadiologyServices = _context.BookRadiologyService.ToList();

                List<ResponseBookRadiologyServiceViewModel> responseModel =
                    new List<ResponseBookRadiologyServiceViewModel>();
                if (getBookedRadiologyServices.Count > 0)
                {
                    foreach (var i in getBookedRadiologyServices)
                    {
                        var bookedRadiologyService = new ResponseBookRadiologyServiceViewModel();
                        bookedRadiologyService.RadiologyServicePlanId = i.RadiologyServicePlanId;
                        bookedRadiologyService.BookingDate = i.BookingDate;
                        bookedRadiologyService.BookingTime = i.BookingTime;
                        bookedRadiologyService.Reason = i.Reason;
                        responseModel.Add(bookedRadiologyService);
                    }
                }

                return Ok(
                    new ApiResponseModel
                    {
                        status = true,
                        data = responseModel,
                        message = "Booked Radiology services" + ResponseMessages.msgShownSuccess,
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

        #region AddRadiologyServicePlan
        [HttpPost]
        [Authorize]
        [Route("AddRadiologyServicePlan")]
        public async Task<IActionResult> AddRadiologyServicePlan(
            [FromBody] RequestRadiologyServicesPlanViewModel model
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
                    var RadiologyServicesPlans = new RadiologyServicesPlans()
                    {
                        ServiceName = model.ServiceName,
                        SpecialtyName = model.SpecialtyName,
                        ServiceCode = model.ServiceCode,
                        Value = model.Value,
                    };
                    _context.Add(RadiologyServicesPlans);
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

        #region DeleteRadiologyServicePlan
        [HttpDelete]
        [Authorize]
        [Route("DeleteRadiologyServicePlan")]
        public async Task<IActionResult> DeleteRadiologyServicePlan(int radiologyServicePlanId)
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
                if (user == null && string.IsNullOrEmpty(radiologyServicePlanId.ToString()))
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
                    var DeletePackage = await _context.RadiologyServicesPlans
                        .Where(x => x.RadiologyServicesPlanId == radiologyServicePlanId)
                        .SingleOrDefaultAsync();

                    var result = _context.RadiologyServicesPlans.Remove(DeletePackage);
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

        #region UpdateRadiologyServicePlan
        [HttpPost]
        [Authorize]
        [Route("UpdateRadiologyServicePlan")]
        public async Task<IActionResult> UpdateRadiologyServicePlan(
            [FromBody] RequestUpdateRadiologyServicesPlanViewModel model
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
                    var RadiologyServicesPlans = _context.RadiologyServicesPlans.Find(
                        model.RadiologyServicesPlanId
                    );
                    if (RadiologyServicesPlans != null)
                    {
                        RadiologyServicesPlans.ServiceName = model.ServiceName;
                        RadiologyServicesPlans.SpecialtyName = model.SpecialtyName;
                        RadiologyServicesPlans.ServiceCode = model.ServiceCode;
                        RadiologyServicesPlans.Value = model.Value;
                        _context.Update(RadiologyServicesPlans);
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
