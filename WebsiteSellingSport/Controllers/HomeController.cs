using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;
using WebsiteSellingSport.Repostitory;
using X.PagedList;

namespace WebsiteSellingSport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ProductRepository _productRepo;
        CategoryRepository _categoryRepo;
      

        public HomeController(ILogger<HomeController> logger, ProductRepository productRepository, CategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepo = productRepository;
            _categoryRepo = categoryRepository;
   
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                ProductNews = _productRepo.GetListProductNew(),
                Products = _productRepo.GetListProductNew().OrderBy(c=>c.ProductName).ToList(),
            };


            return View(viewModel);
        }
        [HttpGet]
        public JsonResult GetDataFooter()
        {
            var lstCategory = _categoryRepo.GetAllList().Where(c => c.Status == 1).Take(5).ToList();
           return Json(lstCategory, new System.Text.Json.JsonSerializerOptions());
        }
        //[HttpGet]
        //public IActionResult Shop(string search, decimal? price, decimal? minPrice, decimal? maxPrice, int? categoryid, int? categoryProductid, int? page)
        //{

        //    var lstProduct = _productRepo.GetListProductNew();
        //     //_productRepo.GetListProductShop( search, price,  minPrice, maxPrice, categoryid, categoryProductid);       

        //    // Số lượng phần tử trên một trang
        //    int pageSize = 5;

        //    // Trang hiện tại (mặc định là trang 1)
        //    int pageNumber = page ?? 1;

        //    // Phân trang danh sách sản phẩm
        //    var pagedProducts = lstProduct.ToPagedList(pageNumber, pageSize);


        //      return PartialView("_EmployeeListPartial", pagedProducts);
        //}



        public IActionResult Contact()
        {
           

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
