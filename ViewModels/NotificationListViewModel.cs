using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class NotificationListViewModel
    {
        public string Id { get; set; }

        [MaxLength(450)]
        [Required]
        public string FromUser { get; set; } 
        public string FromUserName { get; set; } 
        public string FromUserProfilePic { get; set; }
        public string FromUserGender { get; set; }

        [MaxLength(450)]
        [Required]
        public string ToUser { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Purpose { get; set; }
        public int TimeInMinutes { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRead { get; set; }
        public long TimeSlotId { get; set; }
        public string Date { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public string PurposeId { get; set; }
        public int AppointmentStatus { get; set; }

    }
}
