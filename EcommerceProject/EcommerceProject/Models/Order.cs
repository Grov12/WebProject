using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItem = new HashSet<OrderItem>();
        }
        [Key]
        public int OrderId { get; set; }
        public int TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public ICollection<OrderItem> OrderItem { get; set; }
    }
}
