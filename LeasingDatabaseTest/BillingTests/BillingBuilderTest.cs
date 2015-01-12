using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeasingDatabase.Billing;
using aulease.Entities;

namespace LeasingDatabaseTest.BillingTests
{
    [TestClass]
    public class BillingBuilderTest
    {
        public static BillingBuilder CreateDefaultChargeBuilder()
        {
            return new BillingBuilder().SetComponentCosts(new System.Collections.Generic.List<decimal>())
                                                             .SetDateRange(DateTime.Now, DateTime.Now.AddMonths(5))
                                                             .SetSystemGroup(new SystemGroup());           
        }

        [TestMethod]
        public void BillingBuilderInstantiates()
        {
            BillingBuilder chargeBuilder = new BillingBuilder();

            if (chargeBuilder == null)
            {
                Assert.Fail("ChargeBuilder did not create an instance");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void BillingBuilderException_NoDatabaseSet()
        {
            BillingBuilder ChargeBuilder = CreateDefaultChargeBuilder();
            ChargeBuilder.Build();
            ChargeBuilder.Apply();
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void BillingBuilderException_MissingMainFields()
        {
            BillingBuilder ChargeBuilder = new BillingBuilder();
            ChargeBuilder.Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BillingBuilderException_BeforeDate_OccursAfter_AfterDate()
        {
            DateTime BeforeDate = DateTime.MaxValue;
            DateTime AfterDate = DateTime.MinValue;

            BillingBuilder ChargeBuilder = CreateDefaultChargeBuilder();
            ChargeBuilder.SetDateRange(BeforeDate, AfterDate);

            ChargeBuilder.Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BillingBuilderException_ComponentListCount_NotEqualTo_ComponentCostCount()
        {
            BillingBuilder ChargeBuilder = CreateDefaultChargeBuilder();

            List<decimal> Costs = new List<decimal>();
            Costs.Add(2.00M);
            Costs.Add(55.0M);

            SystemGroup Group = new SystemGroup();
            Component Component1 = new Component();
            Lease Lease1 = new Lease();
            Lease1.Component = Component1;
            Group.Leases.Add(Lease1);

            ChargeBuilder.SetComponentCosts(Costs)
                         .SetSystemGroup(Group);

            ChargeBuilder.Build();
        }

        [TestMethod]
        public void BillingBuilderException_NoOperation_OnListWith_ZeroComponents()
        {
            
        }
        
    }
}
