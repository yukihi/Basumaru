using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Basumaru.Controllers
{
    public class HomeController : Controller
    {
        public string Minute { get; set; }
        public string Name { get; set; }

        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(string start, string goal, string kijun, string month, string day, string hour, string minute)
        {
            // Sessionに各データをを格納
            Session["start"] = start;
            Session["goal"] = goal;
            Session["kijun"] = kijun;
            Session["month"] = month;
            Session["day"] = day;
            Session["hour"] = hour;
            Session["minute"] = minute;


            // 指定したページを呼び出す
            return RedirectToAction("SearchExpression", "SearchExpression");
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult DataImport()
        {
            return View();
        }

        public ActionResult Busstop()
        {
            return View();
        }

        public ActionResult Route()
        {
            return View();
        }
    }
}