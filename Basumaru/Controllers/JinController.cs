using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Basumaru.Models;

namespace Basumaru.Controllers
{
    public class JinController : Controller
    {
        BasumaruDBContext db = new BasumaruDBContext();

        // GET: Jin/Index
        public ActionResult Delete()
        {
            ViewBag.msg = "すべてのデータを削除する場合は、「削除」ボタンをクリックしてください";
            return View();
        }

        // POST: Jin/Index
        [HttpPost]
        //[ValidateAntiForgeryToken]    
        [ValidateInput(false)]
        public ActionResult DeleteAll()
        {

            foreach (Basutei bs in db.basutei.ToList()){
                db.basutei.Remove(bs);
            }
            db.SaveChanges();

            ViewBag.msg = "削除完了！";

            return View("Delete");
        }



    }
}