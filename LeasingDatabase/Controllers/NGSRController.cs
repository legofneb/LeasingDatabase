using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

//Takes an srNumber and creates a pdf from the database and fills in a form with the data
//AULeaseForm is stored on Dropbox and oitmss-201\Bens Stuff
namespace LeasingDatabase.Controllers
{
    public class NGSRController : Controller
    {

        public ActionResult Index(string srNumber)
        {

            string blankForm = @"C:\Users\jan0018\Documents\auleaseDatabase\LeasingDatabase\Content\AULeaseForm.pdf";
            
            AuleaseEntities db = new AuleaseEntities();
            List<Component> comps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.PO.PONumber == srNumber).OrderBy(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID).ThenBy(p => p.TypeId).ToList();
            if (comps.Count == 0)
            {
                throw new Exception("No SR's found");
            }
            int componentCount = comps.First().SystemGroup.ComponentCount;

            if (componentCount == 0)
            {
                throw new NotSupportedException("Can't accurately determine the System Grouping.");
            }

            List<byte[]> byteList = new List<byte[]>();
            for (int i = 0; i < comps.Count; i = i + componentCount)
            {
                MemoryStream newFileStream = new MemoryStream();
                FileStream existingFileStream = new FileStream(blankForm, FileMode.Open);

                var reader = new PdfReader(existingFileStream);
                var stamper = new PdfStamper(reader, newFileStream);

                AcroFields form = stamper.AcroFields;
                
                BaseFont font = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.WINANSI, BaseFont.EMBEDDED);
                form.AddSubstitutionFont(font);

                form.SetFieldProperty("Department", "textsize", 10f, null);
                form.SetFieldProperty("SR", "textsize", 10f, null);
                form.SetFieldProperty("Order Number", "textsize", 10f, null);
                form.SetFieldProperty("Manufacturer", "textsize", 10f, null);
                form.SetFieldProperty("Model", "textsize", 10f, null);
                form.SetFieldProperty("Component Type", "textsize", 10f, null);
                form.SetFieldProperty("Lease Tag", "textsize", 10f, null);
                form.SetFieldProperty("Serial Number", "textsize", 10f, null);
                form.SetFieldProperty("Monitor Lease Tag", "textsize", 10f, null);
                form.SetFieldProperty("Monitor Serial", "textsize", 10f, null);
                form.SetFieldProperty("2nd Monitor Lease Tag", "textsize", 10f, null);
                form.SetFieldProperty("2nd Monitor Serial", "textsize", 10f, null);
                form.SetFieldProperty("Statement Name", "textsize", 12f, null);
                form.SetFieldProperty("Name", "textsize", 12f, null);
                form.SetFieldProperty("UserID", "textsize", 12f, null);
                form.SetFieldProperty("Notes", "textsize", 10f, null);
                form.SetFieldProperty("OS", "textsize", 10f, null);
                form.SetFieldProperty("RateLevel", "textsize", 10f, null);
                form.SetFieldProperty("Delivery Information", "textsize", 10f, null);
                form.SetFieldProperty("EOL CPU Serial Number", "textsize", 10f, null);
                form.SetFieldProperty("EOL Monitor SerialNumber", "textsize", 10f, null);
                form.SetFieldProperty("EOL Monitor SerialNumber_2", "textsize", 10f, null);
                if (comps[i].InstallSoftware)
                {
                    form.SetField("Maintenance", "Yes");
                }
                form.SetField("Department", comps[i].Department.Name);
                form.SetField("SR", srNumber);
                form.SetField("Order Number", comps[i].OrderNumber);
                form.SetField("Manufacturer", comps[i].Make.Name);
                form.SetField("Model", comps[i].Model.Name);
                form.SetField("Component Type", comps[i].Type.Name);
                form.SetField("Lease Tag", comps[i].LeaseTag);
                form.SetField("Serial Number", comps[i].SerialNumber);
                if (componentCount > 1)
                {
                    form.SetField("Monitor Lease Tag", comps[i + 1].LeaseTag);
                    form.SetField("Monitor Serial", comps[i + 1].SerialNumber);
                }
                if (componentCount > 2)
                {
                    form.SetField("2nd Monitor Lease Tag", comps[i + 2].LeaseTag);
                    form.SetField("2nd Monitor Serial", comps[i + 2].SerialNumber);
                }
                form.SetField("Statement Name", comps[i].CurrentLease.StatementName);
                form.SetField("Name", comps[i].SystemGroup.User.FirstName + " " + comps[i].SystemGroup.User.LastName);
                form.SetField("UserID", comps[i].User.GID);
                form.SetField("Notes", comps[i].Note + " Deliver To: " + (comps[i].Location == null ? "" : comps[i].Location.Building + " " + comps[i].Location.Room + " " + comps[i].User.Phone));
                form.SetField("OS", comps[i].Properties.Any(n => n.Key == "Operating System") ? comps[i].Properties.Where(n => n.Key == "Operating System").FirstOrDefault().Value : null);
                form.SetField("RateLevel", comps[i].CurrentLease.Overhead.RateLevel);
                form.SetField("Delivery Information", comps[i].Note + " Deliver To: " + (comps[i].Location == null ? "" : comps[i].Location.Building + " " + comps[i].Location.Room + " " + comps[i].User.Phone));
                if (comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 0)
                {
                    form.SetField("EOL CPU Serial Number", comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 0 ? comps[i].CurrentLease.SystemGroup.EOLComponents.OrderBy(o => o.TypeId).ThenBy(o => o.Id).Take(1).FirstOrDefault().SerialNumber : "");
                }
                if (comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 1)
                {
                    form.SetField("EOL Monitor SerialNumber", comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 1 ? comps[i].CurrentLease.SystemGroup.EOLComponents.OrderBy(o => o.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().SerialNumber : "");
                }
                if (comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 2)
                {
                    form.SetField("EOL Monitor SerialNumber_2", comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 2 ? comps[i].CurrentLease.SystemGroup.EOLComponents.OrderBy(o => o.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().SerialNumber : "");
                }

                stamper.FormFlattening = true;

                stamper.Close();
                reader.Close();
                existingFileStream.Close();

                byte[] byteArray = newFileStream.ToArray();
                byteList.Add(byteArray);

            }

            byte[] OutputByte = MergePdfForms(byteList);
            MemoryStream OutputStream = new MemoryStream(OutputByte);

            return File(OutputStream, "application/octat-stream", "SRform.pdf");
        }

