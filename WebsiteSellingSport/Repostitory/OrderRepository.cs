using System;
using System.Collections.Generic;
using System.Linq;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;

namespace WebsiteSellingSport.Repostitory
{
    public class OrderRepository : GenericRepository<Order>
    {
        private readonly QLBHQAContext _context;
        public OrderRepository(QLBHQAContext qLBHQAContext)
        {
            _context = qLBHQAContext;
        }
        public bool AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                _context.OrderDetails.Add(orderDetail);
                var temp = _context.ProductColorSizes.Where(c => c.Pcsid == orderDetail.ProductColorSizeId).FirstOrDefault();
                temp.Stock = temp.Stock - orderDetail.Quantity;
                _context.ProductColorSizes.Update(temp);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;

            }
        }
        public List<OrderView> GetListOrder(int stt, DateTime datestart, DateTime dateend )
        {

            var orderViews = (from o in _context.Orders
                              where o.OrderDate >= datestart && o.OrderDate <= dateend
                              orderby o.OrderDate descending
                              select new OrderView
                              {
                                  OrderId = o.OrderId,
                                  CustomerName = _context.Customers.Where(c => c.CustomerId == o.CustomerId).FirstOrDefault() == null ? "Khách vãng lai" : _context.Customers.Where(c => c.CustomerId == o.CustomerId).FirstOrDefault().FullName,
                                  OrderDate = o.OrderDate,
                                  Total = o.TotalAmount + o.TotalShip.Value,
                                  Status = o.Status,
                                  Phone = _context.Customers.Where(c => c.CustomerId == o.CustomerId).FirstOrDefault() == null ? o.Phone : _context.Customers.Where(c => c.CustomerId == o.CustomerId).FirstOrDefault().Phone
                              }).ToList();
            if (stt != 0)
            {
                return orderViews.Where(c => c.Status == stt).ToList();
            }
            return orderViews;
        }
        public Customer GetCustomerByOrder(long id)
        {
            return _context.Customers.Where(c => c.CustomerId == id).FirstOrDefault();

        }
        public Order GetOrder(long id)
        {
            return _context.Orders.Where(c => c.OrderId == id).FirstOrDefault();

        }
        public List<OrderDetailView>GetListOrderDetail(long orderId)
        {

            var x = (from ot in _context.OrderDetails
                     join p in _context.ProductColorSizes on ot.ProductColorSizeId equals p.Pcsid
                     join prd in _context.Products on p.ProductId equals prd.ProductId
                     join c in _context.Colors on p.ColorId equals c.ColorId
                     join s in _context.Sizes on p.SizeId equals s.SizeId
                     where ot.OrderId == orderId
                     select new OrderDetailView
                     {
                         OrderDetailId = ot.OrderDetailId,
                         ProductName = prd.ProductName,
                         ColorName = c.ColorName,
                         SizeName = s.SizeName,
                         Quantity = ot.Quantity,
                         Price = ot.Price,
                         Total = ot.Price * ot.Quantity,
                     }).ToList();



            return x;

        }
        public int UpdateStatusOrder(long orderId, int selectedId)
        {
            if (orderId == 0 || selectedId == 0 )
            {
                return 0;
            }
            try
            {
                var order = _context.Orders.Where(c => c.OrderId == orderId).FirstOrDefault();
                if (order != null)
                {
                    order.Status = selectedId;
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                    return 1;
                }
                return 0;
            }
            catch 
            {
                return 0;

            }
            

        }
        public int CancelOrder(long orderId)
        {
            if (orderId == 0 )
            {
                return 0;
            }
            try
            {
                var order = _context.Orders.Where(c => c.OrderId == orderId).FirstOrDefault();
                if (order != null)
                {
                    order.Status = 5;//Không hoạt động
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                    return 1;
                }
                return 0;
            }
            catch
            {
                return 0;

            }


        }
    }
}
