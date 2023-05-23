using System;

namespace WebsiteSellingSport.ModelView
{
    public class ProductView
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public decimal Price { get; set; }
        public long CategoryID { get; set; }
        public long CategoryProductID { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
