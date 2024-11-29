using Domapel.Data;
using Domapel.Models;
using Domapel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Domapel.Controllers
{
    public class ComissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new ComissionVendor
            {
                Vendors = _context.Vendors.Select(v => new SelectListItem
                {
                    Value = v.VendorId.ToString(), 
                    Text = v.Name
                }).ToList()
            };

            return View(model);
        }
        public IActionResult GetCommissionsByVendorAndOrderRange(int SelectedVendorId, int? OrderRangeStart, int? OrderRangeEnd)
        {
            var orders = _context.Orders
                                 .Where(o => o.Customer.VendorId == SelectedVendorId &&
                                             (!OrderRangeStart.HasValue || o.OrderId >= OrderRangeStart) &&
                                             (!OrderRangeEnd.HasValue || o.OrderId <= OrderRangeEnd))
                                 .Include(o => o.Customer)
                                 .ThenInclude(c => c.Vendor)
                                 .ToList();

            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                FinalValue = !string.IsNullOrEmpty(o.FinalValue) ? o.FinalValue : "0.00",
                CorporateName = o.Customer?.CorporateName ?? "N/A",
                CommissionValue = o.CommissionValue,
                CommissionValuePercent = !string.IsNullOrEmpty(o.CommissionValuePercent) ? o.CommissionValuePercent : "0.00",
                VendorName = o.Customer?.Vendor?.Name ?? "Desconhecido"
            }).ToList();

            var totalCommissions = orders.Sum(o => o.CommissionValue);
            var model = new ComissionVendor
            {
                Orders = orderViewModels,
                TotalCommissions = totalCommissions,
                Vendors = _context.Vendors.Select(v => new SelectListItem
                {
                    Value = v.VendorId.ToString(),
                    Text = v.Name
                }).ToList() 
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult GetCommissionsByVendor(ComissionVendor model)
        {
            if (model.SelectedVendorId.HasValue)
            {
                var ordersQuery = _context.Orders
                    .Include(o => o.Customer)
                    .ThenInclude(c => c.Vendor)
                    .Where(o => o.Customer.VendorId == model.SelectedVendorId.Value);

                if (model.SelectedMonth.HasValue)
                {
                    ordersQuery = ordersQuery.Where(o => o.OrderDate.Month == model.SelectedMonth.Value);
                }

                var orders = ordersQuery.ToList();

                model.Orders = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    VendorName = o.Customer.Vendor?.Name ?? "Sem vendedor",
                    CorporateName = o.Customer?.CorporateName ?? "Sem cliente",
                    FinalValue = o.FinalValue,
                    CommissionValue = o.CommissionValue,
                    CommissionValuePercent = o.CommissionValuePercent
                }).ToList();

                model.TotalCommissions = orders.Sum(o => o.CommissionValue);
            }

            model.Vendors = _context.Vendors.Select(v => new SelectListItem
            {
                Value = v.VendorId.ToString(),
                Text = v.Name
            }).ToList();

            return View("Index", model);
        }


    }
}
