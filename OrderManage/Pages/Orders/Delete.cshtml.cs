#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OrderManage.Data;
using OrderManage.Models;

namespace OrderManage.Pages.Orders
{
    public class DeleteModel : PageModel
    {
        private readonly OrderManage.Data.dbContext _context;

        public DeleteModel(OrderManage.Data.dbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o=>o.OrderDetails)
                .Include(o => o.ShipViaNavigation).FirstOrDefaultAsync(m => m.OrderId == id);

            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var orderFind = (from o  in _context.OrderDetails
                             where o.OrderId == id
                             select o).ToList() ;
            Order =  await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                 .Include(o => o.OrderDetails)
                .Include(o => o.ShipViaNavigation).FirstOrDefaultAsync(m => m.OrderId == id);


            if (Order != null)
            {
               foreach(var order in orderFind)
                {
                    _context.OrderDetails.Remove(order);
                }
                _context.Orders.Remove(Order);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
