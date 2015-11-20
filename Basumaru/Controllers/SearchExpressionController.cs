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


        // GET: SearchExpression
        public ActionResult SearchExpression(string route)
        {

            string hourtemp = (string)Session["hour"];
            int hourtemp2 = int.Parse(hourtemp);
            string minutetemp = (string)Session["minute"];
            int minutetemp2 = int.Parse(minutetemp);

            string oktime;
            oktime = (string)Session["hour"] + (string)Session["minute"];//現在時刻
            if (hourtemp2 < 10)
            {
                oktime = "0" + (string)Session["hour"] + (string)Session["minute"];//6時等を06時に変更
            }
            if (minutetemp2 < 10)
            {
                oktime = (string)Session["hour"] + "0" + (string)Session["minute"];//6分等を06分に変更
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

                for(int k=0; k<szikoku.Length; k++)
                {
                    for (int l = 0; l < gzikoku.Length; l++){
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
                                    if(int.Parse(szikoku[k].Substring(0,2)) == int.Parse(gzikoku[l].Substring(0,2)) || 
                                        int.Parse(szikoku[k].Substring(0,2))-1 == int.Parse(gzikoku[l].Substring(0,2)) ||
                                        int.Parse(szikoku[k].Substring(0,2))+1 == int.Parse(gzikoku[l].Substring(0, 2)))
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
                    Session["ansgbasuteimei"] = "ルートが見つかりませんでした。";
                    Session["anszikoku"] = "";
                    Session["ansgzikoku"] = "";
                    Session["ansrosenmei"] = "";
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
                                    break;
                                }
                                else
                                {
                                    if (int.Parse(szikoku[k].Substring(0, 2)) == int.Parse(gzikoku[l].Substring(0, 2)) ||
                                        int.Parse(szikoku[k].Substring(0, 2)) - 1 == int.Parse(gzikoku[l].Substring(0, 2)) ||
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
                            where (p.basuteimei == stin) & (p.ikisaki == ans) & (p.rosenmei.CompareTo(rosen) == 0) & (p.zikoku.CompareTo(starttime) < 0) & (p.hidukebunrui.CompareTo(daycode) == 0)
                            orderby p.zikoku descending
                            select p;

                if (goal2.Count() < 1 || goal2.Count() < 1)//データが見つかったかどうか判定
                {
                    Session["ansbasuteimei"] = "ルートが見つかりませんでした。";
                    Session["ansgbasuteimei"] = "ルートが見つかりませんでした。";
                    Session["anszikoku"] = "";
                    Session["ansgzikoku"] = "";
                    Session["ansrosenmei"] = "";
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
                        // Customer プロパティを明示的に読み込む。**Reference プロパティは自動的に生成される
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
            return View();
        }
    }
}