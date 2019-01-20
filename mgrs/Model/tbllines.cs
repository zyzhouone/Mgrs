using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/********************************************
 * tbl_lines实体类
 * 
 * *****************************************/
namespace Model
{
    [Table("tbl_lines")]
    public class tbllines
    {
        [Key]
        [Column("lines_id")]
        public string Linesid
        { get;set; }

        [Required(ErrorMessage = "赛事名称不能为空！")]
        [Column("`match_id`")]
        public string Matchid
        { get;set; }

        [Required(ErrorMessage = "线路类型不能为空！")]
        [Column("`line_id`")]
        public string Lineid
        { get;set; }

        [Required(ErrorMessage = "线路名称不能为空！")]
        [Column("`linename`")]
        public string Linename
        { get;set; }

        [Column("`line_no`")]
        public string Lineno
        { get; set; }

        [Column("`status`")]
        public int? Status
        { get;set; }

        [Required(ErrorMessage = "队伍人数不能为空！")]
        [Column("`playercount`")]
        public int? Playercount
        { get;set; }

        [Required(ErrorMessage = "简介不能为空！")]
        [Column("`content`")]
        public string Content
        { get;set; }

        [Column("`url`")]
        public string Url
        { get;set; }

        [Column("`summary`")]
        public string Summary
        { get;set; }

        [Required(ErrorMessage = "标识点数不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "标识点数不合法")]
        [Column("`pointscount`")]
        public int? Pointscount
        { get;set; }

        [Column("`condition_sex`")]
        public int? Condition_Sex
        { get;set; }

        [Column("`condition_age`")]
        public int? Condition_Age
        { get;set; }

        [Column("`condition_subline`")]
        public int? Condition_Subline
        { get;set; }

        [Column("`createtime`")]
        public DateTime? Createtime
        { get;set; }

        [Required(ErrorMessage = "价格不能为空！")]
        [Column("`price`")]
        public string Price
        { get; set; }

        [Column("`notice`")]
        public string Notice
        { get; set; }

        [Required(ErrorMessage = "支付名额不能为空！")]
        [RegularExpression(@"^[0-9]*[0-9][0-9]*$", ErrorMessage = "支付名额不合法")]
        [Column("`paycount`")]
        public int? Paycount
        { get; set; }

    }
}
