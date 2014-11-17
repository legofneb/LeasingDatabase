using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aulease.Entities {

	public partial class StandardComponent {

		public override string ToString() {
			return this.Make.Name + " " + this.Model.Name;
		}

		public IEnumerable<StandardOption> GetStandardOptions() {
			foreach (var j in this.StandardOptionStandardComponents) {
				yield return j.StandardOption;
			}
		}

	}
}
