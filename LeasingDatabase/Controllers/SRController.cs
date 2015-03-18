using aulease.Entities;
using CWSToolkit;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Controllers
{
    // TODO: This whole controller is legacy code that needs to be refactored

    // this is an example change

	[AuthorizeUser("Admin", "Users")]
    public class SRController : Controller
    {
        //
        // GET: /SR/

		public ActionResult Index(string[] SRs)
		{
			int count = SRs.Count();

			int i = 0;

			MemoryStream OutputStream = new MemoryStream();

			Document document = new Document();
			PdfWriter writer = PdfWriter.GetInstance(document, OutputStream);
			document.Open();
			while (i < count)
			{
                AuleaseEntities db = new AuleaseEntities();
                string SRString = SRs[i];
                if (db.POes.Any(n => n.PONumber == SRString))
                {
                    List<Lease> leases = db.POes.Where(n => n.PONumber == SRString).Single().SystemGroups.SelectMany(n => n.Leases).ToList();
                    List<Component> comps = leases.Select(n => n.Component).Where(n => n.SerialNumber == null || n.SerialNumber.Length < 2).ToList();
                    if (comps.Count > 0)
                    { i++; continue; }
                }
                else
                {
                    i++; continue;
                }
                

				DataTable dt = SRrow(SRs[i]);
				int insidecount = dt.Rows.Count;
				int j = 0;

				while (j < insidecount)
				{

					if (i > 0 || j > 0)
					{
						document.NewPage();
					}

					string SR = UnderlineFormat(dt.Rows[j][1].ToString(), 25);
					string formattedDepartment = UnderlineFormat(dt.Rows[j][9].ToString(), 120);
					string Maintenance = dt.Rows[j][15].ToString();
					string OrderNumber = UnderlineFormat(dt.Rows[j][16].ToString(), 25);
					string Manufacturer = UnderlineFormat(dt.Rows[j][13].ToString(), 25);
					string Model = UnderlineFormat(dt.Rows[j][14].ToString(), 25);
					string Component = UnderlineFormat(dt.Rows[j][3].ToString(), 25);
					string AULeaseTag = UnderlineFormat(dt.Rows[j][2].ToString(), 25);
					string SerialNumber = UnderlineFormat(dt.Rows[j][0].ToString(), 40);
					string AULeaseTagMon = UnderlineFormat(dt.Rows[j][20].ToString(), 25);
					string SerialNumberMon = UnderlineFormat(dt.Rows[j][19].ToString(), 40);
					string AULeaseTagMon2 = UnderlineFormat(dt.Rows[j][22].ToString(), 20);
					string SerialNumberMon2 = UnderlineFormat(dt.Rows[j][21].ToString(), 35);
					string Name = UnderlineFormat(dt.Rows[j][7].ToString() + " " + dt.Rows[j][8].ToString(), 60);

					string Notes = UnderlineFormat(dt.Rows[j][12].ToString(), 100);
					string Notes2;
					if (dt.Rows[j][12].ToString().Count() > 100)
					{
						Notes2 = UnderlineFormat(dt.Rows[j][12].ToString().Substring(100, dt.Rows[j][12].ToString().Count() - 100), 200);
					}
					else
					{
						Notes2 = UnderlineFormat("", 0);
					}

                    string OS = UnderlineFormat(dt.Rows[j][26].ToString() + dt.Rows[j][18].ToString(), 30);
					string RateLevel = UnderlineFormat(dt.Rows[j][27].ToString(), 38);
					string StatementName = UnderlineFormat(dt.Rows[j][28].ToString(), 80);
					string UserID = UnderlineFormat(dt.Rows[j][11].ToString(), 25);
					string DeliveryInformation = UnderlineFormat(dt.Rows[j][10].ToString(), 110);
					string EOLCPU = UnderlineFormat(dt.Rows[j][23].ToString(), 40);
					string EOLMon = UnderlineFormat(dt.Rows[j][24].ToString(), 40);
					string EOLMon2 = UnderlineFormat(dt.Rows[j][25].ToString(), 40);



					Phrase Linebreak = new Phrase();
					Linebreak.Add(new Chunk("\n\n", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));

					Paragraph Header = new Paragraph("AU Lease Form\n", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14));
					Header.Alignment = Element.ALIGN_CENTER;

					Phrase FirstLine = new Phrase();
					FirstLine.Add(new Chunk("Department:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					FirstLine.Add(new Chunk("  " + formattedDepartment, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase MaintenanceLine = new Phrase();
					MaintenanceLine.Add(new Chunk("      ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					MaintenanceLine.Add(new Chunk((Maintenance == "True" ? "4" : "p"), FontFactory.GetFont(FontFactory.ZAPFDINGBATS, 16)));
					MaintenanceLine.Add(new Chunk("  Maintenance", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));

					Phrase SRLine = new Phrase();
					SRLine.Add(new Chunk("    SR#:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					SRLine.Add(new Chunk("  " + SR, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase OrderNumberLine = new Phrase();
					OrderNumberLine.Add(new Chunk("              Order Number:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					OrderNumberLine.Add(new Chunk("  " + OrderNumber, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase ManufacturerLine = new Phrase();
					ManufacturerLine.Add(new Chunk("    Manufacturer:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					ManufacturerLine.Add(new Chunk("  " + Manufacturer, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase ModelLine = new Phrase();
					ModelLine.Add(new Chunk("   Model:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					ModelLine.Add(new Chunk("  " + Model, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase ComponentLine = new Phrase();
					ComponentLine.Add(new Chunk("   Component Type:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					ComponentLine.Add(new Chunk("  " + Component, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase NewLeaseLine = new Phrase();
					NewLeaseLine.Add(new Chunk("New Lease Computer", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, Font.BOLD)));

					Phrase AULeaseTagLine = new Phrase();
					AULeaseTagLine.Add(new Chunk("Lease Tag:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					AULeaseTagLine.Add(new Chunk("   " + AULeaseTag, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase SerialNumberLine = new Phrase();
					SerialNumberLine.Add(new Chunk("   Serial Number:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					SerialNumberLine.Add(new Chunk("   " + SerialNumber, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase AULeaseTagMonLine = new Phrase();
					AULeaseTagMonLine.Add(new Chunk("Monitor Lease Tag:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					AULeaseTagMonLine.Add(new Chunk("   " + AULeaseTagMon, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase SerialNumberMonLine = new Phrase();
					SerialNumberMonLine.Add(new Chunk("   Monitor Serial:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					SerialNumberMonLine.Add(new Chunk("   " + SerialNumberMon, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase AULeaseTagMonLine2 = new Phrase();
					AULeaseTagMonLine2.Add(new Chunk("2nd Monitor Lease Tag:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					AULeaseTagMonLine2.Add(new Chunk("   " + AULeaseTagMon2, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase SerialNumberMonLine2 = new Phrase();
					SerialNumberMonLine2.Add(new Chunk("   2nd Monitor Serial:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					SerialNumberMonLine2.Add(new Chunk("   " + SerialNumberMon2, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase PrepAndSoftwareLine = new Phrase();
					PrepAndSoftwareLine.Add(new Chunk("Prep and Software Installation Information", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11, Font.BOLD)));

					Phrase PrepLine1 = new Phrase();
					PrepLine1.Add(new Chunk("   p", FontFactory.GetFont(FontFactory.ZAPFDINGBATS, 10)));
					PrepLine1.Add(new Chunk("  Windows Image", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					PrepLine1.Add(new Chunk("                 p", FontFactory.GetFont(FontFactory.ZAPFDINGBATS, 10)));
					PrepLine1.Add(new Chunk("  Join Domain", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));

					Phrase PrepLine2 = new Phrase();
					PrepLine2.Add(new Chunk("   p", FontFactory.GetFont(FontFactory.ZAPFDINGBATS, 10)));
					PrepLine2.Add(new Chunk("  Install Software", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					PrepLine2.Add(new Chunk("                  p", FontFactory.GetFont(FontFactory.ZAPFDINGBATS, 10)));
					PrepLine2.Add(new Chunk("  Custom Settings", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));

                    Phrase StatementNameLine = new Phrase();
                    StatementNameLine.Add(new Chunk("Statement Name:      ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12)));
                    StatementNameLine.Add(new Chunk("   " + StatementName, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase NameLine = new Phrase();
					NameLine.Add(new Chunk("Name:      ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12)));
					NameLine.Add(new Chunk("   " + Name, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase UserIDLine = new Phrase();
					UserIDLine.Add(new Chunk("      UserID:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12)));
					UserIDLine.Add(new Chunk("   " + UserID, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase PreppedByLine = new Phrase();
					PreppedByLine.Add(new Chunk("Prepped by:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					PreppedByLine.Add(new Chunk("                                                  ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase PreppedDateLine = new Phrase();
					PreppedDateLine.Add(new Chunk("    Date:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					PreppedDateLine.Add(new Chunk("                         ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase NoteLine = new Phrase();
					NoteLine.Add(new Chunk("Notes:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					NoteLine.Add(new Chunk("   " + Notes, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase Note2Line = new Phrase();
					Note2Line.Add(new Chunk(Notes2, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase OSLine = new Phrase();
					OSLine.Add(new Chunk("OS:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					OSLine.Add(new Chunk("   " + OS, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase RateLevelLine = new Phrase();
					RateLevelLine.Add(new Chunk("   RateLevel:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					RateLevelLine.Add(new Chunk("   " + RateLevel, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase DeliveryDateLine = new Phrase();
					DeliveryDateLine.Add(new Chunk("Delivery Date:      ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD)));
					DeliveryDateLine.Add(new Chunk("                                                ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase TimeLine = new Phrase();
					TimeLine.Add(new Chunk("      Time:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD)));
					TimeLine.Add(new Chunk("                                   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase ContactLine = new Phrase();
					ContactLine.Add(new Chunk("Contact:      ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					ContactLine.Add(new Chunk("                                                                     ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase PhoneLine = new Phrase();
					PhoneLine.Add(new Chunk("      Phone:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					PhoneLine.Add(new Chunk("                                   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase DeliveryLine = new Phrase();
					DeliveryLine.Add(new Chunk("Delivery Information:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					DeliveryLine.Add(new Chunk("   " + DeliveryInformation, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase InstalledByLine = new Phrase();
					InstalledByLine.Add(new Chunk("Delivered/Installed By:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					InstalledByLine.Add(new Chunk("                                                             ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase InstallDateLine = new Phrase();
					InstallDateLine.Add(new Chunk("    Date:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					InstallDateLine.Add(new Chunk("                         ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase CustomerLine = new Phrase();
					CustomerLine.Add(new Chunk("Customer Signature:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD)));
					CustomerLine.Add(new Chunk("                                                                       ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase CustomerDateLine = new Phrase();
					CustomerDateLine.Add(new Chunk("    Date:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12)));
					CustomerDateLine.Add(new Chunk("                         ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.UNDERLINE)));

					Phrase EndofLeaseLine = new Phrase();
					EndofLeaseLine.Add(new Chunk("End of Lease Computer", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD)));

					Phrase CPUSerialLine = new Phrase();
					CPUSerialLine.Add(new Chunk("CPU Serial Number:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					CPUSerialLine.Add(new Chunk("   " + EOLCPU, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase MonSerialLine = new Phrase();
					MonSerialLine.Add(new Chunk("Monitor Serial Number:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					MonSerialLine.Add(new Chunk("   " + EOLMon, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase MonSerialLine2 = new Phrase();
					MonSerialLine2.Add(new Chunk("Monitor Serial Number:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					MonSerialLine2.Add(new Chunk("   " + EOLMon2, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

					Phrase DamagesLine = new Phrase();
					DamagesLine.Add(new Chunk("Damages:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					DamagesLine.Add(new Chunk("                                                                       ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

		
                    Phrase CustomerInitialsLine = new Phrase();
					CustomerInitialsLine.Add(new Chunk("    Customer Initial:   ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					CustomerInitialsLine.Add(new Chunk("                         ", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10, Font.UNDERLINE)));

                    document.Open();

					document.Add(Header);
					document.Add(new Paragraph("\n"));
					document.Add(FirstLine);
					document.Add(MaintenanceLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(SRLine);
					document.Add(OrderNumberLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(ManufacturerLine);
					document.Add(ModelLine);
					document.Add(ComponentLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 18)));
					document.Add(NewLeaseLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(AULeaseTagLine);
					document.Add(SerialNumberLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(AULeaseTagMonLine);
					document.Add(SerialNumberMonLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(AULeaseTagMonLine2);
					document.Add(SerialNumberMonLine2);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 18)));
					document.Add(PrepAndSoftwareLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					//document.Add(PrepLine1);
					//document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12)));
					//document.Add(PrepLine2);
					//document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(StatementNameLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
                    document.Add(NameLine);
                    document.Add(UserIDLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(PreppedByLine);
					document.Add(PreppedDateLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(NoteLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10)));
					document.Add(Note2Line);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(OSLine);
					document.Add(RateLevelLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 22)));
					document.Add(DeliveryDateLine);
					document.Add(TimeLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 22)));
					document.Add(ContactLine);
					document.Add(PhoneLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 28)));
					document.Add(DeliveryLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(InstalledByLine);
					document.Add(InstallDateLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 26)));
					document.Add(CustomerLine);
					document.Add(CustomerDateLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 26)));
					document.Add(EndofLeaseLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(CPUSerialLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(MonSerialLine);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
					document.Add(MonSerialLine2);
					document.Add(new Phrase(Environment.NewLine, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 22)));
					document.Add(DamagesLine);
					document.Add(CustomerInitialsLine);

					j++;
				}
				i++;
			}

            try
            {
                document.Close();
            }
            catch 
            {
                // temporary catch against errors, refactor

                document = new Document();

                OutputStream = new MemoryStream();

                writer = PdfWriter.GetInstance(document, OutputStream);

                document.Open();
                document.NewPage();
                document.Add(new Paragraph("There were no scanned systems found with this order"));
                document.Close();

            }

            writer.Flush();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=SRform.pdf");
            Response.AddHeader("Content-length", OutputStream.ToArray().Length.ToString());
            Response.OutputStream.Write(OutputStream.GetBuffer(), 0, OutputStream.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.End();

            return File(OutputStream, "application/octat-stream", "SRform.pdf");

			//return View();
		}

		protected string UnderlineFormat(string word, int characters)
		{
			word = word.TrimEnd('\r', '\n');

			string formattedword;

			int count = word.Length;
			if (count >= characters)
			{
				formattedword = word.Substring(0, characters);
			}
			else
			{
				while (characters > count)
				{
					word += " ";
					count++;
				}
				formattedword = word;
			}

			return formattedword;
		}

		protected DataTable SRrow(string SRNumber)
		{

			AuleaseEntities db = new AuleaseEntities();
			List<Component> comps = db.Components.Where(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.PO.PONumber == SRNumber).OrderBy(n => n.Leases.OrderByDescending(o => o.Timestamp).FirstOrDefault().SystemGroup.User.GID).ThenBy(p => p.Leases.OrderByDescending(q => q.Timestamp).FirstOrDefault().SystemGroupId).ThenBy(p => p.TypeId).ToList();
			if (comps.Count == 0)
			{
				throw new Exception("No SR's found");
			}
			int componentCount = comps.First().SystemGroup.ComponentCount;

			if (componentCount == 0)
			{
				throw new NotSupportedException("Can't accurately determine the System Grouping.");
			}

			DataTable dt = new DataTable();
			dt.Columns.Add("Serial Number", typeof(string));
			dt.Columns.Add("SR", typeof(string));
			dt.Columns.Add("Lease Tag", typeof(string));
			dt.Columns.Add("Component", typeof(string));
			dt.Columns.Add("Fund", typeof(string));
			dt.Columns.Add("Org", typeof(string));
			dt.Columns.Add("Program", typeof(string));
			dt.Columns.Add("First Name", typeof(string));
			dt.Columns.Add("Last Name", typeof(string));
			dt.Columns.Add("Department Name", typeof(string));
			dt.Columns.Add("Location", typeof(string));
			dt.Columns.Add("User ID", typeof(string));
			dt.Columns.Add("Notes", typeof(string));
			dt.Columns.Add("Manufacturer", typeof(string));
			dt.Columns.Add("Model", typeof(string));
			dt.Columns.Add("Maintenance", typeof(string));
			dt.Columns.Add("Order Number", typeof(string));
			dt.Columns.Add("Contract Number", typeof(string));
			dt.Columns.Add("Architecture", typeof(string));
			dt.Columns.Add("Serial Number 2", typeof(string));
			dt.Columns.Add("Lease tag 2", typeof(string));
			dt.Columns.Add("Serial Number 3", typeof(string));
			dt.Columns.Add("Lease Tag 3", typeof(string));
			dt.Columns.Add("EOL Serial", typeof(string));
			dt.Columns.Add("EOL Serial 2", typeof(string));
			dt.Columns.Add("EOL Serial 3", typeof(string));
			dt.Columns.Add("OS", typeof(string));
			dt.Columns.Add("Rate Level", typeof(string));
			dt.Columns.Add("Statement Name", typeof(string));
			int i = 0;
			while (i < comps.Count)
			{
				dt.Rows.Add(comps[i].SerialNumber,
							comps[i].CurrentLease.SystemGroup.PO.PONumber,
							comps[i].LeaseTag,
							comps[i].Type.Name,
							comps[i].CurrentLease.Department.Fund,
							comps[i].CurrentLease.Department.Org,
							comps[i].CurrentLease.Department.Program,
							comps[i].CurrentLease.SystemGroup.User.FirstName,
							comps[i].CurrentLease.SystemGroup.User.LastName,
							comps[i].CurrentLease.Department.Name,
							comps[i].Location == null ? "" : comps[i].Location.Building + " " + comps[i].Location.Room + " " + comps[i].User.Phone,
							comps[i].User.GID,
							comps[i].Note + " Deliver To: " + (comps[i].Location == null ? "" : comps[i].Location.Building + " " + comps[i].Location.Room + " " + comps[i].User.Phone),
							comps[i].Make.Name,
							comps[i].Model.Name,
							comps[i].InstallSoftware.ToString(),
							comps[i].OrderNumber,
							comps[i].CurrentLease.ContractNumber,
							(comps[i].Properties.Where(n => n.Key == "Architecture").Count() > 0) ?  comps[i].Properties.Where(n => n.Key == "Architecture").Select(n => n.Value).FirstOrDefault() : "",
							componentCount > 1 ? comps[i + 1].SerialNumber : "",
							componentCount > 1 ? comps[i + 1].LeaseTag : "",
							componentCount > 2 ? comps[i + 2].SerialNumber : "",
							componentCount > 2 ? comps[i + 2].LeaseTag : "",
							comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 0 ? comps[i].CurrentLease.SystemGroup.EOLComponents.OrderBy(o => o.TypeId).ThenBy(o => o.Id).Take(1).FirstOrDefault().SerialNumber : "",
                            comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 1 ? comps[i].CurrentLease.SystemGroup.EOLComponents.OrderBy(o => o.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().SerialNumber : "",
                            comps[i].CurrentLease.SystemGroup.EOLComponents.Count > 2 ? comps[i].CurrentLease.SystemGroup.EOLComponents.OrderBy(o => o.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().SerialNumber : "",
							comps[i].OperatingSystem,
							(comps[i].CurrentLease.Overhead != null) ? comps[i].CurrentLease.Overhead.RateLevel : "",
							comps[i].CurrentLease.StatementName);
				i = i + componentCount;
			}

			return dt;
		}

        public ActionResult Page()
        {
            return View();
        }

    }
}
