using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/********************************************
 * tbl_points实体类
 * 
 * *****************************************/
namespace Model
{
    [Table("tbl_points")]
    public class tblpoints
    {
        [Key]
        [Column("pointid",Order=1)]
        public string Pointid
        { get;set; }

        [Column("lineguid")]
        public string Lineguid
        { get; set; }

        [Column("id")]
        public int? Id
        { get; set; }

        [Column("`eventid`")]
        public int? Eventid
        { get;set; }

        [Column("`lineid`")]
        public int? Lineid
        { get;set; }

        [Column("`pointname`")]
        public string Pointname
        { get;set; }

        [Column("`content`")]
        public string Content
        { get;set; }

        [Required(ErrorMessage = "序号不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "序号不合法")]
        [Column("`sort`")]
        public int? Sort
        { get;set; }

        [Required(ErrorMessage = "标识点类型不能为空！")]
        [Column("`pointtype`")]
        public int? Pointtype
        { get;set; }

        [Column("`status`")]
        public int? Status
        { get;set; }

        [Column("`creater`")]
        public int? Creater
        { get;set; }

        [Column("`pointno`")]
        public string Pointno
        { get;set; }

    }
}
