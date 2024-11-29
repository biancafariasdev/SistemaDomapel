using System.ComponentModel.DataAnnotations;
namespace Domapel.Models
{
    public class Register
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "A senha e a confirmação da senha não são iguais.")]
        public string ConfirmPassword { get; set; }
    }
}
