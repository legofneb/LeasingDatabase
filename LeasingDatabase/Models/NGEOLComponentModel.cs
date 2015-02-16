using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    public class NGEOLComponentModel
    {
        public string SerialNumber { get; set; }
        public string LeaseTag { get; set; }
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }
}