using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Basumaru;
using Basumaru.Models;


namespace Basumaru.Models
{
    public class RosensController : Controller
    {
        private BasumaruDBContext db = new BasumaruDBContext();

        // GET: Rosens
        public ActionResult Index()
        {
            return View(db.rosen.ToList());
        }

        // GET: Rosens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rosen rosen = db.rosen.Find(id);
            if (rosen == null)
            {
                return HttpNotFound();
            }
            return View(rosen);
        }

        // GET: Rosens/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rosens/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RosenId,kigyou,rosenmei,hidukebunrui,kigoui,komento")] Rosen rosen)
        {
            if (ModelState.IsValid)
            {
                db.rosen.Add(rosen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rosen);
        }

        // GET: Rosens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rosen rosen = db.rosen.Find(id);
            if (rosen == null)
            {
                return HttpNotFound();
            }
            return View(rosen);
        }

        // POST: Rosens/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RosenId,kigyou,rosenmei,hidukebunrui,kigoui,komento")] Rosen rosen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rosen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rosen);
        }

        // GET: Rosens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rosen rosen = db.rosen.Find(id);
            if (rosen == null)
            {
                return HttpNotFound();
            }
            return View(rosen);
        }

        // POST: Rosens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rosen rosen = db.rosen.Find(id);
            db.rosen.Remove(rosen);
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
