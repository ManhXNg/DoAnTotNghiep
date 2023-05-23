using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using WebsiteSellingSport.Models;
using WebsiteSellingSport.Repostitory;

namespace WebsiteSellingSport.Controllers
{
    public class LoginController : Controller
    {
        LoginRepository _loginRepository;
        public LoginController(LoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }
        [HttpPost]
        public IActionResult Register([FromBody] Customer model)
        {
            var check = _loginRepository.CreateCustomer(model);

            if (check == 1)
            {

                return Json(new { success = true, mess = "Đăng ký thành công" });
            }
            else if (check == 2)
            {

                return Json(new { success = false, mess = "Tài khoản đã tồn tại" });
            }
            else
            {
                return Json(new { success = false, mess = "Đăng ký thất bại" });
            }


        }
        [HttpPost]
        public IActionResult Login([FromBody] Customer model)
        {
            var check = _loginRepository.Login(model);

            if (check != null)
            {

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, check.FullName),
                    new Claim("Name", check.FullName),
                    new Claim("CustomerId", check.CustomerId.ToString()),
                    new Claim("Phone", check.Phone.ToString()),
                    new Claim("Email", check.Email),
                    new Claim(ClaimTypes.Role, "Customer")
                };

               

                var identity = new ClaimsIdentity(claims, "MyCookie");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                var auth = new AuthenticationProperties()
                {
                    IsPersistent = true
                };
                HttpContext.SignInAsync("MyCookie", principal, auth);



                return Json(new { success = true, mess = "Đăng nhập thành công" });
            }

            else
            {
                return Json(new { success = false, mess = "Email hoặc mật khẩu không chính xác" });
            }


        }
        [Authorize]

        public IActionResult Logout()
        {

            HttpContext.SignOutAsync("MyCookie");
            return RedirectToAction("Index", "Home");


        }

    }
}
