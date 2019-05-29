using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public partial class OrderItem
    {
        [Key]
        public int ItemId { get; set; }
        public string OrderItemName { get; set; }
        public int OrderItemPrice { get; set; }
        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}
