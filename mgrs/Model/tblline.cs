using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/********************************************
 * tbl_line实体类
 * 
 * *****************************************/
namespace Model
{
    [Table("tbl_line")]
    public class tblline
    {
        [Key]
        [Column("lineid")]
        public string Lineid
        { get;set; }

        [Required(ErrorMessage = "赛事名称不能为空！")]
        [Column("`match_id`")]
        public string Match_id
        { get; set; }

        [Required(ErrorMessage = "线路名称不能为空！")]
        [Column("`name`")]
        public string Name
        { get;set; }

        [Required(ErrorMessage = "队伍人数不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "队伍人数不合法")]
        [Column("`players`")]
        public int? Players
        { get;set; }

        [Required(ErrorMessage = "队伍上限不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "队伍上限不合法！")]
        [Column("`count`")]
        public int? Count
        { get; set; }

        [Column("`content`")]
        public string Content
        { get;set; }

        [Column("`conditions`")]
        public string Conditions
        { get;set; }

        //[Required(ErrorMessage = "个人价格不能为空！")]
        //[RegularExpression(@"^(0|[1-9]\d{0,15})(\.\d{0,2})?$", ErrorMessage = "个人价格不合法！")]
        [Column("`personprice`")]
        public Decimal? personprice
        { get; set; }

        //[Required(ErrorMessage = "团队价格不能为空！")]
        //[RegularExpression(@"^(0|[1-9]\d{0,15})(\.\d{0,2})?$", ErrorMessage = "团队价格不合法！")]
        [Column("`teamprice`")]
        public Decimal? teamprice
        { get; set; }

        [Column("`createtime`")]
        public DateTime? Createtime
        { get; set; }

        [Column("`status`")]
        public int? Status
        { get; set; }
    }
}
