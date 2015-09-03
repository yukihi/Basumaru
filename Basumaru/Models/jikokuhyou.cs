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
    [Table("Jikokuhyou")]
    public class Jikokuhyou
    {
        public int JikokuhyouId { get; set; }

        [StringLength(100)]
        [Display(Name = "運営企業")]
        public string kigyou { get; set; }

        
        [StringLength(200)]
        [Display(Name = "路線名")]
        public string rosenmei { get; set; }

        
        [StringLength(100)]
        [Display(Name = "行き先")]
        public string ikisaki { get; set; }

       
        [StringLength(20)]
        [Display(Name = "日付分類")]
        public string hidukebunrui { get; set; }

  
        [StringLength(100)]
        [Display(Name = "バス停名")]
        public string basuteimei { get; set; }

        [StringLength(4)]
        [Display(Name = "時刻")]
        public string zikoku { get; set; }

        [StringLength(1)]
        [Display(Name = "発着区分")]
        public string hachakuKubun { get; set; }

    }

}