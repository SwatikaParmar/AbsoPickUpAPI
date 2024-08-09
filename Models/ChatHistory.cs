using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class ChatHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ChatHistoryId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int   MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string AppointmentId { get; set; }
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public string DocumentPath { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
