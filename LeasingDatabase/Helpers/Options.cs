using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using aulease.Entities;

namespace LeasingDatabase
{
    public class Options
    {
        public static string Types()
        {
            AuleaseEntities db = new AuleaseEntities();

            string res = "";
            string open = @"<option>";
            string close = @"</option>";

            foreach (string type in db.Types.Select(n => n.Name))
            {
                res += open + type + close;
            }

            return res;
        }
    }
}