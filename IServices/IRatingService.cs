
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface IRatingService
    {
        bool AddUpdateUserRating(UserRating _userRating);
        GetRatingViewModel GetUserRating(string appointmentId);
        List<GetUserReviewViewModel> GetUserReviews(string DoctorId);
    }
}
