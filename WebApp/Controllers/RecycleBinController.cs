using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
    public class RecycleBinController : BaseController
    {
        // GET: CartManagerment
        QuanLiVanBanEntities db_ = new QuanLiVanBanEntities();
        public ActionResult Index()
        {
          
            return View();
        }
        public async Task<ActionResult> GetList(string NameRecycleSearch,string DatetimeCreateStartSearch,string DatetimeCreateEndSearch)
        {
            DateTime _DateStart = new DateTime();
            if (string.IsNullOrEmpty(DatetimeCreateStartSearch) )
            {
                if (!DateTime.TryParseExact(DatetimeCreateStartSearch.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DateStart))
                {
                    return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            DateTime _DateEnd = new DateTime();
            if (string.IsNullOrEmpty(DatetimeCreateStartSearch))
            {
                if (!DateTime.TryParseExact(DatetimeCreateEndSearch.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DateEnd))
                {
                    return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            var _RecycleBins = db_.RecycleBins.AsNoTracking();
            var _Users = db_.Users.AsNoTracking();
            var data = await(from a in _RecycleBins
                             join b in _Users on a.UserCreate equals b.Id
                        join c in _Users on a.UserUpdate equals c.Id
                        where a.isDelete == false
                        where a.NameRecycle.ToLower().Contains(NameRecycleSearch.ToLower()) || string.IsNullOrEmpty(NameRecycleSearch)
                        where _DateEnd >= a.DatetimeCreate && _DateStart<=a.DatetimeCreate
                        select new RecycleBinModels
                        {
                            Id = a.Id,
                            NameRecycle = a.NameRecycle,
                            DescriptionRecycle = a.DescriptionRecycle,
                            TableName = a.TableName,
                            IdParent = a.IdParent,
                            NameUserCreate = b.HoNV+" "+b.TenNV,
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
            ViewBag.RecycleBins = data;
            return PartialView();
        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _DeleteFun(int id)
        {
         
            try
            {
                var data = db_.RecycleBins.Find(id);
                if(data != null)
                {
                    data.DatetimeDelete = DateTime.Now;
                    data.UserDelete = User.UserId;
                    data.isDelete = true;
                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();                    
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