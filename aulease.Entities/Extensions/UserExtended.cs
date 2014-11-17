using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using aulease.Entities;

namespace aulease.Entities
{
	public partial class User
	{
		private string _FirstName;
		private string _LastName;

		public string FirstName {
			get { 
				if (_FirstName != null) {
					return _FirstName;
				} else {
				using (var mdb = new MetaDbContext()) {
					try {
						return mdb.MetaUsers.Find(this.GID).givenName;
					} catch (Exception e) {
						return null;
					}
		}
			}
		}
			set { _FirstName = value; }
		}

		public string LastName {
			get {
				if (_LastName != null) {
					return _LastName;
				} else {
				using (var mdb = new MetaDbContext()) {
					try {
						return mdb.MetaUsers.Find(this.GID).sn;
					} catch (Exception e) {
						return null;
					}
				}
			}
		}
			set { _LastName = value; }
		}

		public string GetStatementName() {
			if (this.SystemGroups.Count() > 0) {
				return this.SystemGroups.OrderByDescending(g => g.Order.Date).First().Leases.First().StatementName;
			} else {
				return this.FirstName + " " + this.LastName;
			}
		}

		public static bool GIDExistsInDatabase(string gid)
		{
			using (AuleaseEntities db = new AuleaseEntities())
			{
				return db.Users.Any(n => n.GID == gid);
			}
		}

	}
}
