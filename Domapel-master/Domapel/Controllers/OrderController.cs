using Domapel.Data;
using Domapel.Models;
using Domapel.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Domapel.Models.Order;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;


[Authorize]

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? orderIdFilter, int? selectedCorporateNameId, string corporateNameFilter, int? selectedVendorId, DateTime? dateFilter, int? page)
    {
        var ordersQuery = _context.Orders
            .Include(o => o.Customer)
            .ThenInclude(c => c.Vendor)
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (selectedCorporateNameId.HasValue)
        {
            ordersQuery = ordersQuery.Where(c => c.CustomerId == selectedCorporateNameId);
        }
        if (orderIdFilter.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.OrderId == orderIdFilter);
        }
        if (!string.IsNullOrWhiteSpace(corporateNameFilter))
        {
            ordersQuery = ordersQuery.Where(o => o.Customer.CorporateName.Contains(corporateNameFilter));
        }
        if (selectedVendorId.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.Customer.VendorId == selectedVendorId);
        }
        if (dateFilter.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.OrderDate.Date == dateFilter.Value.Date);
        }

        var corporateNames = await _context.Customers
                                           .Select(c => new { c.CustomerId, c.CorporateName })
                                           .Distinct()
                                           .OrderBy(c => c.CorporateName)
                                           .ToListAsync();

        var vendors = await _context.Vendors
            .Select(v => new { v.VendorId, v.Name })
            .Distinct()
            .OrderBy(v => v.Name)
            .ToListAsync();

        ViewData["Vendors"] = new SelectList(vendors, "VendorId", "Name", selectedVendorId);
        ViewData["CorporateNames"] = new SelectList(corporateNames, "CustomerId", "CorporateName", selectedCorporateNameId);
        ViewData["orderIdFilter"] = orderIdFilter;
        ViewData["selectedCorporateNameId"] = selectedCorporateNameId;
        ViewData["corporateNameFilter"] = corporateNameFilter;
        ViewData["selectedVendorId"] = selectedVendorId;
        ViewData["dateFilter"] = dateFilter;

        int pageSize = 10; 
        int pageNumber = page ?? 1; 

        var orders = ordersQuery
            .OrderByDescending(o => o.OrderId)
            .Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                CorporateName = o.Customer.CorporateName,
                VendorName = o.Customer.Vendor.Name,
                CommissionValue = o.CommissionValue,
                Discount = o.Discount,
                FinalValue = o.FinalValue,
                ItemCount = o.OrderItems.Count
            })
            .ToPagedList(pageNumber, pageSize); 

        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Customer)
                .ThenInclude(c => c.Vendor)
            .Include(o => o.PaymentMethod)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        var customer = order.Customer;
        if (customer == null)
        {
            return NotFound();
        }

        string imagePath = customer.ImagePath?.Replace("\\", "/") ?? string.Empty;
        ViewBag.ImagePath = Url.Content($"~/{imagePath}");

        var model = new OrderViewModel
        {
            OrderId = order.OrderId,
            Logo = order.Logo,
            OrderDate = order.OrderDate,
            CustomerId = order.CustomerId,
            CorporateName = customer.CorporateName,
            TradeName = customer.TradeName,
            CorporateAdress = customer.Address,
            CorporateAdressNumber = customer.Number,
            CorporateAdressBairro = customer.Neighborhood,
            CorporateCity = customer.City,
            CorporateState = customer.State,
            CorporateCNPJ = customer.CNPJ,
            CorporateIE = customer.StateRegistration,
            CorporatePhone = customer.Phone,
            Observations = order.Observations,
            VendorName = customer.Vendor?.Name,
            CommissionValue = order.CommissionValue,
            Discount = order.Discount?.ToString() ?? "0",
            FinalValue = order.FinalValue?.ToString() ?? "0",
            PaymentMethodName = order.PaymentMethod.PaymentMethodName,
            ItemCount = order.OrderItems.Count,
            OrderItems = order.OrderItems.Select(oi => new OrderItem
            {
                OrderItemId = oi.OrderItemId,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                ValueItem = oi.ValueItem,
                Product = new Product
                {
                    Name = oi.Product.Name
                }
            }).ToList()
        };

        return View("Details", model);
        
    }
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Erro ao excluir o pedido." });
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerInfo(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Vendor)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null || customer.Vendor == null)
        {
            return Json(new { success = false, message = "Vendedor não encontrado." });
        }

        return Json(new { success = true, vendorName = customer.Vendor.Name, HasLogo = customer.ImagePath });
    }

    public async Task<IActionResult> AddEdit(int? id)
    {
        var customersQuery = _context.Customers.AsQueryable();

        try
        {
            var customers = await customersQuery
                .Select(c => new
                {
                    c.CustomerId,
                    Description = $"{c.CorporateName} - CNPJ: {c.CNPJ}",
                    Complement = c.Complement ?? string.Empty,
                    c.VendorId,
                    c.ImagePath
                })
                .ToListAsync();

            var products = await _context.Products
                .Select(p => new
                {
                    p.ProductId,
                    p.Name
                })
                .ToListAsync();

            var paymentMethod = await _context.PaymentMethods
                .Select(p => new
                {
                    p.PaymentMethodId,
                    p.PaymentMethodName
                })
                .ToListAsync();

            ViewBag.PaymentMethods = new SelectList(paymentMethod, "PaymentMethodId", "PaymentMethodName");
            ViewBag.Customers = new SelectList(customers, "CustomerId", "Description");
            ViewBag.Products = new SelectList(products, "ProductId", "Name");

            OrderViewModel model = new OrderViewModel();
            string vendorName = "";

            if (id.HasValue)
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                        .ThenInclude(c => c.Vendor)
                    .FirstOrDefaultAsync(o => o.OrderId == id.Value);

                if (order == null) return NotFound();

                vendorName = order.Customer.Vendor?.Name;

                bool hasLogo = !string.IsNullOrEmpty(order.Customer.ImagePath);


                model = new OrderViewModel
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    CommissionValue = order.CommissionValue,
                    CommissionValuePercent = order.CommissionValuePercent,
                    Discount = order.Discount,
                    Logo = hasLogo,
                    FinalValue = order.FinalValue,
                    PaymentMethodId = order.PaymentMethodId,
                    Observations = order.Observations,
                };

                var orderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product != null ? oi.Product.Name : "Produto não encontrado",
                    Quantity = oi.Quantity,
                    ValueItem = oi.ValueItem
                }).ToList();

                ViewBag.OrderItemsJson = JsonConvert.SerializeObject(orderItems);
            }
            else
            {
                ViewBag.OrderItemsJson = "[]";
            }

            ViewBag.VendorName = vendorName;

            return View(model);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao carregar dados: {ex.Message}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEdit(OrderViewModel model)
    {
        try
        {
            ModelState.Remove("CorporateName");
            ModelState.Remove("VendorName");
            ModelState.Remove("HasInvoice");
            ModelState.Remove("VendorId");
            ModelState.Remove("Product");
            ModelState.Remove("CommissionValue");
            ModelState.Remove("CommissionValuePercent");
            ModelState.Remove("Discount");
            ModelState.Remove("CorporateState");
            ModelState.Remove("PaymentMethodName");

            for (int i = 0; i < model.OrderItems.Count; i++)
            {
                ModelState.Remove($"OrderItems[{i}].Product");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await GetCustomerSelectListAsync();
                ViewBag.PaymentMethod = await GetPaymentMethodSelectListAsync();
                ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
                return View(model);
            }

            Order order = model.OrderId == 0 ? new Order() : await _context.Orders.FindAsync(model.OrderId);

            if (order == null && model.OrderId != 0)
            {
                return NotFound();
            }

            order.CustomerId = model.CustomerId;
            order.OrderDate = model.OrderDate;
            order.CommissionValue = model.CommissionValue;
            order.CommissionValuePercent = model.CommissionValuePercent ?? "0";
            order.Discount = model.Discount ?? "0";
            order.FinalValue = model.FinalValue;
            order.PaymentMethodId = model.PaymentMethodId;
            order.Observations = string.IsNullOrWhiteSpace(model.Observations) ? "sem observações" : model.Observations;
            order.Logo = model.Logo;

            if (model.OrderId == 0)
            {
                _context.Orders.Add(order);
            }

            await _context.SaveChangesAsync();

            if (model.OrderId != 0)
            {
                var existingItems = _context.OrderItems.Where(oi => oi.OrderId == order.OrderId);
                _context.OrderItems.RemoveRange(existingItems);
            }

            foreach (var item in model.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ValueItem = item.ValueItem
                };
                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro no método AddEdit: {ex.Message}");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetVendorByCustomer(int customerId)
    {
        var vendorName = await _context.Customers
            .Where(c => c.CustomerId == customerId)
            .Select(c => c.Vendor.Name)
            .FirstOrDefaultAsync();

        return Json(new { vendorName = vendorName ?? "" });
    }

    private async Task<SelectList> GetCustomerSelectListAsync()
    {
        var customers = await _context.Customers
            .OrderBy(c => c.CorporateName)
            .Select(c => new SelectListItem
            {
                Value = c.CustomerId.ToString(),
                Text = $"{c.CorporateName} - CNPJ: {c.CNPJ}"
            })
            .ToListAsync();

        return new SelectList(customers, "Value", "Text");
    }

    private async Task<SelectList> GetPaymentMethodSelectListAsync()
    {
        var paymentMethod = await _context.PaymentMethods
            .OrderBy(c => c.PaymentMethodName)
            .Select(c => new SelectListItem
            {
                Value = c.PaymentMethodId.ToString(),
                Text = c.PaymentMethodName
            })
            .ToListAsync();

        return new SelectList(paymentMethod, "Value", "Text");
    }

    private OrderViewModel ConvertToViewModel(Order order)
    {
        var viewModel = new OrderViewModel
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            CommissionValue = order.CommissionValue,
            Discount = order.Discount,
            FinalValue = order.FinalValue,
            Observations = order.Observations,
            OrderItems = order.OrderItems.Select(oi => new OrderItem
            {
                OrderItemId = oi.OrderItemId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                ValueItem = oi.ValueItem
            }).ToList()
        };

        return viewModel;
    }


}
