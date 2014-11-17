using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class DepartmentJqGridModel
	{
				public JQGrid OrdersGrid { get; set; }

		public DepartmentJqGridModel()
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
									new JQGridColumn {	DataField = "Name",
														Editable = true,
														DataType = typeof(string)
														},
									new JQGridColumn {	DataField = "Fund",
														Editable = true,
														DataType = typeof(string)
														},
									new JQGridColumn {	DataField = "Org",
														Editable = true,
														DataType = typeof(string)
														},
									new JQGridColumn {	DataField = "Program",
														Editable = true,
														DataType = typeof(string)
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 200;
            OrdersGrid.ToolBarSettings.ShowSearchToolBar = true;
			OrdersGrid.SortSettings.InitialSortColumn = "Fund";
			
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;



		}
	}
}