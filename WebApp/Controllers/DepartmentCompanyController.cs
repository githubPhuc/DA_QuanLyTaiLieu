using DocumentFormat.OpenXml.EMMA;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ToolsApp.Authentication;
using ToolsApp.EntityFramework;
using ToolsApp.Models;
using ToolsApp.Utilities;

namespace ToolsApp.Controllers
{
    [Authorize]
    public class DepartmentCompanyController : BaseController
    {
        // GET: CartManagerment
        QuanLiVanBanEntities db_ = new QuanLiVanBanEntities();
        AppGlobal _AppGlobal = new AppGlobal();
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> GetList(string NameDepartmentSearch)
        {
            var _Departments = db_.Departments.AsNoTracking();
            var _Configs = db_.Configs.AsNoTracking();
            var _Users = db_.Users.AsNoTracking();
            var data = await(from a in _Departments
                             join d in _Configs on a.ParentConFig equals d.Id
                             join b in _Users on a.UserCreate equals b.Id
                        join c in _Users on a.UserUpdate equals c.Id
                        where a.isDelete == false && d.ParentId == _AppGlobal.ID_COMPANY_PARTNER
                             where a.NameDepartment.ToLower().Contains(NameDepartmentSearch.ToLower()) || string.IsNullOrEmpty(NameDepartmentSearch)
                        select new DepartmentCompanyModels
                        {
                            Id = a.Id,
                            ParentConFig = a.ParentConFig,
                            NameConfig = d.NameConfig,
                            NameDepartment = a.NameDepartment,
                            NameUserCreate = b.HoNV+" "+b.TenNV,
                            DescriptionDepartment= a.DescriptionDepartment,
                            AddressDepartment= a.AddressDepartment,
                            Icon = a.Icon,
                            NameUserUpdate = c.HoNV + " " + c.TenNV,
                            Status = a.Status,
                            DatetimeCreate = a.DatetimeCreate,
                            UserCreate = a.UserCreate,
                            DatetimeUpdate = a.DatetimeUpdate,
                            DatetimeDelete = a.DatetimeDelete,
                            UserDelete = a.UserDelete,
                            UserUpdate = a.UserUpdate,
                            isDelete = a.isDelete,
                        }).ToListAsync();
            ViewBag.data = data;
            return PartialView();
        }
        public ActionResult Insert()
        {
            var dataConfig = db_.Configs.Where(a => a.ParentId == _AppGlobal.ID_COMPANY_PARTNER && a.isDelete == false).OrderByDescending(a => a.DatetimeCreate).Select(a => new ConfigModel { Id = a.Id, Name = a.NameConfig, Description = a.DescriptionConfig }).ToList();
            ViewBag.datacompany = dataConfig;
            return PartialView();
        }
        public JsonResult LoadCompany()
        {
            var dataConfig = db_.Configs.Where(a => a.ParentId == _AppGlobal.ID_COMPANY_PARTNER && a.isDelete == false).OrderByDescending(a => a.DatetimeCreate)
                .Select(a => new ConfigModel { Id = a.Id, Name = a.NameConfig, Description = a.DescriptionConfig }).ToList();
            return Json(new { status = 1, title = "", text = "Load dữ liệu thành cônã" +
                "", obj = dataConfig }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(int id)
        {
            var data = db_.Departments.Where(a=>a.Id==id).Select (a=> new DepartmentModels
            {
                Id = a.Id,
                NameDepartment = a.NameDepartment,
                DescriptionDepartment = a.DescriptionDepartment,
                AddressDepartment = a.AddressDepartment,
                Icon = a.Icon,
                Status = a.Status,
                DatetimeCreate = a.DatetimeCreate,
                UserCreate = a.UserCreate,
                DatetimeUpdate = a.DatetimeUpdate,
                DatetimeDelete = a.DatetimeDelete,
                UserDelete = a.UserDelete,
                UserUpdate = a.UserUpdate,
                isDelete = a.isDelete,
            }).FirstOrDefault();
            ViewBag.data = data;
            var dataConfig = db_.Configs.Where(a => a.ParentId == _AppGlobal.ID_COMPANY_PARTNER && a.isDelete == false).Select(a=>new ConfigModel { Id = a.Id,Name = a.NameConfig,Description = a.DescriptionConfig}).ToList();
            ViewBag.datacompany = dataConfig;
            return PartialView();
        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _InsertFun(string NameDepartment,string Address = "", int IdConFig=0 , string AddressDepartment = "", string Icon = "")
        {

            try
            {
                if (IdConFig ==0)
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng chọn công ty", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                if (NameDepartment == "" || NameDepartment == null)
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng nhập tên phòng ban", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                string DescriptionDepartment = _AppGlobal.GenerateRandomString(5).ToUpper();                
                var check = db_.Departments.Where(a => a.DescriptionDepartment == DescriptionDepartment).FirstOrDefault();
                if (check != null)
                {
                    return Json(new { status = -1, title = "", text = "Mã phòng ban này đã tồn tại", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                var model = new Department();
                model.ParentConFig = IdConFig;
                model.NameDepartment = NameDepartment;
                model.DescriptionDepartment = DescriptionDepartment;
                model.AddressDepartment = Address + "-" + AddressDepartment;
                model.Icon = "<i class=\"" + Icon + "\"></i>";
                model.Status = true;
                model.UserCreate = User.UserId;
                model.DatetimeCreate = DateTime.Now;
                model.UserUpdate = User.UserId;
                model.DatetimeUpdate = DateTime.Now;
                model.UserDelete = User.UserId;
                model.DatetimeDelete = DateTime.Now;
                model.isDelete = false;
                db_.Departments.Add(model);
                db_.SaveChanges();

                var fodels = new Folder();
                fodels.DepartmentId = model.Id;
                fodels.NameFolder = DescriptionDepartment+"-"+NameDepartment;
                fodels.IconFolder = "<i class=\"fas fa-file\"></i>";
                fodels.Status = true;
                fodels.UserCreate = User.UserId;
                fodels.DatetimeCreate = DateTime.Now;
                fodels.UserUpdate = User.UserId;
                fodels.DatetimeUpdate = DateTime.Now;
                fodels.UserDelete = User.UserId;
                fodels.DatetimeDelete = DateTime.Now;
                fodels.isDelete = false;
                db_.Folders.Add(fodels);
                db_.SaveChanges();

                return Json(new { status = 1, title = "", text = "Tạo thành công", obj = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }

        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _EditFun(int id,string NameCategory, string Address = "",int IdConFig = 0
       , string AddressDepartment = "", string Icon = "")
        {
         
            try
            {
                if (IdConFig == 0)
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng chọn công ty", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                if (NameCategory == "" || NameCategory == null)
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng nhập tên phòng ban", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                
                var data = db_.Departments.Find(id);
                if(data != null)
                {
                    data.NameDepartment = NameCategory;
                    data.AddressDepartment = Address + "-" + AddressDepartment;
                    data.Icon = "<i class=\"" + Icon + "\"></i>";
                    data.Status = true;
                    data.DatetimeUpdate = DateTime.Now;
                    data.UserUpdate = User.UserId;

                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();
                    return Json(new { status = 1, title = "", text = "Cập nhật thành công", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { status = 1, title = "", text = "Không tìm thấy data hoặc data đã được xóa! vui lòng liên hệ bộ phận kỹ thuật@@", obj = "" }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }
           
        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _DeleteFun(int id)
        {
         
            try
            {
                var data = db_.Departments.Find(id);
                if(data != null)
                {
                    data.DatetimeDelete = DateTime.Now;
                    data.UserDelete = User.UserId;
                    data.isDelete = true;
                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();
                    AppGlobal appGlobal = new AppGlobal();
                    appGlobal.AddRecycleBin(data.Id, "Xóa data chương trình Quản lý phòng ban", "", "Department", User.UserId);
                    return Json(new { status = 1, title = "", text = "Xóa thành công", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu hoặc dữ liệu đã được xóa", obj = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }
           
        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _InsertCompany(string Companyname, string DesCompany)
        {

            try
            {
                
                if (!string.IsNullOrEmpty(Companyname))
                {
                    if (!string.IsNullOrEmpty(DesCompany))
                    {
                        var data = new Config();
                        data.NameConfig = Companyname;
                        data.ParentId = _AppGlobal.ID_COMPANY_PARTNER;
                        data.DescriptionConfig = DesCompany;
                        data.Icon = "<i class=\"fa fa-home\"></i>";
                        data.Status = true;
                        data.UserCreate = User.UserId;
                        data.DatetimeCreate = DateTime.Now;
                        data.UserUpdate = User.UserId;
                        data.DatetimeUpdate = DateTime.Now;
                        data.UserDelete = User.UserId;
                        data.DatetimeDelete = DateTime.Now;
                        data.isDelete = false;
                        db_.Configs.Add(data);
                        db_.SaveChanges();
                        return Json(new { status = 1, title = "", text = "Thêm công ty thành công", obj = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = -1, title = "", text = "Vui lòng nhập mô tả công ty", obj = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng nhập tên công ty", obj = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}