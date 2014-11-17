using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
	public class BillingModel
	{
		public int Id { get; set; }
		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string StatementName { get; set; }
		public DateTime Timestamp { get; set; }
		public string ContractNumber { get; set; }
		public string Fund { get; set; }
		public string Org { get; set; }
		public string Program { get; set; }
		public string RateLevel { get; set; }
		public decimal MonthlyCharge { get; set; }
		public int Term { get; set; }
		public decimal PrimaryCharge { get; set; }
		public decimal SecondaryCharges { get; set; }
	}
}