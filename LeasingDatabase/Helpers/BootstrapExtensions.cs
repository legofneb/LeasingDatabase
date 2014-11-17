using aulease.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeasingDatabase.Helpers
{
    public static class BootstrapExtensions
    {
        public static string Row(this HtmlHelper helper, string innerText)
        {
            return String.Format("<div class=\"row\">{0}</div>", innerText);
        }

        public static string Column(this HtmlHelper helper, string htmlClass, string innerText)
        {
            return String.Format("<div class=\"{0}\">{1}</div>", htmlClass, innerText);
        }

        public static string IconFromType(this HtmlHelper helper, Types type, string extraClass)
        {
            switch (type)
            {
                case Types.CPU:
                    return String.Format("<i class=\"fa fa-hdd-o {0}\"></i>", extraClass);
                case Types.Laptop:
                    return String.Format("<i class=\"fa fa-laptop {0}\"></i>", extraClass);
                case Types.Monitor:
                    return String.Format("<i class=\"fa fa-desktop {0}\"></i>", extraClass);
                case Types.Server:
                    return String.Format("<i class=\"fa fa-hdd-o {0}\"></i>", extraClass);
                case Types.Printer:
                    return String.Format("<i class=\"fa fa-print {0}\"></i>", extraClass);
                case Types.Insurance:
                    return String.Format("<i class=\"fa fa-pencil-square-o {0}\"></i>", extraClass);
                case Types.Warranty:
                    return String.Format("<i class=\"fa fa-pencil-square-o {0}\"></i>", extraClass);
                case Types.Shipping:
                    return String.Format("<i class=\"fa fa-truck {0}\"></i>", extraClass);
                default:
                    return "";
            }
        }

        public static string SearchTags(this HtmlHelper helper, PO SR)
        {
            string Tags = "";
            
            //Add SR
            Tags += SR.PONumber + " ";
            
            //Add Order Info

            foreach (var order in SR.SystemGroups.Select(n => n.Order).Distinct())
            {
                Tags += order.User.GID + " ";
                Tags += order.User.FirstName + " ";
                Tags += order.User.LastName + " ";
            }

            //Add Statement Names

            foreach (var lease in SR.SystemGroups.SelectMany(n => n.Leases))
            {
                Tags += lease.StatementName + " ";
            }

            //Add Department Info

            foreach (var dept in SR.SystemGroups.SelectMany(n => n.Leases).Select(n => n.Department).Distinct())
            {
                Tags += dept.Name + " ";
            }

            return Tags.ToUpper();
        }
    }
}