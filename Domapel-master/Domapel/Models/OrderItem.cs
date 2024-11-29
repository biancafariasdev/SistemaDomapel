using Domapel.Models;
using System.ComponentModel.DataAnnotations;
namespace Domapel.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string ValueItem { get; set; }
        public Product? Product { get; set; } 

    }
}
