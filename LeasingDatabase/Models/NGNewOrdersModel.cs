﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeasingDatabase.Models
{
    public class NGNewOrdersModel
    {
        public int id { get; set; }
        public string Date { get; set; }
        public string OrderNumber { get; set; }
        public string OrdererFirstName { get; set; }
        public string OrdererLastName { get; set; }
        public string OrdererGID { get; set; }
        public string OrdererBuilding { get; set; }
        public string OrdererRoom { get; set; }
        public string OrdererPhone { get; set; }
        public IEnumerable<NGConfigurationModel> Configuration { get; set; }
        public IEnumerable<NGOrderSystemGroupModel> Components { get; set; }
        public string Summary { get; set; }
        public string Notes { get; set; }
    }

    public class NGOrderSystemGroupModel
    {
        public int id { get; set; }
        public string StatementName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GID { get; set; }
        public string DepartmentName { get; set; }
        public string FOP { get; set; }
        public string RateLevel { get; set; }
        public int? Term { get; set; }
        public bool InstallHardware { get; set; }
        public bool InstallSoftware { get; set; }
        public bool Renewal { get; set; }
        public string Phone { get; set; }
        public string Room { get; set; }
        public string Building { get; set; }
        public string OperatingSystem { get; set; }
        public IEnumerable<NGComponentModel> Components { get; set; }
        public IEnumerable<NGComponentModel> EOLComponents { get; set; }

        public string Notes { get; set; }
    }
}