using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class ComponentJqGridModel
	{
		public JQGrid OrdersGrid { get; set; }

		public ComponentJqGridModel()
		{

			OrdersGrid = new JQGrid
							{
								Columns = new List<JQGridColumn>()
								{	
									new JQGridColumn {	DataField = "ComponentID",
														PrimaryKey = true,
														Editable = false,
														Visible = false
														},
									new JQGridColumn {	DataField = "SerialNumber",
														Editable = true,
														DataType = typeof(string)
														},
									new JQGridColumn {	DataField = "LeaseTag",
														Editable = true,
														DataType = typeof(string),
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "SRNumber",
														Editable = true,
														DataType = typeof(string)	,
					                                    Width = 100
														},
									new JQGridColumn {	DataField = "StatementName",
														Editable = true,
														DataType = typeof(string)		
														},
									new JQGridColumn {	DataField = "GID",
														Editable = true,
														DataType = typeof(string),
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "Note",
														Editable = true,
                                                        EditType = Trirand.Web.Mvc.EditType.TextArea,
														DataType = typeof(string),
                                                        Width = 200
														},
									new JQGridColumn {	DataField = "Make",
														Editable = true,
														DataType = typeof(string)
														},
									new JQGridColumn {	DataField = "ComponentType",
														Editable = true,
														DataType = typeof(string)		
														},
									new JQGridColumn {	DataField = "Model",
														Editable = true,
														DataType = typeof(string)	
														},
									new JQGridColumn {	DataField = "BeginDate",
														Editable = true,
														DataType = typeof(DateTime)
														},
									new JQGridColumn {	DataField = "EndDate",
														Editable = true,
														DataType = typeof(DateTime)
														},
									new JQGridColumn {	DataField = "DepartmentName",
														Editable = true,
														DataType = typeof(string),
		                                                Width = 200
														},
									new JQGridColumn {	DataField = "Fund",
														Editable = true,
														DataType = typeof(string),
			                                            Width = 100
														},
									new JQGridColumn {	DataField = "Org",
														Editable = true,
														DataType = typeof(string),
			                                            Width = 100
														},
									new JQGridColumn {	DataField = "Program",
														Editable = true,
														DataType = typeof(string),
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "MonthlyCharge",
														Editable = true,
														DataType = typeof(string),
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "RateLevel",
														Editable = true,
														DataType = typeof(string),
                                                        Width = 100
														},
									new JQGridColumn {	DataField = "Term",
														Editable = true,
														DataType = typeof(int),
                                                        Width = 75
														},
									new JQGridColumn {	DataField = "Phone",
														Editable = true,
														DataType = typeof(string)			
														},
									new JQGridColumn {	DataField = "Building",
														Editable = true,
														DataType = typeof(string)				
														},
									new JQGridColumn {	DataField = "Room",
														Editable = true,
														DataType = typeof(string)
														},
									new JQGridColumn {	DataField = "ContractNumber",
														Editable = true,
														DataType = typeof(string)				
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 700;
			OrdersGrid.EditDialogSettings.Width = 700;

			OrdersGrid.ToolBarSettings.ShowSearchToolBar = true;

			OrdersGrid.SortSettings.InitialSortColumn = "SRNumber";
			OrdersGrid.SortSettings.InitialSortDirection = SortDirection.Desc;
			
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;

			OrdersGrid.ClientSideEvents.GridInitialized = "initGrid";
            OrdersGrid.ClientSideEvents.BeforeEditDialogShown = "BeforeEdit";
            OrdersGrid.ClientSideEvents.AfterEditDialogShown = "AfterEdit";
		}
	}
}