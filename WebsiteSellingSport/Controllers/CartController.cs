using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.ModelView;
using WebsiteSellingSport.Repostitory;

namespace WebsiteSellingSport.Controllers
{
    public class CartController : Controller
    {
        ProductRepository _productRepo ;
        ProductImageRepository _productImageRepo ;
        ProductColorSizeRepository _productCSRepo ;
        public CartController(ProductRepository productRepository, ProductColorSizeRepository productColorSizeRepository, ProductImageRepository  productImageRepository  )
        {
            _productRepo = productRepository;
            _productCSRepo = productColorSizeRepository;
            _productImageRepo = productImageRepository;
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
        [HttpGet]
        public IActionResult GetCarts()
        {


            List<CartItem> data = new List<CartItem>();


            var jsonData = Request.Cookies["Cart"];
            if (jsonData != null)
            {
                data = JsonConvert.DeserializeObject<List<CartItem>>(jsonData);
              
            }
            return Json(data);



        }
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        { 
            if ( id > 0 && quantity > 0 )
            {



                ProductColorSizeView productColorSize = (ProductColorSizeView)_productCSRepo.GetProductCS(id);


                    Product product =(Product) _productRepo.GetByID(productColorSize.ProductId);
                
                
                    var myCart = Carts();
                    var item = myCart.SingleOrDefault(c => c.ProductCsID == id);

                    if (quantity > productColorSize.Stock)
                    {
                        return Json(new { status = false, mess = "Số lượng không có sẵn" });
                    }
                    if (item == null)
                    {

                        item = new CartItem
                        {
                            ProductID = productColorSize.ProductId,
                            ProductCsID = id,
                            ProductName = product.ProductName + " - Color: "+ productColorSize.ColorName+" - Size:  "+ productColorSize.SizeName,
                            Price = product.Price,
                            Quantity = quantity,
                            ProductImage = _productImageRepo.GetFistImageProduct(productColorSize.ProductId).ImageUrl,
                            Total = quantity * product.Price
                            };
                            myCart.Add(item);



                        }
                    else if (myCart.Exists(c => c.ProductCsID == id))
                    {
                        if (quantity + item.Quantity > productColorSize.Stock || item.Quantity > productColorSize.Stock)
                        {

                            return Json(new { status = false, mess = "Số lượng không có sẵn" });
                        }

                        else
                        {
                            item.Quantity += quantity;
                            item.Total = item.Quantity * item.Price;

                    }


                    }
                    else
                    {
                        item.Quantity++;
                        item.Total = item.Quantity * item.Price;
                    }
                    var opt = new CookieOptions() { Expires = new DateTimeOffset(DateTime.Now.AddDays(3)) };

                    var json = System.Text.Json.JsonSerializer.Serialize(myCart);
                    Response.Cookies.Append("Cart", json, opt);
                return Json(new { status = true, mess = "Thêm thành công" });

            }
            return Json( new { status = false, mess = "Lỗi ! Thêm vào giỏ hàng thất bại" });
        }
        [HttpPost]
        public IActionResult UpdateToCart(int id, int quantitynew)
        {
            if (id > 0 && quantitynew > 0)
            {



                ProductColorSizeView productColorSize = (ProductColorSizeView)_productCSRepo.GetProductCS(id);

                var myCart = Carts();
                var item = myCart.SingleOrDefault(c => c.ProductCsID == id);

                if ( productColorSize.Stock >= quantitynew )
                {
                    item.Quantity = quantitynew;
                    item.Total = item.Quantity * item.Price;
                }
                else
                {
                    return Json(new { status = false, mess = "Số lượng hàng không có sẵn !" });
                }
                

               var opt = new CookieOptions() { Expires = new DateTimeOffset(DateTime.Now.AddDays(3)) };

                var json = System.Text.Json.JsonSerializer.Serialize(myCart);
                Response.Cookies.Append("Cart", json, opt);
                return Json(new { status = true, mess = "Update thành công" });

            }
            return Json(new { status = false, mess = "Lỗi ! Update giỏ hàng thất bại" });
        } 
        [HttpPost]
        public IActionResult DeleteCart(int id)
        {
            if (id > 0)
            {
                ProductColorSizeView productColorSize = (ProductColorSizeView)_productCSRepo.GetProductCS(id);

                var myCart = Carts();
                var item = myCart.SingleOrDefault(c => c.ProductCsID == id);

                if (item != null)
                {
                    myCart.Remove(item);

                }
                else
                {
                    return Json(new { status = false, mess = "Xóa thất bại" });
                }


                var opt = new CookieOptions() { Expires = new DateTimeOffset(DateTime.Now.AddDays(3)) };

                var json = System.Text.Json.JsonSerializer.Serialize(myCart);
                Response.Cookies.Append("Cart", json, opt);
                return Json(new { status = true, mess = "Xóa thành công" });


               
            }
            return Json(new { status = false, mess = "Lỗi ! Xóa giỏ hàng thất bại" });
        }

    }

}

