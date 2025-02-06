using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.Models.FriendShop
{
    public class FriendBook
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string BookName { get; set; }

        public string BookDescription { get; set; }
        public string BookImage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Rating { get; set; }

        public Guid PublisherId { get; set; }
        public FriendPublisher Publisher { get; set; }

        public List<FriendAuthor> Authors { get; set; } = new();
    }
}
