using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class tblordersView
    {
        public string Id
        { get; set; }

        public string Match_Id
        { get; set; }

        public string Matchname
        { get; set; }

        public string Orderid
        { get; set; }

        public string Teamid
        { get; set; }

        public string Teamname
        { get; set; }

        public string Lineid
        { get; set; }

        public string Linename
        { get; set; }

        public string Linesname
        { get; set; }

        public string Userid
        { get; set; }

        public string Mobile
        { get; set; }

        public string Ordertotal
        { get; set; }

        public int? Status
        { get; set; }

        public int? OrderStatus
        { get; set; }

        public DateTime? Createtime
        { get; set; }

        public string Title
        { get; set; }

        public int? Teamtype
        { get; set; }

        public DateTime? Paytime
        { get; set; }

        public string Paytype { get; set; }
    }
}
