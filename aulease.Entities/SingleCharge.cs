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
    
    public partial class SingleCharge
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string GID { get; set; }
        public string Note { get; set; }
        public System.DateTime Date { get; set; }
        public bool HasPaid { get; set; }
        public int DepartmentId { get; set; }
    
        public virtual Department Department { get; set; }
    }
}
