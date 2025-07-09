using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BarkBuddyApp.Models;

namespace BarkBuddyApp.Controllers
{
    public class ToysController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Toys
        //public ActionResult Index()
        //{
        //    return View(db.Toys.ToList());
        //}

        public async Task<ActionResult> Index(string searchString)
        {
            ViewBag.ShowSearchForm2 = true;
            var toys = db.Toys.AsQueryable();
            if (string.IsNullOrEmpty(searchString))
            {
               
                return View(toys.ToList());
            }
            else
            {
    
               
                    // Ensure the search is being applied to the 'Name' property
                    toys = toys.Where(p => p.Name.Contains(searchString));
                

                var filteredProducts = await toys.ToListAsync();  // Execute query
                return View(filteredProducts);
            }
        }

        // GET: Toys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Toys toys = db.Toys.Find(id);
            if (toys == null)
            {
                return HttpNotFound();
            }
            return View(toys);
        }

        // GET: Toys/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Toys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ImageUrl,Description,Price")] Toys toys)
        {
            if (ModelState.IsValid)
            {
                db.Toys.Add(toys);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toys);
        }

        // GET: Toys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Toys toys = db.Toys.Find(id);
            if (toys == null)
            {
                return HttpNotFound();
            }
            return View(toys);
        }

        // POST: Toys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ImageUrl,Description,Price")] Toys toys)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toys).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toys);
        }

        // GET: Toys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Toys toys = db.Toys.Find(id);
            if (toys == null)
            {
                return HttpNotFound();
            }
            return View(toys);
        }

        // POST: Toys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Toys toys = db.Toys.Find(id);
            db.Toys.Remove(toys);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
