using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Basumaru.Models;
using System.Collections;
using System.Diagnostics;
using System.Configuration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
namespace Basumaru.Controllers
{
    public class HomeController : Controller
    {
        public string Minute { get; set; }
        public string Name { get; set; }
        private BasumaruDBContext db = new BasumaruDBContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult On_Button1()
        {
            return RedirectToAction("Busstop", "Home");
        }

        [HttpPost]
        public ActionResult Index(string start, string goal, string kijun, string month, string day, string hour, string minute)
        {

            // 必要な変数を宣言する
            DateTime dtNow = DateTime.Now;

            // 年 (Year) を取得する
            int year = dtNow.Year;
            // Sessionに各データをを格納
            // Sessionに各データをを格納
            Session["start"] = start;
            Session["goal"] = goal;
            Session["kijun"] = kijun;
            Session["month"] = month;
            Session["day"] = day;
            Session["hour"] = hour;
            Session["minute"] = minute;

            int iyear = year;
            int imonth = int.Parse(month);
            int iday = int.Parse(day);
            int ihour = int.Parse(hour);
            int iminute = int.Parse(minute);

            // 選択された月のの日数を取得する
            int iDaysInMonth = DateTime.DaysInMonth(iyear, imonth);
            bool hidukeerror = false; //存在しない日がある
            bool starterror = false; //出発に存在しないバス停が選択されている
            bool goalerror = false; //到着に存在しないバス停が選択されている
            bool sameerror = false; //出発と到着が同じ
            //検査
            if (iday > iDaysInMonth)
            {
                hidukeerror = true;

            }

            if (start == goal)
            {
                sameerror = true;

            }


            var cstart = from p in db.basutei
                         where p.basuteimei == start
                         select p;
            if (cstart.Count() < 1)
            {
                starterror = true;

            }


            var cgoal = from p in db.basutei
                        where p.basuteimei == goal
                        select p;
            if (cgoal.Count() < 1)
            {
                goalerror = true;


            }

            Session["starterror"] = "0";
            Session["goalerror"] = "0";
            Session["hidukeerror"] = "0";
            Session["sameerror"] = "0";

            if (hidukeerror == true || starterror == true || goalerror == true　|| sameerror == true)
            {
                if (starterror == true)
                {
                    ViewBag.starterror = "出発バス停が選択されていません<br>";
                    Session["starterror"] = "1";
                }
                if (goalerror == true)
                {
                    ViewBag.goalerror = "到着バス停が選択されていません<br>";
                    Session["goalerror"] = "1";
                }
                if (hidukeerror == true)
                {
                    ViewBag.hidukeerror = "存在しない日付です<br>";
                    Session["hidukeerror"] = "1";
                }
                if (sameerror == true　&& start == goal && starterror != true && goalerror != true)
                {
                    ViewBag.sameerror = "出発と到着のバス停が同じです<br>";
                    Session["sameerror"] = "1";
                }

                //return View("Index");
            }



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