        //merges multiple pdf files into one pdf file
        public byte[] MergePdfForms(List<byte[]> files)
        {
            if (files.Count > 1)
            {
                string[] names;
                PdfStamper stamper;
                MemoryStream msTemp = null;
                PdfReader pdfTemplate = null;
                PdfReader pdfFile;
                Document doc;
                PdfWriter pCopy;
                MemoryStream msOutput = new MemoryStream();

                pdfFile = new PdfReader(files[0]);

                doc = new Document();
                pCopy = new PdfSmartCopy(doc, msOutput);
                pCopy.PdfVersion = PdfWriter.VERSION_1_7;

                doc.Open();

                for (int k = 0; k < files.Count; k++)
                {
                    for (int i = 1; i < pdfFile.NumberOfPages + 1; i++)
                    {
                        msTemp = new MemoryStream();
                        pdfTemplate = new PdfReader(files[k]);

                        stamper = new PdfStamper(pdfTemplate, msTemp);

                        names = new string[stamper.AcroFields.Fields.Keys.Count];
                        stamper.AcroFields.Fields.Keys.CopyTo(names, 0);
                        foreach (string name in names)
                        {
                            stamper.AcroFields.RenameField(name, name + "_file" + k.ToString());
                        }

                        stamper.Close();
                        pdfFile = new PdfReader(msTemp.ToArray());
                        ((PdfSmartCopy)pCopy).AddPage(pCopy.GetImportedPage(pdfFile, i));
                        pCopy.FreeReader(pdfFile);
                    }
                }

                pdfFile.Close();
                pCopy.Close();
                doc.Close();

                return msOutput.ToArray();
            }
            else if (files.Count == 1)
            {
                return new MemoryStream(files[0]).ToArray();
            }

            return null;
        }
    }
}
