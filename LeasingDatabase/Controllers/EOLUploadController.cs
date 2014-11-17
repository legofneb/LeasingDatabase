using aulease.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    public class EOLUploadController : Controller
    {
        //
        // GET: /EOLUpload/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            List<string> errorMessages = new List<string>();
            for (int j = 0; j < Request.Files.Count; j++)
            {
                HttpPostedFileBase f = Request.Files[j];

                using (ExcelPackage p = new ExcelPackage())
                {
                    using (Stream stream = f.InputStream)
                    {
                        p.Load(stream);

                        AuleaseEntities db = new AuleaseEntities();

                        ExcelWorksheet ws = p.Workbook.Worksheets.First();
                        int endRow = ws.Dimension.End.Row;

                        for (int i = 2; i <= endRow; i++)
                        {
                            string SerialNumber = ws.Cells[i, 6].Value.ToString();

                            DateTime EOLDate = DateTime.Parse(ws.Cells[i, 11].Value.ToString());

                            if (db.Components.Any(n => n.SerialNumber == SerialNumber))
                            {
                                Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
                                comp.ReturnDate = EOLDate;
                            }
                            else
                            {
                                errorMessages.Add("No Component with the Serial Number of: " + SerialNumber);
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }

            

            if (errorMessages.Count == 0)
            {
                errorMessages.Add("Completed with 0 errors!");
            }

            ViewBag.errors = errorMessages;

            return View();
        }

    }
}
