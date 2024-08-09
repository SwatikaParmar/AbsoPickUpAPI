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
    public class ToDoNotesController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private IToDoNotesService _toDoNotesService;
        public ToDoNotesController(UserManager<ApplicationUser> userManager, IToDoNotesService toDoNotesService)
        {
            _userManager = userManager;
            _toDoNotesService = toDoNotesService;
        }
        #region "AddToDoNotes"
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("AddToDoNotes")]
        public async Task<IActionResult> AddToDoNotes(AddDoctorNotesViewModel model)
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
                if(string.IsNullOrEmpty(CurrentUserId))
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
                if(user != null)
                {
                    var toDoListModel = new DoctorToDoNotes()
                    {
                        UserId = user.Id,
                        Title = model.Title,
                        Description = model.Description,
                        CreatedDate = DateTime.UtcNow

                    };
                    bool result = _toDoNotesService.AddToDoNotes(toDoListModel);
                    if (result)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = ResponseMessages.msgToDoListSaved,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = false,
                            data = new { },
                            message = ResponseMessages.msgSomethingWentWrong,
                            code = StatusCodes.Status200OK
                        });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                }
            }
            catch(Exception ex)
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

        #region "GetToDoNotes"
        [HttpGet]
        [Authorize(Roles = "Doctor")]
        [Route("GetToDoNotes")]
        public async Task<IActionResult> GetToDoNotes([FromQuery] FilterationListViewModel model)
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
                if(user != null)
                { 
                    var List = _toDoNotesService.GetToDoNotes(model, user.Id);
                    return Ok(new ApiResponseModel
                    {
                        status = true,
                        data = List,
                        message = "To do List" + ResponseMessages.msgShownSuccess,
                        code = StatusCodes.Status200OK
                    });
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                }

            }
            catch(Exception ex)
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

        #region UpdateToDoNotes
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [Route("UpdateToDoNotes")]
        public async Task<IActionResult> UpdateToDoNotes(UpdateToDoListViewModel model)
        {
            try
            {
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        data = new { },
                        message = ResponseMessages.msgTokenExpired,
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if(user != null)
                {
                    bool result = await _toDoNotesService.UpdateToDoNotes(model, user.Id);
                    if (result)
                    {
                        return Ok(new ApiResponseModel
                        {

                            status = true,
                            data = new { },
                            message = "To Do " + ResponseMessages.msgUpdationSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
                    }
                }
                else
                {
                    return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgUserNotFound, code = StatusCodes.Status200OK });
                }
            }
            catch(Exception ex)
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

        #region "DeleteToDoNotes"
        [HttpDelete]
        [Authorize(Roles = "Doctor")]
        [Route("DeleteToDoNotes")]
        public async Task<IActionResult> DeleteToDoNotes(long Id)
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
                string currentUserId = CommonFunctions.getUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Ok(new ApiResponseModel
                    {
                        status = false,
                        message = ResponseMessages.msgTokenExpired,
                        data = new { },
                        code = StatusCodes.Status200OK
                    });
                }
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    bool result = await _toDoNotesService.DeleteToDoNotes(Id, user.Id);
                    if (result == true)
                    {
                        return Ok(new ApiResponseModel
                        {
                            status = true,
                            data = new { },
                            message = "To Do " + ResponseMessages.msgDeletionSuccess,
                            code = StatusCodes.Status200OK
                        });
                    }
                    else
                    {
                        return Ok(new ApiResponseModel { status = false, data = new { }, message = ResponseMessages.msgSomethingWentWrong, code = StatusCodes.Status200OK });
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
