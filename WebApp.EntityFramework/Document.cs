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
    
    public partial class Document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Document()
        {
            this.Deliverys = new HashSet<Delivery>();
            this.Files = new HashSet<File>();
        }
    
        public int Id { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public Nullable<System.DateTime> DatePublish { get; set; }
        public string UrlFile { get; set; }
        public Nullable<int> FolderId { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
        public string FileExtension { get; set; }
        public Nullable<bool> IsDoc { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Delivery> Deliverys { get; set; }
        public virtual Folder Folder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<File> Files { get; set; }
    }
}