using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/

        public PartialViewResult Type()
        {
            return PartialView();
        }

    }
}
