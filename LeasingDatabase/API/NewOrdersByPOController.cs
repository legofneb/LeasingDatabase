using aulease.Entities;
using CWSToolkit;
using LeasingDatabase.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LeasingDatabase.API
{
    public class NewOrdersByPOController : ApiController
    {
        // GET api/newordersbypo
        [AuthorizeUser("Admin", "Users")]
        public IEnumerable<NGNewOrdersByPOModel> Get()
        {
            AuleaseEntities db = new AuleaseEntities();

            IEnumerable<NGNewOrdersByPOModel> Orders = NGNewOrdersByPOModel.GetOrdersFromPOs(db.POes);

            return Orders;
        }

        // POST api/newordersbypo
        [AuthorizeUser("Admin")]
        public void Post(NGNewOrdersByPOModel order)
        {
            AuleaseEntities db = new AuleaseEntities();

            PO SelectedSR = db.POes.Where(n => n.Id == order.id).Single();

            SelectedSR.PONumber = order.SR;
            
            EvaluateConfiguration(db, order.Configuration, order.SystemGroups.Select(n => n.id));

            foreach (var groupModel in order.SystemGroups)
            {
                UpdateComponent(db, groupModel);
            }

            db.SaveChanges();
        }

        private void UpdateComponent(AuleaseEntities db, NGOrderSystemGroupByPOModel systemGroupModel)
        {
            SystemGroup group = db.SystemGroups.Where(n => n.Id == systemGroupModel.id).Single();
            group.User = FindOrCreateUser(db, systemGroupModel.GID);
            group.User.Phone = systemGroupModel.Phone;
            group.User.Location.Building = systemGroupModel.Building;
            group.User.Location.Room = systemGroupModel.Room;

            foreach (var lease in group.Leases)
            {
                lease.StatementName = systemGroupModel.StatementName;
                lease.Department = FindOrCreateDepartment(db, systemGroupModel.FOP);
                lease.Component.OrderNumber = systemGroupModel.OrderNumber;
                lease.Component.InstallHardware = systemGroupModel.InstallHardware;
                lease.Component.InstallSoftware = systemGroupModel.InstallSoftware;
                lease.Component.Renewal = systemGroupModel.Renewal;

                if (systemGroupModel.Term.HasValue)
                {
                    lease.Overhead = FindOverhead(db, systemGroupModel.RateLevel, systemGroupModel.Term.Value);
                }

                if (!String.IsNullOrWhiteSpace(systemGroupModel.OperatingSystem) && (lease.Component.Type.Name == "CPU" || lease.Component.Type.Name == "Laptop"))
                {
                    Property OldProperty = lease.Component.Properties.Where(n => n.Key == "Operating System").SingleOrDefault();

                    if (OldProperty != null)
                    {
                        lease.Component.Properties.Remove(OldProperty);
                    }

                    string os = systemGroupModel.OperatingSystem.Trim();
                    Property NewOS = db.Properties.Where(n => n.Key == "Operating System" && n.Value == os).Single();

                    lease.Component.Properties.Add(NewOS);
                }
            }

            for (int i = 0; i < group.Leases.Select(n => n.Component).Count(); i++)
            {
                if (systemGroupModel.Components.First().SerialNumber == null)
                {
                    continue;
                }

                Component comp = group.Leases.Select(n => n.Component).OrderBy(n => n.TypeId).Skip(i).Take(1).Single();
                if (systemGroupModel.Components.Count() > i)
                {
                    comp.SerialNumber = systemGroupModel.Components.Skip(i).Take(1).Single().SerialNumber;
                    comp.LeaseTag = systemGroupModel.Components.Skip(i).Take(1).Single().LeaseTag;
                }
            }

            foreach (var eolComp in systemGroupModel.EOLComponents)
            {
                if (!group.EOLComponents.Any(n => n.SerialNumber == eolComp.SerialNumber || n.LeaseTag == eolComp.LeaseTag))
                {
                    Component NewEOLComponent = db.Components.Where(n => n.SerialNumber == eolComp.SerialNumber || n.LeaseTag == eolComp.LeaseTag).Single();
                    group.EOLComponents.Add(NewEOLComponent);
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

        private void EvaluateConfiguration(AuleaseEntities db, IEnumerable<NGConfigurationModel> config, IEnumerable<int> systemGroupIDs)
        {
            foreach (int i in systemGroupIDs)
            {
                SystemGroup group = db.SystemGroups.First(n => n.Id == i);

                if (group.Leases.Select(n => n.Component).Count() < config.Count())
                {
                    // Add Components
                    while (group.Leases.Select(n => n.Component).Count() < config.Count())
                    {
                        Component comp = group.Leases.Select(n => n.Component).OrderBy(n => n.TypeId.HasValue).ThenBy(n => n.TypeId).ThenBy(n => n.ModelId.HasValue).ThenBy(n => n.ModelId).Last();
                        Lease lease = comp.Leases.Single();

                        Lease NewLease = lease.Clone();
                        Component NewComponent = comp.Clone();

                        NewLease.Component = NewComponent;
                        NewComponent.Leases.Add(NewLease);

                        group.Leases.Add(NewLease);
                    }
                }
                else if (group.Leases.Select(n => n.Component.Type).Count() > config.Count())
                {
                    while (group.Leases.Select(n => n.Component).Count() > config.Count())
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
                    comp.Type = db.Types.Where(n => n.Name == typeName).Single();
                    comp.Make = FindOrCreateMake(db, config.Skip(j).Take(1).Single().Make);
                    comp.Model = FindOrCreateModel(db, comp.Make, config.Skip(j).Take(1).Single().Model);
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
                db.Users.Add(UserObject);
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

        // PUT api/newordersbypo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/newordersbypo/?action={SR, SystemGroup}&id={5}
        public void Delete(string action, int id)
        {
            AuleaseEntities db = new AuleaseEntities();

            if (action == "SR")
            {
                PO SinglePO = db.POes.Where(n => n.Id == id).Single();
                List<Order> orders = SinglePO.SystemGroups.Select(n => n.Order).ToList();

                foreach (var systemGroup in SinglePO.SystemGroups)
                {
                    foreach (var comp in systemGroup.Leases.Select(n => n.Component).Distinct())
                    {
                        List<Lease> leases = comp.Leases.ToList();
                        List<Charge> Charges = leases.SelectMany(n => n.Charges).ToList();
                        int OrderID = leases.Select(n => n.SystemGroup).SingleOrDefault().OrderId;
                        int? POID = leases.Select(n => n.SystemGroup).SingleOrDefault().POId.HasValue ? (int?)leases.Select(n => n.SystemGroup).Single().POId.Value : null;

                        foreach (var charge in Charges)
                        {
                            db.Entry(charge).State = EntityState.Deleted;
                        }

                        foreach (var lease in leases)
                        {
                            db.Entry(lease).State = EntityState.Deleted;
                        }

                        List<Property> props = comp.Properties.ToList();
                        foreach (var prop in props)
                        {
                            comp.Properties.Remove(prop);
                        }

                        db.Entry(comp).State = EntityState.Deleted;
                    }

                    if (systemGroup.Leases.Count == 0)
                    {
                        db.Entry(systemGroup).State = EntityState.Deleted;
                    }
                }

                if (SinglePO.SystemGroups.Count == 0)
                {
                    db.Entry(SinglePO).State = EntityState.Deleted;
                }

                foreach (var order in orders)
                {
                    if (order.SystemGroups.Count == 0)
                    {
                        db.Entry(order).State = EntityState.Deleted;
                    }
                }

                db.SaveChanges();
            }
            else if (action == "SystemGroup")
            {
                SystemGroup group = db.SystemGroups.Where(n => n.Id == id).Single();

                foreach (var comp in group.Leases.Select(n => n.Component).Distinct())
                {
                    List<Lease> leases = comp.Leases.ToList();
                    List<Charge> Charges = leases.SelectMany(n => n.Charges).ToList();
                    int OrderID = leases.Select(n => n.SystemGroup).SingleOrDefault().OrderId;
                    int? POID = leases.Select(n => n.SystemGroup).SingleOrDefault().POId.HasValue ? (int?)leases.Select(n => n.SystemGroup).Single().POId.Value : null;

                    foreach (var charge in Charges)
                    {
                        db.Entry(charge).State = EntityState.Deleted;
                    }

                    foreach (var lease in leases)
                    {
                        db.Entry(lease).State = EntityState.Deleted;
                    }

                    List<Property> props = comp.Properties.ToList();
                    foreach (var prop in props)
                    {
                        comp.Properties.Remove(prop);
                    }

                    db.Entry(comp).State = EntityState.Deleted;
                }

                if (group.Leases.Count == 0)
                {
                    db.Entry(group).State = EntityState.Deleted;
                }

                db.SaveChanges();
            }
        }
    }
}
