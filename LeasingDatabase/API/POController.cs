using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class POController : ApiController
    {
        // GET api/po
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/po/5
        public string Get(int id)
        {
            return "value";
        }

        public string Get(string condition)
        {
            if (!condition.Equals("new"))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(String.Format("{0} is not a valid condition", condition)),
                    ReasonPhrase = "Invalid Condition"
                };
                throw new HttpResponseException(resp);
            }

            AuleaseEntities db = new AuleaseEntities();

            string LatestSR = db.POes.Where(n => n.PONumber.StartsWith("SR")).Where(n => n.SystemGroups.Select(o => o.Leases).Count() > 0).OrderByDescending(n => n.PONumber).FirstOrDefault().PONumber;
            string NewSR = IncrementPOByOne(LatestSR);

            return NewSR;
        }

        private string IncrementPOByOne(string latestSR)
        {
            int Digits = Int32.Parse(Regex.Match(latestSR, @"\d+").Value);
            Digits++;
            return "SR" + Digits;
        }

        // POST api/po
        public void Post(GroupModel req)
        {
            List<int> cart = req.cart;
            string newSR = req.newSR.ToUpper().Trim();

            AuleaseEntities db = new AuleaseEntities();

            List<Order> orders = db.Orders.Where(n => cart.Contains(n.Id)).ToList();
            PO SR;

            if (db.POes.Any(n => n.PONumber.ToUpper() == newSR))
            {
                SR = db.POes.Where(n => n.PONumber.ToUpper() == newSR).Single();
            }
            else
            {
                SR = new PO() { PONumber = newSR };
                db.POes.Add(SR);
            }

            foreach (var order in orders)
            {
                foreach (var systemGroup in order.SystemGroups)
                {
                    systemGroup.PO = SR;
                }
            }

            db.SaveChanges();
        }

        public class GroupModel
        {
            public List<int> cart { get; set; }
            public string newSR { get; set; }
        }
    }
}
