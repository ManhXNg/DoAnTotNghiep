using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebsiteSellingSport.Areas.Admin.ModelView;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.Repostitory;
using X.PagedList;

namespace WebsiteSellingSport.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepo;
        private readonly CategoryProductRepository _categoryProductRepo;
        private readonly ProductImageRepository _productImgRepo;

        private readonly QLBHQAContext _dbcontext;
        public ProductController(QLBHQAContext context, ProductRepository productRepository, ProductImageRepository productImgRepo, CategoryProductRepository categoryProductRepository)
        {
            _dbcontext = context;
            _productRepo = productRepository;
            _productImgRepo = productImgRepo;
            _categoryProductRepo = categoryProductRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            HttpContext.Session.Remove("productcolorsize");
            ViewBag.ListCategoryProduct = new SelectList(_categoryProductRepo.GetAll(), "CategoryProductId", "CategoryProductName");
            ViewBag.Colors = new SelectList(_dbcontext.Colors.ToList(), "ColorId", "ColorName");
            ViewBag.Sizes = new SelectList(_dbcontext.Sizes.ToList(), "SizeId", "SizeName");
            return View();
        }
        public JsonResult GetListProduct()
        {
            var lstProduct = _dbcontext.Products
             .Include(p => p.CategoryProduct) // kết nối với bảng Category
             .Select(p => new
             {
                 ProductId = p.ProductId,
                 CategoryName = p.CategoryProduct.CategoryProductName,
                 ProductName = p.ProductName,
                 Price = p.Price,
                 Status = p.Status == 1 ? "Hoạt động" : "Không hoạt động",
             })
             .OrderByDescending(c => c.ProductId);

            return Json(lstProduct, new System.Text.Json.JsonSerializerOptions());
        }
        #region Delete Product
        public JsonResult DeleteFlag(long id)
        {
            Product product = _productRepo.GetByID(id);
            product.Status = product.Status == 1 ? 0 : 1;
            _productRepo.Update(product);
            return Json(1);
        }
        #endregion
        #region CreateProduct
        [HttpPost]
        public IActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product();
                product.ProductName = model.ProductName;
                product.Price = model.Price;
                product.Description = model.Description;             
                product.CategoryProductId = model.CategoryProductId;
                product.CreatedDate = DateTime.Now;
                product.Status = 1;
                _dbcontext.Products.Add(product);
                _dbcontext.SaveChanges();
                ;
                if (product.ProductId > 1 && ProductColorSizes.Any())
                {

                    var productColorSizes = ProductColorSizes.Select(item => new ProductColorSize
                    {
                        ProductId = product.ProductId,
                        SizeId = item.SizeId.Value,
                        ColorId = item.ColorId.Value,
                        Stock = item.Stock.Value
                    });

                    _dbcontext.ProductColorSizes.AddRange(productColorSizes);
                    _dbcontext.SaveChanges();
                    HttpContext.Session.Remove("productcolorsize");
                }

                return RedirectToRoute("areas", new { area = "Admin", controller = "Product", action = "Index" });
            }
            else
            {
                // Model không hợp lệ, hiển thị thông báo lỗi cho người dùng
                return RedirectToRoute("areas", new { area = "Admin", controller = "Product", action = "Create" });
            }
        }
        #endregion
        #region EditProduct
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbcontext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryProductId = new SelectList(_dbcontext.CategoryProducts.Where(c => c.Status == 1), "CategoryProductId", "CategoryProductName", product.CategoryProductId);
            ViewBag.Colors = new SelectList(_dbcontext.Colors.ToList(), "ColorId", "ColorName");
            ViewBag.Sizes = new SelectList(_dbcontext.Sizes.ToList(), "SizeId", "SizeName");
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {


                product.UpdateDate = DateTime.Now;
                _dbcontext.Products.Update(product);
                _dbcontext.SaveChanges();


                return RedirectToRoute("areas", new { area = "Admin", controller = "Product", action = "Index" });
            }
            else
            {
                // Model không hợp lệ, hiển thị thông báo lỗi cho người dùng
                return RedirectToRoute("areas", new { area = "Admin", controller = "Product", action = "Edit", id = product.ProductId });
            }
        }
        public JsonResult EditColorSizeFromEdit(long id, int quantity)
        {
            ProductColorSize productColorSize = _dbcontext.ProductColorSizes.Where(c => c.Pcsid == id).FirstOrDefault();
            if (productColorSize != null)
            {
                productColorSize.Stock = quantity;
                _dbcontext.ProductColorSizes.Update(productColorSize);
                _dbcontext.SaveChanges();
                return Json(1);
            }
            return Json(0);
        }
        public JsonResult DeleteColorSizeFromEdit(long id)
        {
            ProductColorSize productColorSize = _dbcontext.ProductColorSizes.Where(c => c.Pcsid == id).FirstOrDefault();
            if (productColorSize != null)
            {

                _dbcontext.ProductColorSizes.Remove(productColorSize);
                _dbcontext.SaveChanges();
                return Json(1);
            }
            return Json(0);
        }
        public JsonResult GetListColorSizeEditById(long id)
        {
            var data = _dbcontext.ProductColorSizes.Where(c => c.Pcsid == id);
            var temp = data.Select(pcs => new
            {

                NameColor = _dbcontext.Colors.Where(c => c.ColorId == pcs.ColorId).Select(c => c.ColorName).FirstOrDefault(),
                NameSize = _dbcontext.Sizes.Where(s => s.SizeId == pcs.SizeId).Select(s => s.SizeName).FirstOrDefault(),
                Stock = pcs.Stock,
                ID = pcs.Pcsid,

            }).FirstOrDefault();

            return Json(temp);
        }
        [HttpPost]
        public IActionResult AddColorSizeFromEdit([FromBody] ProductColorSize productColorSize)
        {

            var temp = _dbcontext.ProductColorSizes.SingleOrDefault(c => c.ProductId == productColorSize.ProductId && c.SizeId == productColorSize.SizeId && c.ColorId == productColorSize.ColorId);
            if (temp != null)
            {
                //Đã tồn tại
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            }
            if (ModelState.IsValid)
            {
                _dbcontext.Add(productColorSize);
                _dbcontext.SaveChanges();
                return Json(1);
            }
            return Json(0);

        }
        public JsonResult GetListEditColorSize(int id)
        {
            var data = _dbcontext.ProductColorSizes.Where(c => c.ProductId == id).ToList();
            var temp = data.Select(pcs => new
            {
                ID = pcs.Pcsid,
                NameColor = _dbcontext.Colors.Where(c => c.ColorId == pcs.ColorId).Select(c => c.ColorName).FirstOrDefault(),
                NameSize = _dbcontext.Sizes.Where(s => s.SizeId == pcs.SizeId).Select(s => s.SizeName).FirstOrDefault(),
                Quantity = pcs.Stock
            });

            return Json(temp);
        }

        #endregion
        #region Add Color - Size

        private List<ProductColorSizeView> ProductColorSizes
        {
            get
            {
                var data = HttpContext.Session.Get<List<ProductColorSizeView>>("productcolorsize");
                if (data == null)
                {

                    data = new List<ProductColorSizeView>();
                }
                return data;
            }
        }
        [HttpPost]
        public IActionResult AddColorSize([FromBody] ProductColorSizeView productColorSize)
        {
            if (productColorSize == null)
            {
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            }
            var lstProductColorSize = ProductColorSizes;
            var temp = lstProductColorSize.SingleOrDefault(c => c.SizeId == productColorSize.SizeId && c.ColorId == productColorSize.ColorId);
            if (temp != null)
            {
                //Đã tồn tại
                return Json(0, new System.Text.Json.JsonSerializerOptions());
            }
            //Add         
            productColorSize.ID = Guid.NewGuid();
            lstProductColorSize.Add(productColorSize);
            HttpContext.Session.Set("productcolorsize", lstProductColorSize);
            var tempObj = new
            {
                ID = productColorSize.ID,
                NameColor = _dbcontext.Colors.Where(c => c.ColorId == productColorSize.ColorId).Select(c => c.ColorName).FirstOrDefault(),
                NameSize = _dbcontext.Sizes.Where(s => s.SizeId == productColorSize.SizeId).Select(s => s.SizeName).FirstOrDefault(),
                Quantity = productColorSize.Stock,
            };
            return Json(tempObj, new System.Text.Json.JsonSerializerOptions());
        }
        public IActionResult DeleteColorSize(Guid id)
        {
            var temp = ProductColorSizes;
            ProductColorSizeView productColorSizeView = temp.Where(C => C.ID == id).FirstOrDefault();
            temp.Remove(productColorSizeView);
            HttpContext.Session.Set("productcolorsize", temp);
            return Json(1);
        }

        public JsonResult EditColorSize(Guid idx, int quantityx)
        {
            var temp = ProductColorSizes;
            ProductColorSizeView productColorSizeView = temp.Where(C => C.ID == idx).FirstOrDefault();
            productColorSizeView.Stock = quantityx;
            HttpContext.Session.Set("productcolorsize", temp);
            return Json(1);
        }


        public JsonResult GetListColorSize()
        {
            var data = HttpContext.Session.Get<List<ProductColorSizeView>>("productcolorsize");
            var temp = data.Select(pcs => new
            {
                ID = pcs.ID,
                NameColor = _dbcontext.Colors.Where(c => c.ColorId == pcs.ColorId).Select(c => c.ColorName).FirstOrDefault(),
                NameSize = _dbcontext.Sizes.Where(s => s.SizeId == pcs.SizeId).Select(s => s.SizeName).FirstOrDefault(),
                Quantity = pcs.Stock
            }).ToList();

            return Json(temp);
        }
        public JsonResult GetListColorSizeById(Guid id)
        {
            var data = HttpContext.Session.Get<List<ProductColorSizeView>>("productcolorsize").Where(c => c.ID == id);
            var temp = data.Select(pcs => new
            {

                NameColor = _dbcontext.Colors.Where(c => c.ColorId == pcs.ColorId).Select(c => c.ColorName).FirstOrDefault(),
                NameSize = _dbcontext.Sizes.Where(s => s.SizeId == pcs.SizeId).Select(s => s.SizeName).FirstOrDefault(),
                Stock = pcs.Stock,

            }).FirstOrDefault();

            return Json(temp);
        }
        #endregion

        #region Image


        [HttpPost]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, long idProduct)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    AddImageToDB(formFile.FileName, idProduct);
                    var fileName = Path.GetFileName(formFile.FileName);
                    var filePath = Path.Combine("wwwroot/ImageProduct", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {

                        await formFile.CopyToAsync(stream);
                    }

                }
            }

            return Ok(new { count = files.Count, size });
        }
        [HttpGet]
        public async Task<IActionResult> LoadImage(long idProduct)
        {
            var lst = await _productImgRepo.GetAll().Where(c => c.ProductId == idProduct).ToListAsync();

            return Json(lst, new System.Text.Json.JsonSerializerOptions());
        }
        [HttpPost]
        public IActionResult DeleteImage(long idImage)
        {
            var productImage = _productImgRepo.GetByID(idImage);
            //Xóa file trên server
            var filePath = Path.Combine("wwwroot/ImageProduct", productImage.ImageUrl);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

            }
            //Xóa file trên db
            return Json(_productImgRepo.Delete(idImage), new System.Text.Json.JsonSerializerOptions());
        }


        private void AddImageToDB(string fileName, long id)
        {
            ProductImage productImage = new ProductImage();
            productImage.ImageUrl = fileName;
            productImage.ProductId = id;
            _productImgRepo.Create(productImage);
        }
        #endregion


    }
}
