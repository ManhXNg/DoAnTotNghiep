using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class ProductColorSize
    {
        public ProductColorSize()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public long Pcsid { get; set; }
        public long ProductId { get; set; }
        public long ColorId { get; set; }
        public long SizeId { get; set; }
        public int Stock { get; set; }

        public virtual Color Color { get; set; }
        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
