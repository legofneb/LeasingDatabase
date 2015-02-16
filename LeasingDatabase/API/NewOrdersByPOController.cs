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
                lease.Department = FindOrCreateDepartment(db, systemGroupModel.FOP, systemGroupModel.DepartmentName);
                lease.Component.OrderNumber = systemGroupModel.OrderNumber;
                lease.Component.InstallHardware = systemGroupModel.InstallHardware;
                lease.Component.InstallSoftware = systemGroupModel.InstallSoftware;
                lease.Component.Renewal = systemGroupModel.Renewal;

                if (systemGroupModel.Term.HasValue)
                {
                    lease.Overhead = FindOverhead(db, systemGroupModel.RateLevel, systemGroupModel.Term.Value);
                }
            }

            for (int i = 0; i < group.Leases.Select(n => n.Component).Count(); i++)
            {
                Component comp = group.Leases.Select(n => n.Component).Skip(i).Take(1).Single();
                comp.SerialNumber = systemGroupModel.Components.Skip(i).Take(1).Single().SerialNumber;
                comp.LeaseTag = systemGroupModel.Components.Skip(i).Take(1).Single().LeaseTag;
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
            if (db.Models.Any(n => n.Make.Id == make.Id && n.Name == model))
            {
                return db.Models.Where(n => n.Make.Id == make.Id && n.Name == model).Single();
            }
            else
            {
                return new Model() { Make = make, Name = model };
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
                return new Make() { Name = make };
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

        // PUT api/newordersbypo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/newordersbypo/5
        public void Delete(int id)
        {
        }
    }
}
