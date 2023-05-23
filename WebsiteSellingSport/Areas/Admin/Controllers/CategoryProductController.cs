using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.Repostitory;
using X.PagedList;

namespace WebsiteSellingSport.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryProductController : Controller
    {
        private readonly CategoryProductRepository categoryProductRepo;
        private readonly CategoryRepository categoryRepo;
        private readonly QLBHQAContext _dbcontext;
        public CategoryProductController(QLBHQAContext dbcontext, CategoryProductRepository categoryProductRepository, CategoryRepository categoryRepository)
        {
            _dbcontext = dbcontext;
            categoryProductRepo = categoryProductRepository;
            categoryRepo = categoryRepository;
        }


        public IActionResult Index(string searchQuery, int? status, int? page)
        {
            var data = _dbcontext.CategoryProducts.Include(c => c.Category).ToList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                data = data.Where(c => c.CategoryProductName.Contains(searchQuery)).ToList();
            }
            if (status != -1 && status != null)
            {
                data = data.Where(c => c.Status == status).ToList();
            }
            int pageSize = 10; // Số bản ghi trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại
            var pagedList = data.OrderByDescending(p => p.CategoryId).ToPagedList(pageNumber, pageSize); // Tạo trang dữ liệu

            ViewBag.SearchQuery = searchQuery; // Lưu lại searchQuery để truyền cho view
            ViewBag.Status = status; // Lưu lại status để truyền cho view
            ViewBag.PageNumber = pageNumber; // Lưu lại status để truyền cho view
            ViewBag.ListCategory = new SelectList(categoryRepo.GetAllList().Where(c => c.Status == 1), "CategoryId", "CategoryName"); // Lưu lại status để truyền cho view
            return View(pagedList); // Trả về view với dữ liệu phân trang và tìm kiếm và lọc trạng thái
        }

        [HttpPost]
        public JsonResult Create(string nameCategoryPrd, int idCategory)
        {
            var lst = categoryProductRepo.GetAll().Where(c => c.CategoryProductName == nameCategoryPrd);
            //Check tên đã có chưa
            if (lst.Count() > 0)
            {
                return Json(2, new System.Text.Json.JsonSerializerOptions());
            }
            CategoryProduct category = new CategoryProduct();
            category.CategoryProductName = nameCategoryPrd;
            category.CategoryId = idCategory;
            category.Status = 1;
            return Json(categoryProductRepo.Create(category), new System.Text.Json.JsonSerializerOptions());

        }

        [HttpPost]
        public JsonResult Update(string nameCategory, int categoryProductId, int categoryId)
        {
            CategoryProduct category = (CategoryProduct)categoryProductRepo.GetByID(categoryProductId);
            if (category == null)
            {
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            }
            category.CategoryProductName = nameCategory;
            category.CategoryId = categoryId;
            category.Status = 1;
            return Json(categoryProductRepo.Update(category), new System.Text.Json.JsonSerializerOptions());

        }

        public IActionResult StatusFlag(int id, int page)
        {

            CategoryProduct category = (CategoryProduct)categoryProductRepo.GetByID(id);
            if (category != null)
            {
                category.Status = category.Status == 1 ? 0 : 1;
                categoryProductRepo.Update(category);
            }
            return RedirectToRoute("areas", new { area = "Admin", controller = "CategoryProduct", action = "Index", page = page }); // Trả về view với dữ liệu phân trang

        }
    }
}
