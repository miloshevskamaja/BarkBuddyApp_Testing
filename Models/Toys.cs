using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class Toys
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be positive.")]
        public int Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price should be >= 0.")]
        public double Price { get; set; }
    }
}