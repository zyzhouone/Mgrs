using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/********************************************
 * tbl_match_pics实体类
 * 
 * *****************************************/
namespace Model
{
    [Table("tbl_match_pics")]
    public class tblmatchpics
    {
        [Key]
        [Column("id", Order = 1)]
        public string Id
        { get; set; }

        [Column("`match_id`")]
        public string Match_id
        { get; set; }

        [Column("`picture`")]
        public string Picture
        { get; set; }

        [Column("`createtime`")]
        public DateTime? Createtime
        { get; set; }

    }
}
