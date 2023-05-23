using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public long OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public long? CustomerId { get; set; }
        public string Note { get; set; }
        public decimal? TotalShip { get; set; }
        public string FullName { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
