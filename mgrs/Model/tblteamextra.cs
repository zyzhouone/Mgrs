using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{

    [Table("tbl_match_extra")]
    public class tblteamextra
    {
        [Key]
        [Column("id", Order = 1)]
        public string id
        { get; set; }

        [Column("`updt`")]
        public DateTime? updt
        { get; set; }

        [Column("`extype`")]
        public string extype
        { get; set; }

        [Column("`teamid`")]
        public string teamid
        { get; set; }

        [Column("`info1`")]
        public string info1
        { get; set; }

        [Column("`info2`")]
        public string info2
        { get; set; }

        [Column("`info3`")]
        public string info3
        { get; set; }

    }
}
