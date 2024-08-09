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
    public class ChatController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IChatService _chatService;
        private IWebHostEnvironment _hostingEnvironment;
        private IUploadFiles _uploadFiles;

        public ChatController(
            UserManager<ApplicationUser> userManager,
            IUploadFiles uploadFiles,
            IChatService chatService,
            IWebHostEnvironment hostingEnvironment
        )
        {
            _userManager = userManager;
            _chatService = chatService;
            _hostingEnvironment = hostingEnvironment;
            _uploadFiles = uploadFiles;
        }

        #region "Send Message"
        [HttpPost]
        [Route("SendMessage")]
        [Authorize]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
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
                            code = StatusCodes.Status406NotAcceptable
                        }
                    );
                }
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status401Unauthorized
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var _chatHistory = new ChatHistory()
                    {
                        SenderId = user.Id,
                        ReceiverId = model.ToId,
                        AppointmentId = model.AppointmentId,
                        Text = model.Text,
                        Type = model.Type,
                        CreatedDate = DateTime.UtcNow
                    };
                    int result = _chatService.SendMessage(_chatHistory);
                    if (result == 1)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = ResponseMessages.msgSentSuccess,
                                code = StatusCodes.Status200OK,
                            }
                        );
                    }
                    else if (result == -1)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgNoAppointment,
                                code = StatusCodes.Status200OK,
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
                            code = StatusCodes.Status404NotFound,
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
                        code = StatusCodes.Status500InternalServerError,
                    }
                );
            }
        }
        #endregion

        #region "Send Image File"
        [HttpPost]
        [Route("SendImageFile")]
        [Authorize]
        public async Task<IActionResult> SendImageFile([FromForm] SendImageFileViewModel model)
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
                            code = StatusCodes.Status406NotAcceptable
                        }
                    );
                }
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status401Unauthorized
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var documentFile = ContentDispositionHeaderValue
                        .Parse(model.ImageFile.ContentDisposition)
                        .FileName.Trim('"');
                    documentFile = CommonFunctions.EnsureCorrectFilename(documentFile);
                    documentFile = CommonFunctions.RenameFileName(documentFile);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(documentFile, GlobalVariables.ChatImagesContainer)))
                    // {
                    //     model.ImageFile.CopyTo(fs);
                    //     fs.Flush();
                    // }
                    // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.ImageFile,
                        GlobalVariables.ChatImagesContainer,documentFile
                    );

                    string documentPath = GlobalVariables.ChatImagesContainer + "/" + documentFile;
                    var _chatHistory = new ChatHistory()
                    {
                        SenderId = user.Id,
                        ReceiverId = model.ToId,
                        AppointmentId = model.AppointmentId,
                        Type = model.Type,
                        ImagePath = documentPath,
                        CreatedDate = DateTime.UtcNow
                    };
                    int result = _chatService.SendMessage(_chatHistory);
                    if (result == 1 )
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = ResponseMessages.msgSentSuccess,
                                code = StatusCodes.Status200OK,
                            }
                        );
                    }
                    else if (result == -1)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgNoAppointment,
                                code = StatusCodes.Status200OK,
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
                            code = StatusCodes.Status404NotFound,
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
                        code = StatusCodes.Status500InternalServerError,
                    }
                );
            }
        }
        #endregion

        #region "Send Document File"
        [HttpPost]
        [Route("SendDocumentFile")]
        [Authorize]
        public async Task<IActionResult> SendDocumentFile(
            [FromForm] SendDocumentFileViewModel model
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
                            code = StatusCodes.Status406NotAcceptable
                        }
                    );
                }
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(
                        new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgTokenExpired,
                            code = StatusCodes.Status401Unauthorized
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var documentFile = ContentDispositionHeaderValue
                        .Parse(model.DocumentFile.ContentDisposition)
                        .FileName.Trim('"');
                    documentFile = CommonFunctions.EnsureCorrectFilename(documentFile);
                    documentFile = CommonFunctions.RenameFileName(documentFile);
                    // using (FileStream fs = System.IO.File.Create(GetPathAndFilename(documentFile, GlobalVariables.ChatDocumentContainer)))
                    // {
                    //     model.DocumentFile.CopyTo(fs);
                    //     fs.Flush();
                    // }

                    // // Upload Image To Server
                    bool uploadStatus = await _uploadFiles.UploadFilesToServer(
                        model.DocumentFile,
                        GlobalVariables.ChatDocumentContainer,documentFile
                    );

                    string documentPath =
                        GlobalVariables.ChatDocumentContainer + "/" + documentFile;
                    var _chatHistory = new ChatHistory()
                    {
                        SenderId = user.Id,
                        ReceiverId = model.ToId,
                        AppointmentId = model.AppointmentId,
                        Type = model.Type,
                        DocumentPath = documentPath,
                        CreatedDate = DateTime.UtcNow
                    };
                    int result = _chatService.SendMessage(_chatHistory);
                    if (result == 1)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = new { },
                                message = ResponseMessages.msgSentSuccess,
                                code = StatusCodes.Status200OK,
                            }
                        );
                    }
                    else if (result == -1)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = false,
                                data = new { },
                                message = ResponseMessages.msgNoAppointment,
                                code = StatusCodes.Status200OK,
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
                            code = StatusCodes.Status404NotFound,
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
                        code = StatusCodes.Status500InternalServerError,
                    }
                );
            }
        }
        #endregion

        #region "GetChatHistory"
        [HttpGet]
        [Route("GetChatHistory")]
        [Authorize]
        public async Task<IActionResult> GetChatHistoryList(
            [FromQuery] FilterationListViewModel model,
            string ReceiverId,
            string AppointmentId
        )
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
                            code = StatusCodes.Status401Unauthorized
                        }
                    );
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    var _chatHistoryList = _chatService.GetChatHistory(
                        model,
                        user.Id,
                        ReceiverId,
                        AppointmentId
                    );
                    if (_chatHistoryList != null)
                    {
                        return Ok(
                            new ApiResponseModel
                            {
                                status = true,
                                data = _chatHistoryList,
                                message = "Chat History got successfully",
                                code = StatusCodes.Status200OK,
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
                                message = "No data",
                                code = StatusCodes.Status404NotFound,
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
                            code = StatusCodes.Status404NotFound,
                        }
                    );
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
                        code = StatusCodes.Status500InternalServerError,
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
