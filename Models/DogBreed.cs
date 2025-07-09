using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class DogBreed
    {
        [Key]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be positive.")]
        public int Id { get; set; }
        public string ImageURL { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public DogBreed()
        {
            Products = new HashSet<Product>();
        }
    }
}