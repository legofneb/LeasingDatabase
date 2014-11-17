using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elmah;

namespace aulease.Entities {

	public partial class StandardOption {

		public override string ToString() {

			const int CPU = 1;
			const int LAPTOP = 2;
			const int SERVER = 4;
			const int MONITOR = 3;

			string s = this.Name + ": ";
			
			StandardComponent primary = this.GetStandardComponents().Where(c => c.Type.Id == CPU || c.Type.Id == LAPTOP || c.Type.Id == SERVER).FirstOrDefault();
			List<StandardComponent> monitors = this.GetStandardComponents().Where(c => c.Type.Id == MONITOR).ToList();
			
			try {
				if (primary != null) {
					s += (primary.Make.Name + " " + primary.Model.Name + ", " + 
						  primary.Properties.Single(p => p.Key == "Processor Model").Value + " " + primary.Properties.Single(p => p.Key == "Clock Speed").Value + ", " + 
						  primary.Properties.Single(p => p.Key == "RAM").Value + " RAM; ");

				if (monitors.Count > 0) {
					s += (monitors.Count + "x " + monitors.First().Properties.First(p => p.Key == "Display Size").Value + " Monitor" + (monitors.Count > 1 ? "s" : ""));
				} else if (primary.Properties != null && primary.Properties.Any(p => p.Key == "Display Size")) {
					s += (primary.Properties.First(p => p.Key == "Display Size").Value + " Display");
				}
				}
			} catch (Exception e) {
				Elmah.ErrorSignal.FromCurrentContext().Raise(new Elmah.ApplicationException("Standard Option " + this.Name + " threw an error. CPUs, Laptops, and Servers must have Properties with keys: 'Processor Model', 'Clock Speed', 'RAM'. Monitors must have a Property with key: 'Display Size'. If this specification changes, update aulease.Entities/Extensions/StandardOption.cs accordingly.", e));
			}

			return s;
		}

		public IEnumerable<StandardComponent> GetStandardComponents() {
			foreach (var j in this.StandardOptionStandardComponents) {
				yield return j.StandardComponent;
			}
		}

		private string SafeProperty(Property p) {
			return p != null ? p.Value : "";
		}

		public Dictionary<string, string> GenerateSpecificationList() {
			
			var specs = new Dictionary<string, string>();
			var primary = this.GetStandardComponents().Single(c => c.Type.Name.Equals("CPU", StringComparison.OrdinalIgnoreCase) || c.Type.Name.Equals("Laptop", StringComparison.OrdinalIgnoreCase));

			specs.Add("Name", this.Name);

			specs.Add("Model", primary.Make.Name + " " + primary.Model.Name);
			
			if (primary.Properties.Any(p => p.Key == "Processor Model" || p.Key == "Processor Manufacturer" || p.Key == "Clock Speed")) {
				specs.Add("Processor", SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Processor Manufacturer")) + " " + 
					                   SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Processor Model")) + " " + 
									   SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Clock Speed")));
			}

			if (primary.Properties.Any(p => p.Key == "Display Size" || p.Key == "Display Type")) {
				specs.Add("Display", SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Display Size")) + " " +
					                 SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Display Type")));
			}

			if (primary.Properties.Any(p => p.Key == "Operating System" || p.Key == "OS Edition" || p.Key == "Architecture")) {
				specs.Add("Operating System", SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Operating System")) + " " +
					                          SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "OS Edition")) + " " +
											  SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Architecture")) + "-bit");
			}

			if (primary.Properties.Any(p => p.Key == "Wireless")) {
				specs.Add("Wireless", primary.Properties.Single(p => p.Key == "Wireless").Value);
			}

			if (primary.Properties.Any(p => p.Key == "RAM")) {
				specs.Add("Memory", primary.Properties.Single(p => p.Key == "RAM").Value);
			}

			if (primary.Properties.Any(p => p.Key == "Hard Drive Size" || p.Key == "Hard Drive Type")) {
				specs.Add("Hard Drive", SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Hard Drive Size")) + " " +
					                    SafeProperty(primary.Properties.SingleOrDefault(p => p.Key == "Hard Drive Type")));
			}

			if (primary.Properties.Any(p => p.Key == "Storage Media")) {
				specs.Add("Storage Media", primary.Properties.Single(p => p.Key == "Storage Media").Value);
			}

			if (this.GetStandardComponents().Any(c => c.Type.Name.Equals("Monitor", StringComparison.OrdinalIgnoreCase))) {
				var monitors = this.GetStandardComponents().Where(c => c.Type.Name.Equals("Monitor", StringComparison.OrdinalIgnoreCase));
				specs.Add("Monitor", monitors.Count() + "\00D7 " + monitors.First().Model.Name + " " + SafeProperty(monitors.First().Properties.SingleOrDefault(p => p.Key == "Display Size")));
			}

			if (primary.Properties.Any(p => p.Key == "Speakers")) {
				specs.Add("Speakers", primary.Properties.Single(p => p.Key == "Speakers").Value);
			}

			if (primary.Properties.Any(p => p.Key == "USB Ports")) {
				specs.Add("USB Ports", primary.Properties.Single(p => p.Key == "USB Ports").Value);
			}

			if (primary.Properties.Any(p => p.Key == "Carrying Case")) {
				specs.Add("Carrying Case", primary.Properties.Single(p => p.Key == "Carrying Case").Value);
			}

			if (primary.Properties.Any(p => p.Key == "Insurance")) {
				specs.Add("Insurance", primary.Properties.Single(p => p.Key == "Insurance").Value);
			}

			if (this.SupportRate24 > 0) {
				specs.Add("Monthly Support Rate (24-month)", "$" + this.SupportRate24.ToString());
			}
			if (this.SupportRate36 > 0) {
				specs.Add("Monthly Support Rate (36-month)", "$" + this.SupportRate36.ToString());
			}
			if (this.BaseRate24 > 0) {
				specs.Add("Monthly Base Rate (24-month)", "$" + this.BaseRate24.ToString());
			}
			if (this.BaseRate36 > 0) {
				specs.Add("Monthly Base Rate (36-month)", "$" + this.BaseRate36.ToString());
			}

			return specs;
		}
	}
}
