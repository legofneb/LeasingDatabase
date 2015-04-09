using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    /// <summary>
    /// The C# Representation of the Javascript billing object, see SROrders angularJS controller for use.
    /// </summary>
    public class BillingJavascriptObject
    {
        public string SR { get; set; }
        public bool useCurrentRates { get; set; }
        public bool usePreviousRates { get; set; }
        public List<decimal> costList { get; set; }
        public decimal insurance { get; set; }
        public decimal warrantyOrShipping { get; set; }
        public DateTime beginBillDate { get; set; }
        public string billingNotes { get; set; }
        public bool suppressEmail { get; set; }
        public bool confirmed { get; set; }
    }

    public class BillingController : ApiController
    {

        // POST api/billing
        [AuthorizeUser("Admin")]
        public List<BillingSummary> Post([FromBody]BillingJavascriptObject billingData)
        {
            if (String.IsNullOrWhiteSpace(billingData.SR))
            {
                throw new NullReferenceException("Must specify SR for Billing");
            }

            if (billingData.useCurrentRates == billingData.usePreviousRates)
            {
                throw new InvalidOperationException("useCurrentRates and usePreviousRates must be opposite values");
            }

            // Correct for Locale Time to UTC

            billingData.beginBillDate = new DateTime(billingData.beginBillDate.Year, billingData.beginBillDate.Month, 1);

            AuleaseEntities db = new AuleaseEntities();

            PO SR = db.POes.Where(n => n.PONumber == billingData.SR).Single();

            List<BillingSummary> Summaries = new List<BillingSummary>();

            foreach (var SystemGroup in SR.SystemGroups)
            {
                BillingBuilder Builder = new BillingBuilder().SetComponentCosts(billingData.costList)
                                                             .SetDatabase(db)
                                                             .SetDateRange(billingData.beginBillDate, billingData.beginBillDate.AddMonths(SystemGroup.Leases.FirstOrDefault().Overhead.Term).AddDays(-1))
                                                             .SetInsuranceCost(billingData.insurance)
                                                             .SetShippingCost(billingData.warrantyOrShipping)
                                                             .SetSystemGroup(SystemGroup);

                Builder.Build();

                if (billingData.confirmed)
                {
                    Builder.Apply();
                }

                Summaries.Add(Builder.GetBillingSummary());
            }

            if (billingData.confirmed && !billingData.suppressEmail)
            {
                SendEmail(db.VendorEmails, SR, billingData.billingNotes);
            }

            return Summaries;
        }

        private void SendEmail(IEnumerable<VendorEmail> VendorEmails, PO SR, string billingNotes)
        {
            Email email = new Email();
            foreach (var mail in VendorEmails)
            {
                email.AddTo(mail.EmailAddress);
            }

            email.From("aulease@auburn.edu");
            email.HTML = true;
            string message = "<p>" + billingNotes + "</p><br /><br /><table border\"1\" bgcolor=\"#ffffff\" cellspacing=\"5\"><caption><b>ECOA</b></caption><thead><tr>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">PO#</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Serial Number</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Component Type</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Order Number</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">igfTerm</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Manufacturer</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Component Cost</font></th>"
                            + "</tr></thead><tbody>";

            foreach (var group in SR.SystemGroups)
            {
                foreach (var comp in group.Leases.Select(n => n.Component))
                {
                    decimal ComponentCost;

                    try
                    {
                        ComponentCost = comp.Leases.OrderBy(n => n.EndDate).FirstOrDefault().Charges.Where(n => n.Type.Id == comp.Type.Id).Single().Price;
                    }
                    catch
                    {
                        ComponentCost = 0.00M;
                    }

                    message += "<tr valign=\"TOP\">"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.Leases.First().SystemGroup.PO.PONumber + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.SerialNumber + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.Type.Name + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.OrderNumber + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + (comp.Leases.First().Overhead.Term + 1).ToString() + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.Make.Name + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + ComponentCost + "</font></td>"
                                    + "</tr>";
                }
            }

            message += "</tbody><tfoot></tfoot></table>";

            email.Subject = "AUBURN UNIVERSITY 0639915 COA " + SR.PONumber;
            email.Body = message;
            email.AddCC("aulease@auburn.edu");

            email.Send();
        }
    }
}
