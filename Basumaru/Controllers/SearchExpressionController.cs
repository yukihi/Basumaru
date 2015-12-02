using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Basumaru.Models;
using System.Data.Entity;
//using System.Data.Objects.SqlClient;

namespace Basumaru.Controllers
{
    public class SearchExpressionController : Controller
    {

        private BasumaruDBContext db = new BasumaruDBContext();

        public ActionResult SearchExpressionError(string route)
        {
            return View();
        }

        public ActionResult SearchExpressionBasuteiError(string route)
        {
            return View();
        }

        public ActionResult SearchExpressionHidukeError(string route)
        {
            return View();
        }

        // GET: SearchExpression
        public ActionResult SearchExpression(string route)
        {
            if ((string)Session["starterror"] == "1" || (string)Session["goalerror"] == "1" || (string)Session["sameerror"] == "1")
            {
                return RedirectToAction("SearchExpressionBasuteiError", "SearchExpression"); ;
            }
            else if ((string)Session["hidukeerror"] == "1")
            {
                return RedirectToAction("SearchExpressionHidukeError", "SearchExpression");
            }

            string hourtemp = (string)Session["hour"];
            int hourtemp2 = int.Parse(hourtemp);
            string minutetemp = (string)Session["minute"];
            int minutetemp2 = int.Parse(minutetemp);

            string oktime;
            oktime = (string)Session["hour"] + (string)Session["minute"];//現在時刻
            if (hourtemp2 < 10 & 10 <= minutetemp2)
            {
                oktime = "0" + (string)Session["hour"] + (string)Session["minute"];//6時等を06時に変更
            }
            else if (minutetemp2 < 10 & 10 <= hourtemp2)
            {
                oktime = (string)Session["hour"] + "0" + (string)Session["minute"];//6分等を06分に変更
            }
            else if (minutetemp2 < 10 & hourtemp2 < 10)
            {
                oktime = "0" + (string)Session["hour"] + "0" + (string)Session["minute"];
            }
            else if (minutetemp2 >= 10 & hourtemp2 >= 10)
            {
                oktime = (string)Session["hour"] + (string)Session["minute"];
            }

            string kizyuntemp = (string)Session["kijun"];
            int flag = int.Parse(kizyuntemp);//0が出発基準　1が到着基準
            string stin = (string)Session["start"];//出発バス停名
            string glin = (string)Session["goal"];//到着バス停名

            /*日時から日付分類コードを割り出す 入力が現在より若い日付なら年を+1する*/
            string temp1 = (string)Session["month"];
            int ansmonth = int.Parse(temp1);
            string temp2 = (string)Session["day"];
            int ansday = int.Parse(temp2);

            // 必要な変数を宣言する
            DateTime dtNow = DateTime.Now;
            //現在日時を取得する
            int iYear = dtNow.Year;
            int imonth = dtNow.Day;
            int iday = dtNow.Month;

            DateTime dateValue = new DateTime(iYear, ansmonth, ansday);
            if ((imonth < ansmonth) || ((imonth == ansmonth) && (iday < ansday)))
            {
                dateValue = new DateTime(iYear + 1, ansmonth, ansday);//未来の日付が選択された場合
            }
            else
            {
                dateValue = new DateTime(iYear, ansmonth, ansday);
            }

            HolidayChecker.HolidayInfo hi = HolidayChecker.Holiday(dateValue);




            int day = (int)dateValue.DayOfWeek;//日が0　土が6
            string daycode = "";//日付分類コード
            if (0 < day && day < 6)
            {
                daycode = "0"; //平日
            }
            else if (day == 6)//土曜なら
            {
                daycode = "1";
            }
            else if (day == 0)//日曜なら
            {
                daycode = "4";
            }
            /*祝日ならここに入る*/
            if (hi.holiday == HolidayChecker.HolidayInfo.HOLIDAY.SYUKUJITSU || hi.holiday == HolidayChecker.HolidayInfo.HOLIDAY.C_HOLIDAY)
            {
                daycode = "4";
            }
 
            if (flag == 0)/*ここから下は出発場所基準*/
            {

                /*ここ検索 出発時刻基準で指定されたバス停の中で近い乗車時刻を探す*/
                /*p.zikoku.CompareTo(oktime)>0　oktimeより大きいやつを割り出し*/
                var start = from p in db.jikokuhyou
                            where (p.basuteimei == stin) & (p.zikoku.CompareTo(oktime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)/*oktime以上を導出*/
                            orderby p.zikoku
                            select p;

                var goal = from p in db.jikokuhyou
                           where (p.basuteimei == glin) & (p.zikoku.CompareTo(oktime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                           orderby p.zikoku
                           select p;

                string ans = "12";
                string[] srosenmei = new string[100];
                string[] sikisaki = new string[100];
                string[] szikoku = new string[100];

                int a = 0;
                foreach(var item1 in start)
                {
                    srosenmei[a] = item1.rosenmei;
                    sikisaki[a] = item1.ikisaki;
                    szikoku[a] = item1.zikoku;
                    a++;
                }

                string[] grosenmei = new string[100];
                string[] gikisaki = new string[100];
                string[] gzikoku = new string[100];

                int b = 0;
                foreach(var item2 in goal)
                {
                    grosenmei[b] = item2.rosenmei;
                    gikisaki[b] = item2.ikisaki;
                    gzikoku[b] = item2.zikoku;
                    b++;
                }

                var f = 1;
                for(int n=0; n<sikisaki.Length; n++)
                {
                    for(int m=0; m<gikisaki.Length; m++)
                    {
                        if (sikisaki[n] != null & gikisaki[m] != null)
                        {
                            if ((srosenmei[n] == grosenmei[m]) & (sikisaki[n] == gikisaki[m]))
                            {
                                f = 0;
                                break;
                            }
                        }
                    }
                }

                Session["f"] = f;

                if (f == 0){
                    for (int k = 0; k < szikoku.Length; k++)
                    {
                        for (int l = 0; l < gzikoku.Length; l++) {
                            if (srosenmei[k] == grosenmei[l] & sikisaki[k] == gikisaki[l])
                            {
                                if (gzikoku[l] != null & szikoku[k] != null)
                                {
                                    if (int.Parse(gzikoku[l]) > int.Parse(szikoku[k]))
                                    {
                                        ans = sikisaki[k];
                                        break;
                                    }
                                    else
                                    {
                                        if (int.Parse(szikoku[k].Substring(0, 2)) == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                            //int.Parse(szikoku[k].Substring(0, 2)) - 1 == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                            int.Parse(szikoku[k].Substring(0, 2)) + 1 == int.Parse(gzikoku[l].Substring(0, 2)))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var start2 = from p in db.jikokuhyou
                                 where (p.basuteimei == stin) & (p.ikisaki == ans) & (p.zikoku.CompareTo(oktime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)/*oktime以上を導出*/
                                 orderby p.zikoku
                                 select p;

                    string[] sz = new string[100];
                    int s = 0;
                    foreach (var item in start2)
                    {
                        sz[s] = item.zikoku;
                        if (sz[s] == null) break;
                        s++;
                    }
                    Session["s"] = s;

                    string rosen = "";

                    foreach (var item in start2)
                    {
                        rosen = item.rosenmei;//同じ路線で検索するので路線名を格納
                        break;
                    }

                    string starttime = "";

                    foreach (var item in start2)
                    {
                        starttime = item.zikoku;//基準となるバス停の乗車時刻を格納
                        break;
                    }

                    /*変換
                    int sttemp = int.Parse(starttime);
                    if (sttemp < 1000)
                    {
                        starttime = "0" + starttime;//6時等を06時に変更
                    }*/

                    var goal2 = from p in db.jikokuhyou
                                where (p.basuteimei == glin) & (p.ikisaki == ans) & (p.rosenmei.CompareTo(rosen) == 0) & (p.zikoku.CompareTo(starttime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                orderby p.zikoku
                                select p;

                    if (start2.Count() < 1 || goal2.Count() < 1)//データが見つかったかどうか判定
                    {
                        Session["ansbasuteimei"] = "ルートが見つかりませんでした。";
                        Session["ansgbasuteimei"] = "";
                        Session["anszikoku"] = "";
                        Session["ansgzikoku"] = "";
                        Session["ansrosenmei"] = "";
                        Session["ansnnbasuteimei"] = "";
                        Session["ansnnzikoku"] = "";
                    }
                    else
                    {
                        int i = 0;
                        if (route == null)
                        {
                            i = 0;//int.Parse(route);
                        }
                        else
                        {
                            i = int.Parse(route);
                        }

                        foreach (var item in start2)
                        {
                            if ((s-1) < i)
                            {
                                Session["anszikoku"] = "";
                                Session["ansnzikoku"] = "";
                                Session["ansbasuteimei"] = "";
                                Session["ansnbasuteimei"] = "";
                                Session["ansrosenmei"] = "";
                                break;
                            }
                            // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成され
                            Session["kigyou"] = item.kigyou;
                            Session["ansrosenmei"] = item.rosenmei;
                            Session["ansbasuteimei"] = item.basuteimei;
                            Session["ansikisaki"] = item.ikisaki;
                            Session["anshidukebunrui"] = item.hidukebunrui;
                            Session["anszikoku_"] = item.zikoku;
                            Session["anszikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            string temp = item.hachakuKubun;
                            int hatemp = int.Parse(temp);
                            if (0 < hatemp)
                            {
                                Session["hachakuKubun"] = item.hachakuKubun;
                            }
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if (route == null)
                        {
                            i = 0;//int.Parse(route);
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach (var item in goal2)
                        {
                            if ((s - 1) < i)
                            {
                                Session["anszikoku"] = "";
                                Session["ansgzikoku"] = "";
                                Session["ansbasuteimei"] = "";
                                Session["ansgbasuteimei"] = "";
                                Session["ansrosenmei"] = "";
                                break;
                            }

                            Session["ansgbasuteimei"] = item.basuteimei;
                            Session["ansnbasuteimei"] = item.basuteimei;
                            Session["ansgzikoku_"] = item.zikoku;
                            Session["ansnzikoku_"] = item.zikoku;
                            Session["ansgzikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            Session["ansnnbasuteimei"] = "なし";
                            Session["ansnnzikoku"] = "なし";
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                    }
                } else if(f == 1){
                    Session["anszikoku"] = "";
                    Session["ansbasuteimei"] = "";
                    Session["hachakuKubun"] = "";
                    Session["ansrosenmei"] = "";
                    Session["ansgzikoku"] = "";
                    Session["ansgbasuteimei"] = "";
                    Session["ansnbasuteimei"] = "";
                    Session["ansnzikoku"] = "";
                    Session["ansnnbasuteimei"] = "";
                    Session["ansnnzikoku"] = "";

                    var ansnorikae = "12";
                    for (int k = 0; k < srosenmei.Length; k++)
                    {
                        for (int g = 0; g < grosenmei.Length; g++)
                        {
                            var aa = srosenmei[k];
                            var bb = grosenmei[g];

                            if (srosenmei[k] != null & grosenmei[g] != null)
                            {
                                var h = from p in db.basutei
                                        where (p.rosenmei == aa)
                                        select p;

                                string[] sbasuteimei = new string[110];
                                int r = 0;
                                foreach (var it in h)
                                {
                                    sbasuteimei[r] = it.basuteimei;
                                    r++;
                                }

                                var w = from p in db.basutei
                                        where (p.rosenmei == bb)
                                        select p;

                                string[] gbasuteimei = new string[110];
                                int y = 0;
                                foreach (var it in w)
                                {
                                    gbasuteimei[y] = it.basuteimei;
                                    y++;
                                }

                                for (int q = 0; q < sbasuteimei.Length; q++)
                                {
                                    for (int p = 0; p < gbasuteimei.Length; p++)
                                    {
                                        if (sbasuteimei[q] != null & gbasuteimei[p] != null)
                                        {
                                            if (sbasuteimei[q].CompareTo(gbasuteimei[p]) == 0)
                                            {
                                                ansnorikae = sbasuteimei[q];
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var norikae = from p in db.jikokuhyou
                                  where (p.basuteimei == ansnorikae) & (p.zikoku.CompareTo(oktime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                  orderby p.zikoku
                                  select p;

                    string an = "12";
                    string anq = "12";
                    string bn = "12";
                    string bnq = "12";
                    string[] nrosenmei = new string[100];
                    string[] nikisaki = new string[100];
                    string[] nzikoku = new string[100];

                    int u = 0;
                    foreach(var it in norikae)
                    {
                        nrosenmei[u] = it.rosenmei;
                        nikisaki[u] = it.ikisaki;
                        nzikoku[u] = it.zikoku;
                        u++;
                    }

                    for (int k = 0; k < szikoku.Length; k++)
                    {
                        for (int l = 0; l < nzikoku.Length; l++)
                        {
                            if (srosenmei[k] == nrosenmei[l] & sikisaki[k] == nikisaki[l])
                            {
                                if (nzikoku[l] != null & szikoku[k] != null)
                                {
                                    if (int.Parse(nzikoku[l]) > int.Parse(szikoku[k]))
                                    {
                                        an = sikisaki[k];
                                        bn = srosenmei[k];
                                        break;
                                    }
                                    else
                                    {
                                        if (int.Parse(szikoku[k].Substring(0, 2)) == int.Parse(nzikoku[l].Substring(0, 2)) ||
                                            //int.Parse(szikoku[k].Substring(0, 2)) - 1 == int.Parse(nzikoku[l].Substring(0, 2)) ||
                                            int.Parse(szikoku[k].Substring(0, 2)) + 1 == int.Parse(nzikoku[l].Substring(0, 2)))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int k = 0; k < nzikoku.Length; k++)
                    {
                        for (int l = 0; l < gzikoku.Length; l++)
                        {
                            if (nrosenmei[k] == grosenmei[l] & nikisaki[k] == gikisaki[l])
                            {
                                if (gzikoku[l] != null & nzikoku[k] != null)
                                {
                                    if (int.Parse(gzikoku[l]) > int.Parse(nzikoku[k]))
                                    {
                                        anq = nikisaki[k];
                                        bnq = nrosenmei[k];
                                        break;
                                    }
                                    else
                                    {
                                        if (int.Parse(nzikoku[k].Substring(0, 2)) == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                            // int.Parse(nzikoku[k].Substring(0, 2)) - 1 == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                            int.Parse(nzikoku[k].Substring(0, 2)) + 1 == int.Parse(gzikoku[l].Substring(0, 2)))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var snorikae = from p in db.jikokuhyou
                                   where (p.basuteimei == stin) & (p.rosenmei == bn) & (p.ikisaki == an) & (p.zikoku.CompareTo(oktime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)/*oktime以上を導出*/
                                   orderby p.zikoku
                                   select p;

                    string[] ssz = new string[100];
                    int ss = 0;
                    foreach (var item in snorikae)
                    {
                        ssz[ss] = item.zikoku;
                        if (ssz[ss] == null) break;
                        ss++;
                    }
                    Session["s"] = ss;

                    string rosen = "";

                    foreach (var item in snorikae)
                    {
                        rosen = item.rosenmei;//同じ路線で検索するので路線名を格納
                        break;
                    }

                    string starttime = "";

                    foreach (var item in snorikae)
                    {
                        starttime = item.zikoku;//基準となるバス停の乗車時刻を格納
                        break;
                    }

                    /*変換
                    int sttemp = int.Parse(starttime);
                    if (sttemp < 1000)
                    {
                        starttime = "0" + starttime;//6時等を06時に変更
                    }*/

                    var nnorikae = from p in db.jikokuhyou
                                   where (p.basuteimei == ansnorikae) & (p.rosenmei == bn) & (p.ikisaki == an) & (p.rosenmei.CompareTo(rosen) == 0) & (p.zikoku.CompareTo(starttime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                   orderby p.zikoku
                                   select p;

                    string norikaetime = "";

                    foreach (var item in nnorikae)
                    {
                        norikaetime = item.zikoku;
                        break;
                    }

                    var nnnorikae = from p in db.jikokuhyou
                                    where (p.basuteimei == ansnorikae) & (p.rosenmei == bnq) & (p.ikisaki == anq) & (p.zikoku.CompareTo(norikaetime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                    orderby p.zikoku
                                    select p;

                    string[] ggz = new string[100];
                    int gg = 0;
                    foreach (var item in nnnorikae)
                    {
                        ggz[gg] = item.zikoku;
                        if (ggz[gg] == null) break;
                        gg++;
                    }

                    if (gg <= ss) Session["s"] = gg;
                    else Session["s"] = ss;

                    string rosen2 = "";
                    
                    foreach(var item in nnnorikae)
                    {
                        rosen2 = item.rosenmei;
                        break;
                    }

                    string goaltime = "";

                    foreach(var item in nnnorikae)
                    {
                        goaltime = item.zikoku;
                        break;
                    }

                    var gnorikae = from p in db.jikokuhyou
                                   where (p.basuteimei == glin) & (p.rosenmei == bnq) & (p.ikisaki == anq) & (p.rosenmei.CompareTo(rosen2) == 0) & (p.zikoku.CompareTo(goaltime) > 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                   orderby p.zikoku
                                   select p; 

                    if (snorikae.Count() < 1 || nnorikae.Count() < 1 || nnnorikae.Count() < 1 || gnorikae.Count() < 1)//データが見つかったかどうか判定
                    {
                        Session["ansbasuteimei"] = "ルートが見つかりませんでした。";
                        Session["ansnnbasuteimei"] = "";
                        Session["anszikoku"] = "";
                        Session["ansnzikoku"] = "";
                        Session["ansrosenmei"] = "";
                        Session["ansgbasuteimei"] = "";
                        Session["ansgzikoku"] = "";
                    }
                    else
                    {
                        int i = 0;
                        var sz = "0";
                        if (route == null)
                        {
                            i = 0;//int.Parse(route);
                        }
                        else
                        {
                            i = int.Parse(route);
                        }

                        foreach (var item in snorikae)
                        {   
                            if(sz == item.zikoku)
                            {
                                Session["anszikoku"] = "";
                                Session["ansgzikoku"] = "";
                                break;
                            }
                             
                            // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成され
                            Session["kigyou"] = item.kigyou;
                            Session["ansrosenmei"] = item.rosenmei;
                            Session["ansbasuteimei"] = item.basuteimei;
                            Session["ansikisaki"] = item.ikisaki;
                            Session["anshidukebunrui"] = item.hidukebunrui;
                            Session["anszikoku_"] = item.zikoku;
                            Session["anszikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            string temp = item.hachakuKubun;
                            int hatemp = int.Parse(temp);
                            if (0 < hatemp)
                            {
                                Session["hachakuKubun"] = item.hachakuKubun;
                            }
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if(route == null)
                        {
                            i = 0;
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach(var item in nnorikae)
                        {
                            Session["ansnbasuteimei"] = item.basuteimei;
                            Session["ansnzikoku_"] = item.zikoku;
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if (route == null)
                        {
                            i = 0;//int.Parse(route);
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach (var item in nnnorikae)
                        {
                            Session["ansnnrosenmei"] = item.rosenmei;
                            Session["ansnnbasuteimei"] = item.basuteimei;
                            Session["ansnnikisaki"] = item.ikisaki;
                            Session["ansnnhidukebunrui"] = item.hidukebunrui;
                            Session["ansnnzikoku_"] = item.zikoku;
                            Session["ansnnzikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if(route == null)
                        {
                            i = 0;
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach(var item in gnorikae)
                        {
                            Session["ansgbasuteimei"] = item.basuteimei;
                            Session["ansgzikoku_"] = item.zikoku;
                            Session["ansgzikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            if(i == 0)
                            {
                                break;
                            }
                            i--;
                        }
                    }
                }



            }
            else/*ここから下は到着基準*/
            {
                /*ここ検索 出発時刻基準で指定されたバス停の中で近い乗車時刻を探す*/
                /*p.zikoku.CompareTo(oktime)<0　oktimeより大きいやつを割り出し*/
                var start = from p in db.jikokuhyou
                            where (p.basuteimei == stin) & (p.zikoku.CompareTo(oktime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)/*oktime以上を導出*/
                            orderby p.zikoku
                            select p;

                var goal = from p in db.jikokuhyou
                           where (p.basuteimei == glin) & (p.zikoku.CompareTo(oktime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                           orderby p.zikoku
                           select p;

                string ans = "12";
                string ans2 = "12";
                string[] srosenmei = new string[100];
                string[] sikisaki = new string[100];
                string[] szikoku = new string[100];

                int a = 0;
                foreach (var item1 in start)
                {
                    srosenmei[a] = item1.rosenmei;
                    sikisaki[a] = item1.ikisaki;
                    szikoku[a] = item1.zikoku;
                    a++;
                }

                string[] grosenmei = new string[100];
                string[] gikisaki = new string[100];
                string[] gzikoku = new string[100];

                int b = 0;
                foreach (var item2 in goal)
                {
                    grosenmei[b] = item2.rosenmei;
                    gikisaki[b] = item2.ikisaki;
                    gzikoku[b] = item2.zikoku;
                    b++;
                }

                var f = 1;
                for (int n = 0; n < sikisaki.Length; n++)
                {
                    for (int m = 0; m < gikisaki.Length; m++)
                    {
                        if (sikisaki[n] != null & gikisaki[m] != null)
                        {
                            if ((srosenmei[n] == grosenmei[m]) & (sikisaki[n] == gikisaki[m]))
                            {
                                f = 0;
                                break;
                            }
                        }
                    }
                }

                Session["f"] = f;

                if(f == 0) { 
                for (int k = 0; k < sikisaki.Length; k++)
                {
                    for (int l = 0; l < gikisaki.Length; l++)
                    {
                        if (srosenmei[k] == grosenmei[l] & sikisaki[k] == gikisaki[l])
                        {
                            if (gzikoku[l] != null & szikoku[k] != null)
                            {
                                if (int.Parse(gzikoku[l]) > int.Parse(szikoku[k]))
                                {
                                    ans = sikisaki[k];
                                    ans2 = srosenmei[k]; 
                                    break;
                                }
                                else
                                {
                                    if (int.Parse(szikoku[k].Substring(0, 2)) == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                        //int.Parse(szikoku[k].Substring(0, 2)) - 1 == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                        int.Parse(szikoku[k].Substring(0, 2)) + 1 == int.Parse(gzikoku[l].Substring(0, 2)))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                var goal2 = from p in db.jikokuhyou
                           where (p.basuteimei == glin) & (p.ikisaki == ans) & (p.zikoku.CompareTo(oktime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                           orderby p.zikoku descending
                           select p;

                string[] sz = new string[100];
                int s = 0;
                foreach (var item in goal2)
                {
                    sz[s] = item.zikoku;
                        if (sz[s] == null) break;
                        s++;
                }
                Session["s"] = s;

                string rosen = "";
                foreach (var item in goal2)
                {
                    rosen = item.rosenmei;
                    break;
                }

                string starttime = "";
                foreach (var item in goal2)
                {
                    starttime = item.zikoku;
                    break;
                }

                var start2 = from p in db.jikokuhyou
                            where (p.basuteimei == stin) & (p.ikisaki == ans) & (p.rosenmei == ans2) & (p.zikoku.CompareTo(starttime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                            orderby p.zikoku descending
                            select p;

                if (start2.Count() < 1 || goal2.Count() < 1)//データが見つかったかどうか判定
                {
                    Session["ansbasuteimei"] = "ルートが見つかりませんでした。";
                    Session["ansgbasuteimei"] = "";
                    Session["anszikoku"] = "";
                    Session["ansgzikoku"] = "";
                    Session["ansrosenmei"] = "";
                    Session["ansnnbasuteimei"] = "";
                    Session["ansnnzikoku"] = "";
                }
                else
                {

                    int i = 0;
                    if (route == null)
                    {
                        i = 0;//int.Parse(route);
                    }
                    else
                    {
                        i = int.Parse(route);
                    }

                    foreach (var item in start2)
                    {
                        // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成される
                        Session["kigyou"] = item.kigyou;
                        Session["ansrosenmei"] = item.rosenmei;
                        Session["ansbasuteimei"] = item.basuteimei;
                        Session["ansikisaki"] = item.ikisaki;
                        Session["anshidukebunrui"] = item.hidukebunrui;
                        Session["anszikoku_"] = item.zikoku;
                        Session["anszikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                        string temp = item.hachakuKubun;
                        int hatemp = int.Parse(temp);
                        if (0 < hatemp)
                        {
                            Session["hachakuKubun"] = item.hachakuKubun;
                        }
                        if (i == 0)
                        {
                            break;
                        }
                        i--;
                    }

                    i = 0;
                    if (route == null)
                    {
                        i = 0;//int.Parse(route);
                    }
                    else
                    {
                        i = int.Parse(route);
                    }

                    foreach (var item in goal2)
                    {
                        if(item.ikisaki == ans & item.rosenmei == ans2) {
                            // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成される
                            Session["ansgbasuteimei"] = item.basuteimei;
                            Session["ansnbasuteimei"] = item.basuteimei;
                            Session["ansgzikoku_"] = item.zikoku;
                            Session["ansnzikoku_"] = item.zikoku;
                            Session["ansgzikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            Session["ansnnbasuteimei"] = "なし";
                            Session["ansnnzikoku"] = "なし";
                        }
                        if (i == 0)
                        {
                            break;
                        }
                        i--;
                    }
                }

            }else if(f == 1)
                {
                    Session["anszikoku"] = "";
                    Session["ansbasuteimei"] = "";
                    Session["hachakuKubun"] = "";
                    Session["ansrosenmei"] = "";
                    Session["ansgzikoku"] = "";
                    Session["ansgbasuteimei"] = "";
                    Session["ansnbasuteimei"] = "";
                    Session["ansnzikoku"] = "";
                    Session["ansnnbasuteimei"] = "";
                    Session["ansnnzikoku"] = "";

                    var ansnorikae = "12";
                    for (int k = 0; k < srosenmei.Length; k++)
                    {
                        for (int g = 0; g < grosenmei.Length; g++)
                        {
                            var aa = srosenmei[k];
                            var bb = grosenmei[g];

                            if (srosenmei[k] != null & grosenmei[g] != null)
                            {
                                var h = from p in db.basutei
                                        where (p.rosenmei == aa)
                                        select p;

                                string[] sbasuteimei = new string[110];
                                int r = 0;
                                foreach (var it in h)
                                {
                                    sbasuteimei[r] = it.basuteimei;
                                    r++;
                                }

                                var w = from p in db.basutei
                                        where (p.rosenmei == bb)
                                        select p;

                                string[] gbasuteimei = new string[110];
                                int y = 0;
                                foreach (var it in w)
                                {
                                    gbasuteimei[y] = it.basuteimei;
                                    y++;
                                }

                                for (int q = 0; q < sbasuteimei.Length; q++)
                                {
                                    for (int p = 0; p < gbasuteimei.Length; p++)
                                    {
                                        if (sbasuteimei[q] != null & gbasuteimei[p] != null)
                                        {
                                            if (sbasuteimei[q].CompareTo(gbasuteimei[p]) == 0)
                                            {
                                                ansnorikae = sbasuteimei[q];
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var norikae = from p in db.jikokuhyou
                                  where (p.basuteimei == ansnorikae) & (p.zikoku.CompareTo(oktime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                  orderby p.zikoku 
                                  select p;

                    string an = "12";
                    string anq = "12";
                    string bn = "12";
                    string bnq = "12";
                    string[] nrosenmei = new string[100];
                    string[] nikisaki = new string[100];
                    string[] nzikoku = new string[100];

                    int u = 0;
                    foreach (var it in norikae)
                    {
                        nrosenmei[u] = it.rosenmei;
                        nikisaki[u] = it.ikisaki;
                        nzikoku[u] = it.zikoku;
                        u++;
                    }

                    for (int k = 0; k < szikoku.Length; k++)
                    {
                        for (int l = 0; l < nzikoku.Length; l++)
                        {
                            if (srosenmei[k] == nrosenmei[l] & sikisaki[k] == nikisaki[l])
                            {
                                if (nzikoku[l] != null & szikoku[k] != null)
                                {
                                    if (int.Parse(nzikoku[l]) > int.Parse(szikoku[k]))
                                    {
                                        an = sikisaki[k];
                                        bn = srosenmei[k];
                                        break;
                                    }
                                    else
                                    {
                                        if (int.Parse(szikoku[k].Substring(0, 2)) == int.Parse(nzikoku[l].Substring(0, 2)) ||
                                            //int.Parse(nzikoku[k].Substring(0, 2)) - 1 == int.Parse(gzikoku[l].Substring(0, 2)) || 
                                            int.Parse(szikoku[k].Substring(0, 2)) + 1 == int.Parse(nzikoku[l].Substring(0, 2)))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int k = 0; k < nzikoku.Length; k++)
                    {
                        for (int l = 0; l < gzikoku.Length; l++)
                        {
                            if (nrosenmei[k] == grosenmei[l] & nikisaki[k] == gikisaki[l])
                            {
                                if (gzikoku[l] != null & nzikoku[k] != null)
                                {
                                    if (int.Parse(gzikoku[l]) > int.Parse(nzikoku[k]))
                                    {
                                        anq = nikisaki[k];
                                        bnq = nrosenmei[k];
                                        break;
                                    }
                                    else
                                    {
                                        if (int.Parse(nzikoku[k].Substring(0, 2)) == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                            //int.Parse(nzikoku[k].Substring(0, 2)) - 1 == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                            int.Parse(nzikoku[k].Substring(0, 2)) + 1 == int.Parse(gzikoku[l].Substring(0, 2)))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var gnorikae = from p in db.jikokuhyou
                                   where (p.basuteimei == glin) & (p.rosenmei == bnq) & (p.ikisaki == anq) & (p.zikoku.CompareTo(oktime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                   orderby p.zikoku descending
                                   select p;

                    string[] ssz = new string[100];
                    int ss = 0;
                    foreach (var item in gnorikae)
                    {
                        ssz[ss] = item.zikoku;
                        if (ssz[ss] == null) break;
                        ss++;
                    }

                    string rosen2 = "";

                    foreach (var item in gnorikae)
                    {
                        rosen2 = item.rosenmei;//同じ路線で検索するので路線名を格納
                        break;
                    }

                    string goaltime = "";

                    foreach (var item in gnorikae)
                    {
                        goaltime = item.zikoku;//基準となるバス停の乗車時刻を格納
                        break;
                    }

                    /*変換
                    int sttemp = int.Parse(starttime);
                    if (sttemp < 1000)
                    {
                        starttime = "0" + starttime;//6時等を06時に変更
                    }*/

                    var nnnorikae = from p in db.jikokuhyou
                                    where (p.basuteimei == ansnorikae) & (p.rosenmei == bnq) & (p.ikisaki == anq) & (p.rosenmei.CompareTo(rosen2) == 0) & (p.zikoku.CompareTo(goaltime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                    orderby p.zikoku descending
                                    select p;

                    string norikaetime = "";

                    foreach (var item in nnnorikae)
                    {
                        norikaetime = item.zikoku;
                        break;
                    }

                    var nnorikae = from p in db.jikokuhyou
                                   where (p.basuteimei == ansnorikae) & (p.rosenmei == bn) & (p.ikisaki == an) & (p.zikoku.CompareTo(norikaetime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                   orderby p.zikoku descending
                                   select p;

                    string[] ggz = new string[100];
                    int gg = 0;
                    foreach (var item in nnorikae)
                    {
                        ggz[gg] = item.zikoku;
                        if (ggz[gg] == null) break;
                        gg++;
                    }

                    if (gg <= ss) Session["s"] = gg;
                    else Session["s"] = ss;

                    string rosen = "";

                    foreach (var item in nnorikae)
                    {
                        rosen = item.rosenmei;
                        break;
                    }

                    string starttime = "";

                    foreach (var item in nnorikae)
                    {
                        starttime = item.zikoku;
                        break;
                    }

                    var snorikae = from p in db.jikokuhyou
                                   where (p.basuteimei == stin) & (p.rosenmei == bn) & (p.ikisaki == an) & (p.rosenmei.CompareTo(rosen) == 0) & (p.zikoku.CompareTo(starttime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                                   orderby p.zikoku descending
                                   select p;

                    if (snorikae.Count() < 1 || nnorikae.Count() < 1 || nnnorikae.Count() < 1 || gnorikae.Count() < 1)//データが見つかったかどうか判定
                    {
                        Session["ansbasuteimei"] = "ルートが見つかりませんでした。";
                        Session["ansnnbasuteimei"] = "";
                        Session["anszikoku"] = "";
                        Session["ansnzikoku"] = "";
                        Session["ansrosenmei"] = "";
                        Session["ansgbasuteimei"] = "";
                        Session["ansgzikoku"] = "";
                    }
                    else
                    {
                        int i = 0;
                        if (route == null)
                        {
                            i = 0;//int.Parse(route);
                        }
                        else
                        {
                            i = int.Parse(route);
                        }

                        foreach (var item in snorikae)
                        {
                            // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成され
                            Session["kigyou"] = item.kigyou;
                            Session["ansrosenmei"] = item.rosenmei;
                            Session["ansbasuteimei"] = item.basuteimei;
                            Session["ansikisaki"] = item.ikisaki;
                            Session["anshidukebunrui"] = item.hidukebunrui;
                            Session["anszikoku_"] = item.zikoku;
                            Session["anszikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            string temp = item.hachakuKubun;
                            int hatemp = int.Parse(temp);
                            if (0 < hatemp)
                            {
                                Session["hachakuKubun"] = item.hachakuKubun;
                            }
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if (route == null)
                        {
                            i = 0;
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach (var item in nnorikae)
                        {
                            Session["ansnbasuteimei"] = item.basuteimei;
                            Session["ansnzikoku_"] = item.zikoku;
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if (route == null)
                        {
                            i = 0;//int.Parse(route);
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach (var item in nnnorikae)
                        {
                            Session["ansnnrosenmei"] = item.rosenmei;
                            Session["ansnnbasuteimei"] = item.basuteimei;
                            Session["ansnnikisaki"] = item.ikisaki;
                            Session["ansnnhidukebunrui"] = item.hidukebunrui;
                            Session["ansnnzikoku_"] = item.zikoku;
                            Session["ansnnzikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }

                        i = 0;
                        if (route == null)
                        {
                            i = 0;
                        }
                        else
                        {
                            i = int.Parse(route);
                        }
                        foreach (var item in gnorikae)
                        {
                            Session["ansgbasuteimei"] = item.basuteimei;
                            Session["ansgzikoku_"] = item.zikoku;
                            Session["ansgzikoku"] = item.zikoku.Substring(0, 2) + ":" + item.zikoku.Substring(2, 2);
                            if (i == 0)
                            {
                                break;
                            }
                            i--;
                        }
                    }
                }

            }

            if ((string)Session["ansbasuteimei"] == "ルートが見つかりませんでした。")
            {
                return RedirectToAction("SearchExpressionError", "SearchExpression");
            }
            else
            {
                return View();
            }

            //return View();
        }
    }
}