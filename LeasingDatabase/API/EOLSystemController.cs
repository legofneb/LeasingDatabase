using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeasingDatabase.Models;
using aulease.Entities;

namespace LeasingDatabase.API
{
    public class EOLSystemController : ApiController
    {
        // GET api/eolsystem
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/eolsystem/5
        public string Get(int id)
        {
            return "value";
        }

        // GET api/EOLSystem?text={{info}}
        public IEnumerable<NGComponentModel> Get(string text)
        {
            string Text = text.Trim();
            IEnumerable<NGComponentModel> EOLSystem;
            AuleaseEntities db = new AuleaseEntities();

            if (IsLeaseTag(Text))
            {
                if (!db.Components.Any(n => n.LeaseTag == Text))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(String.Format("{0} is not a valid lease tag", Text)),
                        ReasonPhrase = "Invalid Lease Tag"
                    };
                    throw new HttpResponseException(resp);
                }

                EOLSystem = db.Components.Where(n => n.LeaseTag == Text).FirstOrDefault().Leases.FirstOrDefault().SystemGroup.Leases.Select(n => new NGComponentModel
                {
                    SerialNumber = n.Component.SerialNumber,
                    LeaseTag = n.Component.LeaseTag
                });
            }
            else
            {
                if (!db.Components.Any(n => n.SerialNumber == Text))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(String.Format("{0} is not a valid Serial Number", Text)),
                        ReasonPhrase = "Invalid Serial Number"
                    };
                    throw new HttpResponseException(resp);
                }

                EOLSystem = db.Components.Where(n => n.LeaseTag == Text).FirstOrDefault().Leases.FirstOrDefault().SystemGroup.Leases.Select(n => new NGComponentModel
                {
                    SerialNumber = n.Component.SerialNumber,
                    LeaseTag = n.Component.LeaseTag
                });
            }

            return EOLSystem;
        }

        private bool IsLeaseTag(string text)
        {
            if (text.ToUpper().StartsWith("AU") && text.Length == 7)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // POST api/eolsystem
        public void Post([FromBody]string value)
        {
        }

        // PUT api/eolsystem/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/eolsystem/5
        public void Delete(int id)
        {
        }
    }
}
