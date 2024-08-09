
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
    public class TermsAndConditionsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public TermsAndConditionsController(ApplicationDbContext context, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet("getTermsAndConditions")]
        public async Task<IActionResult> getTermsAndConditions()
        {
            try
            {
                var getTermsAndConditions = await _context.TermsAndConditions.FirstOrDefaultAsync();
                var termsAndConditionsDto = new TermsAndConditionsViewModel();
                termsAndConditionsDto.TermsAndConditionsContent = getTermsAndConditions.TermsAndConditionsContent;
                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Data shown successfully",
                    data = getTermsAndConditions.TermsAndConditionsContent
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

        [HttpGet("getAdminTermsAndConditions")]
        public async Task<IActionResult> getAdminTermsAndConditions()
        {
            try
            {
                var getTermsAndConditions = await _context.TermsAndConditions.FirstOrDefaultAsync();
                var termsAndConditionsDto = new TermsAndConditionsAdminViewModel();
                termsAndConditionsDto.TermsAndConditionsContent = getTermsAndConditions.TermsAndConditionsContent;
                termsAndConditionsDto.termsAndConditionsId = getTermsAndConditions.TermsAndConditionsId;

                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Data shown successfully",
                    data = getTermsAndConditions
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

        [HttpPost("adminAddTermsAndConditions")]
        public async Task<IActionResult> adminAddTermsAndConditions([FromBody] TermsAndConditionsViewModel model)
        {
            try
            {
                var mapData = new TermsAndConditions();
                mapData.TermsAndConditionsContent = model.TermsAndConditionsContent;
                await _context.AddAsync(mapData);
                await _context.SaveChangesAsync();


                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Terms and conditions added successfully",
                    data = mapData
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


        [HttpPost("adminUpdateTermsAndConditions")]
        public async Task<IActionResult> adminUpdateTermsAndConditions([FromBody] UpdateTermsAndConditionsViewModel model)
        {
            try
            {
                var getData = await _context.TermsAndConditions
                    .Where(i => i.TermsAndConditionsId == model.termsAndConditionsId)
                    .FirstOrDefaultAsync();

                if (getData != null)
                {
                    getData.TermsAndConditionsContent = model.TermsAndConditionsContent;

                    _context.Update(getData);
                    await _context.SaveChangesAsync();
                }


                return StatusCode(200, new
                {
                    Status = 200,
                    Success = true,
                    Message = "Terms and conditions updated successfully",
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
