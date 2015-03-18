using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using aulease.Entities;
using System.Data.Entity;
using LeasingDatabase.Models;
using System.Text.RegularExpressions;
using CWSToolkit;

namespace LeasingDatabase.API
{
    public class NewOrdersController : ApiController
    {
        // GET api/neworders
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<NGNewOrdersModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<NGNewOrdersModel> Orders = db.Orders.Where(n => n.SystemGroups.Any(o => o.Leases.Any(p => p.MonthlyCharge == null)) && n.SystemGroups.Any(o => o.PO.PONumber == null || o.PO.PONumber.Length < 3))
                                                          .OrderByDescending(n => n.Date).ToList()
                .Select(n => new NGNewOrdersModel
            {
                id = n.Id,
                Date = n.Date.ToString("d"),
                OrdererFirstName = n.User.FirstName,
                OrdererLastName = n.User.LastName,
                OrdererGID = n.User.GID,
                OrdererBuilding = n.User.Location.Building,
                OrdererRoom = n.User.Location.Room,
                OrdererPhone = n.User.Phone,
                Configuration = n.SystemGroups.FirstOrDefault().Leases.Select(o => o.Component).OrderBy(o => o.TypeId).Select(o => new NGConfigurationModel {
                    Type = o.Type != null ? o.Type.Name:null,
                    Make = o.Make != null ? o.Make.Name : null,
                    Model = o.Model != null ? o.Model.Name : null
                }),
                Components = n.SystemGroups.Select(o => new NGOrderSystemGroupModel 
                {
                    id = o.Id,
                    StatementName = o.Leases.FirstOrDefault().StatementName,
                    FirstName = o.User.FirstName,
                    LastName = o.User.LastName,
                    GID = o.User.GID,
                    DepartmentName = o.Leases.FirstOrDefault().Department.Name,
                    FOP = o.Leases.FirstOrDefault().Department.GetFOP(),
                    OperatingSystem = o.Leases.Select(p => p.Component).OrderBy(p => p.TypeId).FirstOrDefault().TypeId.HasValue && (o.Leases.Select(p => p.Component).OrderBy(p => p.TypeId).FirstOrDefault().Type.Name == "CPU" || o.Leases.Select(p => p.Component).OrderBy(p => p.TypeId).FirstOrDefault().Type.Name == "Laptop") && o.Leases.Select(p => p.Component).OrderBy(p => p.TypeId).FirstOrDefault().Properties.Where(p => p.Key == "Operating System").Count() > 0 ? o.Leases.Select(p => p.Component).OrderBy(p => p.TypeId).FirstOrDefault().Properties.Where(p => p.Key == "Operating System").FirstOrDefault().Value : null,
                    RateLevel = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.RateLevel : null,
                    Term = o.Leases.FirstOrDefault().Overhead != null ? o.Leases.FirstOrDefault().Overhead.Term : (int?)null,
                    InstallHardware = o.Leases.FirstOrDefault().Component.InstallHardware,
                    InstallSoftware = o.Leases.FirstOrDefault().Component.InstallSoftware,
                    Renewal = o.Leases.FirstOrDefault().Component.Renewal
                }),
                Summary = n.SystemGroups.FirstOrDefault().ToString(),
                Notes = n.Note
            });

            return Orders;
        }

        // POST api/neworders
        [AuthorizeUser("Admin")]
        public void Post([FromBody]NGNewOrdersModel order)
        {
            AuleaseEntities db = new AuleaseEntities();
            Order SelectedOrder;

            if (db.Orders.Any(n => n.Id == order.id))
            {
                SelectedOrder = db.Orders.Where(n => n.Id == order.id).Single();
                SelectedOrder.User = FindOrCreateUser(db, order.OrdererGID);
                SelectedOrder.Note = order.Notes;
            }
            else
            {
                SelectedOrder = new Order();
                SelectedOrder.User = FindOrCreateUser(db, order.OrdererGID);
                SelectedOrder.Note = order.Notes;
                SelectedOrder.Date = DateTime.Now;
                db.Orders.Add(SelectedOrder);
            }

            SelectedOrder.User = FindOrCreateUser(db, order);
            SelectedOrder.Note = order.Notes;

            EvaluateConfiguration(db, SelectedOrder, order.Configuration, order.Components.Select(n => n.id));
            

            UpdateComponent(db, SelectedOrder, order.Components.ToList());

            db.SaveChanges();
        }

