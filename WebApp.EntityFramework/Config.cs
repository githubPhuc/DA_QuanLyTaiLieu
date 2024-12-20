//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ToolsApp.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Config
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Config()
        {
            this.Configs1 = new HashSet<Config>();
            this.Departments = new HashSet<Department>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string NameConfig { get; set; }
        public string DescriptionConfig { get; set; }
        public string Icon { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Config> Configs1 { get; set; }
        public virtual Config Config1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments { get; set; }
    }
}
