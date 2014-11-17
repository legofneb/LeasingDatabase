using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aulease.Entities
{
	public partial class Order
	{
		/// <summary>
		/// Creates a new Order with a particular User as the orderer. If the user does not exist in the database, a new User is created.
		/// </summary>
		/// <param name="username">The auburn ID of the User placing the order</param>
		public Order(string username) : this() {
			
			using (var context = new AuleaseEntities()) {

				this.User = context.Users.FirstOrDefault(u => u.GID == username);

				if (this.User == null) {
					this.User = new User() {
						GID = username
					};
				}
			}
		}

        public IEnumerable<Component> GetConfiguration()
        {
            return this.SystemGroups.FirstOrDefault().GetConfiguration();
        }
	}
}
