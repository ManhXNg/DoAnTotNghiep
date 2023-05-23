using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebsiteSellingSport.Areas.Admin.ModelView;
using WebsiteSellingSport.Models;

namespace WebsiteSellingSport.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class HomeController : Controller
    {
        private QLBHQAContext _dbcontext;

        public HomeController(QLBHQAContext qLBHQAContext)
        {
            _dbcontext = qLBHQAContext;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            string password = string.IsNullOrEmpty(login.Password) ? "" : Common.MD5.EncryptPassword(login.Password);
            var user = _dbcontext.Employees.Where(c => c.UserName == login.UserName && c.Password == password).FirstOrDefault();
            if (user != null)
            {
                var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, user.FullName),
                            new Claim("Name", user.FullName)
                        };
                if (user.Role == 1)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                }
                var identity = new ClaimsIdentity(claims, "MyCookie");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                var auth = new AuthenticationProperties()
                {
                    IsPersistent = true
                };
                await HttpContext.SignInAsync("MyCookie", principal, auth);

                return RedirectToRoute("areas", new { area = "Admin", controller = "Report", action = "Index" });
            }
            else
            {
                ModelState.AddModelError("", "Đăng nhập thất bại vui lòng kiểm tra lại user name hoặc mật khẩu");
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookie");
            return RedirectToRoute("areas", new { area = "Admin", controller = "Home", action = "Login" });
        }
    }
}
