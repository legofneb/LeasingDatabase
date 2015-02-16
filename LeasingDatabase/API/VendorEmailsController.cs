using aulease.Entities;
using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class VendorEmailsController : ApiController
    {
        // GET api/vendoremails
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<VendorEmailJavascriptObject> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            return db.VendorEmails.Select(n => new VendorEmailJavascriptObject
            {
                id = n.Id,
                email = n.EmailAddress
            });
        }

        // POST api/vendoremails
        [AuthorizeUser("Admin")]
        public void Post([FromBody]VendorEmailJavascriptObject vendorEmails)
        {
            AuleaseEntities db = new AuleaseEntities();

            VendorEmail vendor = db.VendorEmails.Where(n => n.Id == vendorEmails.id).Single();

            vendor.EmailAddress = vendorEmails.email;

            db.SaveChanges();
        }

        // PUT api/vendoremails/5
        [AuthorizeUser("Admin")]
        public int Put(string email)
        {
            AuleaseEntities db = new AuleaseEntities();

            VendorEmail NewVendor = new VendorEmail { EmailAddress = email };
            db.VendorEmails.Add(NewVendor);

            db.SaveChanges();

            return NewVendor.Id;
        }

        // DELETE api/vendoremails/5
        [AuthorizeUser("Admin")]
        public void Delete(int id)
        {
            AuleaseEntities db = new AuleaseEntities();

            VendorEmail vendor = db.VendorEmails.Where(n => n.Id == id).Single();

            db.VendorEmails.Remove(vendor);
            db.SaveChanges();
        }

        public class VendorEmailJavascriptObject
        {
            public int id { get; set; }
            public string email { get; set; }
        }
    }
}
