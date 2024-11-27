using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        public async Task<ActionResult> GetList(string NameRecycleSearch, string DatetimeCreateStartSearch, string DatetimeCreateEndSearch)
        {
            DateTime _DateStart = new DateTime();
            if (!string.IsNullOrEmpty(DatetimeCreateStartSearch))
            {
                if (!DateTime.TryParseExact(DatetimeCreateStartSearch.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DateStart))
                {
                    return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                _DateStart = new DateTime(_DateStart.Year, _DateStart.Month, _DateStart.Day,  00, 00, 01);
            }
            DateTime _DateEnd = new DateTime();
            if (!string.IsNullOrEmpty(DatetimeCreateStartSearch))
            {
                if (!DateTime.TryParseExact(DatetimeCreateEndSearch.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DateEnd))
                {
                    return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                _DateEnd = new DateTime(_DateEnd.Year, _DateEnd.Month, _DateEnd.Day, 23, 59, 59);
            }
            var _RecycleBins = db_.RecycleBins.AsNoTracking();
            var _Users = db_.Users.AsNoTracking();
            var data = await (from a in _RecycleBins
                              join b in _Users on a.UserCreate equals b.Id
                              join c in _Users on a.UserUpdate equals c.Id
                              where a.isDelete == false
                              where (a.NameRecycle.ToLower().Contains(NameRecycleSearch.ToLower()) || string.IsNullOrEmpty(NameRecycleSearch))
                              && a.DatetimeCreate <= _DateEnd && a.DatetimeCreate >= _DateStart
                              select new RecycleBinModels
                              {
                                  Id = a.Id,
                                  NameRecycle = a.NameRecycle,
                                  DescriptionRecycle = a.DescriptionRecycle,
                                  TableName = a.TableName,
                                  IdParent = a.IdParent,
                                  NameUserCreate = b.HoNV + " " + b.TenNV,
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
        public async Task<JsonResult> _DeleteFun(int Id)
        {

            try
            {
                var data = db_.RecycleBins.Find(Id);
                if (data != null)
                {
                    db_.RecycleBins.Remove(data);
                    await db_.SaveChangesAsync();
                    return Json(new { status = 1, title = "", text = "Xóa thành công", obj = "" }, JsonRequestBehavior.AllowGet);
                        #region
                    ////var documentType = Type.GetType($"ToolsApp.EntityFramework.{data.TableName}, ToolsApp.EntityFramework");
                    //if (documentType != null)
                    //{
                    //    //var method = db_.Set(documentType).Find(data.IdParent);
                    //    //switch (data.TableName)
                    //    //{
                    //    //    case "Document":
                    //    //        var documentId = (int)method.GetType().GetProperty("Id").GetValue(method);
                    //    //        var delive = db_.Deliverys.Where(a=>a.IdDocument == documentId).ToList();
                    //    //        if (delive.Count() > 0)
                    //    //        {
                    //    //            return Json(new { status = -1, title = "", text = "Đã được lưu hành không thể xóa", obj = "" }, JsonRequestBehavior.AllowGet);
                    //    //        }
                    //    //        else
                    //    //        {
                    //    //            var files = db_.Files.Where(a => a.DoccumentId == documentId).ToList();
                    //    //            if (files.Count() > 0)
                    //    //            {
                    //    //                foreach (var item in files)
                    //    //                {
                    //    //                    var uploadsPath = Server.MapPath(item.UrlFile);
                    //    //                    if (System.IO.File.Exists(uploadsPath))
                    //    //                    {
                    //    //                        System.IO.File.Delete(uploadsPath);
                    //    //                    }
                    //    //                    db_.Files.Remove(item);
                    //    //                }
                    //    //                db_.SaveChanges();
                    //    //            }
                    //    //        }
                    //    //        break;

                    //    //    case "Category":

                    //    //        break;

                    //    //    case "Menu":

                    //    //        break;
                    //    //    case "Page":

                    //    //        break;
                    //    //    case "User":

                    //    //        break;
                    //    //    case "Folder":

                    //    //        break;
                    //    //    case "Config":

                    //    //        break;
                    //    //    case "Delivery":

                    //    //        break;
                    //    //    case "User_Page":

                    //    //        break;

                    //    //    default:
                    //    //        throw new ArgumentException("Unknown table name");
                    //    //}
                    //    //db_.SaveChanges();

                    //    if (method != null)
                    //    {
                    //        //db_.Set(documentType).Remove(method);
                    //        //db_.RecycleBins.Remove(data);
                    //        //db_.SaveChanges();
                    //    }
                    //    return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu khôi phục hoặc dữ liệu khôi phục đã được xóa", obj = "" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { status = -1, title = "", text = "Không tìm thấy table", obj = "" }, JsonRequestBehavior.AllowGet);

                    //}
                    #endregion
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
        public JsonResult _RollBack(int Id)
        {

            try
            {
                var data = db_.RecycleBins.Find(Id);
                if (data != null)
                {
                    string tableName = data.TableName;
                    int parentId = data.IdParent ?? 0;

                    var documentType = Type.GetType($"ToolsApp.EntityFramework.{tableName}, ToolsApp.EntityFramework");
                    if (documentType != null)
                    {
                        var method = db_.Set(documentType).Find(parentId);

                        if (method != null)
                        {
                            var userIdProperty = documentType.GetProperty("UserUpdate");
                            var datetimeUpdateProperty = documentType.GetProperty("DatetimeUpdate");
                            var isDeleteProperty = documentType.GetProperty("isDelete");

                            if (userIdProperty != null && datetimeUpdateProperty != null && isDeleteProperty != null)
                            {
                                userIdProperty.SetValue(method, User.UserId);
                                datetimeUpdateProperty.SetValue(method, DateTime.Now);
                                isDeleteProperty.SetValue(method, false);
                                db_.RecycleBins.Remove(data);
                                db_.SaveChanges();

                                return Json(new { status = 1, title = "", text = "Khôi phục thành công", obj = "" }, JsonRequestBehavior.AllowGet);
                            }

                            return Json(new { status = -1, title = "", text = "Không thể cập nhật dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu khôi phục hoặc dữ liệu khôi phục đã được xóa", obj = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = -1, title = "", text = "Không tìm thấy table", obj = "" }, JsonRequestBehavior.AllowGet);

                    }


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

    }
}