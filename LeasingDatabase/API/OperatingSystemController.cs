using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class OperatingSystemModel
    {
        public int id { get; set; }
        public string Name { get; set; }
    }

    public class OperatingSystemController : ApiController
    {
        // GET api/operatingsystem
        public IEnumerable<OperatingSystemModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<Property> OperatingSystems = db.Properties.Where(n => n.Key == "Operating System");

            return OperatingSystems.Select(n => new OperatingSystemModel
            {
                id = n.Id,
                Name = n.Value
            });
        }

        // GET api/operatingsystem/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/operatingsystem
        public void Post([FromBody]string value)
        {
        }

        // PUT api/operatingsystem/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/operatingsystem/5
        public void Delete(int id)
        {
        }
    }
}
