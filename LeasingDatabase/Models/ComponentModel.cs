using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
	public class ComponentModel
	{
		public int ComponentID { get; set; }
		public string SerialNumber { get; set; }
		public string LeaseTag { get; set; }
		public string Note { get; set; }
		public string Make { get; set; }
		public string ComponentType { get; set; }
		public string Model { get; set; }

		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }
		public string StatementName { get; set; }
		public string ContractNumber { get; set; }
		public string DepartmentName { get; set; }
		public string Fund { get; set; }
		public string Org { get; set; }
		public string Program { get; set; }
		public decimal MonthlyCharge { get; set; }
		public string RateLevel { get; set; }
		public int Term { get; set; }

		public string SRNumber { get; set; }

		public string GID { get; set; }
		public string Phone { get; set; }
		public string Building { get; set; }
		public string Room { get; set; }
	}
}