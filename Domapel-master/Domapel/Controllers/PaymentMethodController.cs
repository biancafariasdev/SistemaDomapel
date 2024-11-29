using Microsoft.AspNetCore.Mvc;
using Domapel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domapel.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Authorization;

namespace Domapel.Controllers
{
    [Authorize]
    public class PaymentMethodController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentMethodController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var paymentMethods = await _context.PaymentMethods.ToListAsync();
            return View(paymentMethods);
        }

        public IActionResult AddEdit(int? id)
        {
            var vendors = _context.PaymentMethods
                .Select(v => new
                {
                    v.PaymentMethodId,
                    v.PaymentMethodName
                })
                .ToList();

            var PaymentSelectList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Selecionar vendedor", Value = "" }
    };
            PaymentSelectList.AddRange(vendors.Select(v => new SelectListItem
            {
                Value = v.PaymentMethodId.ToString(),
                Text = v.PaymentMethodName
            }));

            ViewBag.PaymentMethodId = PaymentSelectList;
            var image = "";

            if (id == null)
            {
                return View(new PaymentMethod());
            }
            else
            {
                var paymentMethod = _context.PaymentMethods
                                       .Where(c => c.PaymentMethodId == id)
                                       .Select(c => new
                                       {
                                           c.PaymentMethodId,
                                           c.PaymentMethodName,
                                           
                                       })
                                       .FirstOrDefault();

                if (paymentMethod == null)
                {
                    return NotFound();
                }

                var paymentModel = new PaymentMethod
                {
                    PaymentMethodId = paymentMethod.PaymentMethodId,
                    PaymentMethodName = paymentMethod.PaymentMethodName,
                };

                return View(paymentModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(int? id, PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    _context.Add(paymentMethod);
                }
                else
                {
                    _context.Update(paymentMethod);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentMethod);
        }

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var paymentMethod = await _context.PaymentMethods
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.PaymentMethodId == id);

                    if (paymentMethod == null)
                    {
                        return Json(new { success = false, message = "Forma de pagamento não encontrada." });
                    }

                    _context.PaymentMethods.Remove(paymentMethod);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Json(new { success = true });
                }
                catch (SqlNullValueException ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Erro ao excluir a forma de pagamento: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return Json(new { success = false, message = "Erro ao excluir a forma de pagamento devido a valores nulos: " + ex.Message });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Erro ao excluir o cliente: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return Json(new { success = false, message = "Erro ao excluir a forma de pagamento: " + ex.Message });
                }
            }
        }


    }
}
