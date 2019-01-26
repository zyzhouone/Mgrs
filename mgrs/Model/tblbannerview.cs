using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    public class tblbannerview
    {      

        public string Banner_Id
        { get; set; }

        public string Banner_Type
        { get; set; }

        public DateTime StartDateTime
        { get; set; }

        public DateTime EndDateTime
        { get; set; }

        public int Sort
        { get; set; }

        public string PicName
        { get; set; }

        public string Picurl
        { get; set; }


        public DateTime Modifydatetime
        { get; set; }

        public string Modifyuser
        { get; set; }

        public string Match_Id
        {
            get;
            set;
        }
        public string Match_Name
        {
            get;
            set;
        }
    }
}
