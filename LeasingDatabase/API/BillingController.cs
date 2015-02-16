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
        public bool? suppressEmail { get; set; }
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
                                                             .SetDateRange(billingData.beginBillDate, billingData.beginBillDate.AddMonths(SystemGroup.Leases.FirstOrDefault().Overhead.Term + 1))
                                                             .SetInsuranceCost(billingData.insurance)
                                                             .SetShippingCost(billingData.warrantyOrShipping)
                                                             .SetSystemGroup(SystemGroup);

                Builder.Build();
                Summaries.Add(Builder.GetBillingSummary());
            }

            return Summaries;
        }
    }
}
