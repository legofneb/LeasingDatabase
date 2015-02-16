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
    public class RepairsViewModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string GID { get; set; }
        public string SerialNumber { get; set; }
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string LeaseTag { get; set; }
        public string Note { get; set; } // Current Repair
        public string Assignee { get; set; } // Current Assignee
        public string DateCreated { get; set; }
        public bool Completed { get; set; }
        public List<RepairHistory> PastRepairs { get; set; }
    }

    public class RepairHistory
    {
        public string Note { get; set; }
        public string Date { get; set; }
        public string Assignee { get; set; }
    }

    public class RepairsController : ApiController
    {
        // GET api/repairs
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<RepairsViewModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            List<Component> RepairedComps = db.Components.Where(n => n.Repairs.Count > 0).ToList(); // Get Components that have been repaired

            List<RepairsViewModel> model = RepairedComps.Select(n => new RepairsViewModel
            {
                id = n.Repairs.OrderByDescending(o => o.Timestamp).FirstOrDefault().Id,
                Name = n.User.FirstName + " " + n.User.LastName,
                SerialNumber = n.SerialNumber,
                LeaseTag = n.LeaseTag,
                GID = n.User.GID,
                Type = n.Type.Name,
                Make = n.Make.Name,
                Model = n.Model.Name,
                Completed = n.Repairs.OrderByDescending(o => o.Timestamp).FirstOrDefault().Status.Name == "Completed Repair" ? true: false,
                Note = n.Repairs.OrderByDescending(o => o.Timestamp).FirstOrDefault().Note,
                Assignee = n.Repairs.OrderByDescending(o => o.Timestamp).FirstOrDefault().Assignee,
                DateCreated = n.Repairs.OrderByDescending(o => o.Timestamp).FirstOrDefault().Timestamp.ToString("d"),
                PastRepairs = n.Repairs.OrderByDescending(o => o.Timestamp).Skip(1).Select(o => new RepairHistory
                {
                    Note = o.Note,
                    Date = o.Timestamp.ToString("d"),
                    Assignee = o.Assignee
                }).ToList()
            }).ToList();

            return model;
        }

        // POST api/repairs
        [AuthorizeUser("Admin", "Users")]
        public void Post([FromBody]dynamic repair)
        {
            string SerialNumber = repair.SerialNumber;
            string Assignee = repair.Assignee;
            string Note = repair.Note;

            AuleaseEntities db = new AuleaseEntities();

            if (db.Components.Any(n => n.SerialNumber == SerialNumber))
            {
                Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
                Status NewRepairStatus = db.Status.Where(n => n.Id == Status.NewRepair).Single();
                Repair NewRepair = new Repair { Assignee = Assignee, Timestamp = DateTime.Now, Note = Note, Component = comp, Status = NewRepairStatus };
                db.Repairs.Add(NewRepair);
                db.SaveChanges();

                sendRepairEmail(Assignee, comp, Note);
                
            }
        }

        // PUT api/repairs/5
        [AuthorizeUser("Admin", "Users")]
        public void Put(int id, [FromBody]dynamic repair)
        {
            string SerialNumber = repair.SerialNumber;
            string Assignee = repair.Assignee;
            string Note = repair.Note;
            bool repairCompleted = repair.Completed == "True";

            AuleaseEntities db = new AuleaseEntities();

            bool completedRepair = db.Components.Where(n => n.SerialNumber == SerialNumber).Single()
                                    .Repairs.OrderByDescending(n => n.Timestamp).FirstOrDefault().Status.Id == Status.CompletedRepair;

            if (completedRepair)
            {
                return;
            }

            if (!repairCompleted)
            {
                // Repair Info Update

                if (db.Components.Any(n => n.SerialNumber == SerialNumber))
                {
                    Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
                    Repair CurrentRepair = comp.Repairs.OrderByDescending(n => n.Timestamp).FirstOrDefault();

                    CurrentRepair.Assignee = Assignee;
                    CurrentRepair.Note = Note;

                    db.SaveChanges();
                }
            }
            else
            {
                // Close Ticket
                if (db.Components.Any(n => n.SerialNumber == SerialNumber))
                {
                    Component comp = db.Components.Where(n => n.SerialNumber == SerialNumber).Single();
                    Repair CurrentRepair = comp.Repairs.OrderByDescending(n => n.Timestamp).FirstOrDefault();

                    CurrentRepair.Assignee = Assignee;
                    CurrentRepair.Note = Note;
                    CurrentRepair.Status = db.Status.Where(n => n.Id == Status.CompletedRepair).Single();

                    db.SaveChanges();
                }

            }
        }

        // DELETE api/repairs/5
        public void Delete(int id)
        {
        }

        public void sendRepairEmail(string Assignee, Component comp, string RepairNote)
        {
            Email email = new Email();
            email.AddTo(Assignee + "@auburn.edu");
            email.AddCC("wds0002@auburn.edu");
            // email.From("aulease@auburn.edu"); //emails from donotreply@auburn.edu if excluded
            email.Subject = "AU Lease Repair Ticket";
            email.HTML = true;
            email.Body = "<p>You have a new ticket assigned to you for an AU Lease machine. Please attend to the machine at your earliest convinience" +
                         " and be sure to update any status updates in the database.</p>" +
                         "<p>Serial Number: " + comp.SerialNumber + "</p>" +
                         "<p>Lease Tag: " + comp.LeaseTag + "</p>" +
                         "<p>Component: " + comp.Type.Name + "</p>" +
                         "<p>Notes: " + RepairNote + "</p>";

            email.Send();
        }
    }
}
