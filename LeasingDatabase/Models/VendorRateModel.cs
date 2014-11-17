using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
	public class VendorRateModel
	{
		public int Id { get; set; }
		public int Term { get; set; }
		public string RateType { get; set; }
		public decimal Rate { get; set; }
	}
}