using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.Models.ViewModels
{
    public class UserVM
    { 
        public ApplicationUser user {  get; set; }
        public IEnumerable<SelectListItem> roles { get; set; }
        public IEnumerable<SelectListItem> companies { get; set; }
        public IEnumerable<SelectListItem> userRoles { get; set; }
    }
}
