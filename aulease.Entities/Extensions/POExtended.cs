using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aulease.Entities
{
	public partial class PO
	{
		public bool POExists()
		{
			using (AuleaseEntities db = new AuleaseEntities())
			{
				return db.POes.Any(n => n.PONumber == this.PONumber);
			}
		}

        public DateTime Date
        {
            get 
            {
                return this.SystemGroups.Select(n => n.Order).OrderBy(n => n.Date).FirstOrDefault().Date; 
            }
        }
	}
}