using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeasingDatabase.Billing
{

    /// <summary>
    /// This is a Builder class to generate Charges for a given SystemGroup.
    /// The Build method builds the class, and the Apply method saves the data to the database.
    /// </summary>
    public class BillingBuilder
    {

        #region Constructor and Builder Methods

        private AuleaseEntities _db;
        private SystemGroup _systemGroup;
        private DateTime _begBillDate;
        private DateTime _endBillDate;
        private List<decimal> _ListOfComponentCosts;
        private decimal _insuranceCost;
        private decimal _warrantyCost;
        private decimal _shippingCost;
        private bool _usePreviousRates;

        public int _term
        {
            get { return _systemGroup.Leases.FirstOrDefault().Overhead.Term; }
        }

        public int _componentCount
        {
            get { return _systemGroup.Leases.Count; }
        }

        public BillingBuilder()
        {
            _insuranceCost = Decimal.Zero;
            _warrantyCost = Decimal.Zero;
            _shippingCost = Decimal.Zero;
            _usePreviousRates = false;
        }

        public BillingBuilder SetDatabase(AuleaseEntities db) { this._db = db; return this; }
        public BillingBuilder SetSystemGroup(SystemGroup group) { this._systemGroup = group; return this; }
        public BillingBuilder SetComponentCosts(List<decimal> costs) { this._ListOfComponentCosts = costs.ToList(); return this; }
        public BillingBuilder SetDateRange(DateTime begBillDate, DateTime endbillDate) { this._begBillDate = begBillDate; this._endBillDate = endbillDate; return this; }
        public BillingBuilder SetInsuranceCost(decimal insuranceCost) { this._insuranceCost = insuranceCost; return this; }
        public BillingBuilder SetWarrantyCost(decimal warrantyCost) { this._warrantyCost = warrantyCost; return this; }
        public BillingBuilder SetShippingCost(decimal shippingCost) { this._shippingCost = shippingCost; return this; }
        public BillingBuilder UsePreviousRates(bool condition) { this._usePreviousRates = condition; return this; }

        #endregion

        #region Public Methods
        public void Build()
        {
            if (_systemGroup == null || _begBillDate == null || _endBillDate == null || _ListOfComponentCosts == null)
            {
                throw new MissingFieldException("ChargerBuilder has missing fields and cannot perform calculation");
            }

            if (_begBillDate > _endBillDate)
            {
                throw new ArgumentOutOfRangeException("End Billing Date must come after Begin Billing Date");
            }

            if (_ListOfComponentCosts.Count() != _systemGroup.Leases.Select(n => n.Component).Count())
            {
                throw new ArgumentOutOfRangeException("An invalid number of costs was given for the SystemGroup");
            }

            ClearPreviousCharges();
            SetBillingFields();
            GenerateMonthlyCharges();
        }

        public void Apply()
        {
            if (this._db == null)
            {
                throw new MissingFieldException("ChargeBuilder database was not set");
            }

            this._db.SaveChanges();
        }
        #endregion

        public BillingSummary GetBillingSummary()
        {
            BillingSummary Summary = new BillingSummary();

            Lease PrimaryLease = _systemGroup.Leases.OrderBy(n => n.Component.TypeId).ThenBy(n => n.Id).FirstOrDefault();

            Summary.StatementName = PrimaryLease.StatementName;
            Summary.Term = _term;
            Summary.BeginBillDate = _begBillDate.ToString();
            Summary.EndBillDate = _endBillDate.ToString();
            Summary.Components = new List<ComponentCost>();

            decimal TotalMonthlyCharge = 0.00M;

            foreach (Lease lease in _systemGroup.Leases.OrderBy(n => n.Component.TypeId).ThenBy(n => n.Id))
            {
                ComponentCost ComponentCost = new ComponentCost();

                ComponentCost.Type = lease.Component.Type.Name;
                ComponentCost.Make = lease.Component.Make.Name;
                ComponentCost.Model = lease.Component.Model.Name;

                ComponentCost.UnitCost = Math.Round(lease.Charges.Where(n => n.Type == lease.Component.Type).Single().Price, 2);
                ComponentCost.LeasingRate = Math.Round(CalculateLeasingRate(lease), 2);
                ComponentCost.SecondaryCosts = Math.Round(CalculateSecondaryCosts(lease), 2);
                ComponentCost.OverheadRate = Math.Round(lease.Overhead.Rate, 2);
                ComponentCost.VendorRate = Math.Round(lease.Charges.Where(n => n.Type == lease.Component.Type).Single().VendorRate.Rate, 2);
                ComponentCost.VendorInsuranceRate = lease.Charges.Where(n => n.Type.Name == "Insurance" || n.Type.Name == "Warranty" || n.Type.Name == "Shipping").Count() > 0 ? (decimal?)Math.Round(lease.Charges.Where(n => n.Type.Name == "Insurance" || n.Type.Name == "Warranty" || n.Type.Name == "Shipping").FirstOrDefault().VendorRate.Rate, 2) : null;
                ComponentCost.Tax = Math.Round(CalculateTax(lease), 2);
                ComponentCost.MonthlyCharge = Math.Round(lease.MonthlyCharge.Value, 2);
                TotalMonthlyCharge += Math.Round(lease.MonthlyCharge.Value, 2);

                Summary.Components.Add(ComponentCost);
            }

            Summary.TotalMonthlyCharge = TotalMonthlyCharge;

            return Summary;
        }

        #region Private Build Methods
        private void ClearPreviousCharges()
        {
            foreach (var lease in _systemGroup.Leases)
            {
                foreach (var charge in lease.Charges.ToList())
                {
                    lease.Charges.Remove(charge);
                }

                foreach (var charge in _db.Charges.Where(n => n.Leases.FirstOrDefault().Id == lease.Id))
                {
                    _db.Entry(charge).State = System.Data.Entity.EntityState.Deleted;
                }
            }
        }

        private void GenerateMonthlyCharges()
        {
            foreach (var lease in _systemGroup.Leases)
            {
                decimal MonthlyCharge = 0.00M;

                foreach (var charge in lease.Charges)
                {
                    if (charge.Type.isNonFinanceType())
                    {
                        MonthlyCharge += (((charge.Price * charge.VendorRate.Rate / 1000) * (1 + charge.Tax.Price)) + (lease.Overhead.Rate)) * ((decimal)_term + 1) / (decimal)_term;
                    }
                    else
                    {
                        MonthlyCharge += (charge.Price / 1000 * charge.VendorRate.Rate) * ((decimal)_term+1) / (decimal)_term;
                    }
                }

                lease.MonthlyCharge = MonthlyCharge;
            }
        }

        private void SetBillingFields()
        {
            IEnumerable<decimal> ComponentCosts = _ListOfComponentCosts.ToList();

            foreach (var lease in _systemGroup.Leases.OrderBy(n => n.Component.TypeId).ThenBy(n => n.Id))
            {
                lease.BeginDate = _begBillDate;
                lease.EndDate = _endBillDate;
                lease.Component.ReturnDate = new DateTime(lease.EndDate.Value.Year, lease.EndDate.Value.Month, 1).AddMonths(2).AddDays(-1); // Setting the return date 1 month after end date.
                CreateChargesForLease(lease, _ListOfComponentCosts.Count == ComponentCosts.Count());
            }

            _ListOfComponentCosts = ComponentCosts.ToList(); // The primary and secondary billing fields dequeue from _ListOfComponent Costs, so this restores the original list.
        }
        
        /// <summary>
        /// Create the Charges for a given Lease in this Builder class
        /// The isPrimaryLease is used to only assign InsuranceCosts 1 primary component (Normally a CPU or LAPTOP but could be the first Monitor as well)
        /// </summary>
        /// <param name="lease"></param>
        /// <param name="isPrimaryLease"></param>
        private void CreateChargesForLease(Lease lease, bool isPrimaryLease)
        {
            aulease.Entities.Type Insurance = _db.Types.Where(n => n.Name == "Insurance").Single();
            aulease.Entities.Type Warranty = _db.Types.Where(n => n.Name == "Warranty").Single();
            aulease.Entities.Type Shipping = _db.Types.Where(n => n.Name == "Shipping").Single();

            lease.Charges.Add(new Charge { Price = DequeueCost(), VendorRate = GetVendorRate(lease), Type = lease.Component.Type, Tax = GetTaxRate() });

            if (_insuranceCost > Decimal.Zero && isPrimaryLease)
            {
                lease.Charges.Add(new Charge { Price = _insuranceCost, VendorRate = GetInsuranceVendorRate(), Type = Insurance, Tax = GetTaxRate() });
            }

            if (_warrantyCost > Decimal.Zero)
            {
                lease.Charges.Add(new Charge { Price = (_warrantyCost / _componentCount), VendorRate = GetInsuranceVendorRate(), Type = Warranty, Tax = GetTaxRate() });
            }

            if (_shippingCost > Decimal.Zero)
            {
                lease.Charges.Add(new Charge { Price = (_shippingCost / _componentCount), VendorRate = GetInsuranceVendorRate(), Type = Shipping, Tax = GetTaxRate() });
            }
        }

        private VendorRate GetInsuranceVendorRate()
        {
            if (!_usePreviousRates)
            {
                return _db.VendorRates.Where(n => n.Term == (_term + 1) && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).FirstOrDefault();
            }
            else
            {
                return _db.VendorRates.Where(n => n.Term == (_term + 1) && n.Type.Name == "Insurance").OrderByDescending(n => n.BeginDate).Skip(1).Take(1).Single();
            }
        }

        private Tax GetTaxRate()
        {
            return _db.Taxes.OrderByDescending(n => n.Timestamp).First();
        }

        private VendorRate GetVendorRate(Lease lease)
        {
            IEnumerable<VendorRate> VendorRates = _db.VendorRates.Where(n => n.Term == (_term + 1) && n.Type.Name == lease.Component.Type.Name).OrderByDescending(n => n.BeginDate);

            if (!_usePreviousRates)
            {
                return VendorRates.FirstOrDefault();
            }
            else
            {
                return VendorRates.Skip(1).Take(1).Single();
            }
        }

        private decimal DequeueCost()
        {
            decimal cost = _ListOfComponentCosts.First();
            _ListOfComponentCosts.RemoveAt(0);
            return cost;
        }

        #endregion


        #region CalculationHelpers

        private decimal CalculateTax(Lease lease)
        {
            Tax tax = lease.Charges.Where(n => n.Type == lease.Component.Type).Single().Tax;

            return tax.Price * CalculateLeasingRate(lease);
        }
        
        private decimal CalculateChargedRate(Lease lease)
        {
            return lease.Overhead.Rate;
        }

        private decimal CalculateSecondaryCosts(Lease lease)
        {
            List<Charge> Charges = lease.Charges.Where(n => !n.Type.isNonFinanceType()).ToList();
            decimal SecondaryCharges = 0M;
            foreach (var charge in Charges)
            {
                SecondaryCharges += (charge.VendorRate.Rate * charge.Price / 1000) * ((_term + 1) / _term);
            }
            return SecondaryCharges;
        }

        private decimal CalculateLeasingRate(Lease lease)
        {
            Charge Charge = lease.Charges.Where(n => n.Type == lease.Component.Type).Single();
            return (Charge.Price * Charge.VendorRate.Rate / 1000);
        }

        #endregion

    }
}
