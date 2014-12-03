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
    public class NewOrdersByPOController : ApiController
    {
        // GET api/newordersbypo
        public IEnumerable<NGNewOrdersByPOModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<NGNewOrdersByPOModel> Orders = NGNewOrdersByPOModel.GetOrdersFromPOs(db.POes);

            return Orders;
        }

        // GET api/newordersbypo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/newordersbypo
        public void Post(NGNewOrdersByPOModel Order)
        {
        }

        // PUT api/newordersbypo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/newordersbypo/5
        public void Delete(int id)
        {
        }
    }
}
