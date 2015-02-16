using aulease.Entities;
using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    
    public class StatementManagementController : ApiController
    {
        // GET api/statementmanagement
        [AuthorizeUser("Admin", "Users")]
        public long Get()
        {
            AuleaseEntities db = new AuleaseEntities();
            DateTime StatementDate = db.Statements.OrderByDescending(n => n.Date).First().Date;

            return StatementDate.ToJavaScriptMilliseconds();
        }

        // POST api/statementmanagement
        [AuthorizeUser("Admin")]
        public List<string> Post([FromBody]DateTime statementDate)
        {
            AuleaseEntities db = new AuleaseEntities();

            DateTime createDate = new DateTime(statementDate.Year, statementDate.Month, 1).AddMonths(1).AddSeconds(-1);
            Statement statement = new Statement { Date = createDate };

            if (createDate <db.Statements.OrderByDescending(n => n.Date).FirstOrDefault().Date )
            {
                return new List<string>() { "You must specify a date greater than the current one given" };
            }
            
            db.Statements.Add(statement);
            db.SaveChanges();

            DateTime billingDate = createDate.AddMonths(-1); // Send statements for 1 month past last lease so customer can check that there are no more billings

            List<Department> activeDepts = db.Leases.Where(n => n.EndDate >= billingDate).Select(n => n.Department).Distinct().ToList();
            List<Coordinator> people = activeDepts.SelectMany(n => n.Coordinators).Where(n => n.BillingAccess == true).Distinct().ToList();

            StatementEmailTemplate EmailTemplate = new StatementEmailTemplate(statementDate.ToString("MMMM"));
            List<string> Errors = new List<string>();

            foreach (var person in people)
            {
                Email email = EmailTemplate.CreateEmail(person.GID);
                
                try
                {
                    email.Send();
                }
                catch
                {
                    Errors.Add("Email failed for " + person.GID);
                }
            }

            if (Errors.Count == 0)
            {
                Errors.Add("No Errors Found");
            }

            return Errors;
        }
    }


    public class StatementEmailTemplate
    {
        private string emailFrom = "aulease@auburn.edu";
        private List<string> email_CC_Collection = new List<string>() { "aulease@auburn.edu" };
        private string subject = "AU Lease Billing Statement";
        private string month;

        public StatementEmailTemplate(string Month)
        {
            month = Month;
        }

        private string CreateBody()
        {
            return "<html><body><p>Your " + month + " statement(s) is now ready. Please visit <a href=\"https://cws.auburn.edu/oit/aulease/statements\">https://cws.auburn.edu/oit/aulease/statements</a> to view your billing statement(s).</p><br /><p>Sincerely,</p><p>AU Lease</p></body></html>";
        }

        public Email CreateEmail(string GID)
        {
            Email email = new Email();
            email.From(emailFrom);
            email_CC_Collection.ForEach(x => email.AddCC(x));
            email.AddTo(GID + "@auburn.edu");
            email.Subject = subject;
            email.Body = CreateBody();
            email.HTML = true;

            return email;
        }
    }

    public static class DateTimeJavascript
    {
        private static readonly long DateTimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static long ToJavaScriptMilliseconds(this DateTime dt)
        {
            return (long)((dt.ToUniversalTime().Ticks - DateTimeMinTimeTicks) / 10000);
        }
    }
}
