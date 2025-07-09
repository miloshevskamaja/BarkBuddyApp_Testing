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
    public class GroomingDogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GroomingDogs
        public ActionResult Index()
        {
            return View(db.GroomingDogs.ToList());
        }

        // GET: GroomingDogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroomingDog groomingDog = db.GroomingDogs.Find(id);
            if (groomingDog == null)
            {
                return HttpNotFound();
            }
            return View(groomingDog);
        }

        // GET: GroomingDogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroomingDogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ImageUrl,PriceForGrooming")] GroomingDog groomingDog)
        {
            if (ModelState.IsValid)
            {
                db.GroomingDogs.Add(groomingDog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groomingDog);
        }

        // GET: GroomingDogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroomingDog groomingDog = db.GroomingDogs.Find(id);
            if (groomingDog == null)
            {
                return HttpNotFound();
            }
            return View(groomingDog);
        }

        // POST: GroomingDogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ImageUrl,PriceForGrooming")] GroomingDog groomingDog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groomingDog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groomingDog);
        }

        // GET: GroomingDogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroomingDog groomingDog = db.GroomingDogs.Find(id);
            if (groomingDog == null)
            {
                return HttpNotFound();
            }
            return View(groomingDog);
        }

        // POST: GroomingDogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroomingDog groomingDog = db.GroomingDogs.Find(id);
            db.GroomingDogs.Remove(groomingDog);
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
