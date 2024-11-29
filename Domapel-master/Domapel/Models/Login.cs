using System.ComponentModel.DataAnnotations;
namespace Domapel.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Lembre de mim?")]
        public bool RememberMe { get; set; }
    }
}
