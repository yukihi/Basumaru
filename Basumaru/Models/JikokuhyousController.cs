using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Basumaru;

namespace Basumaru.Models
{
    public class JikokuhyousController : Controller
    {
        private BasumaruDBContext db = new BasumaruDBContext();

        // GET: Jikokuhyous
        public ActionResult Index()
        {
            return View(db.jikokuhyou.ToList());
        }

        // GET: Jikokuhyous/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jikokuhyou jikokuhyou = db.jikokuhyou.Find(id);
            if (jikokuhyou == null)
            {
                return HttpNotFound();
            }
            return View(jikokuhyou);
        }

        // GET: Jikokuhyous/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Jikokuhyous/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "JikokuhyouId,kigyou,rosenmei,ikisaki,hidukebunrui,basuteimei,zikoku,hachakuKubun")] Jikokuhyou jikokuhyou)
        {
            if (ModelState.IsValid)
            {
                db.jikokuhyou.Add(jikokuhyou);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jikokuhyou);
        }

        // GET: Jikokuhyous/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jikokuhyou jikokuhyou = db.jikokuhyou.Find(id);
            if (jikokuhyou == null)
            {
                return HttpNotFound();
            }
            return View(jikokuhyou);
        }

        // POST: Jikokuhyous/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、http://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JikokuhyouId,kigyou,rosenmei,ikisaki,hidukebunrui,basuteimei,zikoku,hachakuKubun")] Jikokuhyou jikokuhyou)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jikokuhyou).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jikokuhyou);
        }

        // GET: Jikokuhyous/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Jikokuhyou jikokuhyou = db.jikokuhyou.Find(id);
            if (jikokuhyou == null)
            {
                return HttpNotFound();
            }
            return View(jikokuhyou);
        }

        // POST: Jikokuhyous/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Jikokuhyou jikokuhyou = db.jikokuhyou.Find(id);
            db.jikokuhyou.Remove(jikokuhyou);
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
