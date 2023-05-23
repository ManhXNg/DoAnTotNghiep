using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Cart
    {
        public long CartId { get; set; }
        public long CustomerId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
