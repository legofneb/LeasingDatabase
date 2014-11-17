using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    public class NGNewOrdersByPOModel
    {
        public string SR { get; set; }
        public string Summary { get; set; }

        public IEnumerable<NGConfigurationModel> Configuration { get; set; }
        public IEnumerable<NGOrderSystemGroupModel> SystemGroups { get; set; }
    }

    public  class NGConfigurationModel
    {
        public string Type { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
    }

    public class NGDepartmentModel
    {
        public string FOP { get; set; }
        public string DepartmentName { get; set; }
    }

    public class NGUserModel
    {
        public string GID { get; set; }
        public string Phone { get; set; }
    }
}