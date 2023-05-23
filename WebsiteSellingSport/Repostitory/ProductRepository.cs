using System.Collections.Generic;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;

namespace WebsiteSellingSport.Repostitory
{
    public class ProductRepository : GenericRepository<Product>
    {

        public List<ProductView> GetListProductNew()
        {
            var result = (from p in db.Products
                          let firstImage = (from pi in db.ProductImages where pi.ProductId == p.ProductId select pi).FirstOrDefault()
                          join cp in db.CategoryProducts on p.CategoryProductId equals cp.CategoryProductId
                          join ctp in db.Categories on cp.CategoryId equals ctp.CategoryId
                          where p.Status == 1
                          orderby p.CreatedDate descending
                          select new ProductView
                          {
                              ProductId = p.ProductId,
                              ProductName = p.ProductName,
                              Price = p.Price,
                              ProductImg = firstImage.ImageUrl,
                              CategoryProductID = cp.CategoryProductId,
                              CategoryID = ctp.CategoryId,
                          }).Take(15).ToList();
            return result;
        }
        public List<ProductView> GetListProductShop(string search, decimal? minPrice, decimal? maxPrice, long? categoryid, long? categoryProductid, int? order)
        {
            if (minPrice > maxPrice || maxPrice > 9999999999 || minPrice > 999999999)
            {
                return new List<ProductView>();
            }
            var result = db.Products
                .Where(p => p.Status == 1)
                .Select(p => new ProductView
                {
                    ProductId = p.ProductId,
                    CreatedDate = p.CreatedDate,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ProductImg = db.ProductImages.FirstOrDefault(pi => pi.ProductId == p.ProductId).ImageUrl,
                    CategoryProductID = p.CategoryProductId,
                    CategoryID = db.CategoryProducts.FirstOrDefault(cp => cp.CategoryProductId == p.CategoryProductId).Category.CategoryId,
                });

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(e => e.ProductName.Contains(search));
            }

            if (minPrice.HasValue && maxPrice.HasValue)
            {
                result = result.Where(e => e.Price >= minPrice.Value && e.Price <= maxPrice.Value);
            }

            if (categoryid.HasValue)
            {
                result = result.Where(e => e.CategoryID == categoryid.Value);
            }

            if (categoryProductid.HasValue)
            {
                result = result.Where(e => e.CategoryProductID == categoryProductid.Value);
            }

            switch (order)
            {
                case 1:
                    //Mới nhất
                    result = result.OrderByDescending(c => c.CreatedDate);
                    break;
                case 2:
                    //A - Z
                    result = result.OrderBy(c => c.ProductName);
                    break;
                case 3:
                    //Z - A
                    result = result.OrderByDescending(c => c.ProductName);
                    break;
                case 4:
                    //Giá từ thấp đến cao
                    result = result.OrderBy(c => c.Price);
                    break;
                case 5:
                    //Giá từ cao đến thấp
                    result = result.OrderByDescending(c => c.Price);
                    break;
                default:
                    break;
            }

            return result.ToList();
        }

        public ProductColorSize GetTotalProduct(long id, int sizeId, int colorId)
        {
            var productColorSize = db.ProductColorSizes.Where(c => c.ProductId == id && c.SizeId == sizeId && c.ColorId == colorId).FirstOrDefault();
            if (productColorSize != null)
            {
                return productColorSize;

            }
            return null;
        }

    }
}
