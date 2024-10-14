using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    public class InternalDocumentController : BaseController
    {
        // GET: CartManagerment
        QuanLiVanBanEntities db_ = new QuanLiVanBanEntities();
        AppGlobal appglobal_ = new AppGlobal();
        public ActionResult Index()
        {

            return View();
        }
        public async Task<ActionResult> GetList()
        {
            var _Departments = db_.Departments.AsNoTracking();
            var _Deliverys = db_.Deliverys.AsNoTracking();
            var _Categories = db_.Categories.AsNoTracking();
            var _Documents = db_.Documents.AsNoTracking();
            var _Files = db_.Files.AsNoTracking();
            var _Users = db_.Users.AsNoTracking();
            var _Configs = db_.Configs.AsNoTracking();

            var ShippingAddress = await _Departments.Where(a => a.isDelete == false && a.ParentConFig == null).ToListAsync();
            
            var DataDocument = await (from a in _Documents
                                      join c in _Users on a.UserCreate equals c.Id
                                      join d in _Users on a.UserUpdate equals d.Id
                                      where a.isDelete == false
                                      select new
                                      {
                                          a.Id,
                                          a.TitleDocument,
                                          a.CodeDocument,
                                          a.DatePublish,
                                          a.AbstractDocument,
                                          a.Status,
                                          a.UserCreate,
                                          NameUserCreate = c.HoNV + c.TenNV,
                                          a.DatetimeCreate,
                                          a.UserUpdate,
                                          NameUserUpdate = d.HoNV + d.TenNV,
                                          a.DatetimeUpdate
                                      }).ToListAsync();

            var DataDeliverys = await (from a in _Deliverys
                                       join d in _Categories on a.ParentId equals d.Id
                                       join b in _Users on a.UserCreate equals b.Id
                                       join c in _Users on a.UserUpdate equals c.Id
                                       where a.isDelete == false && d.Id == appglobal_.ID_TEXT_INTERNAL
                                       select new
                                       {
                                           a.Id,
                                           a.IdDocument,
                                           a.ShippingAddressId,
                                           a.RecipientAddressId,
                                           a.Status,
                                           a.UserCreate,
                                           NameUserCreate = b.HoNV + b.TenNV,
                                           a.DatetimeCreate,
                                           a.UserUpdate,
                                           NameUserUpdate = c.HoNV + c.TenNV,
                                           a.DatetimeUpdate
                                       }).ToListAsync();
            var result = (from a in DataDeliverys
                          join b in DataDocument on a.IdDocument equals b.Id
                          select new OutgoingDocumentModels
                          {
                              CodeDocument = b.CodeDocument,
                              IdDeliverys = a.Id,
                              IdDocument = b.Id,
                              TitleDocument = b.TitleDocument,
                              NameRecipientAddress = ShippingAddress.Where(z => z.Id == a.RecipientAddressId)
                                                             .Select(z => z.NameDepartment)
                                                             .FirstOrDefault() ?? "",
                              AbstractDocument = b.AbstractDocument.Replace("\"","'"),
                              DatePublish = b.DatePublish,
                              files = _Files.Where(f => f.DoccumentId == b.Id).ToList(),
                              DepartmentName = ShippingAddress.Where(z => z.DescriptionDepartment == (b.CodeDocument.Split('-').Length >= 2 ? b.CodeDocument.Split('-')[2] : "") && z.isDelete == false)
                                                             .Select(z => z.NameDepartment)
                                                             .FirstOrDefault() ?? "",

                              DocumentUserCreate = b.UserCreate,
                              DocumentUserUpdate = b.UserUpdate,
                              DocumentDatetimeCreate = b.DatetimeCreate,
                              DocumentDatetimeUpdate = b.DatetimeUpdate,
                              DocumentNameUserCreate = b.NameUserCreate,
                              DocumentNameUserUpdate = b.NameUserUpdate,
                              UserCreate = a.UserCreate,
                              UserUpdate = a.UserUpdate,
                              DatetimeCreate = a.DatetimeCreate,
                              DatetimeUpdate = a.DatetimeUpdate,
                              NameUserCreate = a.NameUserCreate,
                              NameUserUpdate = a.NameUserUpdate,
                              Status = a.Status,
                          }).OrderBy(a => a.IdDocument).ThenByDescending(a => a.DatetimeCreate).ToList();

            ViewBag.result = result;
            return PartialView();
        }
        public async Task<ActionResult> Edit(int Id)
        {
            ViewBag.Id = Id;
            var _Deliverys = db_.Deliverys.AsNoTracking();
            var data = _Deliverys.Where(z => z.Id == Id).FirstOrDefault();
            if (data != null)
            {
                int idDocument = (int)data.IdDocument;
                var LstDeliverys = _Deliverys.Where(a => a.IdDocument == idDocument && a.GuiId == data.GuiId).Select(a => a.Id).ToList();
                var result = (from a in db_.Departments
                              where !LstDeliverys.Contains(a.Id) && a.isDelete == false && a.ParentConFig== null && (User.Role == "User" && a.UserCreate == User.UserId) || User.Role == "Admin"
                              select new ConfigModel
                              {
                                  Name = a.NameDepartment,
                                  Id = a.Id
                              }).ToList();

                ViewBag.filteredShippingAddress = result;
            }
            var _Documents = await db_.Documents.AsNoTracking().Where(a => a.isDelete == false && ((User.Role == "User" && a.UserCreate == User.UserId) || User.Role == "Admin")).OrderByDescending(a => a.DatetimeCreate).ToListAsync();
            ViewBag._Documents = _Documents;
            ViewBag.data = data;
            return PartialView();
        }
        [ValidateInput(false)]
        [HttpPost]
        public async Task<JsonResult> _CopyItem(int Id)
        {

            try
            {
                if (Id == 0)
                {
                    return Json(new { status = -1, title = "", text = "Không tìm thấy data", obj = "" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var dataCopy = await db_.Deliverys.Where(a => a.Id == Id && a.isDelete == false).FirstOrDefaultAsync();
                    if (dataCopy != null)
                    {
                        var model = new Delivery();
                        model.IdDocument = dataCopy.IdDocument;
                        model.ParentId = appglobal_.ID_TEXT_INGOING;
                        model.GuiId = dataCopy.GuiId;
                        model.ShippingAddressId = dataCopy.ShippingAddressId;
                        model.Status = true;
                        model.UserCreate = User.UserId;
                        model.DatetimeCreate = DateTime.Now;
                        model.UserUpdate = User.UserId;
                        model.DatetimeUpdate = DateTime.Now;
                        model.UserDelete = User.UserId;
                        model.DatetimeDelete = DateTime.Now;
                        model.isDelete = false;
                        db_.Deliverys.Add(model);
                        db_.SaveChanges();
                        return Json(new { status = 1, title = "", text = "Copy thành công", obj = "" }, JsonRequestBehavior.AllowGet);

                    }
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
        public JsonResult _DeleteFun(int id)
        {

            try
            {
                var data = db_.Deliverys.Find(id);
                if (data != null)
                {
                    db_.Deliverys.Remove(data);
                    db_.SaveChanges();
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
        public async Task<ActionResult> Insert()
        {
            var _Departments = db_.Departments.AsNoTracking();
            var _Configs = db_.Configs.AsNoTracking();
            var RecipientAddress = await (from a in _Departments where a.isDelete == false && a.ParentConFig == null
                                          select new ConfigModel
                                          {
                                              Id = a.Id,
                                              Name = a.NameDepartment,
                                              Description = a.DescriptionDepartment,
                                          }).ToListAsync();
            var _DocumentsData = await db_.Documents.AsNoTracking()
                            .Where(a => a.isDelete == false 
                            && ((User.Role == "User" && a.UserCreate == User.UserId) || User.Role == "Admin"))
                            
                            .OrderByDescending(a => a.DatetimeCreate).ToListAsync();
            var tmp = RecipientAddress.Select(z => z.Description).ToList();
            var _Documents = _DocumentsData.Where(a =>
            {
                var parts = a.CodeDocument.Split('-');
                if (parts.Length > 2)
                {
                    return tmp.Contains(parts[2]);
                }
                return false;
            }).ToList();

            ViewBag._Documents = _Documents;
            ViewBag.RecipientAddress = RecipientAddress;

            return PartialView();
        }
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _InsertFun(int IdDocument, int RecipientAddressId)
        {

            try
            {
                if (RecipientAddressId > 0 && IdDocument > 0)
                {
                    var data = new Delivery()
                    {
                        ParentId = appglobal_.ID_TEXT_INTERNAL,
                        IdDocument = IdDocument,
                        RecipientAddressId = RecipientAddressId,
                        Status = true,
                        UserCreate = User.UserId,
                        DatetimeCreate = DateTime.Now,
                        UserUpdate = User.UserId,
                        DatetimeUpdate = DateTime.Now,
                        UserDelete = User.UserId,
                        DatetimeDelete = DateTime.Now,
                        isDelete = false,
                    };
                    db_.Deliverys.Add(data);
                    db_.SaveChanges();
                    return Json(new { status = 1, title = "", text = "Thêm dữ liệu thành công", obj = "" }, JsonRequestBehavior.AllowGet);
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
        public JsonResult _EditFun(int id, int IdDocument, int RecipientAddressId)
        {

            try
            {
                var data = db_.Deliverys.Where(a => a.Id == id).FirstOrDefault();
                if (RecipientAddressId > 0 && IdDocument > 0 && data != null)
                {
                    data.IdDocument = IdDocument;
                    data.RecipientAddressId = RecipientAddressId;
                    data.Status = true;
                    data.UserUpdate = User.UserId;
                    data.DatetimeUpdate = DateTime.Now;
                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();
                    return Json(new { status = 1, title = "", text = "Thêm dữ liệu thành công", obj = "" }, JsonRequestBehavior.AllowGet);
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