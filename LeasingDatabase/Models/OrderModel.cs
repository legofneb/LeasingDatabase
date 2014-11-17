using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
	public class OrderModel
	{
		// Primary Key for Orders
		public int SystemID { get; set; }

		// System Properties
		public string OrderNumber { get; set; }
		public DateTime OrderDate { get; set; }
		public string SR { get; set; }
		public string StatementName { get; set; }
		public bool? InstallHardware { get; set; }
		public bool? InstallSoftware { get; set; }
		public string Note { get; set; }
		public bool? Renewal { get; set; }
		public int? Term { get; set; }
		public string RateLevel { get; set; }

		// System Department
		public string DepartmentName { get; set; }
		public string Fund { get; set; }
		public string Org { get; set; }
		public string Program { get; set; }
		
		// System User
		public string GID { get; set; }
		public string Phone { get; set; }
		// System User Location
		public string Room { get; set; }
		public string Building { get; set; }

		// System Orderer
		public string OrdererGID { get; set; }
        public string OrderNotes { get; set; }

		// System-Component Properties??
		public string OperatingSystem { get; set; }
        public string Architecture { get; set; }
		public string MAC { get; set; }
		public string Status { get; set; }

		// Component1
		public string SerialNumber { get; set; }
		public string LeaseTag { get; set; }
		public string Make { get; set; }
		public string ComponentType { get; set; }
		public string Model { get; set; }
		public string EOLComponent { get; set; }

		//Component 2
		public string SerialNumber2 { get; set; }
		public string LeaseTag2 { get; set; }
		public string ComponentType2 { get; set; }
		public string Make2 { get; set; }
		public string Model2 { get; set; }
		public string EOLComponent2 { get; set; }

		// Component 3
		public string SerialNumber3 { get; set; }
		public string LeaseTag3 { get; set; }
		public string ComponentType3 { get; set; }
		public string Make3 { get; set; }
		public string Model3 { get; set; }
		public string EOLComponent3 { get; set; }
	}
}