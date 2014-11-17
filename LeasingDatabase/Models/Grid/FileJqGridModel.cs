using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;
using System.Web.UI.WebControls;

namespace LeasingDatabase.Models.Grid
{
	public class FileJqGridModel
	{
		public JQGrid FileDataGrid { get; set; }

		public FileJqGridModel()
		{
			FileDataGrid = new JQGrid
			{
				Columns = new List<JQGridColumn>()
                                 {
                                     new JQGridColumn { 
                                                        DataField = "ID",
                                                        PrimaryKey = true,
                                                        Visible = true,
                                                        Sortable = false
                                                      },
                                     new JQGridColumn { 
                                                        DataField = "Name", 
                                                        Width = 350, 
                                                        Sortable = false 
                                                      },
                                     new JQGridColumn { 
                                                        DataField = "Size",
                                                        Width = 100,
                                                        Sortable = false
                                                      },                                     
                                     new JQGridColumn { 
                                                        DataField = "DateCreated", 
                                                        HeaderText = "Date Created",
                                                        Width = 100,
                                                        Sortable = false
                                                      },                                     
                                     new JQGridColumn { 
                                                        DataField = "LastModified", 
                                                        HeaderText = "Last Modified",
                                                        Width = 100,
                                                        Sortable = false
                                                      },
                                     
                                 },

				TreeGridSettings = new TreeGridSettings
				{
					Enabled = true
				}				
			};

			FileDataGrid.PagerSettings.ScrollBarPaging = true;
			FileDataGrid.PagerSettings.PageSize = 100;

		}
	}
}