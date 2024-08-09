using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnfasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _hostingEnvironment;
        private IBlogService _blogService;
        private ApplicationDbContext _context;
        private IUploadFiles _uploadFiles;

        public BlogController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostingEnvironment,
            IBlogService blogService,
            ApplicationDbContext context,
            IUploadFiles uploadFiles
        )
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _blogService = blogService;
            _context = context;
            _uploadFiles = uploadFiles;
        }

        #region CreateBlog
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("CreateBlog")]
        public async Task<IActionResult> CreateBlog([FromForm] AddBlogDetailsViewModel model)
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

                if (model.BlogImage == null)
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
                    var blogImageFile = ContentDispositionHeaderValue
                        .Parse(model.BlogImage.ContentDisposition)
                        .FileName.Trim('"');
                    blogImageFile = CommonFunctions.EnsureCorrectFilename(blogImageFile);
                    blogImageFile = CommonFunctions.RenameFileName(blogImageFile);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(blogImageFile, GlobalVariables.blogImagesContainer)))
                    // {
                    //     model.BlogImage.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    string imgPath = GlobalVariables.blogImagesContainer + "/" + blogImageFile;
                    var _blogDetails = new BlogDetails()
                    {
                        UserId = user.Id,
                        Title = model.Title,
                        BlogImagePath = imgPath,
                        Description = model.Description,
                        CreatedDate = DateTime.UtcNow,
                        IsAdminApproved = false,
                        BlogStatus = (int)GlobalVariables.BlogStatus.Pending
                    };
                    bool result = _blogService.CreateBlog(_blogDetails);

                    //Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.BlogImage,
                        GlobalVariables.blogImagesContainer,blogImageFile
                    );
                    if (result )
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Blog details" + ResponseMessages.msgAdditionSuccess,
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

        #region DeleteBlog
        [HttpDelete]
        [Authorize(Roles = "Doctor")]
        [Route("DeleteBlog")]
        public async Task<IActionResult> DeleteBlog(string blogId)
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
                if (user == null && string.IsNullOrEmpty(blogId))
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
                    var result = await _blogService.DeleteBlog(blogId, user.Id);
                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = result,
                                message = "Blog details" + ResponseMessages.msgDeletionSuccess,
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
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgSomethingWentWrong,
                                code = StatusCodes.Status200OK
                            }
                        );
                    }
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

        #region GetBlogDetail
        [HttpGet]
        [Authorize]
        [Route("GetBlogDetail")]
        public async Task<IActionResult> GetBlogDetail(string blogId)
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
                    var blogDetailResponse = _blogService.GetBlogDetails(blogId);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = blogDetailResponse,
                            message = "Blog Detail" + ResponseMessages.msgShownSuccess,
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

        #region GetBlogList
        [HttpGet]
        [Authorize]
        [Route("GetBlogList")]
        public async Task<IActionResult> GetBlogList(int pageNumber, int pageSize)
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
                    FilterationListViewModel model = new FilterationListViewModel();
                    model.pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
                    model.pageSize = (pageSize <= 0) ? 10 : pageSize;
                    var blogList = _blogService.GetBlogList(model);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = blogList,
                            message = "Blog List" + ResponseMessages.msgShownSuccess,
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

        #region GetAllBlogs
        [HttpPost]
        [Authorize]
        [Route("GetAllBlogs")]
        public async Task<IActionResult> GetAllBlogs(FilterationListViewModel model)
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
                    var blogList = _blogService.GetAllBlogs(model);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = blogList,
                            message = "Blog List" + ResponseMessages.msgShownSuccess,
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

        #region GetUserBlogs
        [HttpPost]
        [Authorize]
        [Route("GetUserBlogs")]
        public async Task<IActionResult> GetUserBlogs(FilterationListViewModel model)
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
                    var blogList = await _blogService.GetUserBlogs(user.Id, model);
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = blogList,
                            message = "Blog List" + ResponseMessages.msgShownSuccess,
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

        #region ApproveBlogStatus
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("ApproveBlogStatus")]
        public async Task<IActionResult> ApproveBlogStatus(UpdateBlogStatusViewModel model)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
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
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    bool result = await _blogService.UpdateBlogStatus(model);

                    if (result)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = "Blog Status" + ResponseMessages.msgUpdationSuccess,
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
                            message = ResponseMessages.msgUserNotFound,
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

        #region GetApprovedBlogStatus
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("GetApprovedBlogStatus")]
        public async Task<IActionResult> GetApprovedBlogStatus(bool IsApproved)
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
                    var result = _context.BlogDetails
                        .Where(x => x.IsAdminApproved == IsApproved)
                        .ToList();

                    List<ResponseBlogDetailViewModel> responseModel =
                        new List<ResponseBlogDetailViewModel>();
                    if (result.Count > 0)
                    {
                        foreach (var i in result)
                        {
                            var ResponseModel = new ResponseBlogDetailViewModel();

                            ResponseModel.Id = i.Id;
                            ResponseModel.UserId = i.UserId;
                            ResponseModel.Title = i.Title;
                            ResponseModel.Description = i.Description;
                            ResponseModel.BlogImagePath = i.BlogImagePath;
                            ResponseModel.BlogStatus = i.BlogStatus;
                            ResponseModel.CreatedDate = (i.CreatedDate).ToString();
                            ResponseModel.IsAdminApproved = i.IsAdminApproved;
                            responseModel.Add(ResponseModel);
                        }
                    }
                    return Ok(
                        new ApiResponseModel
                        {
                            status = true,
                            data = responseModel,
                            message = "Blog Details" + ResponseMessages.msgShownSuccess,
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
