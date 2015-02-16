using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeasingDatabase.Models;
using aulease.Entities;
using CWSToolkit;

namespace LeasingDatabase.API
{
    public class ComponentsController : ApiController
    {
        private const int StandardQuantityToSendToFrontEnd = 100;

        // GET api/components
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<NGComponentsModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();
            return NGComponentsModel.GetPostBillingComponents(db).Take(StandardQuantityToSendToFrontEnd);
        }

        // GET api/components/lastPageNumber={0}&filteredTerms={1}
        // filteredTerms is an array delimited by spaces
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<NGComponentsModel> Get(int lastPageNumber, string filteredTerms)
        {
            AuleaseEntities db = new AuleaseEntities();

            int skipAmount = lastPageNumber * StandardQuantityToSendToFrontEnd;

            if (String.IsNullOrWhiteSpace(filteredTerms) || filteredTerms.ToUpper().Trim() == "undefined".ToUpper())
            {
                return NGComponentsModel.GetPostBillingComponents(db).Skip(skipAmount).Take(StandardQuantityToSendToFrontEnd);
            }
            else
            {
                return NGComponentsModel.GetPostBillingComponents(db, filteredTerms).Skip(skipAmount).Take(StandardQuantityToSendToFrontEnd);
            }
        }

        // POST api/components
        public void Post([FromBody]string value)
        {
        }

        // PUT api/components/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/components/5
        public void Delete(int id)
        {
        }
    }
}
