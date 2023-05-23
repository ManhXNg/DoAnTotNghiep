namespace WebsiteSellingSport.ModelView
{
    public class OrderDetailView
    {
        public long OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string SizeName   { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
    }
}
