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
    [Table("Rosen")]
    public class Rosen
    {
        public int RosenId { get; set; }

        [StringLength(100)]
        [Display(Name = "運営企業")]
        public string kigyou { get; set; }

     
        [StringLength(200)]
        [Display(Name = "路線名")]
        public string rosenmei { get; set; }

  
        [StringLength(20)]
        [Display(Name = "日付分類")]
        public string hidukebunrui { get; set; }

       
        [StringLength(10)]
        [Display(Name = "バス記号")]
        public string kigoui { get; set; }


        [StringLength(200)]
        [Display(Name = "コメント")]
        public string komento { get; set; }
    }
}