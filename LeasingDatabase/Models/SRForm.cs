using aulease.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LeasingDatabase.Models
{
    public class SRForm
    {

        public SRForm(string SR)
        {
        }

        public SRForm(List<SRFormModel> ListOfDataForForms)
        {

        }

        public static SRForm GenerateFormFromSR(string SRNumber, AuleaseEntities db)
        {
            List<SystemGroup> SystemGroups = db.POes.Where(n => n.PONumber == SRNumber).Single().SystemGroups.ToList();

            List<SRFormModel> ListOfDataForForms = new List<SRFormModel>();

            foreach (SystemGroup group in SystemGroups)
            {
                Lease PrimaryLease = group.Leases.OrderBy(n => n.Component.TypeId).FirstOrDefault();
                Component PrimaryComponent = PrimaryLease.Component;

                SRFormModel data = new SRFormModel();

                data.SR = SRNumber;
                data.FOP = group.Leases.FirstOrDefault().Department.GetFOP();
                data.DepartmentName = group.Leases.FirstOrDefault().Department.Name;
                data.FirstName = group.User.FirstName;
                data.LastName = group.User.LastName;
                data.Location = group.User.Location.Room + " " + group.User.Location.Building;
                data.GID = group.User.GID;
                data.Notes = PrimaryComponent.Note;
                data.InstallSoftware = PrimaryComponent.InstallSoftware;
                data.OrderNumber = PrimaryComponent.OrderNumber;
                data.ContractNumber = PrimaryLease.ContractNumber;
                data.Architecture = PrimaryComponent.Properties.Where(n => n.Key == "Architecture").SingleOrDefault().Value;
                data.OperatingSystem = PrimaryComponent.Properties.Where(n => n.Key == "Operating System").SingleOrDefault().Value;
                data.RateLevel = PrimaryLease.Overhead.RateLevel;
                data.StatementName = PrimaryLease.StatementName;

                List<SRFormComponentModel> Components = new List<SRFormComponentModel>();
                List<SRFormEOLComponentModel> EOLComponents = new List<SRFormEOLComponentModel>();

                foreach (Component comp in group.Leases.Select(n => n.Component))
                {
                    SRFormComponentModel Component = new SRFormComponentModel();
                    Component.SerialNumber = comp.SerialNumber;
                    Component.LeaseTag = comp.LeaseTag;
                    Component.Manufacturer = comp.MakeId.HasValue ? comp.Make.Name : "";
                    Component.Type = comp.TypeId.HasValue ? comp.Type.Name : "";
                    Component.Model = comp.ModelId.HasValue ? comp.Model.Name : "";

                    Components.Add(Component);
                }

                foreach (Component comp in group.EOLComponents)
                {
                    SRFormEOLComponentModel EOLComponent = new SRFormEOLComponentModel();
                    EOLComponent.SerialNumber = comp.SerialNumber;
                    EOLComponent.LeaseTag = comp.LeaseTag;

                    EOLComponents.Add(EOLComponent);
                }

                data.Components = Components;
                data.EOLComponents = EOLComponents;

                ListOfDataForForms.Add(data);
            }

            return new SRForm(ListOfDataForForms);
        }

        private static void LineBreak(){
            Phrase Linebreak = new Phrase();
			Linebreak.Add(new Chunk("\n\n", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 16)));
        }

        //MemoryStream
        public void GenerateResponse() {}

    }

    public class SRFormModel
    {
        public string SR { get; set; }
        public string FOP { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }
        public string Location { get; set; }
        public string GID { get; set; }
        public string Notes { get; set; }
        public bool InstallSoftware { get; set; }
        public string OrderNumber { get; set; }
        public string ContractNumber { get; set; }
        public string Architecture { get; set; }
        public List<SRFormComponentModel> Components { get; set; }
        public List<SRFormEOLComponentModel> EOLComponents { get; set; }
        public string OperatingSystem { get; set; }
        public string RateLevel { get; set; }
        public string StatementName { get; set; }
    }

    public class SRFormComponentModel
    {
        public string Type { get; set; } // CPU, Laptop, Monitor, etc...
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string LeaseTag { get; set; }
    }

    public class SRFormEOLComponentModel
    {
        public string SerialNumber { get; set; }
        public string LeaseTag { get; set; }
    }

    public class SRFormList
    {
	    public List<SRForm> forms { get; set; }

        public SRFormList() 
        {
            forms = new List<SRForm>();
        }
        public void PrintForm() { }
        // IList methods

        public void Add(SRForm value)
        {
            forms.Add(value);
        }

        public void Clear()
        {
            forms.Clear();
        }

        public bool Contains(SRForm value)
        {
            return forms.Contains(value);
        }

        public int IndexOf(SRForm value)
        {
            return forms.IndexOf(value);
        }

        public void Insert(int index, SRForm value)
        {
            forms.Insert(index, value);
        }

        public void Remove(SRForm value)
        {
            forms.Remove(value);
        }

        public void RemoveAt(int index)
        {
            forms.RemoveAt(index);
        }

        public int Count
        {
            get { return forms.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return forms.GetEnumerator();
        }
    
    }
}