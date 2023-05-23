using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;
using WebsiteSellingSport.ModelView.GHNViewModel;
using WebsiteSellingSport.Repostitory;

namespace WebsiteSellingSport.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly HttpClient _httpClient;
        OrderRepository _orderRepo;
        ProductRepository _productRepo;
        public CheckOutController(OrderRepository orderRepository, ProductRepository productRepository )
        {
            _httpClient = new HttpClient();
        
            _httpClient.DefaultRequestHeaders.Add("token", "71f04310-864d-11ed-b09a-9a2a48e971b0");
            _orderRepo = orderRepository;
            _productRepo = productRepository;
        }
        //Lấy địa chỉ quận huyện
        public JsonResult GetListDistrict(int idProvin)
        {

            HttpResponseMessage responseDistrict = _httpClient.GetAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id=" + idProvin).Result;

            District lstDistrict = new District();

            if (responseDistrict.IsSuccessStatusCode)
            {
                string jsonData2 = responseDistrict.Content.ReadAsStringAsync().Result;

                lstDistrict = JsonConvert.DeserializeObject<District>(jsonData2);
            }
            return Json(lstDistrict, new System.Text.Json.JsonSerializerOptions());
        }
        //Lấy địa chỉ phường xã
        public JsonResult GetListWard(int idWard)
        {


            HttpResponseMessage responseWard = _httpClient.GetAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=" + idWard).Result;

            Ward lstWard = new Ward();

            if (responseWard.IsSuccessStatusCode)
            {
                string jsonData2 = responseWard.Content.ReadAsStringAsync().Result;

                lstWard = JsonConvert.DeserializeObject<Ward>(jsonData2);
            }
            return Json(lstWard, new System.Text.Json.JsonSerializerOptions());
        }
        public List<CartItem> Carts()
        {


            List<CartItem> data = new List<CartItem>();


            var jsonData = Request.Cookies["Cart"];
            if (jsonData != null)
            {
                data = JsonConvert.DeserializeObject<List<CartItem>>(jsonData);
                return data;
            }
           
            return data;



        }
        //Lấy phí ship ghn
        public JsonResult GetTotalShipping([FromBody] ShippingOrder shippingOrder)
        {
            _httpClient.DefaultRequestHeaders.Add("shop_id", "3630415");

            HttpResponseMessage responseWShipping = _httpClient.GetAsync("https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?service_id=" + shippingOrder.service_id + "&insurance_value=" + shippingOrder.insurance_value + "&coupon=&from_district_id=" + shippingOrder.from_district_id + "&to_district_id=" + shippingOrder.to_district_id + "&to_ward_code=" + shippingOrder.to_ward_code + "&height=" + shippingOrder.height + "&length=" + shippingOrder.length + "&weight=" + shippingOrder.weight + "&width=" + shippingOrder.width + "").Result;

            Shipping shipping = new Shipping();
            if (responseWShipping.IsSuccessStatusCode)
            {
                string jsonData2 = responseWShipping.Content.ReadAsStringAsync().Result;

                shipping = JsonConvert.DeserializeObject<Shipping>(jsonData2);
                HttpContext.Session.SetInt32("shiptotal", shipping.data.total);
            }
            
            shipping.data.totaloder = shipping.data.total + Carts().Sum(c => c.Total);
            return Json(shipping, new System.Text.Json.JsonSerializerOptions());
        }
        public IActionResult Index()
        {
            //Lấy địa chỉ tỉnh thành
            HttpResponseMessage responseProvin = _httpClient.GetAsync("https://online-gateway.ghn.vn/shiip/public-api/master-data/province").Result;

            Provin lstprovin = new Provin();

            if (responseProvin.IsSuccessStatusCode)
            {
                string jsonData2 = responseProvin.Content.ReadAsStringAsync().Result;


                lstprovin = JsonConvert.DeserializeObject<Provin>(jsonData2);
                ViewBag.Provin = new SelectList(lstprovin.data, "ProvinceID", "ProvinceName");
            }
            Customer  customer = new Customer();
            if (User.Identity.Name != null) {

               
                var emailClaim = HttpContext.User.FindFirst("Email");
                var phoneClaim = HttpContext.User.FindFirst("Phone");
                customer.FullName = User.Identity.Name;                           
                customer.Phone =phoneClaim?.Value;               
                customer.Email = emailClaim?.Value;
          
            }
            return View(customer);
        }
        [HttpPost]
        public IActionResult OrderCheckOut([FromBody] Order order)
        {
            if (Carts().Count < 1)
            {
                return RedirectToAction("Index");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<CartItem> lstCarts = new List<CartItem>();
                    var customerIdClaim = HttpContext.User.FindFirst("CustomerId");
                    if (customerIdClaim != null)
                    {
                        order.CustomerId = Convert.ToInt32(customerIdClaim.Value);
                    }
                    
                    foreach (var item in Carts())
                    {
                        var temp = _productRepo.GetByID(item.ProductID);
                        CartItem cartItem = new CartItem();
                        cartItem.ProductID = item.ProductID;
                        cartItem.ProductCsID = item.ProductCsID;
                        cartItem.ProductName = item.ProductName;
                        cartItem.Price = temp.Price;
                        cartItem.Quantity = item.Quantity;
                        cartItem.Total = item.Quantity * temp.Price;
                        lstCarts.Add(cartItem);
                    }
                    order.TotalShip = HttpContext.Session.GetInt32("shiptotal");
                    order.TotalAmount = lstCarts.Sum(c => c.Total);
                    order.OrderDate =DateTime.Now;
                    order.Status = 1;
           
                    _orderRepo.Create(order);
                    if (order.OrderId  > 0)
                    {


                        foreach (var x in lstCarts)
                        {
                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderId = order.OrderId;
                            orderDetail.Quantity = x.Quantity;
                            orderDetail.Price = x.Price;
                            orderDetail.ProductColorSizeId = x.ProductCsID;

                            _orderRepo.AddOrderDetail(orderDetail);
                           
                           

                        }
                       
                    }

                    Response.Cookies.Delete("Cart");
                    return Json(order.OrderId);
                }
                else
                {
                    return Json(0);
                }
                

            }
            catch 
            {

                return Json(1);
            }                     
            
        }
    }
}
