using Amazon.S3;
using Amazon.S3.Model;
using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.Helpers;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class UploadFiles : IUploadFiles
    {
        private readonly IAmazonS3 _s3Client;
        private Aws3Services _aws3Services { get; }

        public UploadFiles(IAmazonS3 s3Client, IOptions<Aws3Services> aws3Services)
        {
            _s3Client = s3Client;
            _aws3Services = aws3Services.Value;
        }

        public async Task<bool> UploadFilesToServer(
            IFormFile file,
            string prefix,
            string fileName
        )
        {
            prefix = "FilesToSave/"+ prefix;
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(_aws3Services.BucketName);
            if (!bucketExists)
                return false;
            var request = new PutObjectRequest()
            {
                BucketName = _aws3Services.BucketName,
                Key = string.IsNullOrEmpty(prefix)
                    ? file.FileName
                    : $"{prefix?.TrimEnd('/')}/{fileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return true;
        }
    }
}
