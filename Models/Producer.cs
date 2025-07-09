using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class Producer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Producer name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Logo URL is required.")]
        [Url(ErrorMessage = "Logo must be a valid URL.")]
        public string Logo { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}