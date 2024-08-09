using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IToDoNotesService
    {
        bool AddToDoNotes(DoctorToDoNotes _toDoListModel);
        FilterationResponseModel<GetToDoListViewModel> GetToDoNotes(FilterationListViewModel model , string UserId);
        Task<bool> UpdateToDoNotes(UpdateToDoListViewModel model , string userId);
        Task<bool> DeleteToDoNotes(long Id, string userId);

    }
}
