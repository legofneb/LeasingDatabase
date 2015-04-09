using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LeasingDatabase.Models;
using aulease.Entities;
using CWSToolkit;
using System.Data.Entity;

namespace LeasingDatabase.API
{
    public class ComponentsController : ApiController
    {
        private const int StandardQuantityToSendToFrontEnd = 100;

        // GET api/components
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<NGComponentsModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();
            return NGComponentsModel.GetPostBillingComponents(db).Take(StandardQuantityToSendToFrontEnd);
        }

        // GET api/components/lastPageNumber={0}&filteredTerms={1}
        // filteredTerms is an array delimited by spaces
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<NGComponentsModel> Get(int lastPageNumber, string filteredTerms)
        {
            AuleaseEntities db = new AuleaseEntities();

            int skipAmount = lastPageNumber * StandardQuantityToSendToFrontEnd;

            if (String.IsNullOrWhiteSpace(filteredTerms) || filteredTerms.ToUpper().Trim() == "undefined".ToUpper())
            {
                return NGComponentsModel.GetPostBillingComponents(db).Skip(skipAmount).Take(StandardQuantityToSendToFrontEnd);
            }
            else
            {
                return NGComponentsModel.GetPostBillingComponents(db, filteredTerms).Skip(skipAmount).Take(StandardQuantityToSendToFrontEnd);
            }
        }

        // POST api/components
        public void Post([FromBody]NGComponentsModel model)
        {
            AuleaseEntities db = new AuleaseEntities();

            foreach (var group in model.SystemGroups)
            {
                SystemGroup DBSystemGroup = db.SystemGroups.Where(n => n.Id == group.id).Single();
                DBSystemGroup.User = FindOrCreateUser(db, group.GID);
                DBSystemGroup.User.Location.Building = group.Building;
                DBSystemGroup.User.Location.Room = group.Room;
                DBSystemGroup.User.Phone = group.Phone;

                foreach (var comp in group.Components)
                {
                    Component DBComponent = db.Components.Where(n => n.Id == comp.id).Single();
                    DBComponent.Type = db.Types.Where(n => n.Name == comp.Type).Single();
                    DBComponent.Make = FindOrCreateMake(db, comp.Make);
                    DBComponent.Model = FindOrCreateModel(db, DBComponent.Make, comp.Model);
                    DBComponent.InstallHardware = comp.InstallHardware;
                    DBComponent.InstallSoftware = comp.InstallSoftware;
                    DBComponent.Renewal = comp.Renewal;
                    DBComponent.LeaseTag = comp.LeaseTag;
                    DBComponent.SerialNumber = comp.SerialNumber;
                    DBComponent.ReturnDate = comp.ReturnDate != null ? new DateTime(comp.ReturnDate.Value) : (DateTime?)null;
                    DBComponent.OrderNumber = comp.OrderNumber;
                    DBComponent.Note = comp.Notes;

                    //taking care of the leases
                    List<Lease> leases = DBComponent.Leases.OrderByDescending(n => n.EndDate).ToList();
                    List<ComponentBillingModel> billModel = comp.BillingData.ToList();

                    int index = 0;

                    while (index < leases.Count && index < comp.BillingData.Count())
                    {
                        // apply the changes to existing leases
                        Lease lease = leases[index];
                        ComponentBillingModel changes = billModel[index];

                        lease.BeginDate = new DateTime(changes.BeginDate);
                        lease.EndDate = new DateTime(changes.EndDate);
                        lease.ContractNumber = changes.ContractNumber;
                        lease.Department = FindOrCreateDepartment(db, changes.FOP);
                        lease.MonthlyCharge = changes.MonthlyCharge;
                        lease.StatementName = changes.StatementName;

                        index++;
                    }

                    if (index == comp.BillingData.Count())
                    {
                        // Extraneous leases need to be deleted
                        while (index < leases.Count)
                        {
                            Lease lease = leases[index];

                            foreach (var charge in lease.Charges)
                            {
                                lease.Charges.Remove(charge);
                            }

                            DBComponent.Leases.Remove(lease);
                            DBSystemGroup.Leases.Remove(lease);

                            Department dept = lease.Department;
                            dept.Leases.Remove(lease);

                            db.Entry(lease).State = EntityState.Deleted;
                            index++;
                        }
                    }
                    else
                    {
                        // leases need to be created
                        while (index < comp.BillingData.Count())
                        {
                            Lease newLease = leases[0].Clone();
                            ComponentBillingModel changes = billModel[index];
                            newLease.BeginDate = new DateTime(changes.BeginDate);
                            newLease.EndDate = new DateTime(changes.EndDate);
                            newLease.ContractNumber = changes.ContractNumber;
                            newLease.Department = FindOrCreateDepartment(db, changes.FOP);
                            newLease.MonthlyCharge = changes.MonthlyCharge;
                            newLease.Component = DBComponent;
                            DBComponent.Leases.Add(newLease);
                            DBSystemGroup.Leases.Add(newLease);

                            index++;
                        }
                    }
                }
            }

            db.SaveChanges();
        }

        // PUT api/components/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/components/5
        public void Delete(int id)
        {
            //unfinalize action
            AuleaseEntities db = new AuleaseEntities();

            PO SR = db.POes.Where(n => n.Id == id).Single();
            List<Lease> leases = SR.SystemGroups.SelectMany(n => n.Leases).ToList();
            foreach (var lease in leases)
            {
                lease.MonthlyCharge = null;
                lease.BeginDate = null;
                lease.EndDate = null;
                lease.ContractNumber = null;
            }

            db.SaveChanges();
        }

        private User FindOrCreateUser(AuleaseEntities db, string userName)
        {
            User UserObject;
            if (db.Users.Any(n => n.GID == userName))
            {
                UserObject = db.Users.Where(n => n.GID == userName).Single();
            }
            else
            {
                UserObject = new User();
                UserObject.GID = userName;
                db.Users.Add(UserObject);
            }

            return UserObject;
        }

        private Model FindOrCreateModel(AuleaseEntities db, Make make, string model)
        {
            if (db.Models.Any(n => n.Make.Name == make.Name && n.Name == model))
            {
                return db.Models.Where(n => n.Make.Name == make.Name && n.Name == model).Single();
            }
            else
            {
                Model NewModel = new Model() { Make = make, Name = model };
                make.Models.Add(NewModel);
                db.Models.Add(NewModel);
                return NewModel;
            }
        }

        private Make FindOrCreateMake(AuleaseEntities db, string make)
        {
            if (db.Makes.Any(n => n.Name == make))
            {
                return db.Makes.Where(n => n.Name == make).Single();
            }
            else
            {
                Make NewMake = new Make() { Name = make };
                db.Makes.Add(NewMake);
                return NewMake;
            }
        }

        private Department FindOrCreateDepartment(AuleaseEntities db, string FOP)
        {
            string Fund;
            string Org;
            string Program;

            FOP = FOP.Replace(" ", String.Empty);

            if (FOP.Length == FOPLength.WithDelimeter)
            {

                string[] FOPArray = FOP.Split(new char[] { '-', ' ', ',' });

                Fund = FOPArray[0];
                Org = FOPArray[1];
                Program = FOPArray[2];

            }
            else if (FOP.Length == FOPLength.WithoutDelimiter)
            {
                Fund = FOP.Substring(0, 6);
                Org = FOP.Substring(6, 6);
                Program = FOP.Substring(12, 4);
            }
            else
            {
                throw new Exception("Unknown FOP Type used");
            }

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                return db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
            }
            else
            {
                return new Department() { Fund = Fund, Org = Org, Program = Program };
            }
        }
    }
}
