using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class EOLDetailedJqGridModel
	{
		public JQGrid OrdersGrid { get; set; }

		public EOLDetailedJqGridModel()
		{

			OrdersGrid = new JQGrid
							{
								Columns = new List<JQGridColumn>()
								{	
									new JQGridColumn {	DataField = "Id",
														PrimaryKey = true,
														Editable = false,
														Visible = false
														},
									new JQGridColumn {	DataField = "SN", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "LeaseTag", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "Term", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "MonthlyCharge", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "SR", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "SerialNumber", // This serial number is truncated to last 7 characters of serial
														Editable = true
														},
									new JQGridColumn {	DataField = "ComponentType",
														Editable = false
														},
									new JQGridColumn {  DataField = "Make",
														Editable = false
														},
									new JQGridColumn {	DataField = "Model",
														Editable = false
														},
									new JQGridColumn {	DataField = "DepartmentName", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "FOP", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "RateLevel", // Full Serial Number
														Editable = false
														},
									new JQGridColumn {	DataField = "StatementName",
														Editable = false
														},
									new JQGridColumn {	DataField = "GID",
														Editable = false
														},
									new JQGridColumn {	DataField = "EndBillingDate",
														Editable = false
														},
									new JQGridColumn {	DataField = "ReturnDate",
														Editable = true
														},
									new JQGridColumn {	DataField = "Damages",
														Editable = true				
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 200;
			OrdersGrid.EditDialogSettings.Width = 700;

			OrdersGrid.SortSettings.InitialSortColumn = "SerialNumber";
			OrdersGrid.SortSettings.InitialSortDirection = SortDirection.Desc;
			
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;

            OrdersGrid.ClientSideEvents.GridInitialized = "initGrid";
		}
	}
}