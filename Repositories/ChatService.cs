using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int SendMessage(ChatHistory chatHistory)
        {
            try
            {
                var _appointmentDetails = _context.Appointments.Where(x => x.Id == chatHistory.AppointmentId
                                           && x.Status == (int)GlobalVariables.AppointmentStatus.Confirmed
                                           && x.IsDeleted == false).FirstOrDefault();
                if(_appointmentDetails !=  null)
                {
                    _context.ChatHistory.Add(chatHistory);
                    _context.SaveChanges();
                    return 1;
                }
                else
                {
                    return -1;
                }
            
            }
            catch (Exception)
            {
                throw;
            }
        }

        public FilterationResponseModel<ChatHistoryResponseViewModel> GetChatHistory(FilterationListViewModel model, string userId, string senderId, string appointmentId)
        {
            try
            {
                if (_context != null)
                {
                    var source = _context.ChatHistory.Where(x => x.ReceiverId == userId
                                                              && x.SenderId == senderId
                                                              && x.AppointmentId == appointmentId)
                                                       .Select(x => new ChatHistoryResponseViewModel
                                                       {
                                                           MessageId = x.MessageId,
                                                           Text = (!string.IsNullOrEmpty(x.Text)) ? x.Text : string.Empty,
                                                           ImagePath = (!string.IsNullOrEmpty(x.ImagePath)) ? x.ImagePath : string.Empty,
                                                           DocumentPath = (!string.IsNullOrEmpty(x.DocumentPath)) ? x.DocumentPath : string.Empty,
                                                           Type = x.Type,
                                                           CreatedDate = x.CreatedDate.ToString("dd-MM-yyyy HH:mm:ss"),
                                                           //SenderId = x.SenderId,
                                                           To = (from au in _context.ApplicationUser
                                                                 join ch in _context.ChatHistory
                                                                 on au.Id equals ch.ReceiverId
                                                                 where ch.ReceiverId == au.Id && au.IsDeleted == false
                                                                 select new commonData
                                                                 {
                                                                     Id = au.Id,
                                                                     Name = au.FirstName + " " + au.LastName,
                                                                     ImagePath = au.ProfilePic

                                                                 }).Distinct().FirstOrDefault(),
                                                           From = (from au in _context.ApplicationUser
                                                                   join ch in _context.ChatHistory
                                                                   on au.Id equals ch.SenderId
                                                                   where ch.SenderId == au.Id && au.IsDeleted == false
                                                                   select new commonData
                                                                   {
                                                                       Id = au.Id,
                                                                       Name = au.FirstName + " " + au.LastName,
                                                                       ImagePath = au.ProfilePic
                                                                   }).Distinct().FirstOrDefault()

                                                       }).AsQueryable();

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
                    FilterationResponseModel<ChatHistoryResponseViewModel> obj = new FilterationResponseModel<ChatHistoryResponseViewModel>();
                    obj.totalCount = TotalCount;
                    obj.pageSize = PageSize;
                    obj.currentPage = CurrentPage;
                    obj.totalPages = TotalPages;
                    obj.previousPage = previousPage;
                    obj.nextPage = nextPage;
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
    }
}
