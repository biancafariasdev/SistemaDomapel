using System.ComponentModel.DataAnnotations;
namespace Domapel.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Pix { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
