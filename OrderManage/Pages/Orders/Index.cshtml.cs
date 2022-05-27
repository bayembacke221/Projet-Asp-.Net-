#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestCom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OrderManage.Data;
using OrderManage.Models;

namespace OrderManage.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly OrderManage.Data.dbContext _context;
        private readonly IConfiguration Configuration;

        public IndexModel(OrderManage.Data.dbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;

        }
        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }


        public IList<Order> Order { get;set; }
        public PaginatedList<Order> Orders { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchOrders { get; set; }
        public async Task OnGetAsync(string sortOrder,
            string currentFilter, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (SearchOrders != null)
            {
                pageIndex = 1;
            }
            else
            {
                SearchOrders = currentFilter;
            }

            CurrentFilter = SearchOrders;


            var orders = from c in _context.Orders
                         select c;
            if (!string.IsNullOrEmpty(SearchOrders))
            {
                orders = orders.Where(s => s.CustomerId.ToLower().Contains(SearchOrders));
            }
            var pageSize = Configuration.GetValue("PageSize", 4);

            Orders = await PaginatedList<Order>.CreateAsync(
                orders.Include(o => o.Customer).Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation).AsNoTracking(), pageIndex ?? 1, pageSize);


        }
    }
}
