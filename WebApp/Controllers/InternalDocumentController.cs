using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
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
        public async Task<ActionResult> GetList(string NameTitleSearch, string DatetimeCreateStartSearch, string DatetimeCreateEndSearch)
        {
            DateTime _DateStart = new DateTime();
            if (!string.IsNullOrEmpty(DatetimeCreateStartSearch))
            {
                if (!DateTime.TryParseExact(DatetimeCreateStartSearch.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DateStart))
                {
                    return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                }
                _DateStart = new DateTime(_DateStart.Year, _DateStart.Month, _DateStart.Day, 00, 00, 01);
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
            if (_DateEnd < _DateStart)
            {
                return Json(new { status = -1, title = "", text = "Ngày ", obj = "" }, JsonRequestBehavior.AllowGet);
            }
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
                                      where a.isDelete == false && a.IsDoc == false
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
                                       where a.isDelete == false && d.Id == appglobal_.ID_TEXT_INTERNAL &&
                                       (a.UserAuthorization.StartsWith(User.UserId.ToString() + ",") || a.UserAuthorization.EndsWith("," + User.UserId.ToString()) || a.UserAuthorization.Contains("," + User.UserId.ToString() + ","))
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
                          }).Where(a => (a.TitleDocument.ToLower().Contains(NameTitleSearch.ToLower())&&string.IsNullOrEmpty(NameTitleSearch)) && a.DatePublish <= _DateEnd && a.DatePublish >= _DateStart)
                          .OrderBy(a => a.IdDocument).ThenByDescending(a => a.DatetimeCreate).ToList();

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
            var _Documents = await db_.Documents.AsNoTracking().Where(a => a.isDelete == false && a.IsDoc == false && ((User.Role == "User" && a.UserCreate == User.UserId) || User.Role == "Admin")).OrderByDescending(a => a.DatetimeCreate).ToListAsync();
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
                    data.isDelete = true;
                    data.UserDelete = User.UserId;
                    data.DatetimeDelete = DateTime.Now;
                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();
                    AppGlobal appGlobal = new AppGlobal();
                    appGlobal.AddRecycleBin(data.Id, "Xóa data chương trình quản lý văn bản đi", "", "Delivery", User.UserId);
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
        public ActionResult Insert_File()
        {
            return PartialView();
        }
        public ActionResult View_Fodels()
        {
            var _Folders = db_.sp_LoadNameFodelChild().ToList();
            if (User.Role == "User")
            {
                _Folders = _Folders.Where(a => a.DepartmentId == User.PhongBan).ToList();
            }
            ViewBag._Folders = _Folders;
            return PartialView();
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
                            .Where(a => a.isDelete == false && a.IsDoc == false
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
            var dataDepartments = (from a in db_.Departments
                                   
                                   where a.isDelete == false && a.ParentConFig == null
                                   select new ConfigModel
                                   {
                                       Name = a.NameDepartment,
                                       Id = a.Id
                                   }).ToList();
            ViewBag.dataDepartments = dataDepartments;
            ViewBag._Documents = _Documents;
            ViewBag.RecipientAddress = RecipientAddress;

            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public async Task<JsonResult> _InsertFun(InsertDeliverDocumentModels model)
        {

            try
            {
                if (model.RecipientAddressId > 0)
                {
                    if (model.files.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(model.TitleDocument))
                        {
                            if (model.IdDepartment > 0)
                            {
                                if (!string.IsNullOrEmpty(model.AbstractDocument))
                                {
                                    //if (model.DatePublish != null)
                                    if (!string.IsNullOrEmpty(model.DatePublish))
                                    {
                                        if (model.FolderId > 0)
                                        {

                                            DateTime _DatePublish = new DateTime();

                                            if (!DateTime.TryParseExact(model.DatePublish, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DatePublish))
                                            {
                                                return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                                            }
                                            DateTime _datenow = DateTime.Now;
                                            string codeNew = _datenow.Day.ToString() + _datenow.Month.ToString() + _datenow.Year.ToString() + "-" + appglobal_.GenerateRandomString(5).ToUpper() + "-";
                                            var dataDepartment = await db_.Departments.Where(a => a.Id == model.IdDepartment).FirstOrDefaultAsync();
                                            if (dataDepartment != null)
                                            {
                                                codeNew += dataDepartment.DescriptionDepartment ?? "";
                                            }
                                            var dataDoc = new Document()
                                            {
                                                TitleDocument = model.TitleDocument,
                                                AbstractDocument = model.AbstractDocument,
                                                DatePublish = _DatePublish,
                                                CodeDocument = codeNew,
                                                FolderId = model.FolderId,
                                                Status = true,
                                                isDelete = false,
                                                UserCreate = User.UserId,
                                                UserUpdate = User.UserId,
                                                UserDelete = User.UserId,
                                                DatetimeCreate = DateTime.Now,
                                                DatetimeUpdate = DateTime.Now,
                                                DatetimeDelete = DateTime.Now,
                                                IsDoc = false
                                            };
                                            db_.Documents.Add(dataDoc);
                                            await db_.SaveChangesAsync();
                                            foreach (var file in model.files)
                                            {
                                                var uploadsPath = Server.MapPath("~/Uploads");
                                                string uniqueFileName = Guid.NewGuid().ToString();
                                                string fileExtension = Path.GetExtension(file.FileName);
                                                string fileName = uniqueFileName + fileExtension;
                                                var filePath = Path.Combine(uploadsPath, fileName);
                                                if (!Directory.Exists(uploadsPath))
                                                {
                                                    Directory.CreateDirectory(uploadsPath);
                                                }
                                                file.SaveAs(filePath);
                                                var relativePath = $"~/Uploads/{fileName}";
                                                var dataFile = new EntityFramework.File()
                                                {
                                                    ParentId = model.FolderId,
                                                    DoccumentId = dataDoc.Id,
                                                    FileName = file.FileName,
                                                    FileExtension = fileExtension,
                                                    UrlFile = relativePath,
                                                    Status = true,
                                                    isDelete = false,
                                                    UserCreate = User.UserId,
                                                    UserUpdate = User.UserId,
                                                    UserDelete = User.UserId,
                                                    DatetimeCreate = DateTime.Now,
                                                    DatetimeUpdate = DateTime.Now,
                                                    DatetimeDelete = DateTime.Now,
                                                };
                                                db_.Files.Add(dataFile);
                                                await db_.SaveChangesAsync();
                                            }
                                            List<int> resultUser = new List<int>();
                                            string value = "";
                                            if (model.AuthenticateDepartment.Count() > 0)
                                            {
                                                foreach (var department in model.AuthenticateDepartment)
                                                {
                                                    var dataTmp = department.Split(',');
                                                    if (dataTmp.Contains("-99"))
                                                    {
                                                        var _mainDerpart = await db_.Departments.Where(a => a.ParentConFig == null && a.isDelete == false)
                                                                                                  .Select(d => d.Id)
                                                                                                  .ToListAsync();

                                                        resultUser = await db_.Users
                                                                               .Where(u => _mainDerpart.Contains((int)u.PhongBan))
                                                                               .Select(u => u.Id)
                                                                               .ToListAsync();
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        foreach (var item in dataTmp)
                                                        {
                                                            int itemId;
                                                            if (int.TryParse(item, out itemId))
                                                            {
                                                                var _mainDerpart = await db_.Departments
                                                                    .Where(a => a.ParentConFig == null && a.isDelete == false && a.Id == itemId)
                                                                    .FirstOrDefaultAsync();

                                                                if (_mainDerpart != null)
                                                                {
                                                                    resultUser.AddRange(await db_.Users
                                                                        .Where(u => u.PhongBan == _mainDerpart.Id)
                                                                        .Select(u => u.Id)
                                                                        .ToListAsync());
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            model.AuthenticateStaff.Add(User.UserId.ToString());
                                            if (model.AuthenticateStaff.Count() > 0)
                                            {
                                                foreach (var user in model.AuthenticateStaff)
                                                {
                                                    var dataTmp = user.Split(',');
                                                    if (dataTmp.Count() > 0)
                                                    {
                                                        if (dataTmp.Contains("-99"))
                                                        {

                                                            resultUser = await db_.Users
                                                                                   .Where(u => u.isDelete == false)
                                                                                   .Select(u => u.Id)
                                                                                   .ToListAsync();
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            foreach (var item in dataTmp)
                                                            {
                                                                int itemId = Convert.ToInt32(item);
                                                                var rerult = resultUser.Where(a => a == itemId).FirstOrDefault();

                                                                if (rerult != null)
                                                                {
                                                                    continue;
                                                                }
                                                                else
                                                                {
                                                                    resultUser.Add(itemId);
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                            var data = new Delivery()
                                            {
                                                ParentId = appglobal_.ID_TEXT_INTERNAL,
                                                IdDocument = dataDoc.Id,
                                                RecipientAddressId = model.RecipientAddressId,
                                                Status = true,
                                                UserCreate = User.UserId,
                                                DatetimeCreate = DateTime.Now,
                                                UserUpdate = User.UserId,
                                                DatetimeUpdate = DateTime.Now,
                                                UserDelete = User.UserId,
                                                DatetimeDelete = DateTime.Now,
                                                isDelete = false,
                                                UserAuthorization = string.Join(",", resultUser.Select(a => a.ToString()))
                                            };
                                            db_.Deliverys.Add(data);
                                            db_.SaveChanges();
                                            return Json(new { status = 1, title = "", text = "Thêm dữ liệu thành công", obj = "" }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu foldel lưu trữ", obj = "" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu DatePublish", obj = "" }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu trích yếu", obj = "" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu phòng ban soạn thảo", obj = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu title", obj = "" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = 1, title = "", text = "Không tìm thấy dữ liệu file", obj = "" }, JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> Update(int id)
        {
            var data = await db_.Documents.Where(a => a.isDelete == false && a.IsDoc == false && a.Id == id).FirstOrDefaultAsync();
            ViewBag.data = data;
            var file = await db_.Files.Where(a => a.isDelete == false && a.DoccumentId == id).ToListAsync();
            ViewBag.file = file;
            var dataDepartments = (from a in db_.Departments
                                   join b in db_.Configs on a.ParentConFig equals b.Id into joined
                                   from b in joined.DefaultIfEmpty()
                                   where a.isDelete == false
                                   select new ConfigModel
                                   {
                                       Name = b != null ? b.NameConfig + " - " + a.NameDepartment : a.NameDepartment,
                                       Description = a.DescriptionDepartment,
                                       Id = a.Id
                                   }).ToList();
            ViewBag.dataDepartments = dataDepartments;
            var _Folders = db_.sp_LoadNameFodelChild().ToList();
            if (User.Role == "User")
            {
                _Folders = _Folders.Where(a => a.DepartmentId == User.PhongBan).ToList();
            }
            ViewBag._Folders = _Folders;
            return View();
        }
    }
}