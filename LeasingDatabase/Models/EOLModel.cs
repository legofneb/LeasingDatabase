using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
	public class EOLModel
	{
		public int Id { get; set; }
        public string SN { get; set; }
        public string LeaseTag { get; set; }
		public string SerialNumber { get; set; }
		public string ComponentType { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }

        public int Term { get; set; }
        public decimal MonthlyCharge { get; set; }

        public string DepartmentName { get; set; }

        public string FOP { get; set; }
        public string RateLevel { get; set; }
        public string SR { get; set; }

        public string StatementName { get; set; }
		public string GID { get; set; }
        public DateTime? EndBillingDate { get; set; }
        public DateTime? ReturnDate { get; set; }
		public string Damages { get; set; }
	}
}