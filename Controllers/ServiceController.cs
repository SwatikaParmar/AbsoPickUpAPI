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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private UserManager<ApplicationUser> _userManager;
        private IAuthService _authService;
        private ApplicationDbContext _context;
        private IWebHostEnvironment _hostingEnvironment;
        private IUploadFiles _uploadFiles;

        public ServiceController(
            ApplicationDbContext context,
            IWebHostEnvironment hostingEnvironment,
            IDoctorService doctorService,
            UserManager<ApplicationUser> userManager,
            IPatientService patientService,
            IAuthService authService,
            IUploadFiles uploadFiles
        )
        {
            _doctorService = doctorService;
            _userManager = userManager;
            _patientService = patientService;
            _authService = authService;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uploadFiles = uploadFiles;
        }

        #region GetServiceDetail
        [HttpGet]
        [Authorize]
        [Route("GetServiceDetail")]
        public IActionResult GetServiceDetail(int serviceTypeValue)
        {
            try
            {
                // Service detail api for home service and lab service
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

                if (
                    serviceTypeValue
                    == Convert.ToInt32(GlobalVariables.AnfasServiceType.HomeService)
                )
                {
                    // Get Home service Detail
                    var getHomeServiceDetail = _context.HomeServices
                        .Include(i => i.HomeServicesFeature)
                        .FirstOrDefault();

                    var responseModel = new ResponseServicesViewModel();
                    if (getHomeServiceDetail != null)
                    {
                        responseModel.ServicesId = getHomeServiceDetail.HomeServicesId;
                        responseModel.Description = getHomeServiceDetail.Description;
                        responseModel.ImagePath = getHomeServiceDetail.ImagePath;
                        responseModel.ContactName = getHomeServiceDetail.ContactName;
                        responseModel.ContactDescription = getHomeServiceDetail.ContactDescription;
                        responseModel.ContactImagePath = getHomeServiceDetail.ContactImagePath;
                        responseModel.ServiceAddedDate = getHomeServiceDetail.HomeServiceAddedDate;
                        List<ResponseServicesFeatureViewModel> res =
                            new List<ResponseServicesFeatureViewModel>();
                        if (getHomeServiceDetail.HomeServicesFeature.Count > 0)
                        {
                            foreach (var i in getHomeServiceDetail.HomeServicesFeature)
                            {
                                var feature = new ResponseServicesFeatureViewModel();
                                feature.FeatureDescription = i.FeatureDescription;
                                feature.ServicesFeatureId = i.HomeServicesFeatureId;
                                res.Add(feature);
                            }
                        }
                        if (res.Count > 0)
                        {
                            responseModel.ServiceFeatures = res;
                        }
                    }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Service Detail" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                //if(serviceType == Convert.ToInt32(GlobalVariables.AnfasServiceType.HomeService))
                else
                {
                    // Get Lab Service Detail
                    var getLabServiceDetail = _context.LabServices
                        .Include(i => i.LabServicesFeature)
                        .FirstOrDefault();

                    var responseModel = new ResponseServicesViewModel();
                    if (getLabServiceDetail != null)
                    {
                        responseModel.ServicesId = getLabServiceDetail.LabServicesId;
                        responseModel.Description = getLabServiceDetail.Description;
                        responseModel.ImagePath = getLabServiceDetail.ImagePath;
                        responseModel.ContactName = getLabServiceDetail.ContactName;
                        responseModel.ContactDescription = getLabServiceDetail.ContactDescription;
                        responseModel.ContactImagePath = getLabServiceDetail.ContactImagePath;
                        responseModel.ServiceAddedDate = getLabServiceDetail.LabServiceAddedDate;
                        responseModel.ServiceFeatures =
                            new List<ResponseServicesFeatureViewModel>();
                        if (getLabServiceDetail.LabServicesFeature.Count > 0)
                        {
                            foreach (var i in getLabServiceDetail.LabServicesFeature)
                            {
                                var feature = new ResponseServicesFeatureViewModel();
                                feature.FeatureDescription = i.FeatureDescription;
                                feature.ServicesFeatureId = i.LabServicesFeatureId;
                                responseModel.ServiceFeatures.Add(feature);
                            }
                        }
                    }

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Service Detail" + ResponseMessages.msgShownSuccess,
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

        #region UpdateService
        [HttpPost]
        [Authorize]
        [Route("UpdateService")]
        public async Task<IActionResult> UpdateService(
            [FromBody] RequestUpdateServicesViewModel model
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
                    if (
                        model.ServiceTypeValue
                        == Convert.ToInt32(GlobalVariables.AnfasServiceType.HomeService)
                    )
                    {
                        var homeServices = _context.HomeServices.Find(model.ServicesId);
                        if (homeServices != null)
                        {
                            homeServices.Description = model.Description;
                            homeServices.ContactName = model.ContactName;
                            homeServices.ContactDescription = model.ContactDescription;
                            _context.Update(homeServices);
                            _context.SaveChanges();
                        }
                        if (model.ServiceFeatures.Count > 0)
                        {
                            foreach (var i in model.ServiceFeatures)
                            {
                                if (i.ServicesFeatureId < 1)
                                {
                                    var newFeature = new HomeServicesFeature();
                                    newFeature.FeatureDescription = i.FeatureDescription;
                                    newFeature.HomeServicesId = model.ServicesId;
                                    _context.HomeServicesFeature.Add(newFeature);
                                    _context.SaveChanges();
                                }
                                var feature = _context.HomeServicesFeature.Find(
                                    i.ServicesFeatureId
                                );
                                if (feature != null)
                                {
                                    feature.FeatureDescription = i.FeatureDescription;
                                    _context.HomeServicesFeature.Update(feature);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Service" + ResponseMessages.msgUpdationSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    //if(model.ServiceType == Convert.ToInt32(GlobalVariables.AnfasServiceType.LabService))
                    else
                    {
                        var labServices = _context.LabServices.Find(model.ServicesId);
                        if (labServices != null)
                        {
                            labServices.Description = model.Description;
                            labServices.ContactName = model.ContactName;
                            labServices.ContactDescription = model.ContactDescription;
                            _context.Update(labServices);
                            _context.SaveChanges();
                        }
                        if (model.ServiceFeatures.Count > 0)
                        {
                            foreach (var i in model.ServiceFeatures)
                            {
                                if (i.ServicesFeatureId < 1)
                                {
                                    var newFeature = new LabServicesFeature();
                                    newFeature.FeatureDescription = i.FeatureDescription;
                                    newFeature.LabServicesId = model.ServicesId;
                                    _context.LabServicesFeature.Add(newFeature);
                                    _context.SaveChanges();
                                }
                                var feature = _context.LabServicesFeature.Find(i.ServicesFeatureId);
                                if (feature != null)
                                {
                                    feature.FeatureDescription = i.FeatureDescription;
                                    _context.LabServicesFeature.Update(feature);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Service" + ResponseMessages.msgUpdationSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
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

        #region UploadServiceImage
        [HttpPost]
        [Authorize]
        [Route("UploadServiceImage")]
        public async Task<IActionResult> UploadServiceImage([FromForm] UploadServiceImage model)
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
                string currentuserid = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentuserid))
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
                var user = await _userManager.FindByIdAsync(currentuserid);
                if (user != null)
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(model.ServiceImage.ContentDisposition)
                        .FileName.Trim('"');
                    filename = CommonFunctions.EnsureCorrectFilename(filename);
                    filename = CommonFunctions.RenameFileName(filename);
                    // using (
                    //     FileStream fs = System.IO.File.Create(
                    //         GetPathAndFilename(filename, GlobalVariables.ServicePictureContainer)
                    //     )
                    // )
                    // {
                    //     model.ServiceImage.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.ServiceImage,
                        GlobalVariables.ServicePictureContainer, filename
                    );

                    if (
                        model.ServiceTypeValue
                        == Convert.ToInt32(GlobalVariables.AnfasServiceType.HomeService)
                    )
                    {
                        int HomeServiceId = model.ServiceId;
                        HomeServices ServiceImage = _context.HomeServices
                            .Where(x => x.HomeServicesId == model.ServiceId)
                            .FirstOrDefault();
                        ServiceImage.ImagePath =
                            GlobalVariables.ServicePictureContainer + "/" + filename;
                        _context.Update(ServiceImage);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int LabServicesId = model.ServiceId;
                        LabServices ServiceImage = _context.LabServices
                            .Where(x => x.LabServicesId == model.ServiceId)
                            .FirstOrDefault();
                        ServiceImage.ImagePath =
                            GlobalVariables.ServicePictureContainer + "/" + filename;
                        _context.Update(ServiceImage);
                        _context.SaveChanges();
                    }
                    // else
                    // {
                    //     //int RadiologyServicesId = model.ServiceId;
                    //     // RadiologyServices ServiceImage = _context.RadiologyServices.Where(x => x.RadiologyServicesId == model.ServiceId).FirstOrDefault();
                    //     // ServiceImage.ImagePath = GlobalVariables.ServicePictureContainer + filename;
                    //     // _context.Update(ServiceImage);
                    //     // _context.SaveChanges();
                    // }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Service image" + ResponseMessages.msgAdditionSuccess,
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
                            message = ResponseMessages.msgCouldNotFoundAssociatedUser,
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

        #region UploadContactServiceImage
        [HttpPost]
        [Authorize]
        [Route("UploadContactServiceImage")]
        public async Task<IActionResult> UploadContactServiceImage(
            [FromForm] UploadServiceImage model
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
                string currentuserid = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentuserid))
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
                var user = await _userManager.FindByIdAsync(currentuserid);
                if (user != null)
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(model.ServiceImage.ContentDisposition)
                        .FileName.Trim('"');
                    filename = CommonFunctions.EnsureCorrectFilename(filename);
                    filename = CommonFunctions.RenameFileName(filename);
                    using (
                        FileStream fs = System.IO.File.Create(
                            GetPathAndFilename(filename, GlobalVariables.ServicePictureContainer)
                        )
                    )
                    {
                        model.ServiceImage.CopyTo(fs);
                        fs.Flush();
                    }
                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.ServiceImage,
                        GlobalVariables.ServicePictureContainer, filename
                    );

                    if (
                        model.ServiceTypeValue
                        == Convert.ToInt32(GlobalVariables.AnfasServiceType.HomeService)
                    )
                    {
                        int HomeServiceId = model.ServiceId;
                        HomeServices ServiceImage = _context.HomeServices
                            .Where(x => x.HomeServicesId == model.ServiceId)
                            .FirstOrDefault();
                        ServiceImage.ContactImagePath =
                            GlobalVariables.ServicePictureContainer + "/" + filename;
                        _context.Update(ServiceImage);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int LabServicesId = model.ServiceId;
                        LabServices ServiceImage = _context.LabServices
                            .Where(x => x.LabServicesId == model.ServiceId)
                            .FirstOrDefault();
                        ServiceImage.ContactImagePath =
                            GlobalVariables.ServicePictureContainer + "/" + filename;
                        _context.Update(ServiceImage);
                        _context.SaveChanges();
                    }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Contact service image" + ResponseMessages.msgAdditionSuccess,
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
                            message = ResponseMessages.msgCouldNotFoundAssociatedUser,
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

        #region Private Methods
        private string GetPathAndFilename(string filename, string foldername)
        {
            string path = _hostingEnvironment.WebRootPath + "//" + foldername + "//";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path + filename;
        }

        #endregion

        #region DeleteServiceFeature
        [HttpDelete]
        [Authorize]
        [Route("DeleteServiceFeature")]
        public async Task<IActionResult> DeleteServiceFeature(int featureId, int serviceTypeValue)
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
                if (user == null && string.IsNullOrEmpty(featureId.ToString()))
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
                    if (
                        serviceTypeValue
                        == Convert.ToInt32(GlobalVariables.AnfasServiceType.HomeService)
                    )
                    {
                        var deleteFeature = _context.HomeServicesFeature
                            .Where(x => x.HomeServicesFeatureId == featureId)
                            .FirstOrDefault();
                        var result = _context.HomeServicesFeature.Remove(deleteFeature);
                        _context.SaveChanges();
                    }
                    // if (Convert.ToInt32(serviceType) == Convert.ToInt32(GlobalVariables.AnfasServiceType.LabService))
                    else
                    {
                        var deleteFeature = _context.LabServicesFeature
                            .Where(x => x.LabServicesFeatureId == featureId)
                            .FirstOrDefault();
                        var result = _context.LabServicesFeature.Remove(deleteFeature);
                        _context.SaveChanges();
                    }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            message = "Service Feature" + ResponseMessages.msgDeletionSuccess,
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

        #region GetServicesPlanList
        [HttpGet]
        [Authorize]
        [Route("GetServicesPlanList")]
        public IActionResult GetServicesPlanList(int serviceTypeValue)
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
                if (serviceTypeValue == Convert.ToInt32(GlobalVariables.ServiceType.HomeService))
                {
                    // Get Home service Detail
                    var getHomeServicePlans = _context.HomeServicesPlans.ToList();

                    List<ResponseServicesPlanViewModel> responseModel =
                        new List<ResponseServicesPlanViewModel>();
                    if (getHomeServicePlans.Count > 0)
                    {
                        foreach (var i in getHomeServicePlans)
                        {
                            var plan = new ResponseServicesPlanViewModel();
                            plan.ServicesPlanId = i.HomeServicesPlanId;
                            plan.PlanAmount = i.PlanAmount;
                            plan.PlanDescription = i.PlanDescription;
                            plan.PlanName = i.PlanName;
                            plan.PlanAddedDate = i.PlanAddedDate;

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
                    // Get Lab service Detail
                    var getLabServicePlans = _context.LabServicesPlan.ToList();

                    List<ResponseServicesPlanViewModel> responseModel =
                        new List<ResponseServicesPlanViewModel>();
                    if (getLabServicePlans.Count > 0)
                    {
                        foreach (var i in getLabServicePlans)
                        {
                            var plan = new ResponseServicesPlanViewModel();
                            plan.ServicesPlanId = i.LabServicesPlanId;
                            plan.PlanAmount = i.PlanAmount;
                            plan.PlanDescription = i.PlanDescription;
                            plan.PlanName = i.PlanName;
                            plan.PlanAddedDate = i.PlanAddedDate;

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

        #region BookService
        [HttpPost]
        [Authorize(Roles = "Patient")] //(Roles = "Patient")
        [Route("BookService")]
        public IActionResult BookServices(RequestServiceViewModel model)
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
                if (
                    model.ServiceTypeValue
                    == Convert.ToInt32(GlobalVariables.ServiceType.HomeService)
                )
                {
                    var bookHomeService = new BookHomeService();
                    bookHomeService.HomeServiceId = model.ServiceId;
                    bookHomeService.HomeServicesPlanId = model.ServicesPlanId;
                    bookHomeService.FromDate = model.FromDate;
                    bookHomeService.ToDate = model.ToDate;
                    bookHomeService.FromTime = model.FromTime;
                    bookHomeService.ToTime = model.ToTime;
                    bookHomeService.Reason = model.Reason;
                    bookHomeService.AddressStreet = model.AddressStreet;
                    bookHomeService.AddressCountry = model.AddressCountry;
                    bookHomeService.AddressLong = model.AddressLong;
                    bookHomeService.AddressLat = model.AddressLat;
                    bookHomeService.PatientId = CurrentUserId;
                    bookHomeService.Status = (int)GlobalVariables.AppointmentStatus.Pending;

                    _context.BookHomeService.Add(bookHomeService);
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            // data = responseModel,
                            message = "Home service booked successfully",
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    var bookLabService = new BookLabService();
                    bookLabService.LabServiceId = model.ServiceId;
                    bookLabService.LabServicesPlanId = model.ServicesPlanId;
                    bookLabService.FromDate = model.FromDate;
                    bookLabService.ToDate = model.ToDate;
                    bookLabService.FromTime = model.FromTime;
                    bookLabService.ToTime = model.ToTime;
                    bookLabService.Reason = model.Reason;
                    bookLabService.AddressStreet = model.AddressStreet;
                    bookLabService.AddressCountry = model.AddressCountry;
                    bookLabService.AddressLong = model.AddressLong;
                    bookLabService.AddressLat = model.AddressLat;
                    bookLabService.PatientId = CurrentUserId;
                    bookLabService.Status = (int)GlobalVariables.AppointmentStatus.Pending;

                    _context.BookLabService.Add(bookLabService);
                    _context.SaveChanges();

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            // data = responseModel,
                            message = "Lab service booked successfully",
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

        #region GetBookedServicesList
        [HttpGet]
        [Authorize]
        [Route("GetBookedServicesList")]
        public IActionResult GetBookedServicesList(int serviceTypeValue)
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
                if (serviceTypeValue == Convert.ToInt32(GlobalVariables.ServiceType.HomeService))
                {
                    // Get Home service Detail
                    var getBookedHomeServices = _context.BookHomeService.ToList();

                    List<ResponseBookHomeServiceViewModel> responseModel =
                        new List<ResponseBookHomeServiceViewModel>();
                    if (getBookedHomeServices.Count > 0)
                    {
                        foreach (var i in getBookedHomeServices)
                        {
                            var bookedHomeService = new ResponseBookHomeServiceViewModel();
                            bookedHomeService.PatientId = CurrentUserId;
                            bookedHomeService.FromDate = i.FromDate;
                            bookedHomeService.ToDate = i.ToDate;
                            bookedHomeService.ToTime = i.ToTime;
                            bookedHomeService.FromTime = i.FromTime;
                            bookedHomeService.Reason = i.Reason;
                            bookedHomeService.AddressStreet = i.AddressStreet;
                            bookedHomeService.AddressLat = i.AddressLat;
                            bookedHomeService.AddressLong = i.AddressLong;
                            bookedHomeService.AddressCountry = i.AddressCountry;

                            responseModel.Add(bookedHomeService);
                        }
                    }

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Booked Home services" + ResponseMessages.msgShownSuccess,
                            code = StatusCodes.Status200OK
                        }
                    );
                }
                else
                {
                    // Get Lab service Detail
                    var getBookedLabServices = _context.BookLabService.ToList();

                    List<ResponseBookLabServiceViewModel> responseModel =
                        new List<ResponseBookLabServiceViewModel>();
                    if (getBookedLabServices.Count > 0)
                    {
                        foreach (var i in getBookedLabServices)
                        {
                            var bookedLabService = new ResponseBookLabServiceViewModel();
                            bookedLabService.PatientId = CurrentUserId;
                            bookedLabService.FromDate = i.FromDate;
                            bookedLabService.ToDate = i.ToDate;
                            bookedLabService.ToTime = i.ToTime;
                            bookedLabService.FromTime = i.FromTime;
                            bookedLabService.Reason = i.Reason;
                            bookedLabService.AddressStreet = i.AddressStreet;
                            bookedLabService.AddressLat = i.AddressLat;
                            bookedLabService.AddressLong = i.AddressLong;
                            bookedLabService.AddressCountry = i.AddressCountry;

                            responseModel.Add(bookedLabService);
                        }
                    }

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Booked Lab services" + ResponseMessages.msgShownSuccess,
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

        #region AddServicePlan
        [HttpPost]
        [Authorize]
        [Route("AddServicePlan")]
        public async Task<IActionResult> AddServicePlan(
            [FromBody] RequestServicePlanViewModel model
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
                    if (
                        model.ServiceTypeValue
                        == Convert.ToInt32(GlobalVariables.ServiceType.HomeService)
                    )
                    {
                        var homeServicesPlans = new HomeServicesPlans()
                        {
                            PlanName = model.PlanName,
                            PlanDescription = model.PlanDescription,
                            PlanAddedDate = model.PlanAddedDate,
                            PlanAmount = model.PlanAmount,
                        };
                        _context.Add(homeServicesPlans);
                        _context.SaveChanges();

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message =
                                    "Service plan" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        var labServicesPlans = new LabServicesPlan()
                        {
                            PlanName = model.PlanName,
                            PlanDescription = model.PlanDescription,
                            PlanAddedDate = model.PlanAddedDate,
                            PlanAmount = model.PlanAmount,
                        };
                        _context.Add(labServicesPlans);
                        _context.SaveChanges();

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message =
                                    "Service plan" + ResponseMessages.msgAdditionSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
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

        #region DeleteServicePlan
        [HttpDelete]
        [Authorize]
        [Route("DeleteServicePlan")]
        public async Task<IActionResult> DeleteServicePlan(int PlanID, int serviceTypeValue)
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
                if (user == null && string.IsNullOrEmpty(PlanID.ToString()))
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
                    if (
                        serviceTypeValue == Convert.ToInt32(GlobalVariables.ServiceType.HomeService)
                    )
                    {
                        var validId = await _context.BookHomeService.ToListAsync();
                        int goForDelete = 0;
                        foreach (var item in validId)
                        {
                            if (item.HomeServicesPlanId == PlanID)
                            {
                                goForDelete = 1;
                            }
                        }
                        if (goForDelete == 0)
                        {
                            var DeletePackage = await _context.HomeServicesPlans
                                .Where(x => x.HomeServicesPlanId == PlanID)
                                .SingleOrDefaultAsync();

                            var result = _context.HomeServicesPlans.Remove(DeletePackage);
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
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = true,
                                    message = "Can't delete, This plan is booked",
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                    else
                    {
                        var validId = await _context.BookLabService.ToListAsync();
                        int goForDelete = 0;
                        foreach (var item in validId)
                        {
                            if (item.LabServicesPlanId == PlanID)
                            {
                                goForDelete = 1;
                            }
                        }
                        if (goForDelete == 0)
                        {
                            var DeletePackage = await _context.LabServicesPlan
                                .Where(x => x.LabServicesPlanId == PlanID)
                                .SingleOrDefaultAsync();

                            var result = _context.LabServicesPlan.Remove(DeletePackage);
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
                        else
                        {
                            return Ok(
                                new ApiResponseModel
                                {
                                    status = true,
                                    message = "Can't delete, This plan is booked",
                                    data = new { },
                                    code = StatusCodes.Status200OK
                                }
                            );
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Ok(
                    new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgSomethingWentWrong,
                        code = StatusCodes.Status500InternalServerError
                    }
                );
            }
        }
        #endregion

        #region UpdateServicePlan
        [HttpPost]
        [Authorize]
        [Route("UpdateServicePlan")]
        public async Task<IActionResult> UpdateServicePlan(
            [FromBody] RequestupdateServicePlanViewModel model
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
                    if (
                        model.ServiceTypeValue
                        == Convert.ToInt32(GlobalVariables.ServiceType.HomeService)
                    )
                    {
                        var homeServicesPlans = _context.HomeServicesPlans.Find(
                            model.ServicesPlanId
                        );
                        homeServicesPlans.PlanName = model.PlanName;
                        homeServicesPlans.PlanDescription = model.PlanDescription;
                        homeServicesPlans.PlanAddedDate = model.PlanAddedDate;
                        homeServicesPlans.PlanAmount = model.PlanAmount;
                        _context.Update(homeServicesPlans);
                        _context.SaveChanges();

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Home Service Plan" + ResponseMessages.msgUpdationSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
                    else
                    {
                        var LabServicesPlan = _context.LabServicesPlan.Find(model.ServicesPlanId);
                        LabServicesPlan.PlanName = model.PlanName;
                        LabServicesPlan.PlanDescription = model.PlanDescription;
                        LabServicesPlan.PlanAddedDate = model.PlanAddedDate;
                        LabServicesPlan.PlanAmount = model.PlanAmount;
                        _context.Update(LabServicesPlan);
                        _context.SaveChanges();

                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Lab Service Plan" + ResponseMessages.msgUpdationSuccess,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
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

        #region RequestReport
        [HttpPost]
        [Authorize]
        [Route("RequestReport")]
        public async Task<IActionResult> RequestReport(
            [FromBody] RequestReportViewModel model
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
                    var requestReport = await _context.RequestReport.Where(a => a.PatientId == CurrentUserId).FirstOrDefaultAsync();
                    if (requestReport != null)
                    {

                        requestReport.PatientEmail = model.PatientEmail;

                        _context.Update(requestReport);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var addRequestReport = new RequestReport()
                        {
                            PatientEmail = model.PatientEmail,
                            PatientId = CurrentUserId,
                            Status = 0
                        };
                        _context.Add(addRequestReport);
                        await _context.SaveChangesAsync();
                    }

                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message =
                                "Requested successfully.",
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
