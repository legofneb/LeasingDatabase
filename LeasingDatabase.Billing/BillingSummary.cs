using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeasingDatabase.Billing
{
    /// <summary>
    /// The billing summary for a given systemGroup. Meant as a return value to POST for /OrdersByPO/#Billing controller as a result of using BillingBuilder
    /// </summary> 
    public class BillingSummary
    {
        public decimal Term { get; set; }
        public string StatementName { get; set; }
        public string BeginBillDate { get; set; }
        public string EndBillDate { get; set; }
        public decimal TotalMonthlyCharge { get; set; }
        public List<ComponentCost> Components { get; set; }
    }

    public class ComponentCost
    {
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }

        public decimal UnitCost { get; set; }
        public decimal LeasingRate { get; set; }
        public decimal SecondaryCosts { get; set; } // Insurance, Warranty, or Shipping Cost
        public decimal OverheadRate { get; set; }
        public decimal VendorRate { get; set; }
        public decimal? VendorInsuranceRate { get; set; }
        public decimal Tax { get; set; }
        public decimal MonthlyCharge { get; set; }
    }
}
