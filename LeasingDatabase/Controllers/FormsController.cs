using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class FormsController : Controller
    {
        //
        // GET: /Forms/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Editor()
        {
            AuleaseEntities db = new AuleaseEntities();
            string form = db.FormTemplates.OrderByDescending(n => n.Id).First().HTML.Replace(System.Environment.NewLine, @"\r\n");

            ViewBag.HTML = form;//HttpUtility.HtmlEncode(form);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditorChanged(string html)
        {
            html = HttpUtility.HtmlDecode(html);

            AuleaseEntities db = new AuleaseEntities();
            FormTemplate form = new FormTemplate();
            form.HTML = html;
            form.Id = db.FormTemplates.OrderByDescending(n => n.Id).First().Id + 1;

            db.FormTemplates.Add(form);
            db.SaveChanges();
            @ViewBag.HTML = html;
            return View();
        }

        public ActionResult FormGenerate(string tag)
        {
            AuleaseEntities db = new AuleaseEntities();

            @ViewBag.HTML = db.FormTemplates.OrderByDescending(n => n.Id).FirstOrDefault().HTML;
            @ViewBag.Tag = tag;
            return View();
        }

        public ActionResult FormCreate(string html, string tag)
        {
            AuleaseEntities db = new AuleaseEntities();
            Component comp = db.Components.Where(n => n.LeaseTag == tag).Single();
            SystemGroup group = comp.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault().SystemGroup;

            Form form = new Form();
            form.HTML = html;

            group.Forms.Add(form);
            db.SaveChanges();

            return View();
        }
    }
}
