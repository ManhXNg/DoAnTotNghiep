using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;

namespace WebsiteSellingSport.Repostitory
{
    public class ProductColorSizeRepository:GenericRepository<ProductColorSize>
    {
        private QLBHQAContext _context;
        public ProductColorSizeRepository(QLBHQAContext dbContext)
        {
            _context = dbContext;
        }
        public  ProductColorSizeView GetProductCS(long id)
        {
            var x = (from pcs in _context.ProductColorSizes
                     join sz in _context.Sizes on pcs.SizeId equals sz.SizeId
                     join cl in _context.Colors on pcs.ColorId equals cl.ColorId
                     where pcs.Pcsid == id
                     select new ProductColorSizeView
                     {
                         Pcsid = pcs.Pcsid,
                         SizeName = sz.SizeName,
                         ColorName = cl.ColorName,
                         ProductId = pcs.ProductId,
                         Stock = pcs.Stock,

                     }).FirstOrDefault();
            return x;
        }
      
    }
}
