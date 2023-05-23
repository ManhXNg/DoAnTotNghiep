using System.ComponentModel.DataAnnotations;

namespace WebsiteSellingSport.Areas.Admin.ModelView
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Danh mục sản phẩm là bắt buộc")]
        public int CategoryProductId { get; set; }


    }
}
