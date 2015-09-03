using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Basumaru.Models
{
    /// <summary>
    /// モデル変更時クラス
    /// </summary>
    public class BasumaruInitializer : DropCreateDatabaseIfModelChanges<BasumaruDBContext>
    {
        /// <summary>
        /// モデル変更時メゾット
        /// </summary>
        /// <param name="context">DB接続情報</param>
        protected override void Seed(BasumaruDBContext context)
        {
            //モデル変更時に初期レコードをそうにゅすうる場合に使用する
            //現在うまく動いていない
            //var conversionTables = new List<ConversionTable>{
            //    new ConversionTable {ConversionTableId=1,
            //                         MojiCodeB = "1111",
            //                         MojiB ="あ",
            //                         MojiCodeA ="2222",
            //                         MojiA ="い",
            //                         CreateAt=DateTime.Now,
            //                         UpdateAt=DateTime.Now}

            //};

            //conversionTables.ForEach(b => context.ConversionTables.Add(b));
            //context.SaveChanges();
            //
            //base.Seed(context);
        }


    }
}