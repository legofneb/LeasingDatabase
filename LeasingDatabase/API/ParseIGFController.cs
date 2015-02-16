using aulease.Entities;
using CWSToolkit;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class ParseIGFController : ApiController
    {
        // POST api/parseigf
        [AuthorizeUser("Admin")]
        public List<string> Post()
        {
            var httpRequest = HttpContext.Current.Request;
            List<string> errorMessages = new List<string>();
            for (int j = 0; j < httpRequest.Files.Count; j++)
            {
                var file = httpRequest.Files[j];

                using (ExcelPackage p = new ExcelPackage())
                {
                    using (Stream stream = file.InputStream)
                    {
                        p.Load(stream);

                        AuleaseEntities db = new AuleaseEntities();

                        ExcelWorksheet ws = p.Workbook.Worksheets.First();
                        int endRow = ws.Dimension.End.Row;
                        int EndDateColumnIndex = ScanForEndDateColumn(ws, ws.Dimension.End.Column);
                        int SerialNumberColumnIndex = ScanForSerialNumberColumn(ws, ws.Dimension.End.Column);

                        for (int i = 2; i <= endRow; i++)
                        {
                            string SerialNumber = ws.Cells[i, SerialNumberColumnIndex].Value.ToString();

                            DateTime EOLDate = DateTime.Parse(ws.Cells[i, EndDateColumnIndex].Value.ToString());

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

            return errorMessages;

        }

        private int ScanForSerialNumberColumn(ExcelWorksheet ws, int columnLimit)
        {
            for (int i = 1; i <= columnLimit; i++)
            {
                if (ws.Cells[1, i].GetValue<string>().ToUpper() == "Manufacturer serial number".ToUpper())
                {
                    return i;
                }
            }

            return 6;
        }

        private int ScanForEndDateColumn(ExcelWorksheet ws, int columnLimit)
        {
            for (int i=1; i <= columnLimit; i++)
            {
                if (ws.Cells[1, i].GetValue<string>().ToUpper() == "End of lease date".ToUpper())
                {
                    return i;
                }
            }

            return 13;
        }
    }
}
