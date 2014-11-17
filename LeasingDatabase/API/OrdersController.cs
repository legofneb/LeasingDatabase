using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using aulease.Entities;

namespace LeasingDatabase.API
{
    public class OrdersController : ApiController
    {
        // GET api/orders
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/orders/5
        public string Get(int id)
        {
            return "value";
        }

        // GET api/orders/condition
        // where condition returns a subset of all orders; ex. if condition = new, return just the new orders

        public IEnumerable<Order> Get(string condition)
        {
            AuleaseEntities db = new AuleaseEntities();
            IEnumerable<Order> Orders;

            if (condition.Equals("new", StringComparison.OrdinalIgnoreCase))
            {
                Orders = db.Orders.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber == null)).OrderByDescending(n => n.Date).ToList();
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(String.Format("{0} is not a valid condition", condition)),
                    ReasonPhrase = "Invalid Condition"
                };
                throw new HttpResponseException(resp);
            }

            return Orders;
        }

        // POST api/orders
        public void Post([FromBody]string value)
        {
        }

        // PUT api/orders/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/orders/5
        public void Delete(int id)
        {
        }
    }
}
