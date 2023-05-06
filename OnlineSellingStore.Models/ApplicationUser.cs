using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.Models
{
    [Table("AspNetUser")]
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

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company? Company { get; set; }
    }
}
