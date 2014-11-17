using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aulease.Entities {
	
	public partial class Type {


		public string GetIconClass() {

			var Icons = new Dictionary<string, string>() {
				{ "CPU", "icon-hdd" },
				{ "LAPTOP", "icon-laptop" },
				{ "MONITOR", "icon-desktop" },
				{ "PRINTER", "icon-print" },
				{ "SERVER", "icon-hdd" }
			};

			string value = "";
			Icons.TryGetValue(this.Name.ToUpper(), out value);
			return value;
		}

        public Types ToEnum() {
            switch (this.Id)
            {
                case 1:
                    return Types.CPU;
                case 2:
                    return Types.Laptop;
                case 3:
                    return Types.Monitor;
                case 4:
                    return Types.Server;
                case 5:
                    return Types.Printer;
                case 6:
                    return Types.Insurance;
                case 7:
                    return Types.Warranty;
                case 8:
                    return Types.Shipping;
                default:
                    return Types.CPU;
            }
            
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Types))
            {
                Types StatedType = (Types)obj;
                if (this.Id == (int)StatedType)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return base.Equals(obj);
        }
	}

    public enum Types
    {
        CPU = 1,
        Laptop = 2,
        Monitor = 3,
        Server = 4,
        Printer = 5,
        Insurance = 6,
        Warranty = 7,
        Shipping = 8
    }
}
