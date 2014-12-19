using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class BillingController : ApiController
    {
        // GET api/billing
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/billing/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/billing
        public void Post([FromBody]string value)
        {
        }

        // PUT api/billing/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/billing/5
        public void Delete(int id)
        {
        }
    }
}
