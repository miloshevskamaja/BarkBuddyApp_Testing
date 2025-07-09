using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class Grooming
    {
        public int Id { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public string DogBreed { get; set; }
        public int DogAge { get; set; }
        public string Details { get; set; }
    }
}