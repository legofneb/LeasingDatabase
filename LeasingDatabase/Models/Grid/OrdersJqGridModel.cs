using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class OrdersJqGridModel
	{
		public JQGrid OrdersGrid { get; set; }

		public OrdersJqGridModel()
		{
			AuleaseEntities db = new AuleaseEntities();

			List<string> Terms = db.Overheads.Select(n => SqlFunctions.StringConvert((double)n.Term)).Distinct().ToList();
            Terms.Add("");
			List<string> RateLevel = db.Overheads.Select(n => n.RateLevel).Distinct().ToList();
            RateLevel.Add("");
			List<string> Make = db.Makes.Select(n => n.Name).Distinct().ToList();
			Make.Add(""); // For null values

			List<string> Type = new List<string>();
			Type.Add("CPU");
			Type.Add("Laptop");
			Type.Add("Monitor");
			Type.Add("Server");
			Type.Add("");

			List<string> TypeMonitor = new List<string>();
			TypeMonitor.Add("Monitor");
			TypeMonitor.Add("");

			List<string> OperatingSystems = db.Properties.Where(n => n.Key == "Operating System").Select(n => n.Value).ToList();
			OperatingSystems.Add("");

            List<string> Architectures = db.Properties.Where(n => n.Key == "Architecture").Select(n => n.Value).ToList();
            Architectures.Add("");

			OrdersGrid = new JQGrid
							{
								Columns = new List<JQGridColumn>()
								{
									new JQGridColumn {  DataField = "OrderDate",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 1,
														DataType = typeof(DateTime),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "SR",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 2,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "OrderNumber",
														Editable = true,
                                                        EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 3,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "SystemID",
														PrimaryKey = true,
														Editable = false,
														Visible = false
														},
									new JQGridColumn {	DataField = "StatementName",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 4,
														DataType = typeof(string),
														Searchable = true,
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "GID",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 5,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "DepartmentName",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 4,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 200
														},
									new JQGridColumn {	DataField = "SerialNumber",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 12,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "LeaseTag",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 13,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Make",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = Make.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 10,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "ComponentType",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = Type.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 9,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains		
														},
									new JQGridColumn {	DataField = "Model",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 11,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "RateLevel",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = RateLevel.Select(n => new SelectListItem { Text = n, Value = n}).ToList(),
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 3,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "InstallHardware",
														Editable = true,
														Formatter = new CheckBoxFormatter(),
														EditType = EditType.CheckBox,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 8,
														DataType = typeof(bool),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "InstallSoftware",
														Editable = true,
														Formatter = new CheckBoxFormatter(),
														EditType = EditType.CheckBox,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 9,
														DataType = typeof(bool),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains	
														},
									new JQGridColumn {	DataField = "Renewal",
														Editable = true,
														Formatter = new CheckBoxFormatter(),
														EditType = EditType.CheckBox,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 10,
														DataType = typeof(bool),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains				
														},
									new JQGridColumn {	DataField = "Term",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = Terms.Select(n => new SelectListItem { Text = n, Value = n}).ToList(),
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 2,
														DataType = typeof(int),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 75
														},
									new JQGridColumn {	DataField = "Note",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 1,
														DataType = typeof(string),
														SearchCaseSensitive = false,
                                                        EditType = Trirand.Web.Mvc.EditType.TextArea,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Phone",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 6,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains	
														},
									new JQGridColumn {	DataField = "Room",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 7,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Building",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 8,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "OrdererGID",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 13,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "OperatingSystem",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 11,
														EditList = OperatingSystems.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditType = EditType.DropDown,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
                                    new JQGridColumn {	DataField = "Architecture",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 12,
														EditList = Architectures.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditType = EditType.DropDown,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Status",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 14,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "SerialNumber2",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 17,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "LeaseTag2",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 18,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "ComponentType2",
														Editable = true,
														EditType = EditType.DropDown,
                                                        EditDialogRowPosition = 14,
														EditList = TypeMonitor.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditDialogColumnPosition = 1,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Make2",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = Make.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 15,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Model2",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 16,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "SerialNumber3",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 22,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "LeaseTag3",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 23,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "ComponentType3",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = TypeMonitor.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 19,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Make3",
														Editable = true,
														EditType = EditType.DropDown,
														EditList = Make.Select(n => new SelectListItem { Value = n, Text = n}).ToList(),
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 20,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Model3",
														Editable = true,
														EditDialogColumnPosition = 1,
                                                        EditDialogRowPosition = 21,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "Fund",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 5,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "Org",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 6,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "Program",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 7,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains,
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "EOLComponent",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 15,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "EOLComponent2",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 16,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														},
									new JQGridColumn {	DataField = "EOLComponent3",
														Editable = true,
														EditDialogColumnPosition = 2,
                                                        EditDialogRowPosition = 17,
														DataType = typeof(string),
														SearchCaseSensitive = false,
														SearchToolBarOperation = SearchOperation.Contains
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 700;
			OrdersGrid.AddDialogSettings.Width = 700;
			OrdersGrid.EditDialogSettings.Width = 800;
			
			OrdersGrid.SortSettings.InitialSortColumn = "OrderDate";
			OrdersGrid.SortSettings.InitialSortDirection = SortDirection.Desc;
			
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowAddButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.ToolBarSettings.ShowSearchButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;
			OrdersGrid.AddDialogSettings.CloseAfterAdding = true;

			OrdersGrid.ToolBarSettings.ShowSearchToolBar = true;

			OrdersGrid.ClientSideEvents.GridInitialized = "initGrid";
			OrdersGrid.ClientSideEvents.BeforeAddDialogShown = "BeforeAddDialog";
			OrdersGrid.ClientSideEvents.BeforeEditDialogShown = "BeforeEdit";
			OrdersGrid.ClientSideEvents.AfterEditDialogShown = "AfterEdit";


		}
	}
}