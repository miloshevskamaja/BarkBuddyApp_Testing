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
        [Range(1, int.MaxValue, ErrorMessage = "Id must be positive.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "ProductName is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ProductName must be between 2 and 100 characters.")]
        public string ProductName { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public double Price { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "OrderId must be positive.")]
        public int OrderId { get; set; }
        public virtual OrderViewModel Order { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be positive.")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}