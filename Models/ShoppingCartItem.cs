using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }  
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public virtual OrderViewModel Order { get; set; }  
    }
}