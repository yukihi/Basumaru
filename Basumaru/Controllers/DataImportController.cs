using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Basumaru.Models;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;



namespace Basumaru.Controllers

{
    public class DataImportController : Controller
    {
        private BasumaruDBContext db = new BasumaruDBContext();
        //日付
        DateTime dt = DateTime.Now;



        /// <summary>   
        /// DataImport ビュー初期表示用
        /// </summary>
        /// <returns></returns>
        public ActionResult DataImport()
        {
            //View読み込み            
            return View();
        }

        /// <summary>
        /// DataImport ビュー　外字変換処理
        /// </summary>
        /// <param name="uploadFile">クライアントからUploadされたファイル</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DataImport(HttpPostedFileWrapper uploadFile,string id)
        {

            string bname = id;

                //アップロードファイルが存在する場合のみ処理を実行
            if (uploadFile != null)
            {    

                //uploadされたファイルの拡張子を取得
                string FileExtension = Path.GetExtension(uploadFile.FileName);

                //uploadされたファイルの名称を取得
                string BeforeFileName = Path.GetFileName(uploadFile.FileName);

                //拡張子の判別
                switch(FileExtension)
                {
                    case ".csv":
                        //Do Nothing
                        break;
                    default:
                        //CSVファイルでない場合
                        ViewBag.OperationMessage = "選択されたファイルの拡張子が、テキストファイルではありません。";
                        ViewBag.OperationMessage2 = "CSVファイルを選択してください。";
                        return View("DataImport");
                }

                //現在の日付時間を取得
                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");

                //サーバーに格納するフォルダのパスをWebConfigから取得
                string SaveFilePath = ConfigurationManager.AppSettings["ServerFileSavePath"];

                //フォルダの作成
                string newfol = SaveFilePath + "/" + dt;

                System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(newfol);

                //Uploadされたファイルをサーバーにコピー
                //変換前ファイル格納
                System.IO.File.Copy(uploadFile.FileName, newfol + "/" + BeforeFileName);
                //uploadFile.SaveAs(newfol + "/" + BeforeFileName);

                //Viewから指定されたファイルを取得
                FileStream stream = new FileStream(newfol + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[stream.Length];
                //byte配列に読み込む
                stream.Read(bs, 0, bs.Length);

                //文字コードを取得する
                System.Text.Encoding enc = GetCode(bs);

                if (enc.BodyName == "utf-8" || enc.BodyName == "utf-16")
                {
                    //ファイルがUnicodeの場合
                    //FileStreamを一度読み込んだので、再度読み込みを行う
                    //stream = new FileStream(uploadFile.FileName, FileMode.Open, FileAccess.Read);
                    stream = new FileStream(newfol + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    //ファイルがUnicodeでない場合
                    ViewBag.OperationMessage = "指定されたファイルの文字コードがUnicodeではありません。";
                    ViewBag.OperationMessage2 = "変換できるファイルは、文字コードがUnicodeのファイルのみです。";
                    return View("DataImport");
                }

                StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));

                //バス停のテーブルの確認処理
                //バス停のCSVファイルの確認
                if (bname == "basutei")
                {
                    //一行ずつ格納する
                    string line = reader.ReadLine();
                    //カンマごとに格納する
                    string[] fields = line.Split(',');

                    string bl = fields.Length.ToString();

                    //バス停テーブルのデータの場合
                    if (bl == "8")
                    {
                        while (reader.EndOfStream == false)
                        {
                            //一行ずつ格納する
                            string bline = reader.ReadLine();
                            //カンマごとに格納する
                            string[] bfields = line.Split(',');

                            for (int i = 0; i < fields.Length-1; i++)
                            {
                                if (fields[i] == "")
                                {
                                    //ファイルがUnicodeでない場合
                                    ViewBag.OperationMessage = "指定されたCSVファイルにデータの欠落があります";
                                    return View("DataImport");
                                }
                            }

                        }
                    }
                    //異なったテーブルの場合
                    else
                    {
                        ViewBag.OperationMessage = "バス停のCSVファイルを選択してください";
                        return View("DataImport");
                    }

                }

                //時刻表のCSVファイルの確認
                else if (bname == "jikokuhyou")
                {
                    //一行ずつ格納する
                    string line = reader.ReadLine();
                    //カンマごとに格納する
                    string[] fields = line.Split(',');

                    string jl = fields.Length.ToString();

                    //時刻表テーブルのデータの場合
                    if (jl == "6")
                    {
                        while (reader.EndOfStream == false)
                        {
                            //一行ずつ格納する
                            string bline = reader.ReadLine();
                            //カンマごとに格納する
                            string[] bfields = line.Split(',');

                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (fields[i] == "")
                                {
                                    //ファイルがUnicodeでない場合
                                    ViewBag.OperationMessage = "指定されたCSVファイルにデータの欠落があります";
                                    return View("DataImport");
                                }
                            }

                        }
                    }
                    //異なったテーブルの場合
                    else
                    {
                        ViewBag.OperationMessage = "時刻表のCSVファイルを選択してください";
                        return View("DataImport");
                    }

                }

                //路線のCSVファイルの確認
                else if (bname == "rosen")
                {
                    //一行ずつ格納する
                    string line = reader.ReadLine();
                    //カンマごとに格納する
                    string[] fields = line.Split(',');

                    string rl = fields.Length.ToString();

                    //路線テーブルのデータの場合
                    if (rl == "5")
                    {
                        while (reader.EndOfStream == false)
                        {
                            //一行ずつ格納する
                            string bline = reader.ReadLine();
                            //カンマごとに格納する
                            string[] bfields = line.Split(',');

                            for (int i = 0; i < fields.Length-2; i++)
                            {
                                if (fields[i] == "")
                                {
                                    //ファイルがUnicodeでない場合
                                    ViewBag.OperationMessage = "指定されたCSVファイルにデータの欠落があります";
                                    return View("DataImport");
                                }
                            }

                        }
                    }
                    //異なったテーブルの場合
                    else
                    {
                        ViewBag.OperationMessage = "時刻表のCSVファイルを選択してください";
                        return View("DataImport");
                    }

                }



                reader.Close();
                stream.Close();

                //作業用変数
                double number;
                int inumber;

                //ここから分岐処理
                switch (bname)
                {
                    //バス停のボタンが押されたときの処理
                    case "basutei":
                        //データベースの全件削除
                        foreach (Basutei bt in db.basutei.ToList())
                        {
                            db.basutei.Remove(bt);
                        }
                        db.SaveChanges();

                        stream = new FileStream(newfol + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));

                        while (reader.EndOfStream == false)
                        {
                            //一行ずつ格納する
                            string line = reader.ReadLine();
                            //カンマごとに格納する
                            string[] fields = line.Split(',');

                            Basutei bt = new Basutei();
                            //運営企業
                            bt.kigyou = fields[0];
                            //路線名
                            bt.rosenmei = fields[1];
                            //バス停名
                            bt.basuteimei = fields[2];

                            //緯度
                            if (Double.TryParse(fields[3], out number))
                            {
                                bt.ido = double.Parse(fields[3]);
                            }
                            else
                            {
                                ViewBag.OperationMessage = "緯度または経度が数値ではありません";
                                return View("DataImport");
                            }
                            //経度
                            if (Double.TryParse(fields[4], out number))
                            {
                                bt.keido = double.Parse(fields[4]);
                            }
                            else
                            {
                                ViewBag.OperationMessage = "緯度または経度が数値ではありません";
                                return View("DataImport");
                            }
                            //屋根のFlg処理
                            if (fields[5].ToString() == "なし")
                            {
                                bt.yaneFlg = "0";
                            }
                            else
                            {
                                bt.yaneFlg = "1";
                            }
                            //ベンチのFlg処理
                            if (fields[6].ToString() == "なし")
                            {
                                bt.benchiFlg = "0";
                            }
                            else
                            {
                                bt.benchiFlg = "1";
                            }
                            //乗り場
                            bt.noriba = fields[7];


                            db.basutei.Add(bt);
                        }
                        //データベースの更新
                        db.SaveChanges();
                        break;

                    //時刻表のボタンが押されたときの処理
                    case "jikokuhyou":
                        //データベースの全件削除
                        foreach (Jikokuhyou jh in db.jikokuhyou.ToList())
                        {
                            db.jikokuhyou.Remove(jh);
                        }
                        db.SaveChanges();
                        // ViewBag.OperationMessage = "削除完了";

                        stream = new FileStream(newfol + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));

