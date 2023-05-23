using System;
using System.Collections.Generic;

#nullable disable

namespace WebsiteSellingSport.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryProducts = new HashSet<CategoryProduct>();
        }

        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}
