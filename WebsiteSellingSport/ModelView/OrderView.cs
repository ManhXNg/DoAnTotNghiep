using System;

namespace WebsiteSellingSport.ModelView
{
    public class OrderView
    {
        public long OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public int Status { get; set; }
        public string Phone { get; set; }

    }
}
