using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Color
    {
        public Color()
        {
            ProductColorSizes = new HashSet<ProductColorSize>();
        }

        public long ColorId { get; set; }
        [Required(ErrorMessage = "ColorName không được để trống")]
        public string ColorName { get; set; }
        [Required(ErrorMessage = "ColorCode không được để trống")]
        public string ColorCode { get; set; }

        public virtual ICollection<ProductColorSize> ProductColorSizes { get; set; }
    }
}
