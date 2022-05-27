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

namespace OrderManage.Pages.Customers
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

        public IList<Customer> Customer { get;set; }
        public PaginatedList<Customer> Customers { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchCustomers { get; set; }


        public async Task OnGetAsync(string sortOrder,
            string currentFilter, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (SearchCustomers != null)
            {
                pageIndex = 1;
            }
            else
            {
                SearchCustomers = currentFilter;
            }

            CurrentFilter = SearchCustomers;

            var customers = from c in _context.Customers
                            select c;
            if (!string.IsNullOrEmpty(SearchCustomers))
            {
                customers = customers.Where(s => s.ContactName.ToLower().Contains(SearchCustomers));
            }
            var pageSize = Configuration.GetValue("PageSize", 4);

            Customers = await PaginatedList<Customer>.CreateAsync(
                customers.AsNoTracking(), pageIndex ?? 1, pageSize);

        }
    }
}
