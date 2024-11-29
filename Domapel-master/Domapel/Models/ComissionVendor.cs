using Domapel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domapel.Models
{
    public class ComissionVendor
    {
        public int? SelectedVendorId { get; set; }
        public List<SelectListItem> Vendors { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public decimal TotalCommissions { get; set; }
        public decimal TotalCommission { get; set; }
        public SelectList VendorList { get; set; }
        public int? SelectedMonth { get; set; }  
        public int? OrderRangeStart { get; set; }  
        public int? OrderRangeEnd { get; set; }  

    }
}
