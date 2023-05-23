namespace WebsiteSellingSport.ModelView
{
    public class CartItem
    {
        
            public long ProductID { get; set; }
            public long ProductCsID { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string ProductImage { get; set; }
            public decimal Total { get; set; }
            
    }
}
