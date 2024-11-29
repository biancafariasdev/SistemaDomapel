using System.ComponentModel.DataAnnotations;
namespace Domapel.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}


