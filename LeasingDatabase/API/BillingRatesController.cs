using aulease.Entities;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class BillingRatesController : ApiController
    {
        // GET api/billingrates
        public List<NGRateInfoModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            return NGRateInfoModel.GetRates(db);
        }

        // GET api/billingrates/5
        public string Get(string id)
        {
            return "value";
        }

        // POST api/billingrates
        public void Post([FromBody]string value)
        {
        }

        // PUT api/billingrates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/billingrates/5
        public void Delete(int id)
        {
        }
    }
}
