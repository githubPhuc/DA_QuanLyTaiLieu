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
    
    public partial class Folder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Folder()
        {
            this.Files = new HashSet<File>();
            this.Folders1 = new HashSet<Folder>();
            this.Documents = new HashSet<Document>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> CategoryDocumentId { get; set; }
        public string NameFolder { get; set; }
        public string IconFolder { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
        public Nullable<int> DepartmentId { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<File> Files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Folder> Folders1 { get; set; }
        public virtual Folder Folder1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}