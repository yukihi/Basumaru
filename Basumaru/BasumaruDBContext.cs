using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Basumaru
{
    /// <summary>
    /// DB接続情報
    /// </summary>
    public class BasumaruDBContext : DbContext
    {

        /// <summary>
        /// basuteiテーブル
        /// </summary>
        /// <value>
        /// The basutei.
        /// </value>
        public DbSet<Basumaru.Models.Basutei> basutei { get; set; }

        /// <summary>
        /// jikokuhyouテーブル
        /// </summary>
        /// <value>
        /// The jikokuhyou.
        /// </value>
        public DbSet<Basumaru.Models.Jikokuhyou> jikokuhyou { get; set; }

        /// <summary>
        /// rosenテーブル
        /// </summary>
        /// <value>
        /// The rosen.
        /// </value>
        public DbSet<Basumaru.Models.Rosen> rosen { get; set; }

    }
}