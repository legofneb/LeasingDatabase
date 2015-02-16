using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class BillingRatesController : ApiController
    {
        // GET api/billingrates
        [AuthorizeUser("Admin", "Users")]
        public List<NGRateInfoModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            return NGRateInfoModel.GetRates(db);
        }

        // GET api/billingrates/5
        public string Get(string id)
        {
            return "value";
        }

        // POST api/billingrates
        public void Post([FromBody]IEnumerable<NGRateInfoModel> rateInfoModel)
        {
            AuleaseEntities db = new AuleaseEntities();

            foreach (var model in rateInfoModel)
            {
                VendorRate rate = new VendorRate();
                rate.Term = model.Term;
                rate.Type = db.Types.Where(n => n.Name == model.Type).Single();
                rate.BeginDate = DateTime.Now;
                rate.Rate = model.CurrentRate.Rate;
                db.VendorRates.Add(rate);
            }

            db.SaveChanges();
        }

        // PUT api/billingrates/5
        public void Put([FromBody]IEnumerable<NGRateInfoModel> rateInfoModel)
        {
            AuleaseEntities db = new AuleaseEntities();

            foreach (var model in rateInfoModel)
            {
                VendorRate rate = db.VendorRates.Where(n => n.Term == model.Term && n.Type.Name == model.Type).OrderByDescending(n => n.BeginDate).FirstOrDefault();

                rate.Rate = model.CurrentRate.Rate;
            }

            db.SaveChanges();
        }

        // DELETE api/billingrates/5
        public void Delete(int id)
        {

        }
    }
}
