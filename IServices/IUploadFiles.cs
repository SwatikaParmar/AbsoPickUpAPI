using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IUploadFiles
    {
        Task<bool> UploadFilesToServer(IFormFile file, string prefix,string fileName);
    }
}
