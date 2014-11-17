using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace LeasingDatabase.Models.Grid
{
	public class BillingCoordinatorJqGridModel
	{
		public JQGrid OrdersGrid { get; set; }

		public BillingCoordinatorJqGridModel()
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
									new JQGridColumn {	DataField = "GID",
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
                                    new JQGridColumn {	DataField = "BillingAccess",
														Editable = true,
                                                        Formatter = new CheckBoxFormatter(),
														EditType = EditType.CheckBox,
														DataType = typeof(bool)
														}
								}
							};

			OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
			OrdersGrid.PagerSettings.ScrollBarPaging = true;
			OrdersGrid.PagerSettings.PageSize = 100;
			OrdersGrid.Height = 200;

			OrdersGrid.SortSettings.InitialSortColumn = "GID";

			OrdersGrid.ToolBarSettings.ShowAddButton = true;
			OrdersGrid.ToolBarSettings.ShowEditButton = true;
			OrdersGrid.ToolBarSettings.ShowDeleteButton = true;
			OrdersGrid.EditDialogSettings.CloseAfterEditing = true;



		}
	}
}