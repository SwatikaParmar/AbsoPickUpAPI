using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IBlogService
    {
        bool CreateBlog(BlogDetails _blogDetails);
        Task<bool> DeleteBlog(string blogId, string userId);
        BlogDetailViewModel GetBlogDetails(string blogId);
        FilterationResponseModel<BlogDetailViewModel> GetBlogList(FilterationListViewModel model);
        FilterationResponseModel<BlogDetailViewModel> GetAllBlogs(FilterationListViewModel model);
        Task<FilterationResponseModel<BlogDetails>> GetUserBlogs(string userId, FilterationListViewModel model);
        Task<bool> UpdateBlogStatus(UpdateBlogStatusViewModel model);
    }
}
