using System.Collections.Generic;
using WebsiteSellingSport.Models;

namespace WebsiteSellingSport.ModelView
{
    public class HomeProductDetailView
    {
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductView> ProductViews { get; set; }
        public Product Product { get; set; }
        public CategoryProduct CategoryProduct { get; set; }
        public Category Category { get; set; }
        public List<Color> Colors { get; set; }
        public List<Size> Sizes { get; set; }
    }
}
