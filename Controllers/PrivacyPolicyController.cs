
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using AnfasAPI.ViewModels;
using AnfasAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.Collections.Generic;
using System;
using AnfasAPI.Data;

namespace ChefMiddleEast.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrivacyPolicyController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public PrivacyPolicyController(ApplicationDbContext context, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet("getPrivacyPolicy")]
        public async Task<IActionResult> getPrivacyPolicy()
        {
            try
            {
                var getPrivacyPolicy = await _context.PrivacyPolicy.FirstOrDefaultAsync();
                var maper = new PrivacyPolicyViewModel();
                maper.privacyPolicyContent = getPrivacyPolicy.PrivacyPolicyContent;

                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Data shown successfully",
                    data = maper.privacyPolicyContent
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message,
                });
            }
        }

        [HttpGet("getAdminPrivacyPolicy")]
        public async Task<IActionResult> getAdminPrivacyPolicy()
        {
            try
            {
                var getPrivacyPolicy = await _context.PrivacyPolicy.FirstOrDefaultAsync();
                var maper = new PrivacyPolicyAdminViewModels();
                maper.privacyPolicyContent = getPrivacyPolicy.PrivacyPolicyContent;
                maper.privacyPolicyId=getPrivacyPolicy.PrivacyPolicyId;

                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Data shown successfully",
                    data = getPrivacyPolicy
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message,
                });
            }
        }

        [HttpPost("adminAddPrivacyPolicy")]
        public async Task<IActionResult> adminAddPrivacyPolicy([FromBody] PrivacyPolicyViewModel model)
        {
            try
            {
                var data= new PrivacyPolicy();
                data.PrivacyPolicyContent=model.privacyPolicyContent;
                

                await _context.AddAsync(data);
                await _context.SaveChangesAsync();


                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Privacy Policy added successfully",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message,
                });
            }
        }


        [HttpPost("adminUpdatePrivacyPolicy")]
        public async Task<IActionResult> adminUpdatePrivacyPolicy([FromBody] UpdatePrivacyPolicyViewModels model)
        {
            try
            {
                var getData = await _context.PrivacyPolicy
                    .Where(i => i.PrivacyPolicyId == model.privacyPolicyId)
                    .FirstOrDefaultAsync();

                if (getData != null)
                {
                    getData.PrivacyPolicyContent = model.privacyPolicyContent;

                    _context.Update(getData);
                    await _context.SaveChangesAsync();
                }


                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Privacy policy updated successfully",
                    data = getData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Success = false,
                    Message = ex.Message,
                });
            }
        }

    }


}
