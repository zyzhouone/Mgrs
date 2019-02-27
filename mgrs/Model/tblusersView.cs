using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class tblusersView
    {
        public string userid
        { get; set; }

        public string Name
        { get; set; }

        public int? Playerid
        { get; set; }

        public string Mobile
        { get; set; }

        public string Passwd
        { get; set; }
        public string sexy
        { get; set; }

        public string cardtype
        { get; set; }

        public string cardno
        { get; set; }

        public string mono
        { get; set; }

        public DateTime? birthday
        { get; set; }

        public DateTime? Last_Time
        { get; set; }

        public int? Status
        { get; set; }

        private string devtoken = "-";
        public string DeviceToken
        {
            get { return devtoken; }
            set { devtoken = value; }
        }

        public string Isupt
        { get; set; }

        public string Type
        { get; set; }


        public string NickName
        { get; set; }
    }
}
