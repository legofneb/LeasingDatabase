using ConnectUNCWithCredentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CWSToolkit;
using System.Security.Principal;
using System.Web.Configuration;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class DocumentController : Controller
    {
        //
        // GET: /Document/
		
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Documents(string SR)
		{
            string userName = WebConfigurationManager.AppSettings["NetworkCredentialsUsername"];
            string password = WebConfigurationManager.AppSettings["NetworkCredentialsPassword"];
            string domain = WebConfigurationManager.AppSettings["NetworkDomain"];
            string networkPath = WebConfigurationManager.AppSettings["NetworkPath"];

			string startingPath = networkPath;

			// Take a snapshot of the file system.
			System.IO.DirectoryInfo dir;

			IEnumerable<DirectoryInfo> dirList;
			IEnumerable<FileInfo> fileList;

			using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
			{
				if (unc.NetUseWithCredentials(startingPath, userName, domain, password))
				{
					dir = new System.IO.DirectoryInfo(startingPath);
					dirList = dir.GetDirectories("*.*",
							System.IO.SearchOption.TopDirectoryOnly);

					var SRdir = dirList.Where(n => n.Name.ToUpper().Contains(SR.ToUpper())).FirstOrDefault();
					ViewBag.Folder = SRdir.Name;
					fileList = SRdir.GetFiles("*.*",
							System.IO.SearchOption.TopDirectoryOnly);
				}
				else
				{
					throw new Exception("Access to the Network Share Failed.");
				}
			}

			ViewBag.fileList = fileList;

			return View();
		}
    }
}
