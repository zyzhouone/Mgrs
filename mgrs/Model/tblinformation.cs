using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

/********************************************
 * tbl_information实体类
 * 
 * *****************************************/
namespace Model
{
     [Table("tbl_infomation")]
    public class tblinformation
    {
        [Key]
        [Column("infoid", Order = 1)]
        public string Infoid
        { get; set; }

        [Column("`type`")]
        public string Type
        { get; set; }

        [Column("`createtime`")]
        public DateTime? Createtime
        { get; set; }

        [Column("`userid`")]
        public string Userid
        { get; set; }

        [Column("`moblie`")]
        public string Mobile
        { get; set; }

        [Column("`context`")]
        public string Context
        { get; set; }

        [Column("`status`")]
        public int? Status
        { get; set; }

        [Column("`url`")]
        public string Url
        { get; set; }

        [Column("`note`")]
        public string Note
        { get; set; }
    }
}
