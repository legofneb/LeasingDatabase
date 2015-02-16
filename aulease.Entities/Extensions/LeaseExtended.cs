using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aulease.Entities
{
    public partial class Lease
    {
        public Lease(DateTime timestamp)
            : this()
        {
            this.Timestamp = timestamp;
        }

        public bool IsActive()
        {
            return this.SystemGroup.Leases.Where(l => l.Component == this.Component).OrderByDescending(l => l.Timestamp).First() == this && this.Timestamp < this.EndDate;
        }

        public bool IsActiveOrLatest()
        {
            return this.SystemGroup.Leases.Where(l => l.Component == this.Component).OrderByDescending(l => l.Timestamp).First() == this;
        }

        public string RateLevel
        {
            get
            {
                if (this.Overhead == null || this.Overhead.RateLevel == null)
                {
                    return "";
                }
                else
                {
                    return this.Overhead.RateLevel;
                }
            }
        }

        public string GetTermAsString()
        {
            if (this.Overhead == null || this.Overhead.Term == null)
            {
                return "";
            }
            else
            {
                return this.Overhead.Term.ToString();
            }
        }

        public Lease Clone()
        {
            Lease lease = new Lease();
            lease.BeginDate = this.BeginDate;
            lease.EndDate = this.EndDate;
            lease.StatementName = this.StatementName;
            lease.Timestamp = this.Timestamp;
            lease.ContractNumber = this.ContractNumber;
            lease.Department = this.Department;
            lease.MonthlyCharge = this.MonthlyCharge;
            lease.Overhead = this.Overhead;

            return lease;
        }
    }
}
