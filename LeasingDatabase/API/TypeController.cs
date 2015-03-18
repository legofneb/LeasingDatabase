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
    public class TypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [AuthorizeUser("Admin", "Users")]
    public class TypeController : ApiController
    {
        // GET api/type
        public IEnumerable<TypeViewModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<TypeViewModel> Types = db.Types.ToList().Where(n => n.isNonFinanceType()).OrderBy(n => n.Id).Select(n => new TypeViewModel { Id = n.Id, Name = n.Name });
            return Types;
        }
    }
}
