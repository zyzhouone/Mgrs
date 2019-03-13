using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace Model
{
    public class tblteamsVew
    {
        public string teamid
        { get; set; }

        public string matchuserid
        { get; set; }

        public string match_id
        { get; set; }

        public string matchname
        { get; set; }

        public string Teamno
        { get; set; }

        public string Teamname
        { get; set; }

        public string Userid
        { get; set; }

        public string Moblie
        { get; set; }

        public string Username
        { get; set; }

        public string Nickname
        { get; set; }

        public string Company
        { get; set; }

        /// <summary>
        /// zzy 20190103 add
        /// </summary>
        public string CompanyText
        { get; set; }

        public string Lineid
        { get; set; }

        public string Linesname
        { get; set; }

        public int? Leader
        { get; set; }

        public string Linename
        { get; set; }

        public DateTime? Createtime
        { get; set; }

        public int? Eventid
        { get; set; }

        public string Eventname
        { get; set; }

        public int? Status
        { get; set; }

        [Column("`type_`")]
        public string Type
        { get; set; }

        public string Type_
        { get; set; }

        public string Chglines
        { get; set; }

        public int? paystatus
        { get; set; }

        public string paytotal
        { get; set; }

        public int iscoupon
        { get; set; }

        public int? Teamtype
        { get; set; }

        public DateTime? paytime
        { get; set; }


        public string info1 { get; set; }
        public string info2 { get; set; }
        public string info3 { get; set; }
        public string cardtype { get; set; }
        public string birthday { get; set; }
        public string sexy { get; set; }
    }
}
