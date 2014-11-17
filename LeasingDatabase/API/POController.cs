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

            string LatestSR = db.POes.OrderByDescending(n => n.PONumber).FirstOrDefault().PONumber;
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
        public void Post([FromBody]List<string> orderIds)
        {
            AuleaseEntities db = new AuleaseEntities();
        }

        // PUT api/po/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/po/5
        public void Delete(int id)
        {
        }
    }
}
