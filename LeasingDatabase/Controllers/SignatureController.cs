using aulease.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LeasingDatabase.Controllers
{
    public class SignatureController : Controller
    {
        //
        // GET: /Signature/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();
            List<Lease> leases = db.Leases.Where(n => n.ContractNumber == null && n.Component.LeaseTag != null).ToList();
            List<Component> comps = leases.Select(n => n.Component).Where(n => n.LeaseTag.Length > 2).OrderBy(n => n.LeaseTag).ToList();

            ViewBag.Components = comps;

            return View();
        }

        public ActionResult Select(string tag)
        {
            AuleaseEntities db = new AuleaseEntities();
            Component comp = db.Components.Where(n => n.LeaseTag == tag).Single();
            List<SystemGroup> groups = comp.Leases.FirstOrDefault().SystemGroup.PO.SystemGroups.ToList();

            ViewBag.Groups = groups;
            ViewBag.SystemGroup = comp.Leases.FirstOrDefault().SystemGroup;

            return View();
        }

        public ActionResult Sign(List<int> Id)
        {
            Session["Id"] = Id;
            return View();
        }

        public JsonResult Save(string imageData)
        {
            List<int> Id = Session["Id"] as List<int>;
            Signature sig = new Signature();
            sig.MIME = imageData;

            return Json(true);
        }

        public ActionResult SavePage()
        {
            return View();
        }

        public ActionResult Download(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            Component comp = db.Components.Where(n => n.Id == Id).Single();

            byte[] fileBytes = GetBytes(comp.Signature.MIME);

            return File(fileBytes, "image/png", "sig.png");
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
       
    }
}
