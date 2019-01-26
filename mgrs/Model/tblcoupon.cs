using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    [Table("tbl_coupon")]
    public class tblcoupon
    {
        [Key]
        [Column("couponid", Order = 1)]
        public string Couponid
        { get; set; }

        [Column("`couponchar`")]
        public string Couponchar
        { get; set; }

        [Column("`matchid`")]
        public string Matchid
        { get; set; }

        [Column("`lineid`")]
        public string Lineid
        { get; set; }

        [Column("`linesid`")]
        public string Linesid
        { get; set; }

        [Column("`userid`")]
        public string Userid
        { get; set; }

        [Column("`teamid`")]
        public string Teamid
        { get; set; }

        [Column("`createtime`")]
        public DateTime? Createtime
        { get; set; }

        [Column("`usedtime`")]
        public DateTime? Usedtime
        { get; set; }

        [Column("`status`")]
        public string Status
        { get; set; }

        [Column("`type`")]
        public string Type
        { get; set; }

        [Column("`company`")]
        public string Company
        { get; set; }

        [Column("`mobile`")]
        public string Mobile
        { get; set; }
        
        
    }
}
