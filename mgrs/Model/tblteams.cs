using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

/********************************************
 * tbl_teams实体类
 * 
 * *****************************************/
namespace Model
{
    [Table("tbl_teams")]
    public class tblteams
    {
        [Key]
        [Column("`teamid`", Order = 1)]
        public string teamid
        { get; set; }

        [Column("`match_id`")]
        public string match_id
        { get; set; }

        [Column("`teamno`")]
        public string Teamno
        { get; set; }

        [Remote("CheckTeamName", "Team", ErrorMessage = "该名称已经存在！")]
        [Required(ErrorMessage = "队伍名称不能为空！")]
        [Column("`teamname`")]
        public string Teamname
        { get; set; }

        [Column("`userid`")]
        public string Userid
        { get; set; }

        [Column("`company`")]
        public string Company
        { get; set; }

        [Column("`lineid`")]
        public string Lineid
        { get; set; }

        [Column("`linesid`")]
        public string Linesid
        { get; set; }

        [Column("`createtime`")]
        public DateTime? Createtime
        { get; set; }

        [Column("`eventid`")]
        public int? Eventid
        { get; set; }

        [Column("`status`")]
        public int? Status
        { get; set; }

        [Column("`type_`")]
        public string Type
        { get; set; }


        [Column("`chglines`")]
        public string Chglines
        { get; set; }

        [Column("`teamtype`")]
        public int? Teamtype
        { get; set; }
    }
}
