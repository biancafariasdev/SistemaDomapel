using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domapel.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public bool Logo { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Column(TypeName = "money")]
        public decimal CommissionValue { get; set; }
        public string CommissionValuePercent { get; set; }
        public string Discount { get; set; }
        public string FinalValue { get; set; } 
        public int PaymentMethodId { get; set; } 
        public string Observations { get; set; } 
        public class OrderItemViewModel
        {
            public string Name { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string ValueItem { get; set; }
        }
        public PaymentMethod PaymentMethod { get; set; }

    }
}
