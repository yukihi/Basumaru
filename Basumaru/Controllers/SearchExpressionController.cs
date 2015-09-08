using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Basumaru.Models;
using System.Data.Entity;

namespace Basumaru.Controllers
{
    public class SearchExpressionController : Controller
    {

        private BasumaruDBContext db = new BasumaruDBContext();


        // GET: SearchExpression
        public ActionResult SearchExpression()
        {
            //ViewBag.MyMessage = "橋本" + (String)Session["pppp"];

            int oktime = 1800;//現在時刻
            string anstime;//現在時刻に近い乗車時刻
            int[] time = new int[100];//時刻を格納

            /*ここ検索
            var result = from p in db.jikokuhyou
                         where p.basuteimei == "郡山駅"
                         select p;
            ここまで*/

            /*指定の時間に近い時間を割り出す　ここから*
            int i = 0;
            foreach (var item in result)
            {
                time[i] = int.Parse(item.zikoku);
                i++;
            }

            int sa = time[0] - oktime;
            int ans = 0;
            for (i = 1; i < 3; i++)
            {
                if (oktime < time[i])
                {
                    if ((time[i] - oktime) < sa)
                    {
                        sa = time[i] - oktime;
                        ans = i;
                    }
                }
            }
            anstime = time[ans].ToString();
            ここまで*/


            /*ここ検索
            var result2 = from k in db.jikokuhyou
                          where k.basuteimei == "郡山駅"
                          where k.zikoku == anstime/*目的の時刻のみ検索
                          select k;
            ここまで*/
            /*
            foreach (var item in result2)
            {
                // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成される
                ViewBag.vkihyou = item.kigyou;
                ViewBag.vrosenmei = item.rosenmei;
                ViewBag.vikisaki = item.ikisaki;
                ViewBag.vhidukebunrui = item.hidukebunrui;
                ViewBag.vbasuteimei = item.basuteimei;
                ViewBag.vzikoku = item.zikoku;
                ViewBag.vhachakuKubun = item.hachakuKubun;
            }
            */

            return View();
        }
    }
}