using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestReport, PatientReportRequestViewModel>().ReverseMap();
        }
    }
}