        private void UpdateComponent(AuleaseEntities db, Order order, List<NGOrderSystemGroupModel> systemGroupModels)
        {
            foreach (SystemGroup group in order.SystemGroups)
            {

                NGOrderSystemGroupModel model;
                if (group.Id == 0)
                {
                    int index = systemGroupModels.IndexOf(systemGroupModels.Where(n => n.id == 0).FirstOrDefault());
                    model = systemGroupModels[index];
                    systemGroupModels.RemoveAt(index);
                }
                else
                {
                    model = systemGroupModels.Where(n => n.id == group.Id).Single();
                }

                group.User = FindOrCreateUser(db, model.GID);

                foreach (var lease in group.Leases)
                {
                    lease.StatementName = model.StatementName;
                    lease.Department = FindOrCreateDepartment(db, model.FOP, model.DepartmentName);
                    lease.Component.InstallHardware = model.InstallHardware;
                    lease.Component.InstallSoftware = model.InstallSoftware;
                    lease.Component.Renewal = model.Renewal;

                    if (model.Term.HasValue)
                    {
                        lease.Overhead = FindOverhead(db, model.RateLevel, model.Term.Value);
                    }

                    if (!String.IsNullOrWhiteSpace(model.OperatingSystem) && lease.Component.TypeId.HasValue && (lease.Component.Type.Name == "CPU" || lease.Component.Type.Name == "Laptop"))
                    {
                        Property OldProperty = lease.Component.Properties.Where(n => n.Key == "Operating System").SingleOrDefault();

                        if (OldProperty != null)
                        {
                            lease.Component.Properties.Remove(OldProperty);
                        }

                        string os = model.OperatingSystem.Trim();
                        Property NewOS = db.Properties.Where(n => n.Key == "Operating System" && n.Value == os).Single();

                        lease.Component.Properties.Add(NewOS);
                    }
                }
            }

        }

        private Overhead FindOverhead(AuleaseEntities db, string rateLevel, int term)
        {
            if (db.Overheads.Any(n => n.Term == term && n.RateLevel == rateLevel))
            {
                return db.Overheads.Where(n => n.Term == term && n.RateLevel == rateLevel).OrderByDescending(n => n.BeginDate).First();
            }
            else
            {
                throw new Exception("Overhead not found");
            }
        }

