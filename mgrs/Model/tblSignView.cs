using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace Model
{
    public class tblSignVew
    {
        public string match_name
        { get; set; }

        public string Teamno
        { get; set; }

        public string Teamname
        { get; set; }

        public string Mobile
        { get; set; }

        public string Name
        { get; set; }

        public string Linename
        { get; set; }

        public DateTime? dtime
        { get; set; }
    }
}
