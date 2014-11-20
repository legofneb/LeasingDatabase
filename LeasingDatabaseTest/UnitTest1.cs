using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using aulease.Entities;
using System.Linq;

namespace LeasingDatabaseTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            AuleaseEntities db = new AuleaseEntities("TestDB");
            int count = db.Components.Count();

            Assert.Equals(count, 0);

        }
    }
}
