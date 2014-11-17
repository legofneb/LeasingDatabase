using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeasingDatabase.Models;
using aulease.Entities;
using Trirand.Web.Mvc;
using System.Data.Entity.Validation;
using System.Diagnostics;
using CWSToolkit;
using System.Data.Entity;

namespace LeasingDatabase.Controllers
{
    [AuthorizeUser("Admin")]
    public class OrdersController : Controller
    {
        //
        // GET: /Orders/

        public ActionResult Index()
        {
            var gridModel = new LeasingDatabase.Models.Grid.OrdersJqGridModel();
            var ordersGrid = gridModel.OrdersGrid;

            SetUpGrid(ordersGrid);

            return View(gridModel);
        }

        public JsonResult DataRequested()
        {
            var gridModel = new LeasingDatabase.Models.Grid.OrdersJqGridModel();
            var db = new AuleaseEntities();

            SetUpGrid(gridModel.OrdersGrid);

            var comps = db.SystemGroups.Where(n => n.Leases.Count > 0 && n.Leases.FirstOrDefault().MonthlyCharge == null).OrderByDescending(n => n.Order.Date);

            var model = CreateOrderModel(comps);

            JQGridState gridState = gridModel.OrdersGrid.GetState();
            Session["gridState"] = gridState;

            return gridModel.OrdersGrid.DataBind(model.ToList().AsQueryable());
        }

        private static IQueryable<OrderModel> CreateOrderModel(IQueryable<SystemGroup> comps)
        {
            var model = comps.AsQueryable().Select(x => new { PrimaryComp = x.Leases.OrderBy(p => p.Component.TypeId).ThenBy(p => p.Id).FirstOrDefault(), lease = x.Leases.FirstOrDefault(), order = x }).Select(n => new OrderModel
            {
                SystemID = n.order.Id,
                OrderNumber = n.lease.Component.OrderNumber,
                OrderDate = n.order.Order.Date,
                SR = (n.order.POId != null) ? n.order.PO.PONumber : null,
                StatementName = n.lease.StatementName,
                InstallHardware = n.PrimaryComp.Component.InstallHardware,
                InstallSoftware = n.PrimaryComp.Component.InstallSoftware,
                Note = n.PrimaryComp.Component.Note,
                Renewal = n.PrimaryComp.Component.Renewal,
                Term = n.lease.Overhead.Term,
                RateLevel = n.lease.Overhead.RateLevel,
                DepartmentName = n.lease.Department.Name,
                Fund = n.lease.Department.Fund,
                Org = n.lease.Department.Org,
                Program = n.lease.Department.Program,
                GID = n.order.User.GID,
                Phone = n.order.User.Phone,
                Room = n.order.Location.Room,
                Building = n.order.Location.Building,
                OrdererGID = n.order.Order.User.GID,
                OperatingSystem = n.PrimaryComp.Component.Properties.Where(o => o.Key == "Operating System").Select(o => o.Value).FirstOrDefault(),
                Architecture = n.PrimaryComp.Component.Properties.Where(o => o.Key == "Architecture").Select(o => o.Value).FirstOrDefault(),
                Status = n.PrimaryComp.Component.Status.Name,
                SerialNumber = n.PrimaryComp.Component.SerialNumber,
                LeaseTag = n.PrimaryComp.Component.LeaseTag,
                Make = n.PrimaryComp.Component.Make.Name,
                ComponentType = n.PrimaryComp.Component.Type.Name,
                Model = n.PrimaryComp.Component.Model.Name,
                EOLComponent = n.order.EOLComponents.OrderBy(o => o.TypeId).OrderBy(o => o.Id).Skip(0).Take(1).FirstOrDefault().SerialNumber,
                
                OrderNotes = n.order.Order.Note,

                SerialNumber2 = n.order.Leases.Count > 1 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().Component.SerialNumber : null,
                LeaseTag2 = n.order.Leases.Count > 1 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().Component.LeaseTag : null,
                Make2 = n.order.Leases.Count > 1 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().Component.Make.Name : null,
                ComponentType2 = n.order.Leases.Count > 1 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().Component.Type.Name : null,
                Model2 = n.order.Leases.Count > 1 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().Component.Model.Name : null,
                EOLComponent2 = n.order.EOLComponents.OrderBy(o => o.TypeId).OrderBy(o => o.Id).Skip(1).Take(1).FirstOrDefault().SerialNumber,

                SerialNumber3 = n.order.Leases.Count > 2 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().Component.SerialNumber : null,
                LeaseTag3 = n.order.Leases.Count > 2 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().Component.LeaseTag : null,
                Make3 = n.order.Leases.Count > 2 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().Component.Make.Name : null,
                ComponentType3 = n.order.Leases.Count > 2 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().Component.Type.Name : null,
                Model3 = n.order.Leases.Count > 2 ? n.order.Leases.OrderBy(o => o.Component.TypeId).ThenBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().Component.Model.Name : null,
                EOLComponent3 = n.order.EOLComponents.OrderBy(o => o.TypeId).OrderBy(o => o.Id).Skip(2).Take(1).FirstOrDefault().SerialNumber

            });
            return model;
        }

