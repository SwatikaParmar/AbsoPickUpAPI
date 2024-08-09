using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _hostingEnvironment;
        private IBannerService _bannerService;
        private IUploadFiles _uploadFiles;

        public BannerController(
            UserManager<ApplicationUser> userManager,
            IUploadFiles uploadFiles,
            IWebHostEnvironment hostingEnvironment,
            IBannerService bannerService
        )
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _bannerService = bannerService;
            _uploadFiles = uploadFiles;
        }

        #region AddBanner
        [HttpPost]
        // [Authorize(Roles = "SuperAdmin")]
        [Route("AddBanner")]
        public async Task<IActionResult> AddBanner([FromForm] AddBannerViewModel model)
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

                if (model.BannerImage == null)
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgImageFieldRequired,
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
                    var bannerImageFile = ContentDispositionHeaderValue
                        .Parse(model.BannerImage.ContentDisposition)
                        .FileName.Trim('"');
                    bannerImageFile = CommonFunctions.EnsureCorrectFilename(bannerImageFile);
                    bannerImageFile = CommonFunctions.RenameFileName(bannerImageFile);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(bannerImageFile, GlobalVariables.bannerImagesContainer)))
                    // {
                    //     model.BannerImage.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    string imgPath = GlobalVariables.bannerImagesContainer + "/" + bannerImageFile;
                    var _banner = new Banner()
                    {
                        BannerImage = imgPath,
                        BannerType = model.BannerType,
                        BannerTypeId = model.BannerTypeId,
                    };
                    bool result = _bannerService.CreateBanner(_banner);

                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.BannerImage,
                        GlobalVariables.bannerImagesContainer,bannerImageFile
                    );
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = _banner,
                                message = "Banner" + ResponseMessages.msgAdditionSuccess,
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

        #region GetBannerList
        [HttpGet]
        [Authorize]
        [Route("GetBannerList")]
        public async Task<IActionResult> GetBannerList()
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
                if (user != null)
                {
                    var bannerList = _bannerService.GetBannerList();
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = bannerList,
                            message = "Banner List" + ResponseMessages.msgShownSuccess,
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


        #region "private Methods"
        private string GetPathAndFilename(string filename, string foldername)
        {
            string path = _hostingEnvironment.WebRootPath + "//" + foldername + "//";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path + filename;
        }
        #endregion
    }
}
