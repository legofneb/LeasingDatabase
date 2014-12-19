using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeasingDatabase.Billing;
using aulease.Entities;

namespace LeasingDatabaseTest.BillingTests
{
    [TestClass]
    public class ChargeBuilderTest
    {
        public static ChargeBuilder CreateDefaultChargeBuilder()
        {
            return new ChargeBuilder().SetComponentCosts(new System.Collections.Generic.List<decimal>())
                                                             .SetDateRange(DateTime.Now, DateTime.Now.AddMonths(5))
                                                             .SetSystemGroup(new SystemGroup());           
        }

        [TestMethod]
        public void ChargeBuilderInstantiates()
        {
            ChargeBuilder chargeBuilder = new ChargeBuilder();

            if (chargeBuilder == null)
            {
                Assert.Fail("ChargeBuilder did not create an instance");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void ChargeBuilderException_NoDatabaseSet()
        {
            ChargeBuilder ChargeBuilder = CreateDefaultChargeBuilder();
            ChargeBuilder.Build();
            ChargeBuilder.Apply();
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFieldException))]
        public void ChargeBuilderException_MissingMainFields()
        {
            ChargeBuilder ChargeBuilder = new ChargeBuilder();
            ChargeBuilder.Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChargeBuilderException_BeforeDate_OccursAfter_AfterDate()
        {
            DateTime BeforeDate = DateTime.MaxValue;
            DateTime AfterDate = DateTime.MinValue;

            ChargeBuilder ChargeBuilder = CreateDefaultChargeBuilder();
            ChargeBuilder.SetDateRange(BeforeDate, AfterDate);

            ChargeBuilder.Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChargeBuilderException_ComponentListCount_NotEqualTo_ComponentCostCount()
        {
            ChargeBuilder ChargeBuilder = CreateDefaultChargeBuilder();

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
        public void ChargeBuilderException_NoOperation_OnListWith_ZeroComponents()
        {
            
        }
        
    }
}
