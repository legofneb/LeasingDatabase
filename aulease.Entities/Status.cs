//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace aulease.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Status
    {
        public Status()
        {
            this.Components = new HashSet<Component>();
            this.Repairs = new HashSet<Repair>();
            this.Tasks = new HashSet<Task>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Component> Components { get; set; }
        public virtual ICollection<Repair> Repairs { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}