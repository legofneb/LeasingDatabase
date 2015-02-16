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
    public class NGTransferModel
    {
        public string oldSR { get; set; }
        public int id { get; set; } // the Id of the SystemGroup being transferred
        public string newSR { get; set; }
    }

    public class TransferController : ApiController
    {
        // POST api/transfer
        [AuthorizeUser("Admin")]
        public void Post([FromBody]NGTransferModel model)
        {
            // Sanitizing input
            model.newSR = model.newSR.Trim();
            model.oldSR = model.newSR.Trim();

            if (String.IsNullOrWhiteSpace(model.newSR))
            {
                throw new Exception("You must specify a new SR");
            }

            AuleaseEntities db = new AuleaseEntities();

            SystemGroup group = db.SystemGroups.First(n => n.Id == model.id);

            group.PO = FindOrCreatePO(db, model.newSR);
        }

        private PO FindOrCreatePO(AuleaseEntities db, string SR)
        {
            if (db.POes.Any(n => n.PONumber == SR))
            {
                return db.POes.First(n => n.PONumber == SR);
            }
            else
            {
                return new PO() { PONumber = SR };
            }
        }
    }
}
