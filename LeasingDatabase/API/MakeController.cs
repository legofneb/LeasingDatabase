﻿using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class MakeViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class MakeController : ApiController
    {

        // GET api/make
        public IEnumerable<MakeViewModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            DateTime SearchDate = DateTime.Now.AddMonths(-36);

            IEnumerable<Make> Makes = db.Orders.Where(n => n.Date >= SearchDate).SelectMany(n => n.SystemGroups).SelectMany(n => n.Leases).Select(n => n.Component).Where(n => n.MakeId != null).Select(n => n.Make).Distinct();

            // Get Makes from the last 36 months
            //IEnumerable<Make> Makes = db.Leases.Where(n => n.MonthlyCharge !=null && n.EndDate.HasValue && n.EndDate.Value > SearchDate).Select(n => n.Component).Select(n => n.Make).Distinct().ToList();

            // Get Makes from new leases
            //IEnumerable<Make> NewMakes = db.Leases.Where(n => n.MonthlyCharge == null || !n.EndDate.HasValue).Select(n => n.Component).Where(n => n.MakeId != null).Select(n => n.Make).Distinct().ToList();

            return Makes.Distinct().Select(n => new MakeViewModel() {Id=n.Id, Name=n.Name});
        }

        // GET api/make/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/make
        public void Post([FromBody]string value)
        {
        }

        // PUT api/make/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/make/5
        public void Delete(int id)
        {
        }
    }
}
