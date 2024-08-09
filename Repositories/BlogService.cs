using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class BlogService : IBlogService
    {

        private ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateBlog(BlogDetails _blogDetails)
        {
            try
            {
                _context.BlogDetails.Add(_blogDetails);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBlog(string blogId, string userId)
        {
            var blogDetails = await _context.BlogDetails.Where(x => x.Id == blogId && x.UserId == userId).SingleOrDefaultAsync();
            if (blogDetails != null)
            {
                _context.BlogDetails.Remove(blogDetails);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public BlogDetailViewModel GetBlogDetails(string blogId)
        {
            try
            {
                var details = (from b in _context.BlogDetails
                               join u in _context.ApplicationUser
                               on b.UserId equals u.Id
                               where b.Id == blogId
                               select new BlogDetailViewModel
                               {
                                   Id = b.Id,
                                   UserId = b.UserId,
                                   Title = b.Title,
                                   Description = b.Description,
                                   BlogImagePath = b.BlogImagePath,
                                   CreatedDate = b.CreatedDate.ToString(GlobalVariables.DefaultDateFormat),
                                   DoctorName = u.FirstName + " " + u.LastName,
                                   DoctorImagePath = u.ProfilePic,
                                   IsAdminApproved = b.IsAdminApproved,
                                   BlogStatus = b.BlogStatus,

                               }).FirstOrDefault();

                if (details != null)
                {
                    if (details.BlogStatus == (int)GlobalVariables.BlogStatus.Pending)
                    {
                        details.BlogStatusDisplay = "Pending";
                    }

                    if (details.BlogStatus == (int)GlobalVariables.BlogStatus.Approved)
                    {
                        details.BlogStatusDisplay = "Approved";
                    }

                    if (details.BlogStatus == (int)GlobalVariables.BlogStatus.Rejected)
                    {
                        details.BlogStatusDisplay = "Rejected";
                    }
                }
                return details;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FilterationResponseModel<BlogDetailViewModel> GetBlogList(FilterationListViewModel model)
        {
            try
            {
                if (_context != null)
                {
                    var source = (from b in _context.BlogDetails
                                  join u in _context.ApplicationUser
                                  on b.UserId equals u.Id
                                  where b.IsAdminApproved == true
                                  orderby b.CreatedDate descending
                                  select new BlogDetailViewModel
                                  {
                                      Id = b.Id,
                                      UserId = b.UserId,
                                      Title = b.Title,
                                      Description = string.Empty, //b.Description,
                                      BlogImagePath = b.BlogImagePath,
                                      CreatedDate = b.CreatedDate.ToString(GlobalVariables.DefaultDateFormat),
                                      DoctorName = u.FirstName + " " + u.LastName,
                                      DoctorImagePath = u.ProfilePic,
                                      IsAdminApproved = b.IsAdminApproved,
                                      BlogStatus = b.BlogStatus

                                  }).AsQueryable();


                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x =>
                                                    x.Title.ToLower().Contains(search) ||
                                                    x.DoctorName.ToLower().Contains(search)
                                                    );
                    }

                    // Get's No of Rows Count   
                    int count = source.Count();

                    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                    int CurrentPage = model.pageNumber;

                    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                    int PageSize = model.pageSize;

                    // Display TotalCount to Records to User  
                    int TotalCount = count;

                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";

                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    // Returing List of Customers Collections  
                    FilterationResponseModel<BlogDetailViewModel> obj = new FilterationResponseModel<BlogDetailViewModel>();
                    obj.totalCount = TotalCount;
                    obj.pageSize = PageSize;
                    obj.currentPage = CurrentPage;
                    obj.totalPages = TotalPages;
                    obj.previousPage = previousPage;
                    obj.nextPage = nextPage;
                    obj.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    obj.dataList = items.ToList();
                    return obj;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FilterationResponseModel<BlogDetailViewModel> GetAllBlogs(FilterationListViewModel model)
        {
            try
            {
                if (_context != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.filterBy))
                    {
                        var source = (from b in _context.BlogDetails
                                  join u in _context.ApplicationUser
                                  on b.UserId equals u.Id
                                  where b.BlogStatus == Convert.ToInt32(model.filterBy)
                                  orderby b.CreatedDate descending
                                  select new BlogDetailViewModel
                                  {
                                      Id = b.Id,
                                      UserId = b.UserId,
                                      Title = b.Title,
                                      Description = string.Empty, //b.Description,
                                      BlogImagePath = b.BlogImagePath,
                                      CreatedDate = b.CreatedDate.ToString(GlobalVariables.DefaultDateFormat),
                                      DoctorName = u.FirstName + " " + u.LastName,
                                      DoctorImagePath = u.ProfilePic,
                                      IsAdminApproved = b.IsAdminApproved,
                                      BlogStatus = b.BlogStatus
                                  }).AsQueryable();


                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x =>
                                                    x.Title.ToLower().Contains(search) ||
                                                    x.DoctorName.ToLower().Contains(search)
                                                    );
                    }

                    // Get's No of Rows Count   
                    int count = source.Count();

                    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                    int CurrentPage = model.pageNumber;

                    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                    int PageSize = model.pageSize;

                    // Display TotalCount to Records to User  
                    int TotalCount = count;

                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";

                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    // Returing List of Customers Collections  
                    FilterationResponseModel<BlogDetailViewModel> obj = new FilterationResponseModel<BlogDetailViewModel>();
                    obj.totalCount = TotalCount;
                    obj.pageSize = PageSize;
                    obj.currentPage = CurrentPage;
                    obj.totalPages = TotalPages;
                    obj.previousPage = previousPage;
                    obj.nextPage = nextPage;
                    obj.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    obj.dataList = items.ToList();
                    return obj;
                    }
                    else
                    {
                        var source = (from b in _context.BlogDetails
                                  join u in _context.ApplicationUser
                                  on b.UserId equals u.Id
                                  //where b.BlogStatus == Convert.ToInt32(model.filterBy)
                                  orderby b.CreatedDate descending
                                  select new BlogDetailViewModel
                                  {
                                      Id = b.Id,
                                      UserId = b.UserId,
                                      Title = b.Title,
                                      Description = string.Empty, //b.Description,
                                      BlogImagePath = b.BlogImagePath,
                                      CreatedDate = b.CreatedDate.ToString(GlobalVariables.DefaultDateFormat),
                                      DoctorName = u.FirstName + " " + u.LastName,
                                      DoctorImagePath = u.ProfilePic,
                                      IsAdminApproved = b.IsAdminApproved,
                                      BlogStatus = b.BlogStatus
                                  }).AsQueryable();


                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x =>
                                                    x.Title.ToLower().Contains(search) ||
                                                    x.DoctorName.ToLower().Contains(search)
                                                    );
                    }

                    // Get's No of Rows Count   
                    int count = source.Count();

                    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                    int CurrentPage = model.pageNumber;

                    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                    int PageSize = model.pageSize;

                    // Display TotalCount to Records to User  
                    int TotalCount = count;

                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";

                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    // Returing List of Customers Collections  
                    FilterationResponseModel<BlogDetailViewModel> obj = new FilterationResponseModel<BlogDetailViewModel>();
                    obj.totalCount = TotalCount;
                    obj.pageSize = PageSize;
                    obj.currentPage = CurrentPage;
                    obj.totalPages = TotalPages;
                    obj.previousPage = previousPage;
                    obj.nextPage = nextPage;
                    obj.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    obj.dataList = items.ToList();
                    return obj;
                    }
                    
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FilterationResponseModel<BlogDetails>> GetUserBlogs(string userId, FilterationListViewModel model)
        {
            try
            {
                if (_context != null)
                {
                    var source = await _context.BlogDetails.Where(x => x.UserId == userId).OrderBy(x => x.CreatedDate).ToListAsync();

                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x =>
                                                    x.Title.ToLower().Contains(search)
                                                    ).ToList();
                    }

                    // Get's No of Rows Count   
                    int count = source.Count();

                    // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                    int CurrentPage = model.pageNumber;

                    // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                    int PageSize = model.pageSize;

                    // Display TotalCount to Records to User  
                    int TotalCount = count;

                    // Calculating Totalpage by Dividing (No of Records / Pagesize)  
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

                    // Returns List of Customer after applying Paging   
                    var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    // if CurrentPage is greater than 1 means it has previousPage  
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";

                    // if TotalPages is greater than CurrentPage means it has nextPage  
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    // Returing List of Customers Collections  
                    FilterationResponseModel<BlogDetails> obj = new FilterationResponseModel<BlogDetails>();
                    obj.totalCount = TotalCount;
                    obj.pageSize = PageSize;
                    obj.currentPage = CurrentPage;
                    obj.totalPages = TotalPages;
                    obj.previousPage = previousPage;
                    obj.nextPage = nextPage;
                    obj.searchQuery = string.IsNullOrEmpty(model.searchQuery) ? "no parameter passed" : model.searchQuery;
                    obj.dataList = items.ToList();
                    return obj;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateBlogStatus(UpdateBlogStatusViewModel model)
        {
            try
            {
                var blogDetails = await _context.BlogDetails.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (blogDetails != null)
                {
                    blogDetails.IsAdminApproved = model.IsAdminApproved;
                    if (blogDetails.IsAdminApproved == true)
                    {
                        blogDetails.BlogStatus = 1;
                    }
                    else{
                    blogDetails.BlogStatus = 2;
                    }
                    _context.Update(blogDetails);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}

