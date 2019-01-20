using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("tbl_banner")]
    public class tblbanner
    {      

        [Key]
        [Column("banner_id", Order = 1)]
        public string Banner_Id
        { get; set; }

        [Column("`banner_type`")]
        public string Banner_Type
        { get; set; }

        [Column("`startdatetime`")]
        public DateTime StartDateTime
        { get; set; }

        [Column("`enddatetime`")]
        public DateTime EndDateTime
        { get; set; }

        [Column("`sort`")]
        public int Sort
        { get; set; }

        [Column("`picname`")]
        public string PicName
        { get; set; }

        [Column("`PicUrl`")]
        public string Picurl
        { get; set; }


        [Column("`ModifyDateTime`")]
        public DateTime Modifydatetime
        { get; set; }

        [Column("`ModifyUser`")]
        public string Modifyuser
        { get; set; }


    }
}
