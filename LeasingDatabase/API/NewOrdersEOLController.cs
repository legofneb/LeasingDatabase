using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeasingDatabase.Models;
using aulease.Entities;
using System.Web.Mvc;
using CWSToolkit;

namespace LeasingDatabase.API
{
    public class NewOrdersEOLController : ApiController
    { 
        // POST api/neworderseol
        [AuthorizeUser("Admin", "Users")]
        public NGEOLComponentModel Post([FromBody]NGEOLComponentModel eolModel)
        {
            AuleaseEntities db = new AuleaseEntities();

            string SerialNumber = null;
            string LeaseTag = null;

            SerialNumber = eolModel.SerialNumber;
            LeaseTag = eolModel.LeaseTag;

            if (String.IsNullOrWhiteSpace(SerialNumber) && String.IsNullOrWhiteSpace(LeaseTag))
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            Component comp = FindComponent(db, SerialNumber, LeaseTag);

            if (comp == null)
            {
                throw new HttpResponseException(HttpStatusCode.NoContent);
            }

            NGEOLComponentModel EOLComponent = new NGEOLComponentModel
            {
                SerialNumber = comp.SerialNumber,
                LeaseTag = comp.LeaseTag,
                Make = comp.MakeId.HasValue ? comp.Make.Name : null,
                Model = comp.ModelId.HasValue ? comp.Model.Name : null,
                Type = comp.TypeId.HasValue ? comp.Type.Name : null
            };

            return EOLComponent;
        }

        private Component FindComponent(AuleaseEntities db, string SerialNumber, string LeaseTag)
        {
            if (String.IsNullOrWhiteSpace(SerialNumber))
            {
                if (db.Components.Any(n => n.LeaseTag == LeaseTag))
                {
                    return db.Components.Where(n => n.LeaseTag == LeaseTag).Single();
                }
                else
                {
                    return null;
                }
            }
            else if (String.IsNullOrWhiteSpace(LeaseTag))
            {
                if (db.Components.Any(n => n.SerialNumber == SerialNumber))
                {
                    return db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (db.Components.Any(n => n.SerialNumber == SerialNumber || n.LeaseTag == LeaseTag))
                {
                    return db.Components.Where(n => n.SerialNumber == SerialNumber || n.LeaseTag == LeaseTag).Single();
                }
                else
                {
                    return null;
                }
            }
        }

        // PUT api/neworderseol/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/neworderseol/5
        public void Delete(int id)
        {
        }
    }
}
