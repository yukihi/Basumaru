using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Basumaru.Models
{
    [Table("Basutei")]
    public class Basutei
    {

        public int BasuteiId { get; set; }

        [StringLength(100)]
        [Display(Name = "運営企業")]
        public string kigyou { get; set; }
      
        [StringLength(200)]
        [Display(Name = "路線名")]
        public string rosenmei { get; set; }

        
        [StringLength(100)]
        [Display(Name = "バス停名")]
        public string basuteimei { get; set; }


        [Display(Name = "緯度")]
        public double ido { get; set; }

        [Display(Name = "経度")]
        public double keido { get; set; }

        [StringLength(1)]
        [Display(Name = "屋根")]
        public string yaneFlg { get; set; }

        [StringLength(1)]
        [Display(Name = "ベンチ")]
        public string benchiFlg { get; set; }

        [StringLength(100)]
        [Display(Name = "乗り場")]
        public string noriba { get; set; }
    }
}