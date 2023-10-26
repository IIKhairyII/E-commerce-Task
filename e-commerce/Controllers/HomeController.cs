using e_commerce.Data;
using e_commerce.Models;
using e_commerce.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace e_commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 6;
            pageNumber = (pageNumber == null) ? 1 : pageNumber;
            var products = PaginatedList<Products>.CreateAsync(_context.Products.OrderBy(x => x.Key).ToList(),
                pageNumber ?? 1, pageSize);
            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}