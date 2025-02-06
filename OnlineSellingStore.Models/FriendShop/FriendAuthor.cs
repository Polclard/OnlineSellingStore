using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.Models.FriendShop
{
    public class FriendAuthor
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string AuthorName { get; set; }

        public string Bio { get; set; }

    }
}
