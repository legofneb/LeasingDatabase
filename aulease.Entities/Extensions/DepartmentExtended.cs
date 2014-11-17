using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aulease.Entities
{
	public partial class Department 
	{
		public static bool FOPExistsInDatabase(string fund, string org, string program) {
			using (var context = new AuleaseEntities()) {
				return context.Departments.Any(d => d.Fund == fund && d.Org == org && d.Program == program);
			}
		}

		public string GetFOP() {
			return this.Fund + " " + this.Org + " " + this.Program;
		}
	}
}