using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Main.Models
{
    public class ResponseModel
    {
        public int status { get; set; }
        public string desc { get; set; }
        public object data { get; set; }
    }
}