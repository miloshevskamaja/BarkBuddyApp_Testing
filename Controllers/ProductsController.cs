using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BarkBuddyApp.Models;

namespace BarkBuddyApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController()
        {
            _context = new ApplicationDbContext();
        }

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public ActionResult Index(string searchString)
        {
            ViewBag.ShowSearchForm = true;
            IQueryable<Product> productsQuery = _context.Products;

            if (!string.IsNullOrEmpty(searchString))
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));

            return View(productsQuery.ToList());
        }


        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();

            product.Producer = _context.Producers.Find(product.ProducerId);
            ViewBag.Name = product.Producer?.Name;
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.ProducerId = new SelectList(_context.Producers, "Id", "Name");
            ViewBag.DogBreeds = new MultiSelectList(_context.DogBreeds, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,Price2,ImageUrl,ProducerId")] Product product, int[] DogBreeds)
        {
            if (ModelState.IsValid)
            {
                product.Producer = _context.Producers.Find(product.ProducerId);

                if (DogBreeds != null && DogBreeds.Any())
                    product.DogBreeds = _context.DogBreeds.Where(b => DogBreeds.Contains(b.Id)).ToList();

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProducerId = new SelectList(_context.Producers, "Id", "Name", product.ProducerId);
            ViewBag.DogBreeds = new MultiSelectList(_context.DogBreeds, "Id", "Name");
            return View(product);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

    
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return HttpNotFound();

         
            ViewBag.ProducerId = new SelectList(_context.Producers, "Id", "Name", product.ProducerId);

         
            var dogBreedIds = product.DogBreeds?.Select(b => b.Id).ToArray() ?? new int[0];
            ViewBag.DogBreeds = new MultiSelectList(_context.DogBreeds, "Id", "Name", dogBreedIds);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,Price2,ImageUrl,ProducerId")] Product product, int[] DogBreeds)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.Include(p => p.DogBreeds).FirstOrDefault(p => p.Id == product.Id);
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
                        existingProduct.DogBreeds = _context.DogBreeds.Where(b => DogBreeds.Contains(b.Id)).ToList();

                    _context.Entry(existingProduct).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.ProducerId = new SelectList(_context.Producers, "Id", "Name", product.ProducerId);
            ViewBag.DogBreeds = new MultiSelectList(_context.DogBreeds, "Id", "Name", DogBreeds);
            return View(product);
        }
 
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "User")]
        public ActionResult AddToCart(int? id, int quantity = 1)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = _context.Products.Find(id);
            if (product == null) return HttpNotFound();

            var cart = Session["ShoppingCart"] as ShoppingCart ?? new ShoppingCart();
            cart.AddProduct(product.Name, product.Price, quantity);
            Session["ShoppingCart"] = cart;

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public ActionResult AddToCart2(int? id, int quantity = 1)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = _context.Products.Find(id);
            if (product == null) return HttpNotFound();

            var cart = Session["ShoppingCart"] as ShoppingCart ?? new ShoppingCart();
            cart.AddProduct(product.Name, product.Price2, quantity);
            Session["ShoppingCart"] = cart;

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public ActionResult AddToCart3(int? id, int quantity = 1)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var toy = _context.Toys.Find(id);
            if (toy == null) return HttpNotFound();

            var cart = Session["ShoppingCart"] as ShoppingCart ?? new ShoppingCart();

            int productIndex = cart.BuyingProducts.IndexOf(toy.Name);
            if (productIndex >= 0)
                cart.Quantities[productIndex] += quantity;
            else
                cart.AddProduct(toy.Name, toy.Price, quantity);

            Session["ShoppingCart"] = cart;
            return RedirectToAction("Index");
        }

        public ActionResult ShoppingCart()
        {
            var cart = Session["ShoppingCart"] as ShoppingCart ?? new ShoppingCart();
            return View(cart);
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int index, string action)
        {
            var cart = Session["ShoppingCart"] as ShoppingCart;
            if (cart == null) return RedirectToAction("ShoppingCart");

            if (action == "increase") cart.Quantities[index]++;
            else if (action == "decrease" && cart.Quantities[index] > 1) cart.Quantities[index]--;

            Session["ShoppingCart"] = cart;
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        public ActionResult RemoveItem(int index)
        {
            var cart = Session["ShoppingCart"] as ShoppingCart;
            cart.BuyingProducts.RemoveAt(index);
            cart.Prices.RemoveAt(index);
            cart.Quantities.RemoveAt(index);
            Session["ShoppingCart"] = cart;
            return RedirectToAction("ShoppingCart");
        }

        public ActionResult Orders()
        {
         
            var orders = _context.Orders.ToList(); 
            return View(orders);
        }

        [HttpPost]
        public ActionResult RemoveItem2(int orderId)
        {
      
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }


   
        public ActionResult Groomings()
        {
            return View(_context.Groomings.ToList());
        }

        [HttpPost]
        public ActionResult RemoveItem3(int id)
        {
            var grooming = _context.Groomings.Find(id);
            if (grooming != null)
            {
                _context.Groomings.Remove(grooming);
                _context.SaveChanges();
            }
            return RedirectToAction("Groomings");
        }

        [HttpGet]
        public ActionResult ScheduleGrooming() => View();

        [HttpPost]
        public ActionResult ScheduleGrooming(Grooming model)
        {
            if (ModelState.IsValid)
            {
                var exists = _context.Groomings.FirstOrDefault(g => g.ReservationDateTime == model.ReservationDateTime);
                if (exists != null)
                {
                    ModelState.AddModelError("", "The selected time is already reserved.");
                    return View(model);
                }

                _context.Groomings.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Succ");
            }
            return View(model);
        }

        public ActionResult Succ() => View();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
