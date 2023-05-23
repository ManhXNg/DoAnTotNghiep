using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;
using WebsiteSellingSport.Repostitory;
using static WebsiteSellingSport.Areas.Admin.Controllers.OrderController;

namespace WebsiteSellingSport.Controllers
{


    public class CustomerController : Controller
    {
        CustomerRepository _customerRepo;
        OrderRepository _orderRepo;
        public CustomerController(CustomerRepository customerRepository, OrderRepository orderRepo)
        {
            _customerRepo = customerRepository;
            _orderRepo = orderRepo;
        }
        [HttpPost]
        public IActionResult UpdateStatusOrder(long orderId)
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerId");
            if (customerIdClaim == null)
            {
                return Json(0);
            }
            try
            {
                var order = _customerRepo.CheckOrder(Convert.ToInt64(customerIdClaim.Value), orderId);
                if (order != null)
                {
                    order.Status = 4;
                    _orderRepo.Update(order);
                    return Json(1);
                }


            }
            catch 
            {
                return Json(0);
                throw;
            }

            return Json(0);
        }

        public IActionResult OrderCustomer()
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerId");
            if (customerIdClaim  == null)
            {
                return NotFound();
            }
            return View(_customerRepo.GetListOrderByCusotmer(Convert.ToInt64(customerIdClaim.Value)));
        }
        public IActionResult OrderDetailCustomer(long id)
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerId");
            if (customerIdClaim != null)
            {

                var order = _customerRepo.CheckOrder(Convert.ToInt64(customerIdClaim.Value), id);
                if (order == null)
                {
                    return NotFound();
                }
                var viewmodel = new OrderDetailViewModel
                {
                    OrderDetailViews = _orderRepo.GetListOrderDetail(order.OrderId),
                    Order = order,
                   
                };
                return View(viewmodel);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult EditCustomer()
        {
            var customerIdClaim = HttpContext.User.FindFirst("CustomerId");
            if (customerIdClaim != null)
            {
                var customer = _customerRepo.GetByID(Convert.ToInt64(customerIdClaim.Value));
                CustomerViewModel customerView = new CustomerViewModel();
                customerView.CustomerId = customer.CustomerId;
                customerView.FullName = customer.FullName;
                customerView.Phone = customer.Phone;
                customerView.Email = customer.Email;
                customerView.DateBirth = customer.DateBirth;
                customerView.Address = customer.Address;
                customerView.Sex = customer.Sex;
                return View(customerView);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCustomer(CustomerViewModel customerView)
        {
           
            if (ModelState.IsValid && customerView.CustomerId != 0)
            {
                try
                {
                    Customer customer = _customerRepo.GetByID(customerView.CustomerId);                  
                    customer.FullName = customerView.FullName;
                    customer.Phone = customerView.Phone;                 
                    customer.DateBirth = customerView.DateBirth;
                    customer.Address = customerView.Address;
                    customer.Sex = customerView.Sex;
                    _customerRepo.Update(customer);
                    customerView.Email = customer.Email;
                    return View(customerView);

                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            else
            {
                
                // Add error messages to ModelState
                if (string.IsNullOrEmpty(customerView.FullName))
                {
                    ModelState.AddModelError("FullName", "Họ và tên không được để trống");
                }
                if (string.IsNullOrEmpty(customerView.Phone))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại không được để trống");
                }
                else if (!Regex.IsMatch(customerView.Phone, @"^(\d{10}|\d{11})$"))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại không hợp lệ");
                }
                
                if (customerView.DateBirth == DateTime.MinValue)
                {
                    ModelState.AddModelError("DateBirth", "Ngày sinh không được để trống");
                }
                if (string.IsNullOrEmpty(customerView.Address))
                {
                    ModelState.AddModelError("Address", "Địa chỉ không được để trống");
                }
                if (customerView.Sex != true && customerView.Sex != false)
                {
                    ModelState.AddModelError("Sex", "Giới tính không hợp lệ");
                }
                return View(customerView);
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(string passwordold, string passwordnew)
        {
            var customerId = HttpContext.User.FindFirst("CustomerId").Value;
            if (customerId != null && passwordnew != null && passwordold != null)
            {
                try
                {
                    var customer = _customerRepo.GetByID(Convert.ToInt64(customerId));

                    string password_old = string.IsNullOrEmpty(customer.Password) ? "" : Common.MD5.EncryptPassword(passwordold);
                    string password_new = string.IsNullOrEmpty(customer.Password) ? "" : Common.MD5.EncryptPassword(passwordnew);
                    if (customer.Password != password_old)
                    {
                        return Json(3);
                    }
                    if (customer.Password == password_new)
                    {
                        return Json(2);
                    }
                    customer.Password = password_new;
                    _customerRepo.Update(customer);
                    return Json(1);
                }
                catch
                {

                    return Json(0);
                }
            }
            return Json(0);
        }
    }
}
