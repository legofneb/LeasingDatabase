using aulease.Entities;
using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class EmailController : Controller
    {
        //
        // GET: /Email/

        public ActionResult Index()
        {
            AuleaseEntities db = new AuleaseEntities();
            DateTime StatementMonth = db.Statements.OrderByDescending(n => n.Date).First().Date;
            ViewBag.StatementMonth = StatementMonth.AddMonths(1).ToString("MMMM");
            return View();
        }

        public ActionResult Vendor()
        {
            AuleaseEntities db = new AuleaseEntities();
            List<VendorEmail> emails = db.VendorEmails.ToList();
            ViewBag.Emails = emails;
            return View();
        }

        public ActionResult CreateEmail()
        {
            return View();
        }

        public ActionResult SubmitEmail(string email)
        {
            AuleaseEntities db = new AuleaseEntities();
            VendorEmail newEmail = new VendorEmail();
            newEmail.EmailAddress = email;
            db.VendorEmails.Add(newEmail);
            db.SaveChanges();
            return RedirectToAction("Vendor");
        }

        public ActionResult EditEmail(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            VendorEmail email = db.VendorEmails.Where(n => n.Id == Id).Single();
            ViewBag.Email = email;
            return View();
        }

        public ActionResult ChangeEmail(int Id, string Email)
        {
            AuleaseEntities db = new AuleaseEntities();
            VendorEmail email = db.VendorEmails.Where(n => n.Id == Id).Single();
            email.EmailAddress = Email;
            db.SaveChanges();
            return RedirectToAction("Vendor");
        }

        public ActionResult RemoveEmail(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            VendorEmail email = db.VendorEmails.Where(n => n.Id == Id).Single();
            db.VendorEmails.Remove(email);
            db.SaveChanges();
            return RedirectToAction("Vendor");
        }

        public ActionResult SendBillingStatements(string Month)
        {
            string Errors = "Statements have been sent!";
            DateTime date = DateTime.ParseExact(Month, "MMMM", CultureInfo.CurrentCulture);
            DateTime createDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddSeconds(-1);

            Statement stat = new Statement { Date = createDate };
            AuleaseEntities db = new AuleaseEntities();
            db.Statements.Add(stat);
            db.SaveChanges();

            DateTime billingDate = createDate.AddMonths(-1); // Send statements for 1 month past last lease so customer can check that there are no more billings

            List<Department> activeDepts = db.Leases.Where(n => n.EndDate >= billingDate).Select(n => n.Department).Distinct().ToList();
            List<Coordinator> people = activeDepts.SelectMany(n => n.Coordinators).Where(n => n.BillingAccess == true).Distinct().ToList();

            foreach (var person in people)
            {
                // uncomment for production
                Email email = new Email();
                email.From("aulease@auburn.edu");
                email.AddTo(person.GID + "@auburn.edu");
                email.AddCC("aulease@auburn.edu");
                email.Subject = "AU Lease Billing Statement";
                email.Body = "<html><body><p>Your " + Month + " statement(s) is now ready. Please visit <a href=\"https://cws.auburn.edu/oit/aulease/statements\">https://cws.auburn.edu/oit/aulease/statements</a> to view your billing statement(s).</p><br /><p>Sincerely,</p><p>AU Lease</p></body></html>";
                email.HTML = true;
                try
                {
                    email.Send();
                }
                catch
                {
                    Errors += "\n Email failed for:" + person.GID;
                }
            }

            ViewBag.error = Errors;
            return View();
        }

        public ActionResult SendEOLStatements(string Month)
        {
            string Errors = "Statements have been sent!";
            DateTime date = DateTime.ParseExact(Month, "MMMM", CultureInfo.CurrentCulture);
            DateTime createDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddSeconds(-1);

            DateTime billingDate = createDate.AddMonths(-1); // Send statements for 1 month past last lease so customer can check that there are no more billings

            AuleaseEntities db = new AuleaseEntities();

            List<Department> activeDepts = db.Leases.Where(n => n.EndDate >= billingDate).Select(n => n.Department).Distinct().ToList();
            List<Coordinator> people = activeDepts.SelectMany(n => n.Coordinators).Distinct().ToList();

            foreach (var person in people)
            {
                // uncomment for production
                //Email email = new Email();
                //email.From("aulease@auburn.edu");
                //email.AddTo(person.GID + "@auburn.edu");
                //email.AddCC("aulease@auburn.edu");
                //email.Subject = "AU Lease End of Lease Statement";
                //email.Body = "<html><body><p>Your " + Month + " end of lease statement(s) is now ready. Please visit <a href=\"cws.auburn.edu/oit/aulease/management\">cws.auburn.edu/oit/aulease/management</a> to view your statement(s) to place future orders.</p><br /><p>Sincerely,</p><p>AU Lease</p></body></html>";
                //email.HTML = true;
                //try
                //{
                //    email.Send();
                //}
                //catch
                //{
                //    Errors += "\n Email failed for:" + person.GID;
                //}
            }

            Email email = new Email();
            email.From("aulease@auburn.edu");
            email.AddTo("aulease@auburn.edu");
            email.AddCC("aulease@auburn.edu");
            email.Subject = "AU Lease End of Lease Statement";
            email.Body = "<html><body><p>Your " + Month + " end of lease statement(s) is now ready. Please visit <a href=\"cws.auburn.edu/oit/aulease/management\">cws.auburn.edu/oit/aulease/management</a> to view your statement(s) to place future orders.</p><br /><p>Sincerely,</p><p>AU Lease</p></body></html>";
            email.HTML = true;
            email.Send();

            ViewBag.error = Errors;
            return View();
        }

    }
}
