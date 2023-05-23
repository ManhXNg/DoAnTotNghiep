using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductColorSizes = new HashSet<ProductColorSize>();
            ProductImages = new HashSet<ProductImage>();
        }

        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public long CategoryProductId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int Status { get; set; }

        public virtual CategoryProduct CategoryProduct { get; set; }
        public virtual ICollection<ProductColorSize> ProductColorSizes { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
