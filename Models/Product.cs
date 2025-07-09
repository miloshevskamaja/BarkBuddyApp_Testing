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
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name="Price for 1kg")]
        public double Price { get; set; }
        [Display(Name = "Price for 10kg")]
        public double Price2 { get; set; }
        public string ImageUrl { get; set; }
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