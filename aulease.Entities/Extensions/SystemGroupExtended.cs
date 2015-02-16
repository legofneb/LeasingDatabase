using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aulease.Entities {

	internal class ComponentTypeComparer : IComparer<string> {
		public int Compare(string a, string b) {
			
			Dictionary<string, int> Rank = new Dictionary<string,int>() {
				{ "CPU", 1 },
				{ "Laptop", 2 },
				{ "Monitor", 3 },
				{ "Server", 4 },
				{ "Printer", 5 }
			};

			int aRank = 6, bRank = 6;
			Rank.TryGetValue(a, out aRank);
			Rank.TryGetValue(b, out bRank);

			return aRank.CompareTo(bRank);
		}
		}

	public partial class SystemGroup {
		
		public Lease GetPrimaryLease() {
			return this.Leases.SkipWhile(l => l.Component.Type == null).OrderBy(l => l.Component.Type.Name, new ComponentTypeComparer()).ThenByDescending(l => l.Timestamp).FirstOrDefault() ?? this.Leases.OrderByDescending(l => l.Timestamp).First();
		}

		public IEnumerable<Component> GetMonitors() {
			foreach (var l in this.Leases.SkipWhile(l => l.Component.Type == null).Where(l => l.Component.Type.Name.Equals("Monitor", StringComparison.OrdinalIgnoreCase) && l.IsActiveOrLatest())) {
				yield return l.Component;
			}
		}

        public IEnumerable<Component> GetConfiguration()
        {
            return this.Leases.SkipWhile(n => n.Component.Type == null).OrderBy(n => n.Component.Type.Name, new ComponentTypeComparer()).ThenByDescending(n => n.Timestamp).Select(n => n.Component);
        }

		public int ComponentCount
		{
			get
			{
				return this.ActiveLeases.Count();
			}
		}

		public IEnumerable<Lease> ActiveLeases
		{
			get
			{
				IEnumerable<Component> comps = this.Leases.Select(n => n.Component);
				return comps.Select(n => n.CurrentLease);
			}
		}

		public IEnumerable<Component> Components
		{
			get
			{
				return this.ActiveLeases.Select(n => n.Component);
			}
		}

        public override string ToString()
        {
            string finalString = "";

            if (this.GetPrimaryLease().Component.Make == null)
            {
                return "No Configuration";
            }

            string PrimaryCompMake = this.GetPrimaryLease().Component.Make.Name.ToString();
            string PrimaryCompModel = this.GetPrimaryLease().Component.Model.Name.ToString();

            if (this.GetPrimaryLease().Component.Type.Name != "Monitor" || this.ComponentCount == 1)
            {
                finalString = String.Format("1 x {0} {1}", PrimaryCompMake, PrimaryCompModel);
            }
            else
            {
                finalString = String.Format("2 x {0} {1}", PrimaryCompMake, PrimaryCompModel);
            }

            if (this.ComponentCount > 1)
            {
                string SecondaryCount = this.GetMonitors().Count().ToString();
                string SecondaryCompMake = HasNoNullMembersForMake(this) ? this.GetMonitors().FirstOrDefault().CurrentLease.Component.Make.Name.ToString() : "";
                string SecondaryCompModel = HasNoNullMembersForModel(this) ? this.GetMonitors().FirstOrDefault().CurrentLease.Component.Model.Name.ToString() : "";
                if (this.GetMonitors().Count() == 1)
                {
                    finalString += String.Format(", 1 x {0} {1}", SecondaryCompMake, SecondaryCompModel);
                }
                else
                {
                    finalString += String.Format(", {0} x {1} {2}", SecondaryCount, SecondaryCompMake, SecondaryCompModel);
                }
            }
            return finalString;
        }

        private bool HasNoNullMembersForMake(SystemGroup group)
        {
            if (group.GetMonitors() == null) { return false; }
            if (group.GetMonitors().FirstOrDefault() == null) { return false; }
            if (group.GetMonitors().FirstOrDefault().CurrentLease == null) { return false; }
            if (group.GetMonitors().FirstOrDefault().CurrentLease.Component == null) { return false; }
            if (group.GetMonitors().FirstOrDefault().CurrentLease.Component.Make == null) { return false; }

            return true;
        }

        private bool HasNoNullMembersForModel(SystemGroup group)
        {
            if (group.GetMonitors() == null) { return false; }
            if (group.GetMonitors().FirstOrDefault() == null) { return false; }
            if (group.GetMonitors().FirstOrDefault().CurrentLease == null) { return false; }
            if (group.GetMonitors().FirstOrDefault().CurrentLease.Component == null) { return false; }
            if (group.GetMonitors().FirstOrDefault().CurrentLease.Component.Model == null) { return false; }

            return true;
        }
	}
}
