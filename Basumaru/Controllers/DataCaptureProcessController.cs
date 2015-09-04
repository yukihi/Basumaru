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


namespace Basumaru.Controllers

{
    public class DataCaptureProcessController : Controller
    {
        private BasumaruDBContext db = new BasumaruDBContext();

        /// <summary>   
        /// ConversionExecute ビュー初期表示用
        /// </summary>
        /// <returns></returns>
        public ActionResult ConversionExecute()
        {
            //View読み込み            
            return View("ConversionExecute");
        }

        /// <summary>
        /// ConversionExecute ビュー　外字変換処理
        /// </summary>
        /// <param name="uploadFile">クライアントからUploadされたファイル</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConversionExecute(HttpPostedFileWrapper uploadFile)
        {
            //アップロードファイルが存在する場合のみ処理を実行
            if (uploadFile != null)
            {    
                //ログインデックスの最大値取得処理
                long conversionlogMaxNum = 0;

                var RecordNullCheck = db.ConversionLogs.ToList();

                if (RecordNullCheck.Count == 0)
                {
                    //ConversionLogsのレコード件数が0の場合
                    conversionlogMaxNum = 0;
                }
                else
                {
                    //ConversionLogsのレコード件数が1以上の場合
                    conversionlogMaxNum = db.ConversionLogs.Select(s => s.ConversionLogId).Max();
                }

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
                        //テキストファイルでない場合
                        ViewBag.OperationMessage = "選択されたファイルの拡張子が、テキストファイルではありません。";
                        ViewBag.OperationMessage2 = "テキストファイルを選択してください。";
                        return View("ConversionExecute");
                }


                //サーバーに格納するフォルダのパスをWebConfigから取得
                string SaveFilePath = ConfigurationManager.AppSettings["ServerFileSavePath"];

                //フォルダの名称は、ConversionLogIdの値
               // System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(SaveFilePath + (conversionlogMaxNum + 1).ToString());

                //Uploadされたファイルをサーバーにコピー
                //変換前ファイル格納
                //System.IO.File.Copy(uploadFile.FileName, SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + BeforeFileName);
                uploadFile.SaveAs(SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + BeforeFileName);

                //Viewから指定されたファイルを取得
                FileStream stream = new FileStream(SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
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
                    stream = new FileStream(SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + BeforeFileName, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    //ファイルがUnicodeでない場合
                    ViewBag.OperationMessage = "指定されたファイルの文字コードがUnicodeではありません。";
                    ViewBag.OperationMessage2 = "変換できるファイルは、文字コードがUnicodeのファイルのみです。";
                    return View("ConversionExecute");
                }

                StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));

                //データベースから全レコードを取得
                var ConversionTablesAllRecord = db.ConversionTables.ToList();

