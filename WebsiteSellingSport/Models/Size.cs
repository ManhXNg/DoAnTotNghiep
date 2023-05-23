using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Size
    {
        public Size()
        {
            ProductColorSizes = new HashSet<ProductColorSize>();
        }

        public long SizeId { get; set; }
        [Required(ErrorMessage = "SizeName không được để trống")]
        public string SizeName { get; set; }
        [Required(ErrorMessage = "SizeCode không được để trống")]
        public string SizeCode { get; set; }

        public virtual ICollection<ProductColorSize> ProductColorSizes { get; set; }
    }
}
