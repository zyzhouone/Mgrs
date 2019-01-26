using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
     [Table("sys_match_user")]
    public class sysmatchuser
    {
        [Key]
        [Column("id", Order = 1)]
        public string Id
        { get; set; }

        [Required(ErrorMessage = "账户不能为空！")]
        [Column("`account`")]
        public string Account
        { get; set; }

        [Column("`name`")]
        public string Name
        { get; set; }

        [Required(ErrorMessage = "密码不能为空！")]
        [Column("`pwd`")]
        public string Pwd
        { get; set; }

        [Column("`match_id`")]
        public string Match_id
        { get; set; }

        [Column("`role`")]
        public string Role
        { get; set; }

        [Column("`linesid`")]
        public string Linesid
        { get; set; }

        [Column("`status`")]
        public string Status
        { get; set; }
    }
}
