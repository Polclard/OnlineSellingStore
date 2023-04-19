using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineSellingStore.Models
{
    public class Category
    {
        [Key]//Data Annotations
        public int Id { get; set; }
        [Required]
        [DisplayName("Display Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100)]
        public int DisplayOrder { get; set; }

    }
}
