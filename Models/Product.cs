using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
        [Display(Name="Price for 1kg")]
        public double Price { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price2 must be positive.")]
        [Display(Name = "Price for 10kg")]
        public double Price2 { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProducerId must be greater than 0.")]
        [Display(Name="Producer")]
        public int ProducerId { get; set; }
        public Producer Producer { get; set; }
     

        public virtual ICollection<DogBreed> DogBreeds { get; set; }
        public Product()
        {
  
            DogBreeds=new HashSet<DogBreed>();
        }
    }
}