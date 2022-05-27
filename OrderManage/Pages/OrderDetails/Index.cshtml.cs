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

namespace OrderManage.Pages.OrderDetails
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

        public IList<OrderDetail> OrderDetail { get;set; }
        public PaginatedList<OrderDetail> OrderDetails { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchOrderDetails { get; set; }

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, int? pageIndex)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (SearchOrderDetails != null)
            {
                pageIndex = 1;
            }
            else
            {
                SearchOrderDetails = currentFilter;
            }

            CurrentFilter = SearchOrderDetails;
            var orderDetails = from c in _context.OrderDetails
                           select c;
            if (!string.IsNullOrEmpty(SearchOrderDetails))
            {
                orderDetails = orderDetails.Where(s => s.Product.ProductName.ToLower().Contains(SearchOrderDetails));
            }
            var pageSize = Configuration.GetValue("PageSize", 4);
            OrderDetails = await PaginatedList<OrderDetail>.CreateAsync(
               orderDetails.Include(o => o.Order)
                .Include(o => o.Product).AsNoTracking(), pageIndex ?? 1, pageSize);



            
        }
    }
}
