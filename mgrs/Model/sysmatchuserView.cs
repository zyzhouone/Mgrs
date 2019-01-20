using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model
{
    public class sysmatchuserView
    {
        public string Id
        { get; set; }

        public string Account
        { get; set; }

        public string Name
        { get; set; }

        public string Pwd
        { get; set; }

        public string Match_id
        { get; set; }

        public string Match_name
        { get; set; }

        public string Role
        { get; set; }

        public string Linesid
        { get; set; }

        public string Linesname
        { get; set; }

        public string Status
        { get; set; }
    }
}