                //データベースから取得した、変換文字をReplaceする
                if ((reader.Peek() > 0))
                {
                    //ファイルから取得した文字列を格納する変数を定義
                    System.Text.StringBuilder FileStr = new System.Text.StringBuilder(reader.ReadToEnd());

                    //データベースから取得したレコード分、文字変換処理を行う
                    foreach (var item in ConversionTablesAllRecord)
                    {
                        //MojiBをMojiAに変換する
                        FileStr.Replace(item.MojiB, item.MojiA);
                    }
                    
                    /*
                    //Uploadされたファイルをサーバーにコピー
                    //変換前ファイル格納
                    System.IO.File.Copy(uploadFile.FileName, SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + BeforeFileName);
                    */
                      
                    //変換後のファイル名を取得
                    string AfterFileName = BeforeFileName.Replace(".", "After.");
                    //変換後ファイル格納
                    StreamWriter Writer = new StreamWriter(SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + AfterFileName, false, System.Text.Encoding.GetEncoding("utf-8"));
                    Writer.Write(FileStr);
                    Writer.Close();
                    //ログファイル名称
                    string LogFileName = BeforeFileName.Replace(FileExtension, "ログ" + ".log");

                    //ログテーブルにデータを挿入
                    //変数に格納
                    ConversionLog conversionlog = new ConversionLog();

                    conversionlog.ConversionLogId = conversionlogMaxNum + 1;
                    conversionlog.ConvDate = DateTime.Now;
                    conversionlog.ConvInputFile = BeforeFileName;
                    conversionlog.ConvOutputFile = AfterFileName;
                    conversionlog.ConvErrorFile = LogFileName;
                    
                    if (ModelState.IsValid)
                    {
                        //ログテーブル更新
                        db.ConversionLogs.Add(conversionlog);
                        db.SaveChanges();
                    }

                    //ログファイル格納
                    StreamWriter LogWriter = new StreamWriter(SaveFilePath + (conversionlogMaxNum + 1).ToString() + "/" + LogFileName, false, System.Text.Encoding.GetEncoding("utf-8"));

                    //ログファイル出力処理
                    if (NotConversionStr == "")
                    {
                        //変換エラー文字なし
                        //ファイル処理結果
                        LogWriter.WriteLine("ファイル変換処理結果：成功");
                        //文字数出力
                        LogWriter.WriteLine("ファイル格納文字数：" + StrCount.ToString());
                        LogWriter.Close();

                        //メッセージ表示
                        ViewBag.OperationMessage = "ファイルの変換処理が終了しました。";
                    }
                    else
                    {
                        //変換エラー文字あり
                        //ファイル処理結果
                        LogWriter.WriteLine("ファイル変換処理結果：一部変換不可");                        
                        //文字数出力
                        LogWriter.WriteLine("ファイル格納文字数：" + StrCount.ToString());
                        //変換エラー文字数
                        LogWriter.WriteLine("変換不可文字数：" + NotConversionCount.ToString());
                        //変換不可文字
                        LogWriter.WriteLine("変換不可文字：" + NotConversionStr);
                        LogWriter.Close();

                        //メッセージ表示
                        ViewBag.OperationMessage = "ファイルの変換処理が終了しました。";
                        ViewBag.OperationMessage2 = "変換できない外字が存在しましたので、ログを確認し、文字コードを新たに登録してください。";
                    }

                    ViewBag.BeforeFileNameLabel = "変換ファイル：";
                    ViewBag.AfterFileNameLabel = "変換後ファイル：";
                
                    ViewBag.BeforeFileName = BeforeFileName;
                    ViewBag.AfterFileName = AfterFileName;
                    ViewBag.ConversionNoLabel = "変換番号：";
                    ViewBag.ConversionNo = (conversionlogMaxNum + 1).ToString();

                    ViewBag.LogFileNameLabel = "ログファイル：";
                    ViewBag.LogFileName = LogFileName;

                    //エクスプローラー表示
                    //System.Diagnostics.Process.Start("EXPLORER.EXE", @"/select," + AfterFilePathName);
                }
                else
                {
                    //ファイルに文字が存在しない場合
                    ViewBag.OperationMessage = "選択されたファイルには、文字列が存在しません。";
                }
                stream.Close();
                reader.Close();
            }
            else
            {
                //uploadファイルがない場合
                ViewBag.OperationMessage = "ファイルを選択してから、アップロードボタンを押してください。";
            }

            //View再読み込み            
            //return RedirectToAction("ConversionExecute");
            return View("ConversionExecute");
        }

        /// <summary>
        /// 変換後ファイルダウンロード処理
        /// </summary>
        /// <param name="No">変換ログ番号</param>
        /// <param name="FileName">変換後ファイル名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AfterFileDownLoad(string No,string FileName)
        {
            if (No != "" && FileName != "")
            {
                //サーバーに格納するフォルダのパスをWebConfigから取得
                string SaveFilePath = ConfigurationManager.AppSettings["ServerFileSavePath"];
                //FilePathResult DownloadFile = new FilePathResult("D:/Data/" + No + "/" + FileName, "text/csv");


                //FilePathResult DownloadFile = new FilePathResult(SaveFilePath + No + "/" + FileName, "text/csv");
                var result = new FilePathResult(SaveFilePath + No + "\\" + FileName, "text/csv");

                if (Request.Browser.Browser == "IE")
                {
                    result.FileDownloadName = HttpUtility.UrlEncode(FileName);
                }
                else
                {
                    result.FileDownloadName = FileName;
                }

                return result;
            }
            else
            {
                //uploadファイルがない場合
                ViewBag.OperationMessage = "変換を行うファイルがアップロードされていません。";
                return View("ConversionExecute");
            }
        }

        /// <summary>
        /// ログファイルダウンロード処理
        /// </summary>
        /// <param name="No">変換ログ番号</param>
        /// <param name="FileName">ログファイル名</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogFileDownLoad(string No, string FileName)
        {
            if (FileName != "" && No == "")
            {
                //uploadファイルがない場合
                ViewBag.OperationMessage = "変換を行うファイルがアップロードされていません。";
                return View("ConversionExecute");
            }
            else if (No != "")
            {
                //ログファイル、番号が存在する場合
                //サーバーに格納するフォルダのパスをWebConfigから取得
                string SaveFilePath = ConfigurationManager.AppSettings["ServerFileSavePath"];
                //FilePathResult DownloadFile = new FilePathResult("D:/Data/" + No + "/" + FileName, "text/csv");
                FilePathResult DownloadFile = new FilePathResult(SaveFilePath + No + "/" + FileName, "text/csv");

                if (Request.Browser.Browser == "IE")
                {
                    DownloadFile.FileDownloadName = HttpUtility.UrlEncode(FileName);
                }
                else
                {
                    DownloadFile.FileDownloadName = FileName;
                }
                
                //DownloadFile.FileDownloadName = FileName;
                return DownloadFile;
            }
            else
            {
                return View("ConversionExecute");
            }
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
