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

namespace OrderManage.Pages.Products
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

        public IList<Product> Product { get; set; }

        public PaginatedList<Product> Products { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchProducts { get; set; }

        public async Task OnGetAsync(string sortOrder,
            string currentFilter, int? pageIndex)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (SearchProducts != null)
            {
                pageIndex = 1;
            }
            else
            {
                SearchProducts = currentFilter;
            }

            CurrentFilter = SearchProducts;
            var products = from c in _context.Products
                           select c;
            if (!string.IsNullOrEmpty(SearchProducts))
            {
                products = products.Where(s => s.ProductName.ToLower().Contains(SearchProducts));
            }
            var pageSize = Configuration.GetValue("PageSize", 4);
            Products = await PaginatedList<Product>.CreateAsync(
               products.Include(p => p.Category)
                .Include(p => p.Supplier).AsNoTracking(), pageIndex ?? 1, pageSize);

        }
       
    }
}
