using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class TypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TypeController : ApiController
    {
        // GET api/type
        public IEnumerable<TypeViewModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<TypeViewModel> Types = db.Types.Select(n => new TypeViewModel { Id = n.Id, Name = n.Name });
            return Types;
        }

        // GET api/type/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/type
        public void Post([FromBody]string value)
        {
        }

        // PUT api/type/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/type/5
        public void Delete(int id)
        {
        }
    }
}
