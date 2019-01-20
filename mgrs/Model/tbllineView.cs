using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class tbllineView
    {
        public string Lineid
        { get; set; }

        public string Matchname
        { get; set; }

        public string Name
        { get; set; }

        public int? Players
        { get; set; }

        public int? Count
        { get; set; }

        public string Content
        { get; set; }

        public Decimal? personprice
        { get; set; }

        public Decimal? teamprice
        { get; set; }

        public string Conditions
        { get; set; }

        public DateTime? Createtime
        { get; set; }

        public int? Status
        { get; set; }
    }
}
