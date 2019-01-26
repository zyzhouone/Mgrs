using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class tbllinesView
    {
        public string lines_id
        { get; set; }

        public string match_id
        { get; set; }

        public string Matchname
        { get; set; }

        public string Price
        { get; set; }

        public string line_id
        { get; set; }

        public string Line_name
        { get; set; }

        public string Linename
        { get; set; }

        public int? Status
        { get; set; }

        public int? Playercount
        { get; set; }

        public string Content
        { get; set; }

        public string Url
        { get; set; }

        public string Summary
        { get; set; }

        public int? Pointscount
        { get; set; }

        public int? Paycount
        { get; set; }

        public int? Condition_Sex
        { get; set; }

        public int? Condition_Age
        { get; set; }

        public int? Condition_Subline
        { get; set; }

        public DateTime? Createtime
        { get; set; }

    }
}