                        while (reader.EndOfStream == false)
                        {
                            //一行ずつ格納する
                            string line = reader.ReadLine();
                            //カンマごとに格納する
                            string[] fields = line.Split(',');

                            Jikokuhyou jk = new Jikokuhyou();
                            //運営企業
                            jk.kigyou = fields[0];
                            //路線名
                            jk.rosenmei = fields[1];
                            //行き先
                            jk.ikisaki = fields[2];
                            //日付分類
                            switch (fields[3])
                            {
                                case "平日":
                                    jk.hidukebunrui = "0";
                                    break;
                                case "土":
                                    jk.hidukebunrui = "1";
                                    break;
                                case "日":
                                    jk.hidukebunrui = "2";
                                    break;
                                case "土日":
                                    jk.hidukebunrui = "3";
                                    break;
                                case "日祝":
                                    jk.hidukebunrui = "4";
                                    break;
                                case "土日祝":
                                    jk.hidukebunrui = "5";
                                    break;
                                default:
                                    //曜日の表記が違う場合
                                    ViewBag.OperationMessage = "日付分類の表記が異なっています";
                                    return View("DataImport");
                                    break;
                            }
                            //バス停名
                            jk.basuteimei = fields[4];
                            //時刻
                            string f5 = fields[5].Length.ToString();
                            if (f5=="6" && int.TryParse(fields[5].ToString().Substring(0, 2), out inumber) && int.TryParse(fields[5].ToString().Substring(3, 2), out inumber))
                            {
                                jk.zikoku = fields[5].ToString().Substring(0, 2) + fields[5].ToString().Substring(3, 2);
                            }
                            else
                            {
                                //正しい表記でない場合
                                ViewBag.OperationMessage = "時刻の表記が正しくありません";
                                return View("DataImport");
                            }
                            //発着区分
                            if (fields[5].ToString().Substring(fields[5].Length - 1) == "発")
                            {
                                jk.hachakuKubun = "1";
                            }
                            else if (fields[5].ToString().Substring(fields[5].Length - 1) == "着")
                            {
                                jk.hachakuKubun = "0";
                            }
                            else
                            {
                                //発、着以外の文字が含まれていた場合
                                ViewBag.OperationMessage = "時刻の末尾を「発」、または「着」にして下さい。";
                                return View("DataImport");
                            }
                            db.jikokuhyou.Add(jk);

                        }
                        db.SaveChanges();
                        ViewBag.OperationMessage = "削除完了";
                        break;


