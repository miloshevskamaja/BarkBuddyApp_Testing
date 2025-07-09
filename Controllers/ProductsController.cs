using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using BarkBuddyApp.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BarkBuddyApp.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly ApplicationDbContext _context;

        public ProductsController()
        {
            _context = new ApplicationDbContext(); // Initialize your DbContext
        }
       

        // GET: Products
      
        [HttpGet]
        public async Task<ActionResult> Index(string searchString)
        {
            ViewBag.ShowSearchForm = true;
      
            if (string.IsNullOrEmpty(searchString))
            {
                var products = db.Products.Include(p => p.Producer);
                return View(products.ToList());
            }
            else {
                var products = db.Products.Include(p => p.Producer);


                if (!string.IsNullOrEmpty(searchString))
                {
                    // Ensure the search is being applied to the 'Name' property
                    products = products.Where(p => p.Name.Contains(searchString));
                }

                var filteredProducts = await products.ToListAsync();  // Execute query
                return View(filteredProducts);
            }
        }

      

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            Producer nov=db.Producers.Find(product.ProducerId);
            product.Producer = nov;
            string name= product.Producer.Name;
            ViewBag.Name = name;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.ProducerId = new SelectList(db.Producers, "Id", "Name");
            ViewBag.DogBreeds = new MultiSelectList(db.DogBreeds, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Price2,ImageUrl,ProducerId")] Product product, int[] DogBreeds)
        {
            if (ModelState.IsValid)
            {
                Producer producer = db.Producers.Find(product.ProducerId);
               
                if (producer != null)
                {
                    product.Producer = producer;
                }
                product.DogBreeds = db.DogBreeds.Where(b => DogBreeds.Contains(b.Id)).ToList();

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProducerId = new SelectList(db.Producers, "Id", "Name", product.ProducerId);
            ViewBag.DogBreeds = new MultiSelectList(db.DogBreeds, "Id", "Name");
            return View(product);
        }

        public ActionResult DeliveryInfo()
        {

            ShoppingCart shoppingCart = (ShoppingCart)Session["ShoppingCart"];

            
            var model = new OrderViewModel
            {
                Buyer = new Buyer(),
                ShoppingCartItems = shoppingCart.BuyingProducts.Select((product, index) => new ShoppingCartItem
                {
                    ProductName = product,
                    Price = shoppingCart.Prices[index],
                    Quantity = shoppingCart.Quantities[index]
                }).ToList()
            };

            return View(model); 
        }

     
        [HttpPost]
        public ActionResult DeliveryInfo(Buyer buyer)
        {
            ShoppingCart shoppingCart = (ShoppingCart)Session["ShoppingCart"];
            if (ModelState.IsValid)
            {
         
                db.Buyers.Add(buyer);
                db.SaveChanges();

                
                var orderViewModel = new OrderViewModel
                {
                    Buyer = buyer,
                    ShoppingCartItems = shoppingCart.BuyingProducts.Select((product, index) => new ShoppingCartItem
                    {
                        ProductName = product,
                        Price = shoppingCart.Prices[index],
                        Quantity = shoppingCart.Quantities[index]
                        
                    }).ToList()
                };

         
                db.Orders.Add(orderViewModel);
                db.SaveChanges();
                Session["ShoppingCart"] = null;

                

            
                return RedirectToAction("Succ");
            }

            
            return View(buyer);
        }



        public ActionResult Orders()
        {
            var orderData = db.Orders
                               .Include(o => o.Buyer)  
                               .ToList();  

            return View(orderData);
        }




        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Include(p => p.DogBreeds).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProducerId = new SelectList(db.Producers, "Id", "Name", product.ProducerId);
            ViewBag.DogBreeds = new MultiSelectList(db.DogBreeds, "Id", "Name", product.DogBreeds.Select(b => b.Id).ToArray());
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Price2,ImageUrl,ProducerId")] Product product, int[] DogBreeds)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.Include(p => p.DogBreeds).FirstOrDefault(p => p.Id == product.Id);

                if (existingProduct != null)
                {
                    
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Price2 = product.Price2;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.ProducerId = product.ProducerId;

                    existingProduct.DogBreeds.Clear();
                    if (DogBreeds != null && DogBreeds.Any())
                    {
                        existingProduct.DogBreeds = db.DogBreeds.Where(b => DogBreeds.Contains(b.Id)).ToList();
                    }
                    db.Entry(existingProduct).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ProducerId = new SelectList(db.Producers, "Id", "Name", product.ProducerId);
            ViewBag.DogBreeds = new MultiSelectList(db.DogBreeds, "Id", "Name", DogBreeds);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles ="User")]
        public ActionResult AddToCart(int? id, int quantity = 1)
        {
            Console.WriteLine("AddToCart called with Product ID: " + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve or initialize shopping cart from session
            ShoppingCart cart = Session["ShoppingCart"] as ShoppingCart;
            if (cart == null)
            {
                cart = new ShoppingCart();
            }

            // Add the product to the cart
            cart.AddProduct(product.Name, product.Price, quantity);

            // Save the cart back to the session
            Session["ShoppingCart"] = cart;

            return RedirectToAction("Index");  // Redirect back to the product list or another page

        }
        [Authorize(Roles = "User")]
        public ActionResult AddToCart2(int? id, int quantity = 1)
        {
            Console.WriteLine("AddToCart called with Product ID: " + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Retrieve or initialize shopping cart from session
            ShoppingCart cart = Session["ShoppingCart"] as ShoppingCart;
            if (cart == null)
            {
                cart = new ShoppingCart();
            }

            // Add the product to the cart
            cart.AddProduct(product.Name, product.Price2, quantity);

            // Save the cart back to the session
            Session["ShoppingCart"] = cart;

            return RedirectToAction("Index");  // Redirect back to the product list or another page
        }


        [Authorize(Roles = "User")]
        public ActionResult AddToCart3(int? id, int quantity = 1)
        {
            Console.WriteLine("AddToCart called with Product ID: " + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Toys toy = db.Toys.Find(id);
            if (toy == null)
            {
                return HttpNotFound();
            }

           
           ShoppingCart cart = Session["ShoppingCart"] as ShoppingCart;
            if (cart == null)
            {
                cart = new ShoppingCart();
            }

     
            int productIndex = cart.BuyingProducts.IndexOf(toy.Name);
            if (productIndex >= 0)
            {
                
                cart.Quantities[productIndex] += quantity;
            }
            else
            {
              
                cart.AddProduct(toy.Name, toy.Price, quantity);
            }

        
            Session["ShoppingCart"] = cart;

            return RedirectToAction("Index");  
        }

       

        public ActionResult ShoppingCart()
        {
            ShoppingCart cart = Session["ShoppingCart"] as ShoppingCart;
            if (cart == null)
            {
                cart = new ShoppingCart(); 
            }

            return View(cart);
        }
        public ActionResult UpdateQuantity(int index, string action)
        {
            ShoppingCart cart = (ShoppingCart)Session["ShoppingCart"];

            if (cart == null)
            {
               
                return RedirectToAction("ShoppingCart");
            }

            if (action == "increase")
            {
                cart.Quantities[index]++;
            }
            else if (action == "decrease" && cart.Quantities[index] > 1)
            {
                cart.Quantities[index]--;
            }

            Session["ShoppingCart"] = cart;

            return RedirectToAction("ShoppingCart");
        }

        //this func removes product from shopping cart
        [HttpPost]
        public ActionResult RemoveItem(int index)
        {
            ShoppingCart cart = (ShoppingCart)Session["ShoppingCart"];

            cart.BuyingProducts.RemoveAt(index);
            cart.Prices.RemoveAt(index);
            cart.Quantities.RemoveAt(index);

            Session["ShoppingCart"] = cart;
            return RedirectToAction("ShoppingCart");
        }

        //this func removes order from the orders table
        [HttpPost]
        public ActionResult RemoveItem2(int orderId)
        {
            // Retrieve the list of orders from the database
            var orders = db.Orders.ToList(); // Assuming you have a list of orders in your database

            // Find the order to remove
            var orderToRemove = orders.FirstOrDefault(o => o.Id == orderId);

            if (orderToRemove != null)
            {
                // Remove the order from the list
                db.Orders.Remove(orderToRemove);

                // Save changes to the database
                db.SaveChanges();
            }

            // Redirect to the Orders view
            return RedirectToAction("Orders");
        }

        //this func removes reservation from the Grooming Schedule table
        [HttpPost]
        public ActionResult RemoveItem3(int Id)
        {
            // Retrieve the list of orders from the database
            var reservations = db.Groomings.ToList(); // Assuming you have a list of orders in your database

            // Find the order to remove
            var reservationToRemove = reservations.FirstOrDefault(o => o.Id == Id);

            if (reservationToRemove != null)
            {
                // Remove the order from the list
                db.Groomings.Remove(reservationToRemove);

                // Save changes to the database
                db.SaveChanges();
            }

            // Redirect to the Orders view
            return RedirectToAction("Groomings");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpGet]
        public ActionResult ScheduleGrooming()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ScheduleGrooming(Grooming model)
        {
            if (ModelState.IsValid)
            {
                // Check if the selected time is already reserved
                var existingReservation = db.Groomings
                    .FirstOrDefault(r => r.ReservationDateTime == model.ReservationDateTime);

                if (existingReservation != null)
                {
                    ModelState.AddModelError("", "The selected time is already reserved.");
                    return View(model);
                }

                // Save the reservation
              
                db.Groomings.Add(model);
                db.SaveChanges();

                return RedirectToAction("Succ");
            }

            return View(model);
        }

        
        public ActionResult Groomings()
        {
            var reservations = db.Groomings.ToList();
            return View(reservations);
        }

        public ActionResult Succ()
        {
            return View();
        }
    }
}
