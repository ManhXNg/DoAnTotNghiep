using System.ComponentModel.DataAnnotations;

namespace WebsiteSellingSport.Areas.Admin.ModelView
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
