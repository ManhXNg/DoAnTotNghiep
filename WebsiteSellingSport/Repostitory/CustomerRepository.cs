using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;

namespace WebsiteSellingSport.Repostitory
{
    public class CustomerRepository:GenericRepository<Customer>
    {
        private readonly QLBHQAContext _context;
        public CustomerRepository(QLBHQAContext qLBHQAContext)
        {
            _context = qLBHQAContext;
        }
        public List<OrderView> GetListOrderByCusotmer(long customerId)
        {
            if (customerId > 0)
            {
                var orders = _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .Select(o => new OrderView
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.Customer.FullName,
                        OrderDate = o.OrderDate,
                        Total = o.TotalAmount + o.TotalShip.Value,
                        Status = o.Status,
                        Phone = o.Customer.Phone
                    }).OrderByDescending(c=>c.OrderDate)
                    .ToList();
                return orders;
            }
            return new List<OrderView>();
        }
        public Order CheckOrder(long customerId,long orderId)
        {
            if (customerId > 0 && orderId > 0)
            {
                var orders = _context.Orders.Where(o => o.CustomerId == customerId && o.OrderId == orderId).FirstOrDefault();
                    
                return orders;
            }
            return null;
        }
    }
}
