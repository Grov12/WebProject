using System;
using System.Collections.Generic;

namespace EcommerceProject.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSubCategory { get; set; }
      
    }
}
