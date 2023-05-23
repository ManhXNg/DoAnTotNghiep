using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebsiteSellingSport.Repostitory;
using X.PagedList;

namespace WebsiteSellingSport.ViewComponents
{
    public class HeaderListViewComponent : ViewComponent
    {
        CategoryRepository _categoryRepo;
        CategoryProductRepository _categoryProductRepo;
        public HeaderListViewComponent(CategoryProductRepository categoryProductRepository, CategoryRepository categoryRepository)
        {
            _categoryProductRepo = categoryProductRepository;
            _categoryRepo = categoryRepository;
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.CategoryHeader = await _categoryRepo.GetAllList().Where(c=>c.Status == 1).ToListAsync();
            ViewBag.CategoryProductHeader = await _categoryProductRepo.GetAllList().Where(c => c.Status == 1).ToListAsync();
            return View();
        }
    }
}
