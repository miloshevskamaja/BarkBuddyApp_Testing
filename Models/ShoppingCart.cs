using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarkBuddyApp.Models
{
    public class ShoppingCart
    {
        public List<string> BuyingProducts { get; set; }
        public List<double> Prices { get; set; }
        public List<int> Quantities { get; set; }

        public ShoppingCart()
        {
            BuyingProducts = new List<string>();
            Prices = new List<double>();
            Quantities = new List<int>();
        }

        public void AddProduct(string productName, double price, int quantity)
        {
            BuyingProducts.Add(productName);
            Prices.Add(price);
            Quantities.Add(quantity);
        }
    }
}