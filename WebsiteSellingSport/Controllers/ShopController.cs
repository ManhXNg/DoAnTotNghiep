using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;
using WebsiteSellingSport.Repostitory;
using X.PagedList;

namespace WebsiteSellingSport.Controllers
{
    public class ShopController : Controller
    {
        CategoryRepository _categoryRepo;
        CategoryProductRepository _categoryProductRepo;
        ProductRepository _productRepo;
        ProductImageRepository _productImageRepo;
        private readonly QLBHQAContext _context;
        public ShopController(ProductRepository productRepository, CategoryRepository categoryRepository, CategoryProductRepository categoryProductRepository, ProductImageRepository productImageRepo, QLBHQAContext context)
        {
            _categoryRepo = categoryRepository;
            _categoryProductRepo = categoryProductRepository;
            _productRepo = productRepository;
            _productImageRepo = productImageRepo;
            _context = context;
        }
      
        public IActionResult ProductDetail(long? id)
        {
            Product product = _productRepo.GetByID(id.Value);
            if (product == null)
            {
                RedirectToAction("Index");
            }
            //Danh mục sản phẩm
            var CategoryPrd = _context.CategoryProducts.Where(c => c.CategoryProductId == product.CategoryProductId && c.Status == 1).FirstOrDefault();   
            
            //Damnh mục
            var Category = _categoryRepo.GetAllList().Where(c => c.CategoryId == CategoryPrd.CategoryId && c.Status == 1).FirstOrDefault();
            //Ảnh sản phẩm
            var productImg = _context.ProductImages.Where(c => c.ProductId == product.ProductId).ToList();
        
            //Sản phẩm liên quan
            var productlienquan = _productRepo.GetListProductNew().Where(c => c.CategoryProductID == CategoryPrd.CategoryProductId && c.ProductId != id).ToList();

            var lstColor = (from pcs in _context.ProductColorSizes
                            join cl in _context.Colors on pcs.ColorId equals cl.ColorId
                            where pcs.ProductId == id
                            group pcs by cl.ColorId into g
                            select new Color
                            {
                                ColorId = g.Key,
                                ColorName = _context.Colors.Where(c => c.ColorId == g.Key).Select(c => c.ColorName).Distinct().FirstOrDefault(),
                                ColorCode = _context.Colors.Where(c => c.ColorId == g.Key).Select(c => c.ColorCode).FirstOrDefault()
                            }).ToList();
            var lstSize = (from pcs in _context.ProductColorSizes
                           join sz in _context.Sizes on pcs.SizeId equals sz.SizeId
                           where pcs.ProductId == id
                           group pcs by sz.SizeId into g
                           select new Size { 
                            SizeId = g.Key,
                            SizeName = _context.Sizes.Where(c => c.SizeId == g.Key).Select(c => c.SizeName).Distinct().FirstOrDefault(),
                            SizeCode = _context.Sizes.Where(c => c.SizeId == g.Key).Select(c => c.SizeCode).Distinct().FirstOrDefault(),

                        }).ToList();



            var viewModel = new HomeProductDetailView
            {
                Product = product,
                CategoryProduct = CategoryPrd,
                Category = Category,
                ProductImages = productImg,
                ProductViews = productlienquan,
                Colors = lstColor,
                Sizes = lstSize,
              
            };
            return View(viewModel);
        }
        public JsonResult GetTotalProduct(long id, int sizeId,int colorId)
        {         
            return Json(_productRepo.GetTotalProduct(id, sizeId, colorId));
        }
       
        public IActionResult Index(int? page,string search,decimal? minprice, decimal? maxprice, long? categoryid, long? categoryProductid,int? order)
        {
            ViewBag.Category = _categoryRepo.GetAllList().Where(c=>c.Status == 1);
            ViewBag.CategoryProduct = _categoryProductRepo.GetAllList().Where(c => c.Status == 1);
            ViewBag.Search = search;
            if (minprice == null || maxprice == null)
            {
                minprice = 0;
                maxprice = 999999;
            }
            ViewBag.MinPrice = minprice;
            ViewBag.MaxPrice = maxprice;
            ViewBag.CategoryId = categoryid;
            if (categoryProductid != null )
            {
                ViewBag.CategoryProductName = _categoryProductRepo.GetByID(categoryProductid.Value).CategoryProductName;
            }
            if (categoryid != null)
            {
                ViewBag.CategoryName = _categoryRepo.GetByID(categoryid.Value).CategoryName;
            }
            ViewBag.CategoryProductId = categoryProductid;
            ViewBag.Order = order;
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            
            var products = _productRepo.GetListProductShop(search, minprice,maxprice,categoryid,categoryProductid, order);
            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }
    }
}