        private Department FindOrCreateDepartment(AuleaseEntities db, string FOP, string DepartmentName)
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
                return new Department() { Fund = Fund, Org = Org, Program = Program, Name = DepartmentName };
            }
        }

        private void EvaluateConfiguration(AuleaseEntities db, Order order, IEnumerable<NGConfigurationModel> config, IEnumerable<int> systemGroupIDs)
        {
            foreach (int i in systemGroupIDs.Where(n => n == 0))
            {
                SystemGroup group = new SystemGroup();
                Lease lease = new Lease();
                Component comp = new Component();
                comp.StatusId = 1;
                comp.Leases.Add(lease);
                lease.Component = comp;
                lease.Timestamp = DateTime.Now;
                group.Leases.Add(lease);
                
                order.SystemGroups.Add(group);
            }

            List<SystemGroup> SystemGroupsToDelete = new List<SystemGroup>();
            foreach (SystemGroup group in order.SystemGroups.Where(n => !systemGroupIDs.Contains(n.Id)))
            {
                // delete systemgroup
                SystemGroupsToDelete.Add(group);
            }

            foreach (var group in SystemGroupsToDelete)
            {
                order.SystemGroups.Remove(group);
            }

            foreach (SystemGroup group in order.SystemGroups)
            {

                if (group.Leases.Select(n => n.Component).Count() < config.Count())
                {
                    // Add Components
                    while (group.Leases.Select(n => n.Component).Count() < config.Count())
                    {
                        Component comp = group.Leases.Select(n => n.Component).OrderBy(n => n.TypeId.HasValue).ThenBy(n => n.TypeId).ThenBy(n => n.ModelId.HasValue).ThenBy(n => n.ModelId).Last();
                        Lease lease = comp.Leases.Single();

                        Lease NewLease = lease.Clone();
                        Component NewComponent = comp.Clone();

                        NewComponent.Leases.Add(NewLease);
                        NewLease.Component = NewComponent;

                        group.Leases.Add(NewLease);
                    }
                }
                else if (group.Leases.Select(n => n.Component.Type).Count() > config.Count())
                {
                    while(group.Leases.Select(n => n.Component).Count() > config.Count())
                    {
                        Component comp = group.Leases.Select(n => n.Component).OrderBy(n => n.TypeId.HasValue).ThenBy(n => n.TypeId).ThenBy(n => n.ModelId.HasValue).ThenBy(n => n.ModelId).Last();
                        Lease lease = comp.Leases.Single();

                        group.Leases.Remove(lease);

                        db.Entry(lease).State = EntityState.Deleted;
                        db.Entry(comp).State = EntityState.Deleted;
                    }
                }

                for (int j = 0; j < config.Count(); j++)
                {
                    Component comp = group.Leases.Select(n => n.Component).Skip(j).Take(1).Single();
                    string typeName = config.Skip(j).Take(1).Single().Type;
                    if (typeName != null)
                    {
                        comp.Type = db.Types.Where(n => n.Name == typeName).Single();
                        comp.Make = FindOrCreateMake(db, config.Skip(j).Take(1).Single().Make);
                        comp.Model = FindOrCreateModel(db, comp.Make, config.Skip(j).Take(1).Single().Model);
                    }
                    else
                    {
                        comp.Type = null;
                        comp.Make = null;
                        comp.Model = null;
                    }
                }
            }
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
            }

            return UserObject;
        }

        private User FindOrCreateUser(AuleaseEntities db, NGNewOrdersModel order)
        {
            User UserObject;
            if (db.Users.Any(n => n.GID == order.OrdererGID))
            {
                UserObject = db.Users.Where(n => n.GID == order.OrdererGID).Single();
            }
            else
            {
                UserObject = new User();
                UserObject.GID = order.OrdererGID;
            }

            UserObject.Location.Building = order.OrdererBuilding;
            UserObject.Location.Room = order.OrdererRoom;
            UserObject.Phone = order.OrdererPhone;

            return UserObject;
        }

        // PUT api/neworders/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/neworders/5
        public void Delete(int id)
        {
            AuleaseEntities db = new AuleaseEntities();
            Order Order = db.Orders.Where(n => n.Id == id).Single();

            List<Component> Components = Order.SystemGroups.SelectMany(n => n.Leases).Select(n => n.Component).ToList();
            List<Lease> Leases = Order.SystemGroups.SelectMany(n => n.Leases).ToList();
            List<Charge> Charges = Leases.SelectMany(n => n.Charges).ToList();
            List<Component> EOLComponents = Order.SystemGroups.SelectMany(n => n.EOLComponents).ToList();
            List<SystemGroup> SystemGroups = Order.SystemGroups.ToList();

            foreach (var comp in Components)
            {
                foreach (var lease in Leases)
                {
                    comp.Leases.Remove(lease);
                }
                db.Entry(comp).State = EntityState.Deleted;
            }

            foreach (var lease in Leases)
            {
                foreach (var charge in Charges)
                {
                    lease.Charges.Remove(charge);
                }

                db.Entry(lease).State = EntityState.Deleted;
            }

            foreach (var group in SystemGroups)
            {
                foreach (var lease in Leases)
                {
                    group.Leases.Remove(lease);
                }

                db.Entry(group).State = EntityState.Deleted;
            }

            foreach (var group in SystemGroups)
            {
                Order.SystemGroups.Remove(group);
            }

            db.Entry(Order).State = EntityState.Deleted;

            db.SaveChanges();
        }
    }

    public static class FOPLength
    {
        public static int WithDelimeter 
        {
            get 
            {
                return 18;
            }
        }

        public static int WithoutDelimiter
        {
            get
            {
                return 16;
            }
        }
    }
}
