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

            // 必要な変数を宣言する
            DateTime dtNow = DateTime.Now;

            // 年 (Year) を取得する
            int year = dtNow.Year;
            // Sessionに各データをを格納
            Session["start"] = start;
            Session["goal"] = goal;
            Session["kijun"] = kijun;
            Session["year"] = year;
            Session["month"] = month;
            Session["day"] = day;
            Session["hour"] = hour;
            Session["minute"] = minute;


            // 指定したページを呼び出す
            return RedirectToAction("About");
        }

        public ActionResult SearchExpression()
        {

            ViewBag.MyMessage0 = (string)Session["start"];
            ViewBag.MyMessage1 = (string)Session["goal"];
            ViewBag.MyMessage2 = (string)Session["kijun"];
            ViewBag.MyMessage3 = (string)Session["month"];
            ViewBag.MyMessage4 = (string)Session["day"];
            ViewBag.MyMessage5 = (string)Session["hour"];
            ViewBag.MyMessage6 = (string)Session["minute"];
            return View();

        }

        public ActionResult About()
        {

            ViewBag.MyMessage0 = (string)Session["start"];
            ViewBag.MyMessage1 = (string)Session["goal"];
            ViewBag.MyMessage2 = (string)Session["kijun"];
            ViewBag.MyMessage3 = (string)Session["month"];
            ViewBag.MyMessage4 = (string)Session["day"];
            ViewBag.MyMessage5 = (string)Session["hour"];
            ViewBag.MyMessage6 = (string)Session["minute"];
            return View();

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