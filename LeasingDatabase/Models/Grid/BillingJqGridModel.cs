using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class BillingJqGridModel
	{
		
		public JQGrid OrdersGrid { get; set; }

		public BillingJqGridModel()
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
									new JQGridColumn {	DataField = "BeginDate",
														Editable = true
														},
									new JQGridColumn {	DataField = "EndDate",
														Editable = true
														},
									new JQGridColumn {  DataField = "StatementName",
														Editable = true
														},
									new JQGridColumn {	DataField = "Timestamp",
														Editable = true
														},
									new JQGridColumn {	DataField = "ContractNumber",
														Editable = true
														},
									new JQGridColumn {	DataField = "Fund",
														Editable = true				
														},
									new JQGridColumn {	DataField = "Org",
														Editable = true
														},
									new JQGridColumn {	DataField = "Program",
														Editable = true							
														},
									new JQGridColumn {	DataField = "RateLevel",
														Editable = true
														},
									new JQGridColumn {	DataField = "MonthlyCharge",
														Editable = true				
														},
									new JQGridColumn {	DataField = "Term",
														Editable = true				
														},
									new JQGridColumn {	DataField = "PrimaryCharge",
														Editable = true				
														},
									new JQGridColumn {	DataField = "SecondaryCharges",
														Editable = true				
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 200;
			OrdersGrid.EditDialogSettings.Width = 700;

			OrdersGrid.SortSettings.InitialSortColumn = "Timestamp";
			OrdersGrid.SortSettings.InitialSortDirection = SortDirection.Desc;
			
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;

		}
	}
}