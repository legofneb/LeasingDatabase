using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeasingDatabase.Controllers
{
    class Program
    {
        static void Main(string[] args)
        {
            AuleaseEntities db = new AuleaseEntities();
            string SRNumber = "SR5000";
            List<Component> comps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.PO.PONumber == SRNumber).OrderBy(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID).ThenBy(p => p.Leases.OrderByDescending(q => q.Timestamp).FirstOrDefault().SystemGroupId).ThenBy(p => p.TypeId).ToList();
            for (int i = 0; i < comps.Count; i++ )
                Console.Out.WriteLine(comps[i].CurrentLease.SystemGroup.EOLComponents.Take(1).FirstOrDefault().SerialNumber);

            Console.ReadLine();
        }
    }
}
