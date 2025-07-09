using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BarkBuddyApp.Models;

namespace BarkBuddyApp.Controllers
{
    public class DogBreedsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DogBreeds
        public ActionResult Index()
        {
            return View(db.DogBreeds.ToList());
        }

        // GET: DogBreeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DogBreed dogBreed = db.DogBreeds.Find(id);
            if (dogBreed == null)
            {
                return HttpNotFound();
            }
            return View(dogBreed);
        }

        // GET: DogBreeds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogBreeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ImageURL,Name,Description")] DogBreed dogBreed)
        {
            if (ModelState.IsValid)
            {
                db.DogBreeds.Add(dogBreed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dogBreed);
        }

        // GET: DogBreeds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DogBreed dogBreed = db.DogBreeds.Find(id);
            if (dogBreed == null)
            {
                return HttpNotFound();
            }
            return View(dogBreed);
        }

        // POST: DogBreeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ImageURL,Name,Description")] DogBreed dogBreed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dogBreed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dogBreed);
        }

        // GET: DogBreeds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DogBreed dogBreed = db.DogBreeds.Find(id);
            if (dogBreed == null)
            {
                return HttpNotFound();
            }
            return View(dogBreed);
        }

        // POST: DogBreeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DogBreed dogBreed = db.DogBreeds.Find(id);
            db.DogBreeds.Remove(dogBreed);
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
