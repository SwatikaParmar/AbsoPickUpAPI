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
    public class RatingService : IRatingService
    {
        private ApplicationDbContext _context;
        public RatingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddUpdateUserRating(UserRating _userRating)
        {
            try
            {
                var model = _context.UserRating.Where(x => x.AppointmentId == _userRating.AppointmentId).FirstOrDefault();
                if (model != null)
                {
                    model.Rating = _userRating.Rating;
                    model.Comments = _userRating.Comments;
                    model.UpdateDate = DateTime.UtcNow;
                    _context.UserRating.Update(model);
                }
                else
                {
                    _context.Add(_userRating);
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public GetRatingViewModel GetUserRating(string AppointmentId)
        {
            try
            {
                var _ratingDetails = _context.UserRating.Where(x => x.AppointmentId == AppointmentId).FirstOrDefault();
                GetRatingViewModel obj = new GetRatingViewModel();
                if (_ratingDetails != null)
                {
                    obj.UserRating = _ratingDetails.Rating;
                    
                }
                else
                {
                    obj.UserRating = 0;
                }
                return obj;
               
            }
            catch(Exception)
            {
                throw;
            }
           
        }

        public List<GetUserReviewViewModel> GetUserReviews(string DoctorId)
        {
            try
            {
                var _reviewsList = (from u in _context.ApplicationUser
                                    join ur in _context.UserRating
                                    on u.Id equals ur.PatientId
                                    where ur.DoctorId == DoctorId
                                    && (u.IsDeleted == false) && (ur.Comments != " ")
                                    select new GetUserReviewViewModel
                                    {
                                        PatientId = ur.PatientId,
                                        Name = u.FirstName + " " + u.LastName,
                                        ProfilePic = u.ProfilePic,
                                        Reviews = ur.Comments,
                                        Date = ur.CreatedDate.ToString("dd-MM-yyyy HH:mm:ss")
                                        
                                    }).ToList();
                return _reviewsList;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
