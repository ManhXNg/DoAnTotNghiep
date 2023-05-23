using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class ProductImage
    {
        public long ProductImageId { get; set; }
        public string ImageUrl { get; set; }
        public long ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
