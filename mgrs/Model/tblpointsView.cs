using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class tblpointsView
    {
        public string Pointid
        { get; set; }

        public string Lineguid
        { get; set; }

        public string Linename
        { get; set; }

        public int? Id
        { get; set; }

        public int? Eventid
        { get; set; }

        public int? Lineid
        { get; set; }

        public string Pointname
        { get; set; }

        public string Content
        { get; set; }

        public int? Sort
        { get; set; }

        public int? Pointtype
        { get; set; }

        public int? Status
        { get; set; }

        public int? Creater
        { get; set; }

        public string Pointno
        { get; set; }

    }
}
