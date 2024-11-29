using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domapel.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public bool Logo { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }
        public string? CorporateName { get; set; }
        public string? TradeName { get; set; }
        public string? CorporateAdress { get; set; }
        public string? CorporateAdressNumber { get; set; }
        public string? CorporateAdressBairro { get; set; }
        public string? CorporateCNPJ { get; set; }
        public string? CorporateCity { get; set; }
        public string CorporateState { get; set; }
        public string? CorporateIE { get; set; }
        public string? CorporatePhone { get; set; }

        public string? Observations { get; set; }
        public string? VendorName { get; set; }

        [Column(TypeName = "money")]
        public decimal CommissionValue { get; set; }
        public string CommissionValuePercent { get; set; } 
        public string Discount { get; set; }
        public string FinalValue { get; set; }
        public string PaymentMethodName { get; set; }
        public int ItemCount { get; set; }
        public int PaymentMethodId { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
