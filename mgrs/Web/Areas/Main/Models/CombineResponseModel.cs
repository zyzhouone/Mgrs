using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Main.Models
{
    public class CombineResponseModel
    {
        public int iResultCode { get; set; }
        public int iCount { get; set; }
        public string strMsg { get; set; }
    }
}