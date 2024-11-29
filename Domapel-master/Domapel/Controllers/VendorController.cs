using Domapel.Data;
using Domapel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class VendorController : Controller
{
    private readonly ApplicationDbContext _context;

    public VendorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var vendors = _context.Vendors.ToList();
        return View(vendors);
    }

    public IActionResult AddEdit(int? id)
    {
        if (id == null)
            return View(new Vendor());
        else
        {
            var vendor = _context.Vendors.Find(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEdit(int? id, Vendor vendor)
    {
        ModelState.Remove("Customers");

        if (ModelState.IsValid)
        {
            if (id == null)
            {
                _context.Add(vendor);
            }
            else
            {
                try
                {
                    _context.Update(vendor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Vendors.Any(e => e.VendorId == vendor.VendorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(vendor);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor != null)
        {
            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Erro ao excluir o vendedor." });
    }




}
