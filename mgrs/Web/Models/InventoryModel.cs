using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class InventoryModel
    {
        public string[] valueList { get; set; }
        public string[] keyList { get; set; }
    }

    public class PayCountModel
    {
        public string payCount { get; set; }
        public string inventory { get; set; }
    }
}