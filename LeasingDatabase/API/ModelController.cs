using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class ModelController : ApiController
    {
        public class ModelViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // GET api/model
        public IEnumerable<ModelViewModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            DateTime SearchDate = DateTime.Now.AddMonths(-36);

            // Get Model from the last 36 months
            IEnumerable<Model> Models = db.Leases.Where(n => n.MonthlyCharge != null && n.EndDate.HasValue && n.EndDate.Value > SearchDate).Select(n => n.Component).Select(n => n.Model).Distinct();

            return Models.Select(n => new ModelViewModel() { Id = n.Id, Name = n.Name });
        }

        // GET api/model/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/model
        public void Post([FromBody]string value)
        {
        }

        // PUT api/model/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/model/5
        public void Delete(int id)
        {
        }
    }
}
