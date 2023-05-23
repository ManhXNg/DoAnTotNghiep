using System;

namespace WebsiteSellingSport.Areas.Admin.ModelView
{
    public class ProductColorSizeView
    {
        public Guid? ID { get; set; }
        //public long? Pcsid { get; set; }     
        //public long? ProductId { get; set; }
        public long? ColorId { get; set; }
        public long? SizeId { get; set; }
        public int? Stock { get; set; }
    }
}
