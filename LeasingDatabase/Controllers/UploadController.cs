using CWSToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public ActionResult FillSN()
		{
			string fileText = "";
			for (int i = 0; i < Request.Files.Count; i++)
			{
				HttpPostedFileBase f = Request.Files[i];
				using (StreamReader sr = new StreamReader(f.InputStream))
				{
					fileText = sr.ReadToEnd();
				}
			}

            List<string> errors = new List<string>();

			string[] SRGroup = Core.Scan.Parse(fileText, ref errors);

            ViewBag.Errors = errors;

			string query = "";
			int count = 0;
			foreach (string item in SRGroup)
			{
				if (count > 0)
				{
					query += "&";
				}
				query += "SRs[" + count + "]=" + item.ToString();
				count++;
			}

			if (SRGroup.Count() > 0)
			{
				return Redirect(Url.Action("Index", "SR") + "?" + query);
			}

			return RedirectToAction("Index", "Upload");
		}

    }
}
