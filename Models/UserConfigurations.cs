using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class UserConfigurations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; }
        public bool PushNotificationsStatus { get; set; }
        public bool EmailNotificationStatus { get; set; }
        public bool SMSNotificationStatus { get; set; }
        public int AppLanguageId { get; set; }
    }
}
