using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class Grooming
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be positive.")]
        public int Id { get; set; }
        public DateTime ReservationDateTime { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public string DogBreed { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Age should be >= 0.")]
        public int DogAge { get; set; }
        public string Details { get; set; }
    }
}