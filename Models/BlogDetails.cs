using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Models
{
    public class BlogDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public string Id { get; set; }

        [MaxLength(256)]
        [Required]
        public string UserId { get; set; }
       
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string BlogImagePath { get; set; } 
        public DateTime CreatedDate { get; set; }
        public bool IsAdminApproved { get; set; }
        public int? BlogStatus { get; set; }
    }
}
