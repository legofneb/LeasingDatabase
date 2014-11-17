using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
	public class DepartmentModel
	{
		public int Id { get; set; }
		public string Fund { get; set; }
		public string Org { get; set; }
		public string Program { get; set; }
		public string Name { get; set; }
	}
}