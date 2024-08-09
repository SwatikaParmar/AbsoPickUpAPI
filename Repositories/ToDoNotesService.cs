using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class ToDoNotesService : IToDoNotesService
    {
        private ApplicationDbContext _context;
        public ToDoNotesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddToDoNotes(DoctorToDoNotes _toDoListModel)
        {
            try
            {
                _context.DoctorToDoNotes.Add(_toDoListModel);
                _context.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public FilterationResponseModel<GetToDoListViewModel> GetToDoNotes(FilterationListViewModel model ,string userId)
        {
            
            try
            {
                if (_context != null)
                {
                    var source = _context.DoctorToDoNotes.Where(x => x.UserId == userId).Select(
                                                                                  x => new GetToDoListViewModel()
                                                                                 {
                                                                                    Id = x.Id,
                                                                                    Title = x.Title,
                                                                                    Description = x.Description,
                                                                                    Date = x.CreatedDate.Value.ToString(GlobalVariables.DefaultDateFormat),

                                                                                 }) .AsQueryable();


                    // searching
                    if (!string.IsNullOrWhiteSpace(model.searchQuery))
                    {
                        var search = model.searchQuery.ToLower();
                        source = source.Where(x => x.Title.ToLower().Contains(search));                  
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
                    FilterationResponseModel<GetToDoListViewModel> obj = new FilterationResponseModel<GetToDoListViewModel>();
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

        public async Task<bool> UpdateToDoNotes(UpdateToDoListViewModel model , string userId)
        {
            try
            {
                var _toDoList = await _context.DoctorToDoNotes.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (_toDoList != null)
                {
                    _toDoList.Title = model.Title;
                    _toDoList.Description = model.Description;
                    _context.DoctorToDoNotes.Update(_toDoList);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteToDoNotes(long Id, string userId)
        {
            var _toDoList = await _context.DoctorToDoNotes.Where(x => x.Id == Id && x.UserId == userId).SingleOrDefaultAsync();
            if (_toDoList != null)
            {
                _context.DoctorToDoNotes.Remove(_toDoList);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

       
    }
  }

