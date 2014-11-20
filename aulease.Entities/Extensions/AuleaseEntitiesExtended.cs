using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aulease.Entities
{
	public partial class AuleaseEntities
	{
        public AuleaseEntities(string connectionString) : base(connectionString) { }

		public Type TypesMonitor
		{
			get
			{
				return this.Types.Where(n => n.Name == "Monitor").Single();
			}
		}

		public Type TypesInsurance
		{
			get
			{
				return this.Types.Where(n => n.Name == "Insurance").Single();
			}
		}

		public Type TypesCPU
		{
			get
			{
				return this.Types.Where(n => n.Name == "CPU").Single();
			}
		}

		public Type TypesLaptop
		{
			get
			{
				return this.Types.Where(n => n.Name == "Laptop").Single();
			}
		}

		public Type TypesWarranty
		{
			get
			{
				return this.Types.Where(n => n.Name == "Warranty").Single();
			}
		}

		public Type TypesShipping
		{
			get
			{
				return this.Types.Where(n => n.Name == "Shipping").Single();
			}
		}
	}
}