        public ActionResult EditRows()
        {
            var gridModel = new LeasingDatabase.Models.Grid.OrdersJqGridModel();
            var db = new AuleaseEntities();

            var e = gridModel.OrdersGrid.GetEditData(); // Edit Row

            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
            {
                string validationMessage = ValidationEditRow(e);
                if (!String.IsNullOrEmpty(validationMessage))
                { return gridModel.OrdersGrid.ShowEditValidationMessage(validationMessage); }


                // All variables that contain RowData are prefixed by _

                int _SystemID = Convert.ToInt32(e.RowData["SystemID"]);

                string _GID = e.RowData["GID"].ToString().Trim();
                string _Building = e.RowData["Building"].ToString().Trim();
                string _Room = e.RowData["Room"].ToString().Trim();
                string _Phone = e.RowData["Phone"].ToString().Trim();

                string _OrdererGID = e.RowData["OrdererGID"].ToString().Trim();

                string _OrderNumber = e.RowData["OrderNumber"].ToString().Trim();
                string _SR = e.RowData["SR"].ToString().Trim();
                string _StatementName = e.RowData["StatementName"].ToString().Trim();

                string _Note = e.RowData["Note"].ToString().Trim();

                string _Fund = e.RowData["Fund"].ToString().Trim();
                string _Org = e.RowData["Org"].ToString().Trim();
                string _Program = e.RowData["Program"].ToString().Trim();

                string _RateLevel = e.RowData["RateLevel"].ToString().Trim();
                int _Term = Convert.ToInt32(e.RowData["Term"]);
                bool _InstallHardware = Convert.ToBoolean(e.RowData["InstallHardware"]);
                bool _InstallSoftware = Convert.ToBoolean(e.RowData["InstallSoftware"]);
                bool _Renewal = Convert.ToBoolean(e.RowData["Renewal"]);

                string _OperatingSystem = e.RowData["OperatingSystem"].ToString().Trim();
                string _Architecture = e.RowData["Architecture"].ToString().Trim();

                string[] _ComponentTypes = new string[] { e.RowData["ComponentType"].ToString().Trim(), e.RowData["ComponentType2"].ToString().Trim(), e.RowData["ComponentType3"].ToString().Trim() };
                string[] _Makes = new string[] { e.RowData["Make"].ToString().Trim(), e.RowData["Make2"].ToString().Trim(), e.RowData["Make3"].ToString().Trim() };
                string[] _Models = new string[] { e.RowData["Model"].ToString().Trim(), e.RowData["Model2"].ToString().Trim(), e.RowData["Model3"].ToString().Trim() };

                string[] _SerialNumbers = new string[] {	!String.IsNullOrWhiteSpace(e.RowData["SerialNumber"])? e.RowData["SerialNumber"].ToString().Trim():null, 
															!String.IsNullOrWhiteSpace(e.RowData["SerialNumber2"])?e.RowData["SerialNumber2"].ToString().Trim():null, 
															!String.IsNullOrWhiteSpace(e.RowData["SerialNumber3"])?e.RowData["SerialNumber3"].ToString().Trim():null };

                string[] _LeaseTags = new string[] { e.RowData["LeaseTag"].ToString().Trim(), e.RowData["LeaseTag2"].ToString().Trim(), e.RowData["LeaseTag3"].ToString().Trim() };

                List<string> _EOLComponents = new List<string>();
                _EOLComponents.Add(e.RowData["EOLComponent"].ToString().Trim());
                _EOLComponents.Add(e.RowData["EOLComponent2"].ToString().Trim());
                _EOLComponents.Add(e.RowData["EOLComponent3"].ToString().Trim());

                int currentCount = db.SystemGroups.Where(n => n.Id == _SystemID).First().Leases.Count;

                int targetCount = 0;
                if (!String.IsNullOrWhiteSpace(_ComponentTypes[0])) { targetCount++; }
                if (!String.IsNullOrWhiteSpace(_ComponentTypes[1])) { targetCount++; }
                if (!String.IsNullOrWhiteSpace(_ComponentTypes[2])) { targetCount++; }

                if (targetCount != currentCount && targetCount > 0) // Is true when we need to add or remove components
                {
                    if (currentCount > targetCount)
                    {
                        SystemGroup tempSystemGroup = db.SystemGroups.Where(n => n.Id == _SystemID).Single();
                        List<Component> tempcomps = tempSystemGroup.Leases.Select(n => n.Component).Where(n => n.Type != null && n.Type.Name == "Monitor").OrderByDescending(n => n.Id).ToList();

                        if (currentCount - targetCount == 2)
                        {
                            foreach (var tempcomp in tempcomps)
                            {
                                List<Property> properts = tempcomp.Properties.ToList();
                                foreach (var propert in properts)
                                {
                                    tempcomp.Properties.Remove(propert);
                                }

                                // Delete 2
                                Lease templease = tempcomp.Leases.First();
                                db.Entry(templease).State = EntityState.Deleted;
                                db.Entry(tempcomp).State = EntityState.Deleted;
                                
                            }
                        }
                        else
                        {
                            // Delete 1
                            Component tempcomp = tempcomps[0];

                            List<Property> properts = tempcomp.Properties.ToList();
                            foreach (var propert in properts)
                            {
                                tempcomp.Properties.Remove(propert);
                            }
                            Lease templease = tempcomp.Leases.First();
                            db.Entry(templease).State = EntityState.Deleted;
                            db.Entry(tempcomp).State = EntityState.Deleted;
                        }
                    }
                    else
                    {
                        Lease currentLease = db.Leases.Where(n => n.SystemGroupId == _SystemID).First();
                        Component currentComp = currentLease.Component;
                        while (currentCount < targetCount)
                        {
                            Lease lease = new Lease();
                            lease.BeginDate = currentLease.BeginDate;
                            lease.EndDate = currentLease.EndDate;
                            lease.StatementName = currentLease.StatementName;
                            lease.Timestamp = currentLease.Timestamp;
                            lease.ContractNumber = currentLease.ContractNumber;
                            lease.Department = currentLease.Department;
                            lease.MonthlyCharge = currentLease.MonthlyCharge;
                            lease.Overhead = currentLease.Overhead;
                            lease.SystemGroupId = _SystemID;

                            Component comp = new Component();
                            comp.SerialNumber = _SerialNumbers[currentCount];
                            comp.LeaseTag = _LeaseTags[currentCount];
                            comp.OrderNumber = _OrderNumber;
                            comp.InstallSoftware = _InstallSoftware;
                            comp.InstallHardware = _InstallHardware;
                            comp.Note = _Note;
                            comp.Renewal = _Renewal;

                            string _ComponentType = _ComponentTypes[currentCount];
                            string _Make = _Makes[currentCount];
                            string _Model = _Models[currentCount];
                            var type = db.Types.Where(n => n.Name == _ComponentType).Single();
                            Make make = db.Makes.Where(n => n.Name == _Make).Single();
                            Model model;
                            if (db.Models.Any(n => n.Name == _Model))
                            {
                                model = db.Models.Where(n => n.Name == _Model).First();
                            }
                            else
                            {
                                model = new Model { Name = _Model, Make = make };
                            }

                            comp.Make = make;
                            comp.Type = type;
                            comp.StatusId = 1;
                            comp.Model = model;

                            comp.Leases.Add(lease);
                            db.Components.Add(comp);
                            currentCount++;
                        }
                        db.SaveChanges();
                    }
                }

                PO SR;

                if (db.POes.Any(n => n.PONumber == _SR))
                {
                    SR = db.POes.Where(n => n.PONumber == _SR).First();
                }
                else
                {
                    SR = new PO { PONumber = _SR };
                }

                User user;

                if (db.Users.Any(n => n.GID == _GID))
                {
                    user = db.Users.Where(n => n.GID == _GID).Single();
                }
                else
                {
                    Location loc = new Location() { Building = _Building, Room = _Room };
                    user = new User { GID = _GID, Phone = _Phone, Location = loc };
                }

                SystemGroup system = db.SystemGroups.Where(n => n.Id == _SystemID).Single();

                foreach (var comp in system.EOLComponents.ToList())
                {
                    system.EOLComponents.Remove(comp);
                }

                foreach (var comp in _EOLComponents)
                {
                    if (!String.IsNullOrWhiteSpace(comp))
                    {
                        system.EOLComponents.Add(db.Components.Where(n => n.SerialNumber == comp).Single());
                    }
                }

                system.PO = SR;
                system.Location.Building = _Building;
                system.Location.Room = _Room;
                system.User = user;

                Department department;
                if (db.Departments.Any(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program))
                {
                    department = db.Departments.Where(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program).Single();
                }
                else
                {
                    department = new Department { Fund = _Fund, Org = _Org, Program = _Program };
                }

                List<Lease> leases = system.Leases.ToList();
                foreach (var lease in leases)
                {
                    if (lease.Component.Type != null)
                    {
                        lease.StatementName = _StatementName;
                        lease.Department = department;
                        lease.Overhead = db.Overheads.Where(n => n.RateLevel == _RateLevel && n.Term == _Term && n.Type.Name == lease.Component.Type.Name).OrderByDescending(n => n.BeginDate).First();
                    }
                    else if (leases.Count == 1)
                    {
                        lease.Overhead = db.Overheads.Where(n => n.RateLevel == _RateLevel && n.Term == _Term).OrderByDescending(n => n.BeginDate).First();
                    }
                }

                List<Component> comps = new List<Component>();
                foreach (var lease in leases)
                {
                    if (!comps.Contains(lease.Component))
                    {
                        if (lease.Component.Type == null || lease.Component.Type.Name != "Monitor")
                        {
                            comps.Insert(0, lease.Component);
                        }
                        else
                        {
                            comps.Add(lease.Component);
                        }
                    }
                }



                int i = 0;

                foreach (var comp in comps)
                {
                    if (targetCount != 0)
                    {
                        string _ComponentType = _ComponentTypes[i];
                        string _Make = _Makes[i];
                        string _Model = _Models[i];
                        var type = db.Types.Where(n => n.Name == _ComponentType).Single();
                        Make make = db.Makes.Where(n => n.Name == _Make).Single();
                        Model model;
                        if (db.Models.Any(n => n.Name == _Model))
                        {
                            model = db.Models.Where(n => n.Name == _Model).First();
                        }
                        else
                        {
                            model = new Model { Name = _Model, Make = make };
                        }

                        if (_ComponentType != "Monitor")
                        {
                            if (comp.Properties.Any(n => n.Key == "Operating System"))
                            {
                                comp.Properties.Remove(comp.Properties.Where(n => n.Key == "Operating System").Single());
                            }

                            if (comp.Properties.Any(n => n.Key == "Architecture"))
                            {
                                comp.Properties.Remove(comp.Properties.Where(n => n.Key == "Architecture").Single());
                            }


                            if (!String.IsNullOrWhiteSpace(_OperatingSystem))
                            {
                                Property os = db.Properties.Where(n => n.Key == "Operating System" && n.Value == _OperatingSystem).Single();
                                comp.Properties.Add(os);
                            }

                            if (!String.IsNullOrWhiteSpace(_Architecture))
                            {
                                Property architecture = db.Properties.Where(n => n.Key == "Architecture" && n.Value == _Architecture).Single();
                                comp.Properties.Add(architecture);
                            }


                        }

                        comp.Make = make;
                        comp.Model = model;
                        comp.Type = type;
                    }


                    comp.SerialNumber = _SerialNumbers[i];
                    comp.LeaseTag = _LeaseTags[i];
                    comp.OrderNumber = _OrderNumber;
                    comp.InstallHardware = _InstallHardware;
                    comp.InstallSoftware = _InstallSoftware;
                    comp.Note = _Note;
                    comp.Renewal = _Renewal;

                    i++;
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Debug.WriteLine("Property: {0} Error: {1}",
                                       validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }

            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
            {
                // All variables that contain RowData are prefixed by _

                string _GID = e.RowData["GID"].ToString();
                string _Building = e.RowData["Building"].ToString();
                string _Room = e.RowData["Room"].ToString();
                string _Phone = e.RowData["Phone"].ToString();
                string _OrdererGID = e.RowData["OrdererGID"].ToString();
                string _StatementName = e.RowData["StatementName"].ToString();

                string _Fund = e.RowData["Fund"].ToString();
                string _Org = e.RowData["Org"].ToString();
                string _Program = e.RowData["Program"].ToString();

                string _RateLevel = e.RowData["RateLevel"].ToString();
                int _Term = Convert.ToInt32(e.RowData["Term"]);
                bool _InstallHardware = Convert.ToBoolean(e.RowData["InstallHardware"]);
                bool _InstallSoftware = Convert.ToBoolean(e.RowData["InstallSoftware"]);
                bool _Renewal = Convert.ToBoolean(e.RowData["Renewal"]);

                string _OperatingSystem = e.RowData["OperatingSystem"].ToString();

                string _ComponentType = e.RowData["ComponentType"].ToString();
                string _Make = e.RowData["Make"].ToString();
                string _Model = e.RowData["Model"].ToString();

                string _ComponentType2 = e.RowData["ComponentType2"].ToString();
                string _Make2 = e.RowData["Make2"].ToString();
                string _Model2 = e.RowData["Model2"].ToString();

                string _ComponentType3 = e.RowData["ComponentType3"].ToString();
                string _Make3 = e.RowData["Make3"].ToString();
                string _Model3 = e.RowData["Model3"].ToString();


                User orderer;
                if (db.Users.Any(n => n.GID == _OrdererGID))
                {
                    orderer = db.Users.Where(n => n.GID == _OrdererGID).FirstOrDefault();
                }
                else
                {
                    orderer = new User { GID = _OrdererGID, Phone = null, Location = new Location() };
                }

                User user;
                if (db.Users.Any(n => n.GID == _GID))
                {
                    user = db.Users.Where(n => n.GID == _GID).FirstOrDefault();
                }
                else
                {
                    user = new User { GID = _GID, Phone = _Phone, Location = new Location { Building = _Building, Room = _Room } };
                }

                if (_OrdererGID == _GID)
                {
                    orderer = user;
                }

                Order order = new Order { Date = DateTime.Now, User = orderer };


                SystemGroup systemGroup = new SystemGroup();
                systemGroup.Location.Building = _Building;
                systemGroup.Location.Room = _Room;
                systemGroup.User = user;

                Make make1;
                if (db.Makes.Any(n => n.Name == _Make))
                {
                    make1 = db.Makes.Where(n => n.Name == _Make).FirstOrDefault();
                }
                else
                {
                    make1 = new Make() { Name = _Make };
                }

                Model model1;
                if (db.Models.Any(n => n.Name == _Model))
                {
                    model1 = db.Models.Where(n => n.Name == _Model).FirstOrDefault();
                }
                else
                {
                    model1 = new Model() { Name = _Model, Make = make1 };
                }

                Component component1 = new Component();
                component1.InstallHardware = _InstallHardware;
                component1.InstallSoftware = _InstallSoftware;
                component1.Renewal = _Renewal;
                component1.Make = make1;
                component1.Type = db.Types.Where(n => n.Name == _ComponentType).FirstOrDefault();
                component1.StatusId = 1;
                component1.Model = model1;

                if (_ComponentType != "Monitor" && !String.IsNullOrWhiteSpace(_OperatingSystem))
                {
                    Property os = db.Properties.Where(n => n.Value == _OperatingSystem).First();
                    component1.Properties.Add(os);
                }

                Department department;
                if (db.Departments.Any(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program))
                {
                    department = db.Departments.Where(n => n.Fund == _Fund && n.Org == _Org && n.Program == _Program).FirstOrDefault();
                }
                else
                {
                    department = new Department { Fund = _Fund, Org = _Org, Program = _Program };
                }

                Overhead Overhead = db.Overheads.Where(n => n.RateLevel == _RateLevel && n.Term == _Term).OrderByDescending(n => n.BeginDate).FirstOrDefault();

                Lease lease1 = new Lease();
                lease1.StatementName = _StatementName;
                lease1.Timestamp = DateTime.Now;
                lease1.Department = department;
                lease1.Overhead = Overhead;
                lease1.Component = component1;

                systemGroup.Leases.Add(lease1);

                if (!String.IsNullOrWhiteSpace(_Make2))
                {
                    Make make2;
                    if (db.Makes.Any(n => n.Name == _Make2))
                    {
                        make2 = db.Makes.Where(n => n.Name == _Make2).FirstOrDefault();
                    }
                    else
                    {
                        make2 = new Make() { Name = _Make2 };
                    }

                    Model model2;
                    if (db.Models.Any(n => n.Name == _Model2))
                    {
                        model2 = db.Models.Where(n => n.Name == _Model2).FirstOrDefault();
                    }
                    else
                    {
                        model2 = new Model() { Name = _Model2, Make = make2 };
                    }

                    Component component2 = new Component();
                    component2.InstallHardware = _InstallHardware;
                    component2.InstallSoftware = _InstallSoftware;
                    component2.Renewal = _Renewal;
                    component2.Make = make2;
                    component2.Type = db.Types.Where(n => n.Name == _ComponentType2).FirstOrDefault();
                    component2.StatusId = 1;
                    component2.Model = model2;

                    Lease lease2 = new Lease();
                    lease2.StatementName = _StatementName;
                    lease2.Timestamp = DateTime.Now;
                    lease2.Department = department;
                    lease2.Overhead = Overhead;
                    lease2.Component = component2;

                    systemGroup.Leases.Add(lease2);
                }

                if (!String.IsNullOrWhiteSpace(_Make3))
                {
                    Make make3;
                    if (db.Makes.Any(n => n.Name == _Make3))
                    {
                        make3 = db.Makes.Where(n => n.Name == _Make3).FirstOrDefault();
                    }
                    else
                    {
                        make3 = new Make() { Name = _Make3 };
                    }

                    Model model3;
                    if (db.Models.Any(n => n.Name == _Model3))
                    {
                        model3 = db.Models.Where(n => n.Name == _Model3).FirstOrDefault();
                    }
                    else
                    {
                        model3 = new Model() { Name = _Model3, Make = make3 };
                    }

                    Component component3 = new Component();
                    component3.InstallHardware = _InstallHardware;
                    component3.InstallSoftware = _InstallSoftware;
                    component3.Renewal = _Renewal;
                    component3.Make = make3;
                    component3.Type = db.Types.Where(n => n.Name == _ComponentType3).FirstOrDefault();
                    component3.StatusId = 1;
                    component3.Model = model3;

                    Lease lease3 = new Lease();
                    lease3.StatementName = _StatementName;
                    lease3.Timestamp = DateTime.Now;
                    lease3.Department = department;
                    lease3.Overhead = Overhead;
                    lease3.Component = component3;

                    systemGroup.Leases.Add(lease3);
                }

                order.SystemGroups.Add(systemGroup);
                db.Orders.Add(order);
                db.SaveChanges();

            }
            if (gridModel.OrdersGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
            {
                int SystemID = Convert.ToInt32(e.RowData["SystemID"]);
                SystemGroup SystemGroup = db.SystemGroups.Where(n => n.Id == SystemID).First();
                int OrderID = SystemGroup.OrderId;
                int? POID = SystemGroup.POId.HasValue ? (int?)SystemGroup.POId.Value : null;
                List<Component> comps = SystemGroup.Leases.Select(n => n.Component).ToList();
                List<Lease> leases = SystemGroup.Leases.ToList();
                List<Charge> Charges = SystemGroup.Leases.SelectMany(n => n.Charges).ToList();
                List<Component> expiredComponents = SystemGroup.EOLComponents.ToList();


                foreach (var comp in expiredComponents)
                {
                    SystemGroup.EOLComponents.Remove(comp);
                }

                db.Entry(SystemGroup).State = EntityState.Deleted;

                foreach (var charge in Charges)
                {
                    db.Entry(charge).State = EntityState.Deleted;
                }

                foreach (var lease in leases)
                {
                    db.Entry(lease).State = EntityState.Deleted;
                }

                foreach (var comp in comps)
                {
                    List<Property> props = comp.Properties.ToList();
                    foreach (var prop in props)
                    {
                        comp.Properties.Remove(prop);
                    }
                    db.Entry(comp).State = EntityState.Deleted;
                }


                db.SaveChanges();

                if (db.Orders.Where(n => n.Id == OrderID).Single().SystemGroups.Count == 0)
                {
                    db.Entry(db.Orders.Where(n => n.Id == OrderID).Single()).State = EntityState.Deleted;
                }

                if (POID != null && db.POes.Where(n => n.Id == POID).Single().SystemGroups.Count == 0)
                {
                    db.Entry(db.POes.Where(n => n.Id == POID).Single()).State = EntityState.Deleted;
                }

                db.SaveChanges();
            }

            return new RedirectResult(Url.Action("DataRequested", "Orders"));
        }

        public JsonResult Finalize(string SystemGroupID, string BegBillDate, string EndBillDate,
                                            string CPUCost, string MonitorCost, string MonitorCost2,
                                            string Insurance, string Warranty, string Shipping, string BillNotes)
        {
            AuleaseEntities db = new AuleaseEntities();
            int SystemID = Convert.ToInt32(SystemGroupID);
            PO po = db.SystemGroups.Where(n => n.Id == SystemID).Single().PO;
            List<SystemGroup> groups = po.SystemGroups.ToList();

            foreach (var group in groups)
            {
                Core.Billing.FillLeasingInfo(group.Id, BegBillDate, EndBillDate, CPUCost, MonitorCost, MonitorCost2, Insurance, Warranty, Shipping);
            }

            var output = new
            {
                summary = Core.Billing.CreateStringSummary(SystemID)
            };

            return Json(output);
        }

        public JsonResult FinalizeConfirmed(string SystemGroupID, string BegBillDate, string EndBillDate,
                                            string CPUCost, string MonitorCost, string MonitorCost2,
                                            string Insurance, string Warranty, string Shipping, string BillNotes, bool SuppressEmail)
        {
            AuleaseEntities db = new AuleaseEntities();
            int SystemID = Convert.ToInt32(SystemGroupID);
            PO po = db.SystemGroups.Where(n => n.Id == SystemID).Single().PO;
            List<SystemGroup> groups = po.SystemGroups.ToList();

            string status = "Success!";

            foreach (var group in groups)
            {
                try
                {
                    DateTime BegDate = Convert.ToDateTime(BegBillDate);
                    DateTime EndDate = Convert.ToDateTime(EndBillDate);
                    foreach (var lease in db.Leases.Where(n => n.SystemGroupId == group.Id))
                    {
                        Decimal MonthlyCharge = Core.Billing.CalculateMonthlyCharge(lease.Id);
                        lease.MonthlyCharge = MonthlyCharge;
                        lease.BeginDate = BegDate;
                        lease.EndDate = EndDate;
                        lease.Component.ReturnDate = EndDate.AddMonths(1);
                    }
                }
                catch
                {
                    status = "Input is not formatted correctly";
                }
            }

            db.SaveChanges();

            var output = new
            {
                status = status
            };

            if (SuppressEmail == true) { return Json(output); }

            Email email = new Email();
            foreach (var mail in db.VendorEmails)
            {
                email.AddTo(mail.EmailAddress);
            }

            email.From("aulease@auburn.edu");
            email.HTML = true;
            string message = "<p>" + BillNotes + "</p><br /><br /><table border\"1\" bgcolor=\"#ffffff\" cellspacing=\"5\"><caption><b>ECOA</b></caption><thead><tr>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">PO#</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Serial Number</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Component Type</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Order Number</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">igfTerm</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Manufacturer</font></th>"
                            + "<th bgcolor=\"#c0c0c0\" bordercolor=\"#000000\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">Component Cost</font></th>"
                            + "</tr></thead><tbody>";

            foreach (var group in groups)
            {
                foreach (var comp in group.Leases.Select(n => n.Component))
                {
                    decimal ComponentCost;

                    try
                    {
                        ComponentCost = comp.Leases.OrderBy(n => n.EndDate).FirstOrDefault().Charges.Where(n => n.Type.Id == comp.Type.Id).Single().Price;
                    }
                    catch
                    {
                        ComponentCost = 0.00M;
                    }

                    message += "<tr valign=\"TOP\">"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.Leases.First().SystemGroup.PO.PONumber + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.SerialNumber + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.Type.Name + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.OrderNumber + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + (comp.Leases.First().Overhead.Term + 1).ToString() + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + comp.Make.Name + "</font></td>"
                                    + "<td bordercolor=\"#d0d7e5\"><font style=\"FONT-SIZE:11pt\" face=\"Calibri\" color=\"#000000\">" + ComponentCost + "</font></td>"
                                    + "</tr>";
                }
            }

            message += "</tbody><tfoot></tfoot></table>";

            email.Subject = "AUBURN UNIVERSITY 0639915 COA " + db.SystemGroups.Where(n => n.Id == SystemID).Single().PO.PONumber;
            email.Body = message;
            email.AddCC("aulease@auburn.edu");

            email.Send();

            return Json(output);
        }

        public void SetUpGrid(Trirand.Web.Mvc.JQGrid ordersGrid)
        {
            // Customize/change some of the default settings for this model
            // ID is a mandatory field. Must by unique if you have several grids on one page.
            ordersGrid.ID = "OrdersGrid";
            // Setting the DataUrl to an action (method) in the controller is required.
            // This action will return the data needed by the grid
            ordersGrid.DataUrl = Url.Action("DataRequested");
            ordersGrid.EditUrl = Url.Action("EditRows");
        }


        private string ValidationEditRow(JQGridEditData e)
        {
            string errors = "";

            string _GID = e.RowData["GID"].ToString();
            string _Building = e.RowData["Building"].ToString();
            string _Room = e.RowData["Room"].ToString();
            string _Phone = e.RowData["Phone"].ToString();

            string _OrdererGID = e.RowData["OrdererGID"].ToString();

            string _OrderNumber = e.RowData["OrderNumber"].ToString();
            string _SR = e.RowData["SR"].ToString();
            string _StatementName = e.RowData["StatementName"].ToString();

            string _Note = e.RowData["Note"].ToString();

            string _Fund = e.RowData["Fund"].ToString();
            string _Org = e.RowData["Org"].ToString();
            string _Program = e.RowData["Program"].ToString();

            string _RateLevel = e.RowData["RateLevel"].ToString();
            if (String.IsNullOrWhiteSpace(_RateLevel))
            {
                errors += "Please enter a valid Rate Level";
            }

            int _Term;
            if (!String.IsNullOrWhiteSpace(e.RowData["Term"].ToString()))
            {
                _Term = Convert.ToInt32(e.RowData["Term"]);
            }
            else
            {
                errors += "Please enter a valid term";
                _Term = 0;
            }

            bool _InstallHardware = Convert.ToBoolean(e.RowData["InstallHardware"]);
            bool _InstallSoftware = Convert.ToBoolean(e.RowData["InstallSoftware"]);
            bool _Renewal = Convert.ToBoolean(e.RowData["Renewal"]);

            string _OperatingSystem = e.RowData["OperatingSystem"].ToString();

            string[] _ComponentTypes = new string[] { e.RowData["ComponentType"].ToString(), e.RowData["ComponentType2"].ToString(), e.RowData["ComponentType3"].ToString() };
            string[] _Makes = new string[] { e.RowData["Make"].ToString(), e.RowData["Make2"].ToString(), e.RowData["Make3"].ToString() };
            string[] _Models = new string[] { e.RowData["Model"].ToString(), e.RowData["Model2"].ToString(), e.RowData["Model3"].ToString() };

            string[] _SerialNumbers = new string[] {	!String.IsNullOrWhiteSpace(e.RowData["SerialNumber"])? e.RowData["SerialNumber"]:null, 
															!String.IsNullOrWhiteSpace(e.RowData["SerialNumber2"])?e.RowData["SerialNumber2"]:null, 
															!String.IsNullOrWhiteSpace(e.RowData["SerialNumber3"])?e.RowData["SerialNumber3"]:null };

            string[] _LeaseTags = new string[] { e.RowData["LeaseTag"], e.RowData["LeaseTag2"], e.RowData["LeaseTag3"] };

            

            // Check if Component 1 is filled out before Component 2 or 3
            if (String.IsNullOrEmpty(_ComponentTypes[0]))
            {
                if (!String.IsNullOrEmpty(_ComponentTypes[1]) || !String.IsNullOrEmpty(_ComponentTypes[2]))
                {
                    errors += "Please fill the first Component Type before the 2nd or 3rd.<br />";
                }
            }

            // if Component type is a CPU or Laptop, select operating system
            if (!String.IsNullOrEmpty(_ComponentTypes[0]))
            {
                if (_ComponentTypes[0].ToUpper() == "CPU" || _ComponentTypes[0].ToUpper() == "LAPTOP")
                {
                    if (String.IsNullOrEmpty(_OperatingSystem))
                    {
                        errors += "Please select an operating system for the system.<br />";
                    }
                }
            }

            // Make sure Make, Model and Component Type are all filled in together for all 3 Components

            if (!String.IsNullOrEmpty(_ComponentTypes[0]) || !String.IsNullOrEmpty(_Makes[0]) || !String.IsNullOrEmpty(_Models[0]))
            {
                if (String.IsNullOrEmpty(_ComponentTypes[0]) || String.IsNullOrEmpty(_Makes[0]) || String.IsNullOrEmpty(_Models[0]))
                {
                    errors += "Please fill out Type, Make, and Model for Component 1 to submit.";
                }
            }

            if (!String.IsNullOrEmpty(_ComponentTypes[1]) || !String.IsNullOrEmpty(_Makes[1]) || !String.IsNullOrEmpty(_Models[1]))
            {
                if (String.IsNullOrEmpty(_ComponentTypes[1]) || String.IsNullOrEmpty(_Makes[1]) || String.IsNullOrEmpty(_Models[1]))
                {
                    errors += "Please fill out Type, Make, and Model for Component 2 to submit.";
                }
            }

            if (!String.IsNullOrEmpty(_ComponentTypes[2]) || !String.IsNullOrEmpty(_Makes[2]) || !String.IsNullOrEmpty(_Models[2]))
            {
                if (String.IsNullOrEmpty(_ComponentTypes[2]) || String.IsNullOrEmpty(_Makes[2]) || String.IsNullOrEmpty(_Models[2]))
                {
                    errors += "Please fill out Type, Make, and Model for Component 3 to submit.";
                }
            }

            return errors;
        }

        public JsonResult GroupSR(int SystemId, string SR)
        {
            AuleaseEntities db = new AuleaseEntities();
            SR = SR.ToUpper().Trim();

            PO groupSR;

            if (db.POes.Any(n => n.PONumber == SR))
            {
                groupSR = db.POes.Where(n => n.PONumber == SR).Single();
            }
            else
            {
                groupSR = new PO { PONumber = SR };
                db.POes.Add(groupSR);
            }



            List<SystemGroup> systems = db.SystemGroups.Where(n => n.Id == SystemId).Single().Order.SystemGroups.ToList();

            foreach (var system in systems)
            {
                system.PO = groupSR;
            }

            db.SaveChanges();

            return Json(true);
        }

        public JsonResult GroupOrderNumber(string SR, string OrderNumber)
        {
            AuleaseEntities db = new AuleaseEntities();
            List<SystemGroup> groups = db.POes.Where(n => n.PONumber == SR).Single().SystemGroups.ToList();

            foreach (var lease in groups.SelectMany(n => n.Leases))
            {
                lease.Component.OrderNumber = OrderNumber;
            }

            db.SaveChanges();

            return Json(true);
        }

        public JsonResult DuplicateGroup(int groupId, int DuplicateNumber)
        {
            AuleaseEntities db = new AuleaseEntities();
            SystemGroup modelGroup = db.SystemGroups.Where(n => n.Id == groupId).Single();

            int count = DuplicateNumber;

            for (int i = 0; i < count; i++)
            {

                SystemGroup newGroup = new SystemGroup();
                newGroup.Location = modelGroup.Location;
                newGroup.PO = modelGroup.PO;
                newGroup.User = modelGroup.User;
                newGroup.Order = modelGroup.Order;

                List<Lease> leases = modelGroup.Leases.ToList();

                foreach (var modelLease in leases)
                {
                    Component modelComp = modelLease.Component;

                    Lease lease = new Lease();
                    lease.Department = modelLease.Department;
                    lease.Overhead = modelLease.Overhead;
                    lease.StatementName = modelLease.StatementName;
                    lease.Timestamp = modelLease.Timestamp;

                    Component comp = new Component();
                    comp.OrderNumber = modelComp.OrderNumber;
                    comp.InstallHardware = modelComp.InstallHardware;
                    comp.InstallSoftware = modelComp.InstallSoftware;
                    comp.Note = modelComp.Note;
                    comp.Renewal = modelComp.Renewal;
                    comp.Type = modelComp.Type;
                    comp.Properties = modelComp.Properties;
                    comp.Make = modelComp.Make;
                    comp.Model = modelComp.Model;
                    comp.Status = modelComp.Status;
                    comp.Damages = modelComp.Damages;
                    comp.ReturnDate = modelComp.ReturnDate;

                    lease.Component = comp;
                    newGroup.Leases.Add(lease);
                }

                db.SystemGroups.Add(newGroup);
                db.SaveChanges();
            }

            return Json(true);
        }

        public JsonResult FOPCheck(string Fund, string Org, string Program)
        {
            AuleaseEntities db = new AuleaseEntities();
            bool FOPexists = false;

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                Department dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
                if (dept.Leases.Where(n => n.MonthlyCharge.HasValue).Count() > 0)
                {
                    FOPexists = true;
                }
            }

            if (db.Departments.Any(n => n.Fund == Fund && n.Org == Org && n.Program == Program))
            {
                Department dept = db.Departments.Where(n => n.Fund == Fund && n.Org == Org && n.Program == Program).Single();
                if (dept.Coordinators.Count > 0)
                {
                    FOPexists = true;
                }
            }

            string success = "<span style=\"color:green;font-size:12pt;\">&nbsp;&#x2713;</span>"; // Green checkmark
            string failure = "<span style=\"color:red\">&nbsp;&#x2718;</span>"; // Red x

            if (FOPexists)
            {
                return Json(success);
            }
            else
            {
                return Json(failure);
            }
        }

        public JsonResult CopyOrderNotes(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            SystemGroup group = db.SystemGroups.Where(n => n.Id == Id).Single();
            return Json(group.Order.Note);
        }

        public JsonResult SerialFromLeaseTag(string LeaseTag)
        {
            AuleaseEntities db = new AuleaseEntities();
            LeaseTag = LeaseTag.Trim();
            string Serial;
            if (db.Components.Any(n => n.LeaseTag.ToUpper() == LeaseTag.ToUpper()))
            {
                List<Component> comps = db.Components.Where(n => n.LeaseTag.ToUpper() == LeaseTag.ToUpper()).ToList();
                if (comps.Count > 1)
                {
                    return Json("error: 2 Lease tags found");
                }
                else
                {
                    Serial = comps.Single().SerialNumber;
                    return Json(Serial);
                }
            }
            else
            {
                return Json("error: no Serial Found");
            }
        }

        public ActionResult PopUp(int Id)
        {
            AuleaseEntities db = new AuleaseEntities();
            var comp = db.SystemGroups.Where(n => n.Id == Id);

            var OrderModel = CreateOrderModel(comp);
            OrderModel order = OrderModel.FirstOrDefault();

            ViewData["Order"] = order;

            return View(ViewData);
        }
    }
}
