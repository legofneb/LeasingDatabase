using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aulease.Entities
{
	public partial class Component
	{
		public Lease GetCurrentLease() {
			return this.Leases.OrderByDescending(l => l.Timestamp).FirstOrDefault(l => l.EndDate < DateTime.Now);
		}

		public Lease GetLatestLease() {
			return this.Leases.OrderByDescending(l => l.Timestamp).First();
		}

		public string GID()
		{
			return this.CurrentLease.SystemGroup.Order.User.GID;
		}

		public string OperatingSystem
		{
			get
			{
				bool hasOS = this.Properties.Where(n => n.Key == "OS").Count() > 0;

				if (!hasOS)
				{
					return "";
				}
				else
				{
					return this.Properties.Where(n => n.Key == "OS").Select(n => n.Value).FirstOrDefault();
				}
			}
		}

		public Department Department
		{
			get { return this.CurrentLease.Department; }
		}

		public Location Location
		{
			get { return this.CurrentLease.SystemGroup.Location; }
		}

		public User User
		{
			get { return this.CurrentLease.SystemGroup.User; }
		}

		public Lease CurrentLease
		{
			get { return this.Leases.OrderByDescending(n => n.EndDate).FirstOrDefault(); }
		}

		public PO PO
		{
			get { return this.CurrentLease.SystemGroup.PO; }
		}

		public int Term
		{
			get { return this.CurrentLease.Overhead.Term; }
		}

		public int VendorTerm
		{
			get { return (this.CurrentLease.Overhead.Term + 1); }
		}

		public SystemGroup SystemGroup
		{
			get { return this.CurrentLease.SystemGroup; }
		}

        public string GetTypeName()
        {
            if (this.Type == null) {
                return "";
            } else {
                return this.Type.Name;
            }
        }

        public string GetMakeName()
        {
            if (this.Make == null)
            {
                return "";
            }
            else
            {
                return this.Make.Name;
            }
        }

        public string GetModelName()
        {
            if (this.Model == null)
            {
                return "";
            }
            else
            {
                return this.Model.Name;
            }
        }

        public bool isMonitor()
        {
            return this.Type.Name.Equals("Monitor", StringComparison.OrdinalIgnoreCase);
        }

        public Component Clone()
        {
            Component comp = new Component();
            comp.OrderNumber = this.OrderNumber;
            comp.InstallSoftware = this.InstallSoftware;
            comp.InstallHardware = this.InstallHardware;
            comp.Note = this.Note;
            comp.Renewal = this.Renewal;

            return comp;
        }
	}
}