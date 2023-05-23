using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;
using WebsiteSellingSport.Repostitory;

namespace WebsiteSellingSport.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        OrderRepository _orderRepo;     
        public OrderController(OrderRepository orderRepository)
        {
            _orderRepo = orderRepository;
        }
        public IActionResult Index()
        {
           
            return View();
        }
        public class OrderDetailViewModel
        {
            public   List<OrderDetailView>OrderDetailViews { get; set; }
            public   Customer Customer { get; set; }
            public   Order Order { get; set; }

        }
        public IActionResult OrderDetail(long id)
        {
          

            var order = _orderRepo.GetOrder(id);
           
            var viewmodel = new OrderDetailViewModel
            {
                OrderDetailViews = _orderRepo.GetListOrderDetail(id),
                Order = order,
                Customer = order?.CustomerId != null ? _orderRepo.GetCustomerByOrder(order.CustomerId.Value) : null,
            };


            return View(viewmodel);
        }
        public IActionResult GetListOrder(int stt,DateTime datestart, DateTime dateend)
        {
            DateTime dateStartnew = datestart.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime dateEndnew = dateend.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // Đặt giờ thành 23:59:59
            var lstOrder = _orderRepo.GetListOrder(stt, dateStartnew, dateEndnew);
            return Json(lstOrder);
        }
        [HttpPost]
        public IActionResult UpdateStatusOrder(long orderId,int selectedId)
        {           
            return Json(_orderRepo.UpdateStatusOrder(orderId, selectedId));
        }
        [HttpPost]
        public IActionResult CancelOrder(long orderId)
        {           
            return Json(_orderRepo.CancelOrder(orderId));
        }
    }
}
