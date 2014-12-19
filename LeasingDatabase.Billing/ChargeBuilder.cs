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
    public class ChargeBuilder
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

        public ChargeBuilder()
        {
            _insuranceCost = Decimal.Zero;
            _warrantyCost = Decimal.Zero;
            _shippingCost = Decimal.Zero;
        }

        public ChargeBuilder SetDatabase(AuleaseEntities db) { this._db = db; return this; }
        public ChargeBuilder SetSystemGroup(SystemGroup group) { this._systemGroup = group; return this; }
        public ChargeBuilder SetComponentCosts(List<decimal> costs) { this._ListOfComponentCosts = costs; return this; }
        public ChargeBuilder SetDateRange(DateTime begBillDate, DateTime endbillDate) { this._begBillDate = begBillDate; this._endBillDate = endbillDate; return this; }
        public ChargeBuilder SetInsuranceCost(decimal insuranceCost) { this._insuranceCost = insuranceCost; return this; }
        public ChargeBuilder SetWarrantyCost(decimal warrantyCost) { this._warrantyCost = warrantyCost; return this; }
        public ChargeBuilder SetShippingCost(decimal shippingCost) { this._shippingCost = shippingCost; return this; }

        #endregion

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

            if (_ListOfComponentCosts.Count != _systemGroup.Leases.Select(n => n.Component).Count())
            {
                throw new ArgumentOutOfRangeException("An invalid number of costs was given for the SystemGroup");
            }
        }

        public void Apply()
        {
            if (this._db == null) 
            {
                throw new MissingFieldException("ChargeBuilder database was not set");
            }

            this._db.SaveChanges();
        }

    }
}
