using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class OrderDetail
    {
        public long OrderDetailId { get; set; }
        public long OrderId { get; set; }
        public long ProductColorSizeId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual ProductColorSize ProductColorSize { get; set; }
    }
}
