using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class InventoryController : ApiController
    {
        // GET api/inventory
        public IEnumerable<string> Get()
        {
            return new string[] { "connection", "made" };
        }

        // GET api/inventory/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/inventory
        public void Post([FromBody]string serialNumber, string ipAddress, string macAddress)
        {

        }

        // PUT api/inventory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/inventory/5
        public void Delete(int id)
        {
        }
    }
}
