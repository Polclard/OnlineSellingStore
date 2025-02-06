using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.Models.FriendShop
{
    public class FriendPublisher
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string PublisherName { get; set; }

        public string Address { get; set; }
        public string Contact { get; set; }
    }
}
