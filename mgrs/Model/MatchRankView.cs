using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class MatchRankView
    {
        public string teamname
        { get; set; }

        public string linename
        { get; set; }

        public string nickname
        { get; set; }

        public string teamno
        { get; set; }

        public string line_no
        { get; set; }

        public string mobile
        { get; set; }

        public decimal total
        { get; set; }

        public DateTime? begintime
        { get; set; }

        public DateTime? endtime
        { get; set; }
    }
}
