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

namespace Basumaru.Controllers
{
    public class BasuteisController : Controller
    {
        private BasumaruDBContext db = new BasumaruDBContext();

        // GET: Basuteis
        public ActionResult Index()
        {
            return View(db.basutei.ToList());
        }

        // GET: Basuteis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Basutei basutei = db.basutei.Find(id);
            if (basutei == null)
            {
                return HttpNotFound();
            }
            return View(basutei);
        }

        // GET: Basuteis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Basuteis/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BasuteiId,kigyou,rosenmei,basuteimei,ido,keido,yaneFlg,benchiFlg,noriba")] Basutei basutei)
        {
            if (ModelState.IsValid)
            {
                db.basutei.Add(basutei);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(basutei);
        }

        // GET: Basuteis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Basutei basutei = db.basutei.Find(id);
            if (basutei == null)
            {
                return HttpNotFound();
            }
            return View(basutei);
        }

        // POST: Basuteis/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BasuteiId,kigyou,rosenmei,basuteimei,ido,keido,yaneFlg,benchiFlg,noriba")] Basutei basutei)
        {
            if (ModelState.IsValid)
            {
                db.Entry(basutei).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(basutei);
        }

        // GET: Basuteis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Basutei basutei = db.basutei.Find(id);
            if (basutei == null)
            {
                return HttpNotFound();
            }
            return View(basutei);
        }

        // POST: Basuteis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Basutei basutei = db.basutei.Find(id);
            db.basutei.Remove(basutei);
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
