using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();

            var user = db.Users.Where(n => n.GID == "woffojl").FirstOrDefault();

            var groups = db.SystemGroups.Where(n => n.User.GID == "woffojl");

            foreach (var group in groups)
            {
                User oldUser = group.User;

                if (oldUser != user)
                {
                    group.User = user;
                    user.SystemGroups.Add(group);
                    oldUser.SystemGroups.Clear();
                }

                if (oldUser != user)
                {
                    db.Users.Remove(oldUser);
                }
            }

            db.SaveChanges();

            var users = db.Users.GroupBy(n => n.GID).Where(n => n.Count() > 1).Select(n => n.Key).ToList();
            ViewBag.Users = users;

            return View();
        }

        public ActionResult Test()
        {
            AuleaseEntities db = new AuleaseEntities();

            List<Component> activeComps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().MonthlyCharge.HasValue)
                                                       .Where(n => n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().EndDate >= DateTime.Now).ToList();


            int currentDept = -10;
            string currentDeptName = "";
            string currentDeptFOP = "";
            int currentCount = 0;

            var outputStream = Response.OutputStream;
            using (var memStrm = new MemoryStream())
            {
                var streamWriter = new StreamWriter(memStrm, Encoding.Default);

                foreach (var comp in activeComps.OrderBy(n => n.Leases.OrderByDescending(o => o.EndDate).FirstOrDefault().DepartmentId))
                {
                    if (comp.Department.Id != currentDept)
                    {
                        // write department to line

                        if (!String.Equals(currentDeptName, ""))
                        {
                            streamWriter.WriteLine("{0},{1},{2}", currentDeptName, currentDeptFOP, currentCount);
                        }
                        // set new department

                        currentCount = 1;
                        currentDeptName = comp.Department.Name;
                        currentDeptFOP = comp.Department.GetFOP();
                        currentDept = comp.Department.Id;
                    }
                    else
                    {
                        currentCount++;
                    }
                }

                streamWriter.Flush();
                outputStream.Write(memStrm.GetBuffer(), 0, (int)memStrm.Length);
                
            }


            Response.AddHeader("Content-Disposition", "attachment; filename=test.csv");
            Response.End();
            return null;
        }
    }
}
