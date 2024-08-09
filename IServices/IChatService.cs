using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IChatService
    {
        public int SendMessage(ChatHistory chatHistory);
        FilterationResponseModel<ChatHistoryResponseViewModel> GetChatHistory(FilterationListViewModel model, string userId, string senderId, string appointmentId);
        
    }
}
