using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ToolsApp.EntityFramework;

namespace ToolsApp.Models
{
    public partial class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "ResetCode is required")]
        public string ResetCode { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8}$", ErrorMessage = "Password must meet requirements")]
        [StringLength(255, ErrorMessage = "Must be between 8 and 255 characters", MinimumLength = 8)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 8 and 255 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
    public partial class UserJson
    {
        public string username { get; set; }
        public string password { get; set; }
    }
 
    public class ResponseObject
    {
        public string message { get; set; }
        public object data { get; set; }
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public string id { get; set; }
        public string phone { get; set; }
        public string role { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int idDepartment { get; set; }
    }
    public partial class QL_UserViewModel
    {
        public int Id { get; set; }
        public string AccoutType { get; set; }
        public string Username { get; set; }
        public string SalesRepCode { get; set; }
        public string HoNV { get; set; }
        public string TenNV { get; set; }
        public string Email { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
        public string Roles { get; set; }
        public string Pages { get; set; }
        public string Softwares { get; set; }
        public string PrincipalGroups { get; set; }
    }
    public partial class QLKTPPageViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "The field is required.")]
        public int Id { get; set; }
        [Display(Name = "Code")]
        [Required(ErrorMessage = "The field is required.")]
        public string Code { get; set; }
        [Display(Name = "Info")]
        public string Info { get; set; }
        [Display(Name = "Controler")]
        [Required(ErrorMessage = "The field is required.")]
        public string Controler { get; set; }
        [Display(Name = "Action")]
        [Required(ErrorMessage = "The field is required.")]
        public string Action { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
    }
    public partial class QLKTPRoleViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "The field Id is required.")]
        public int Id { get; set; }
        [Display(Name = "Code")]
        [Required(ErrorMessage = "The field Code is required.")]
        public string Code { get; set; }
        [Display(Name = "Role name")]
        [Required(ErrorMessage = "The field RoleName is required.")]
        public string RoleName { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
    }
    public partial class QLKTPMenuViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "The field Id is required.")]
        public int Id { get; set; }
        [Display(Name = "Menu Name")]
        [Required(ErrorMessage = "The field MenuName is required.")]
        public string MenuName { get; set; }
        [Display(Name = "Parent Id")]
        public Nullable<int> ParentId { get; set; }
        [Display(Name = "Page Id")]
        public Nullable<int> PageId { get; set; }
        [Display(Name = "Icon")]
        public string Icon { get; set; }
        [Display(Name = "Order No")]
        [Required(ErrorMessage = "The field OrderNo is required.")]
        public Nullable<int> OrderNo { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
    }
    public partial class QLKTPSoftwareGroupViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "The field Id is required.")]
        public int Id { get; set; }
        [Display(Name = "Title")]
        [Required(ErrorMessage = "The field Title is required.")]
        public string Title { get; set; }
        public string Domain { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
        public string ColorCode { get; set; }
    }
    public partial class QLKTPSoftwareViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "The field Id is required.")]
        public int Id { get; set; }
        [Display(Name = "Title")]
        [Required(ErrorMessage = "The field Title is required.")]
        public string Title { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string Domain { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
        public string Sites { get; set; }
    }
    public partial class QLKTPUserBlackListViewModel
    {
        public long Id { get; set; }
        public string BlackListUser { get; set; }
        public string BlackListFullname { get; set; }
        public string Email { get; set; }
        public string PrincipalId { get; set; }
        public string PrincipalName { get; set; }
        public string Ticket { get; set; }
        public string Reason { get; set; }
        public string UserCreate { get; set; }
        public string UserFullnameCreate { get; set; }
        public DateTime DatetimeCreate { get; set; }
        public string UserUpdate { get; set; }
        public string UserFullnameUpdate { get; set; }
        public DateTime DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public string UserDelete { get; set; }
        public string UserFullnameDelete { get; set; }
        public DateTime DatetimeDelete { get; set; }
    }
    public partial class SitesViewModel
    {
        [Display(Name = "Site Code")]
        [Required(ErrorMessage = "The field SiteCode is required.")]
        [StringLength(3, ErrorMessage = "Must be between 1 and 3 characters", MinimumLength = 1)]
        public string SiteCode { get; set; }
        [Display(Name = "Site Name")]
        [Required(ErrorMessage = "The field SiteName is required.")]
        public string SiteName { get; set; }
    }
    public partial class TaiKhoanViewModels
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public Nullable<bool> IsHieuLuc { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiCapNhat { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
    public partial class Response
    {
        public string Status { get; set; }

        public string Message { get; set; }
    }
  
    public partial class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
    }
    public partial class RegisterViewModel
    {

        public string AccoutType { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SoDienThoai { get; set; }
        public string HoNV { get; set; }
        public string TenNV { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string NoiSinh { get; set; }
        public string DiaChiThuongTru { get; set; }
        public string DiaChiTamTru { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
        public Nullable<int> CCCD { get; set; }
        public DateTime NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string MaSoBHXH { get; set; }
        public string TrinhDoVanHoa { get; set; }
        public string TrinhDoChuyenMon { get; set; }
        public string NganhHoc { get; set; }
        public string NoiDaoTao { get; set; }
        public string TrinhDoChinhTri { get; set; }
        public DateTime NgayVaoDangDuBi { get; set; }
        public DateTime NgayVaoDangChinhThuc { get; set; }
        public int PhongBan { get; set; }
        public string ChucDanh { get; set; }
        public DateTime NgayVaoLam { get; set; }
        public string GhiChu { get; set; }
    }
   
    public partial class CategoryModels
    {
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public string DescriptionCategory { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public string NameUserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public string NameUserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
    }
    public partial class RecycleBinModels
    {
        public int Id { get; set; }
        public string NameRecycle { get; set; }
        public string DescriptionRecycle { get; set; }
        public string TableName { get; set; }
        public Nullable<int> IdParent { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public string NameUserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public string NameUserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
    }
    public partial class ConfigModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public partial class DepartmentModels
    {
        public int Id { get; set; }
        public Nullable<int> ParentConFig { get; set; }
        public string NameDepartment { get; set; }
        public string DescriptionDepartment { get; set; }
        public string AddressDepartment { get; set; }
        public string Icon { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public string NameUserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public string NameUserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
    }
    public partial class DepartmentCompanyModels
    {
        public int Id { get; set; }
        public Nullable<int> ParentConFig { get; set; }
        public string NameConfig { get; set; }
        public string NameDepartment { get; set; }
        public string DescriptionDepartment { get; set; }
        public string AddressDepartment { get; set; }
        public string Icon { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public string NameUserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public string NameUserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
    }
    public partial class DocumentModels
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Category Category { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<System.DateTime> DatePublish { get; set; }
        public string UrlFile { get; set; }
        public Nullable<int> FolderId { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public string NameUserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public string NameUserUpdate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
        public Nullable<int> CountFile { get; set; }
    }
    public class InsertDocumentModels
    {
        public int IdDepartment { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string DatePublish { get; set; }
        public Nullable<int> FolderId { get; set; }
        public IEnumerable<HttpPostedFileBase> files { get; set; }
        
    }
    public class InsertDocument_TaiLieu_Models
    {
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string DatePublish { get; set; }
        public Nullable<int> FolderId { get; set; }
        public IEnumerable<HttpPostedFileBase> files { get; set; }

    }
    public partial class UpdateDocument_TaiLieu_Models
    {
        public int Id { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string DatePublish { get; set; }
        public Nullable<int> FolderId { get; set; }
        public IEnumerable<HttpPostedFileBase> files { get; set; }

    }
    public class InsertDeliverDocumentModels
    {
        public int IdDepartment { get; set; }
        public int RecipientAddressId { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string DatePublish { get; set; }
        public Nullable<int> FolderId { get; set; }
        public List<string> AuthenticateDepartment { get; set; }
        public List<string> AuthenticateStaff { get; set; }
        public IEnumerable<HttpPostedFileBase> files { get; set; }

    }
    public partial class UpdateDocumentModels
    {
        public int Id { get; set; }
        public int IdDepartment { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string DatePublish { get; set; }
        public Nullable<int> FolderId { get; set; }
        public IEnumerable<HttpPostedFileBase> files { get; set; }

    }

    public partial class DoccumentComboboxModels
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string NameDepartment { get; set; }
    }
    public partial class OutgoingDocumentModels
    {
        public int IdDocument { get; set; }
        public int IdDeliverys { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Category Category { get; set; }
        public string CodeDocument { get; set; }
        public string TitleDocument { get; set; }
        public string AbstractDocument { get; set; }
        public string NameRecipientAddress { get; set; }
        public string NameShippingAddress { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<System.DateTime> DatePublish { get; set; }
        public string UrlFile { get; set; }
        public Nullable<int> FolderId { get; set; }
        public Nullable<bool> Status { get; set; }
        public int UserCreate { get; set; }
        public string NameUserCreate { get; set; }
        public System.DateTime DatetimeCreate { get; set; }
        public string NameUserUpdate { get; set; }
        public Nullable<int> UserUpdate { get; set; }
        public Nullable<System.DateTime> DatetimeUpdate { get; set; }
        public int DocumentUserCreate { get; set; }
        public string DocumentNameUserCreate { get; set; }
        public System.DateTime DocumentDatetimeCreate { get; set; }
        public string DocumentNameUserUpdate { get; set; }
        public Nullable<int> DocumentUserUpdate { get; set; }
        public Nullable<System.DateTime> DocumentDatetimeUpdate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<int> UserDelete { get; set; }
        public Nullable<System.DateTime> DatetimeDelete { get; set; }
        public Nullable<int> CountFile { get; set; }
        public List<File> files { get; set; }

    }
}   