                    //路線のボタンが押されたときの処理
                    case "rosen":
                        //データベースの全件削除
                        foreach (Rosen rs in db.rosen.ToList())
                        {
                            db.rosen.Remove(rs);
                        }
                        db.SaveChanges();

                        stream = new FileStream(newfol + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));

                        while (reader.EndOfStream == false)
                        {
                            //一行ずつ格納する
                            string line = reader.ReadLine();
                            //カンマごとに格納する
                            string[] fields = line.Split(',');

                            Rosen rs = new Rosen();
                            //運営企業
                            rs.kigyou = fields[0];
                            //路線名
                            rs.rosenmei = fields[1];
                            switch (fields[2])
                            {
                                case "平日":
                                    rs.hidukebunrui = "0";
                                    break;
                                case "土":
                                    rs.hidukebunrui = "1";
                                    break;
                                case "日":
                                    rs.hidukebunrui = "2";
                                    break;
                                case "土日":
                                    rs.hidukebunrui = "3";
                                    break;
                                case "日祝":
                                    rs.hidukebunrui = "4";
                                    break;
                                case "土日祝":
                                    rs.hidukebunrui = "5";
                                    break;
                            }
                            //記号
                            rs.kigoui = fields[3];
                            //コメント
                            rs.komento = fields[4];

                            db.rosen.Add(rs);
                        }
                        db.SaveChanges();
                        break;
                }


                reader.Close();
                stream.Close();

                ViewBag.OperationMessage = "取り込み完了";

            }
            else
            {
                //uploadファイルがない場合
                ViewBag.OperationMessage = "ファイルを選択してから、アップロードボタンを押してください。";
            }

            //View再読み込み            
            //return RedirectToAction("DataImport");
            return View("DataImport");
        }

      

        /// <summary>
        /// ファイル文字コード判定処理
        /// </summary>
        /// <param name="bytes">ファイル文字列</param>
        /// <returns></returns>
        public static System.Text.Encoding GetCode(byte[] bytes)
        {
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            int len = bytes.Length;
            byte b1, b2, b3, b4;

            //Encode::is_utf8 は無視

            bool isBinary = false;
            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
                {
                    //'binary'
                    isBinary = true;
                    if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
                    {
                        //smells like raw unicode
                        return System.Text.Encoding.Unicode;
                    }
                }
            }
            if (isBinary)
            {
                return null;
            }

            //not Japanese
            bool notJapanese = true;
            for (int i = 0; i < len; i++)
            {
                b1 = bytes[i];
                if (b1 == bEscape || 0x80 <= b1)
                {
                    notJapanese = false;
                    break;
                }
            }
            if (notJapanese)
            {
                return System.Text.Encoding.ASCII;
            }

            for (int i = 0; i < len - 2; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                b3 = bytes[i + 2];

                if (b1 == bEscape)
                {
                    if (b2 == bDollar && b3 == bAt)
                    {
                        //JIS_0208 1978
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bDollar && b3 == bB)
                    {
                        //JIS_0208 1983
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && (b3 == bB || b3 == bJ))
                    {
                        //JIS_ASC
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && b3 == bI)
                    {
                        //JIS_KANA
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    if (i < len - 3)
                    {
                        b4 = bytes[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD)
                        {
                            //JIS_0212
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                        if (i < len - 5 &&
                            b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                        {
                            //JIS_0208 1990
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            //should be euc|sjis|utf8
            //use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
                {
                    //SJIS_C
                    sjis += 2;
                    i++;
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
                {
                    //EUC_C
                    //EUC_KANA
                    euc += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                        (0xA1 <= b3 && b3 <= 0xFE))
                    {
                        //EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
                {
                    //UTF8
                    utf8 += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = bytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF))
                    {
                        //UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            //M. Takahashi's suggestion
            //utf8 += utf8 / 2;

            System.Diagnostics.Debug.WriteLine(
                string.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8));
            if (euc > sjis && euc > utf8)
            {
                //EUC
                return System.Text.Encoding.GetEncoding(51932);
            }
            else if (sjis > euc && sjis > utf8)
            {
                //SJIS
                return System.Text.Encoding.GetEncoding(932);
            }
            else if (utf8 > euc && utf8 > sjis)
            {
                //UTF8
                return System.Text.Encoding.UTF8;
            }

            return null;
        }
    }

}
