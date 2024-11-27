using System;
using System.Collections.Generic;
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
    public class CategoryController : BaseController
    {
        // GET: CartManagerment
        QuanLiVanBanEntities db_ = new QuanLiVanBanEntities();
        public ActionResult Index()
        {
          
            return View();
        }
        public async Task<ActionResult> GetList(string NameCategorySearch)
        {
            var _Categories = db_.Categories.AsNoTracking();
            var _Users = db_.Users.AsNoTracking();
            var data = await(from a in _Categories
                             join b in _Users on a.UserCreate equals b.Id
                        join c in _Users on a.UserUpdate equals c.Id
                        where a.isDelete == false
                        where a.NameCategory.ToLower().Contains(NameCategorySearch.ToLower()) || string.IsNullOrEmpty(NameCategorySearch)
                        select new CategoryModels
                        {
                            Id = a.Id,
                            NameCategory = a.NameCategory,
                            NameUserCreate = b.HoNV+" "+b.TenNV,
                            DescriptionCategory= a.DescriptionCategory,
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
            ViewBag.KetCaus = data;
            return PartialView();
        }
        public ActionResult Insert()
        {
           
            return PartialView();
        }
        public ActionResult Edit(int id)
        {
            var data = db_.Categories.Where(a=>a.Id==id).FirstOrDefault();
            ViewBag.data = data;
            return PartialView();
        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _InsertFun(string NameCategory,string DescriptionCategory="")
        {
            
            try
            {
                if (NameCategory == "" || NameCategory == null)
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng nhập tên loại", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                var model_copy = new Category();
                model_copy.NameCategory = NameCategory;
                model_copy.DescriptionCategory = DescriptionCategory;
                model_copy.Status = true;
                model_copy.UserCreate = User.UserId;
                model_copy.DatetimeCreate = DateTime.Now;
                model_copy.UserUpdate = User.UserId;
                model_copy.DatetimeUpdate = DateTime.Now;
                model_copy.UserDelete = User.UserId;
                model_copy.DatetimeDelete = DateTime.Now;
                model_copy.isDelete = false;

                db_.Categories.Add(model_copy);
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
        public JsonResult _EditFun(int id,string NameCategory, string DescriptionCategory = "")
        {
         
            try
            {

                if (NameCategory == "" || NameCategory == null)
                {
                    return Json(new { status = -1, title = "", text = "Vui lòng nhập tên loại", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                var data = db_.Categories.Find(id);
                if(data != null)
                {
                    data.NameCategory = NameCategory;
                    data.DescriptionCategory = DescriptionCategory;
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

                var data = db_.Categories.Find(id);
                if(data != null)
                {
                    data.DatetimeDelete = DateTime.Now;
                    data.UserDelete = User.UserId;
                    data.isDelete = true;
                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();
                    AppGlobal appGlobal = new AppGlobal();
                    appGlobal.AddRecycleBin(data.Id, "Xóa data chương trình quản lý loại văn bản ", "", "Category", User.UserId);
                    return Json(new { status = 1, title = "", text = "Xóa thành công", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = 1, title = "", text = "Không tìm thấy dữ liệu hoặc dữ liệu đã được xóa", obj = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }
           
        }

    }
}