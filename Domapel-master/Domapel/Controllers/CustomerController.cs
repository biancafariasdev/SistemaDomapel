using Microsoft.AspNetCore.Mvc;
using Domapel.Data;
using Domapel.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domapel.Models.ViewModels;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? selectedCorporateNameId, int? invoiceFilter, int? selectedVendorId, string cnpjFilter)
    {
        var customersQuery = _context.Customers.AsQueryable();
        
        if (selectedCorporateNameId.HasValue)
        {
            customersQuery = customersQuery.Where(c => c.CustomerId == selectedCorporateNameId);
        }
        if (invoiceFilter.HasValue)
        {
            customersQuery = customersQuery.Where(o => o.HasInvoice == invoiceFilter);
        }
        if (selectedVendorId.HasValue)
        {
            customersQuery = customersQuery.Where(c => c.VendorId == selectedVendorId);
        }

        if (!string.IsNullOrWhiteSpace(cnpjFilter))
        {
            var cleanCnpjFilter = new string(cnpjFilter.Where(char.IsDigit).ToArray());
            customersQuery = customersQuery.Where(c => c.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "").Contains(cleanCnpjFilter));
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

        ViewData["CorporateNames"] = new SelectList(corporateNames, "CustomerId", "CorporateName", selectedCorporateNameId);
        ViewData["Vendors"] = new SelectList(vendors, "VendorId", "Name", selectedVendorId);

        var customers = await customersQuery
            .Select(c => new
            {
                c.CustomerId,
                c.CorporateName,
                c.TradeName,
                c.CNPJ,
                c.StateRegistration,
                c.Address,
                c.Phone,
                c.VendorId,
                c.Email,
                c.CEP,
                Complement = c.Complement ?? string.Empty,
                c.Neighborhood,
                c.Number,
                c.State,
                c.ImagePath,
                Vendor = new
                {
                    c.Vendor.VendorId,
                    c.Vendor.Name
                }
            })
            .ToListAsync();

        var customerList = customers.Select(c => new Customer
        {
            CustomerId = c.CustomerId,
            CorporateName = c.CorporateName,
            TradeName = c.TradeName,
            CNPJ = c.CNPJ,
            StateRegistration = c.StateRegistration,
            Address = c.Address,
            Phone = c.Phone,
            VendorId = c.VendorId,
            Email = c.Email,
            CEP = c.CEP,
            Complement = c.Complement,
            Neighborhood = c.Neighborhood,
            Number = c.Number,
            State = c.State,
            ImagePath = c.ImagePath,
            Vendor = new Vendor
            {
                VendorId = c.Vendor.VendorId,
                Name = c.Vendor.Name
            }
        }).ToList();

        return View(customerList);
    }

    public IActionResult AddEdit(int? id)
    {
        var vendors = _context.Vendors
            .Select(v => new
            {
                v.VendorId,
                v.Name
            })
            .ToList();

        var vendorSelectList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Selecionar vendedor", Value = "" }
    };
        vendorSelectList.AddRange(vendors.Select(v => new SelectListItem
        {
            Value = v.VendorId.ToString(),
            Text = v.Name
        }));

        ViewBag.VendorId = vendorSelectList;
        var image = "";

        if (id == null)
        {
            return View(new Customer());
        }
        else
        {
            var customer = _context.Customers
                                   .Where(c => c.CustomerId == id)
                                   .Select(c => new
                                   {
                                       c.CustomerId,
                                       c.CorporateName,
                                       c.TradeName,
                                       c.CNPJ,
                                       c.StateRegistration,
                                       c.Address,
                                       c.Phone,
                                       c.VendorId,
                                       c.Email,
                                       c.CEP,
                                       Complement = c.Complement ?? string.Empty,
                                       c.Neighborhood,
                                       c.Number,
                                       c.State,
                                       c.City,
                                       c.ImagePath,
                                       c.HasInvoice,
                                       Vendor = new
                                       {
                                           c.Vendor.VendorId,
                                           c.Vendor.Name
                                       }
                                   })
                                   .FirstOrDefault();

            if (customer == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(customer.ImagePath))
            {
                var imagePath = customer.ImagePath.Replace("\\", "/");
                ViewBag.ImagePath = Url.Content($"~/{imagePath}");
                image = imagePath;
            }

            var customerModel = new Customer
            {
                CustomerId = customer.CustomerId,
                CorporateName = customer.CorporateName,
                TradeName = customer.TradeName,
                CNPJ = customer.CNPJ,
                StateRegistration = customer.StateRegistration,
                Address = customer.Address,
                Phone = customer.Phone,
                VendorId = customer.VendorId,
                Email = customer.Email,
                CEP = customer.CEP,
                Complement = customer.Complement,
                Neighborhood = customer.Neighborhood,
                Number = customer.Number,
                State = customer.State,
                City = customer.City,
                ImagePath = image,
                HasInvoice = customer.HasInvoice,
                Vendor = new Vendor
                {
                    VendorId = customer.Vendor.VendorId,
                    Name = customer.Vendor.Name
                }
            };

            return View(customerModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEdit(int? id, Customer customer, IFormFile logoImage)
    {
        ViewBag.VendorId = new SelectList(_context.Vendors, "VendorId", "Name");
        ModelState.Remove("Vendor");
        ModelState.Remove("ImagePath");
        ModelState.Remove("logoImage");
        ModelState.Remove("Complement");
        ModelState.Remove("Number");

        if (logoImage != null && logoImage.Length > 0)
        {
            var fileName = Path.GetFileName(logoImage.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/customer", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logoImage.CopyToAsync(stream);
            }

            customer.ImagePath = $"images/customer/{fileName}";
        }
        else
        {
            customer.ImagePath = "";
        }

        if (string.IsNullOrEmpty(customer.Number))
        {
            customer.Number = "0";
        }

        if (ModelState.IsValid)
        {
            if (id == null)
            {
                _context.Add(customer);
            }
            else
            {
                _context.Update(customer);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(customer);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var customer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CustomerId == id);

                if (customer == null)
                {
                    return Json(new { success = false, message = "Cliente não encontrado." });
                }

                if (customer.Complement == null)
                {
                    customer.Complement = ""; 
                }

                var orders = await _context.Orders
                    .Where(o => o.CustomerId == id)
                    .Include(o => o.OrderItems)
                    .ToListAsync();

                if (orders.Any())
                {
                    foreach (var order in orders)
                    {
                        if (order.OrderItems != null && order.OrderItems.Any())
                        {
                            _context.OrderItems.RemoveRange(order.OrderItems);
                        }

                        _context.Orders.Remove(order);
                    }
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Json(new { success = true });
            }
            catch (SqlNullValueException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Erro ao excluir o cliente: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { success = false, message = "Erro ao excluir o cliente devido a valores nulos: " + ex.Message });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Erro ao excluir o cliente: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { success = false, message = "Erro ao excluir o cliente: " + ex.Message });
            }
        }
    }



}
