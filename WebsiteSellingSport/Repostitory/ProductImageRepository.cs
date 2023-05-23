using System.Linq;
using WebsiteSellingSport.Models;

namespace WebsiteSellingSport.Repostitory
{
    public class ProductImageRepository : GenericRepository<ProductImage>
    {
        QLBHQAContext _context;
        public ProductImageRepository(QLBHQAContext context)
        {
            _context = context; 
        }
        public ProductImage GetFistImageProduct(long productId)
        {
           return _context.ProductImages.Where(c=>c.ProductId == productId).FirstOrDefault();
        }
    }
}
