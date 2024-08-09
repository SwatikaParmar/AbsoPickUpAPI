using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class UserAddressViewModel
    {
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [Required]
        public int NationalityId { get; set; }
        [MaxLength(200)]
        public string CurrentAddress { get; set; }
    }
}
