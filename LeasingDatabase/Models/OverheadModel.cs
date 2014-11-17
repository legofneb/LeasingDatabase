using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    public class OverheadModel
    {
        public int Id { get; set; }
        public string RateLevel { get; set; }
        public decimal Rate { get; set; }
        public int Term { get; set; }
        public string Type { get; set; }
    }
}