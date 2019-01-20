using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Model
{
    public class tblteamsEntity
    {
        public string teamid
        { get; set; }

        public string match_id
        { get; set; }

        public string Teamno
        { get; set; }

        [Required(ErrorMessage = "队伍名称不能为空！")]
        [Remote("EditCheckTeamName", "Team", AdditionalFields = "teamid,match_id", ErrorMessage = "该名称已存在！")]
        public string Teamname
        { get; set; }

        public string Userid
        { get; set; }

        public string Company
        { get; set; }

        public string Lineid
        { get; set; }

        public string Linesid
        { get; set; }

        public DateTime? Createtime
        { get; set; }

        public int? Eventid
        { get; set; }

        public int? Status
        { get; set; }

        public string Type_
        { get; set; }

        public string Chglines
        { get; set; }

        public int? Teamtype
        { get; set; }
    }
}
