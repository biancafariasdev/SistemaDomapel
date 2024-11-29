using System.ComponentModel.DataAnnotations;

namespace Domapel.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
