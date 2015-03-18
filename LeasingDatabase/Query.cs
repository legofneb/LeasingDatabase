using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Controllers
{
    public class Query
    {
        static void Main(){

            AuleaseEntities db = new AuleaseEntities();
            string SRNumber = "SR5000";
            List<Component> comps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.PO.PONumber == SRNumber).OrderBy(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID).ThenBy(p => p.Leases.OrderByDescending(q => q.Timestamp).FirstOrDefault().SystemGroupId).ThenBy(p => p.TypeId).ToList();
            Console.Out.WriteLine(comps);
            Console.ReadKey();
        }
    }
}