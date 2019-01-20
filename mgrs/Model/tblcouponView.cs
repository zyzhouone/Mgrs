using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class tblcouponView
    {
        public string Couponid
        { get; set; }

        public string Couponchar
        { get; set; }

        public string Matchid
        { get; set; }

        public string Match_name
        { get; set; }

        public string Lineid
        { get; set; }

        public string Line_name
        { get; set; }

        public string Linesid
        { get; set; }

        public string Lines_name
        { get; set; }

        public string Userid
        { get; set; }

        public string Nickname
        { get; set; }

        [Required(ErrorMessage = "新增数量不能为空！")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "新增数量不合法")]
        public int Count
        { get; set; }

        [Required(ErrorMessage = "标识不能为空！")]
        public string Identifying
        { get; set; }
        

        public string Teamname
        { get; set; }

        public string Teamid
        { get; set; }

        public DateTime? Createtime
        { get; set; }

        public DateTime? Usedtime
        { get; set; }

        public string Status
        { get; set; }

        public string Type
        { get; set; }

        public string Mobile
        { get; set; }

        public string Company
        { get; set; }
        
    }
}
