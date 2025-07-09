using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class OrderViewModel
    {
        [Key]
        public int Id { get; set; }  
        public Buyer Buyer { get; set; }
        public virtual List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}