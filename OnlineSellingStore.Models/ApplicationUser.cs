using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public string ? StreetAddress { get; set; }
        [PersonalData]
        public string ? City { get; set; }
        [PersonalData]
        public string ? State { get; set; }
        [PersonalData]
        public string ? PostalCode { get; set; }
    }
}
