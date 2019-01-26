using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class playerInfo
    {
        public tblmatchusers matchusers
        { get; set; }

        public tblteamsVew teams
        { get; set; }

        public List<tblmatchusers> teammember
        { get; set; }

    }
}
