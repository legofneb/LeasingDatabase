using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class OverheadRateJqGridModel
	{		
		public JQGrid OrdersGrid { get; set; }

		public OverheadRateJqGridModel()
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
									new JQGridColumn {	DataField = "Type",
														Editable = true
														},
									new JQGridColumn {	DataField = "RateLevel",
														Editable = true
														},
									new JQGridColumn {	DataField = "Rate",
														Editable = true
														},
									new JQGridColumn {	DataField = "Term",
														Editable = true
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 200;

			OrdersGrid.SortSettings.InitialSortColumn = "Type";
			
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;



		}
	}
}