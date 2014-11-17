using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace aulease.Entities {

	public class MetaDbContext : DbContext {
		public DbSet<MetaUser> MetaUsers { get; set; }
		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}

	[Table("Inputallusers", Schema = "dbo")]
	public class MetaUser {
		[Key]
		public string aubGid { get; private set; }
		public string givenName { get; private set; }
		public string sn { get; private set; }
		public string ou { get; private set; }
	}
}
