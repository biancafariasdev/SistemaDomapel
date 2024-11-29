using System.ComponentModel.DataAnnotations;
using System.Numerics;
namespace Domapel.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string ImagePath { get; set; }
        public string CorporateName { get; set; } // Razão Social
        public int HasInvoice { get; set; }
        public string TradeName { get; set; } // Nome Fantasia
        [Required]
        public string CNPJ { get; set; }

        public string StateRegistration { get; set; } // Inscrição Estadual

        [Required]
        public string Address { get; set; }
        public string CEP { get; set; }
        public string? Number { get; set; }
        public string Complement { get; set; } 
        public string Neighborhood { get; set; }
        public string State { get; set; }
        public string City { get; set; }

        [Required]
        public string Phone { get; set; }

        public int VendorId { get; set; }

        public string Email { get; set; }
        public Vendor Vendor { get; set; }
    }
}
