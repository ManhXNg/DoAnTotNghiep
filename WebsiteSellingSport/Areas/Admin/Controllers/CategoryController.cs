using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.Repostitory;
using X.PagedList;

namespace WebsiteSellingSport.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        CategoryRepository categoryRepo = new CategoryRepository();

        public IActionResult Index(string searchQuery, int? status, int? page)
        {
            var data = categoryRepo.GetAllList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                data = data.Where(c => c.CategoryName.Contains(searchQuery)).ToList();
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
            return View(pagedList); // Trả về view với dữ liệu phân trang và tìm kiếm và lọc trạng thái
        }

        [HttpPost]
        public JsonResult Create(string nameCategory)
        {
            var lst = categoryRepo.GetAll().Where(c => c.CategoryName == nameCategory);
            //Check tên đã có chưa
            if (lst.Count() > 0)
            {
                return Json(2, new System.Text.Json.JsonSerializerOptions());
            }
            Category category = new Category();
            category.CategoryName = nameCategory;
            category.Status = 1;
            return Json(categoryRepo.Create(category), new System.Text.Json.JsonSerializerOptions());

        }

        [HttpPost]
        public JsonResult Update(string nameCategory, int categoryId)
        {
            var lst = categoryRepo.GetAll().Where(c => c.CategoryName == nameCategory);
            //Check tên đã có chưa
            if (lst.Count() > 0)
            {
                return Json(2, new System.Text.Json.JsonSerializerOptions());
            }
            Category category = (Category)categoryRepo.GetByID(categoryId);
            if (category == null)
            {
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            }
            category.CategoryName = nameCategory;
            category.Status = 1;
            return Json(categoryRepo.Update(category), new System.Text.Json.JsonSerializerOptions());

        }

        public IActionResult StatusFlag(int id, int page)
        {

            Category category = (Category)categoryRepo.GetByID(id);
            if (category != null)
            {
                category.Status = category.Status == 1 ? 0 : 1;
                categoryRepo.Update(category);
            }
            return RedirectToRoute("areas", new { area = "Admin", controller = "Category", action = "Index", page = page }); // Trả về view với dữ liệu phân trang

        }

    }
}
