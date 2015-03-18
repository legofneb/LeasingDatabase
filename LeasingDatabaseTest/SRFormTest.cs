using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeasingDatabase.Models;
using aulease.Entities;
using LeasingDatabase.Controllers;

namespace LeasingDatabaseTest
{
    [TestClass]
    public class SRFormTest
    {
        [TestMethod]
        public void CreateSRFormList_Simple()
        {
            AuleaseEntities db = new AuleaseEntities();

            //SRForm form1 = new SRForm("SR5000", db);
            //SRForm form2 = new SRForm("SR5001", db);

            SRFormList FormList = new SRFormList();
            //FormList.Add(form1);
            //FormList.Add(form2);

            //Assert.AreEqual(FormList.Count, 2);
        }

        [TestMethod]
        public void generateNewSRForm()
        {
            AuleaseEntities db = new AuleaseEntities();

            NGSRController controller = new NGSRController();
            controller.Index("SR5000");

        }
    }
}
