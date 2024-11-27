using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
    public class DocumentManagementController : BaseController
    {
        // GET: CartManagerment
        QuanLiVanBanEntities db_ = new QuanLiVanBanEntities();
        AppGlobal _AppGlobal = new AppGlobal();
        public ActionResult Index()
        {

            return View();
        }
        public async Task<ActionResult> GetList(string TitleDocumentSearch, string CodeDocumentSearch)
        {
            var _Documents = db_.Documents.AsNoTracking();
            var _Users = db_.Users.AsNoTracking();
            var _Departments = db_.Departments.AsNoTracking();
            var _file = db_.Files.AsNoTracking();
            var _Categories = db_.Categories.AsNoTracking();
            var data = await (from a in _Documents
                              join b in _Users on a.UserCreate equals b.Id
                              join c in _Users on a.UserUpdate equals c.Id
                              where a.isDelete == false && a.IsDoc == true
                              where a.TitleDocument.ToLower().Contains(TitleDocumentSearch.ToLower()) || string.IsNullOrEmpty(TitleDocumentSearch)
                              && a.CodeDocument.ToLower().Contains(CodeDocumentSearch.ToLower()) || string.IsNullOrEmpty(CodeDocumentSearch)
                              select new DocumentModels
                              {
                                  Id = a.Id,
                                  CodeDocument = a.CodeDocument,
                                  TitleDocument = a.TitleDocument,
                                  AbstractDocument = a.AbstractDocument,
                                  CountFile = _file.Where(h=>h.DoccumentId ==a.Id ).Count(),
                                  UrlFile = a.UrlFile,
                                  DatePublish = a.DatePublish,
                                  NameUserCreate = b.HoNV + " " + b.TenNV,
                                  NameUserUpdate = c.HoNV + " " + c.TenNV,
                                  Status = a.Status,
                                  DatetimeCreate = a.DatetimeCreate,
                                  UserCreate = a.UserCreate,
                                  DepartmentName = _Departments.Where(z => z.Id == b.PhongBan).FirstOrDefault().NameDepartment,
                                  DatetimeUpdate = a.DatetimeUpdate,
                                  DatetimeDelete = a.DatetimeDelete,
                                  UserDelete = a.UserDelete,
                                  UserUpdate = a.UserUpdate,
                                  isDelete = a.isDelete,
                              }).ToListAsync();
            ViewBag.KetCaus = data;
            return PartialView();
        }
        public async Task<ActionResult> Insert()
        {
            var dataDepartments = (from a in db_.Departments
                          join b in db_.Configs on a.ParentConFig equals b.Id into joined
                          from b in joined.DefaultIfEmpty()
                          where a.isDelete == false 
                          select new ConfigModel
                          {
                              Name = b != null ? b.NameConfig + " - " + a.NameDepartment : a.NameDepartment,
                              Id = a.Id
                          }).ToList();
            ViewBag.dataDepartments = dataDepartments;
            return View();
        }
        public async Task<ActionResult> Update(int id)
        {
            var data = await db_.Documents.Where(a => a.isDelete == false && a.Id == id).FirstOrDefaultAsync();
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
        public ActionResult Insert_File()
        {
            return PartialView();
        }
        public ActionResult View_Fodels()
        {
            var _Folders = db_.sp_LoadNameFodelChild().ToList();
            if(User.Role == "User")
            {
                _Folders = _Folders.Where(a => a.DepartmentId == User.PhongBan).ToList();
            }    
            ViewBag._Folders = _Folders;
            return PartialView();
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _DeleteFun(int id)
        {

            try
            {
                var data = db_.Documents.Find(id);
                if (data != null)
                {
                    data.DatetimeDelete = DateTime.Now;
                    data.UserDelete = User.UserId;
                    data.isDelete = true;
                    db_.Entry(data).State = EntityState.Modified;
                    db_.SaveChanges();
                    _AppGlobal.AddRecycleBin(data.Id, "Xóa data chương trình Quản lý Tài liệu", "", "Document", User.UserId);
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
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult _DeleteFileFull(int id)
        {

            try
            {
                var data = db_.Files.Find(id);
                if (data != null)
                {
                    var uploadsPath = Server.MapPath(data.UrlFile);
                    if (System.IO.File.Exists(uploadsPath))
                    {
                        System.IO.File.Delete(uploadsPath);
                    }
                    db_.Files.Remove(data);
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
        [ValidateInput(false)]
        [HttpPost]
        public async Task<JsonResult> _InsertFun(InsertDocument_TaiLieu_Models model)
        {

            try
            {
                if (model.files.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(model.TitleDocument))
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
                                        string codeNew = _datenow.Day.ToString() + _datenow.Month.ToString() + _datenow.Year.ToString() + "-" + _AppGlobal.GenerateRandomString(5).ToUpper() + "-";
                                        var dataDepartment = await db_.Departments.Where(a => a.Id == User.PhongBan).FirstOrDefaultAsync();
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
                                            IsDoc = true,
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
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }

        }
        [ValidateInput(false)]
        [HttpPost]
        public async Task<JsonResult> _UpdateFun(UpdateDocument_TaiLieu_Models model)
        {

            try
            {

                if (!string.IsNullOrEmpty(model.TitleDocument))
                {
                    if (!string.IsNullOrEmpty(model.AbstractDocument))
                    {
                        //if (model.DatePublish != null)
                        if (!string.IsNullOrEmpty(model.DatePublish))
                        {
                            if (model.FolderId > 0)
                            {

                                DateTime _DatePublish = new DateTime();
                                if (!DateTime.TryParseExact(model.DatePublish.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _DatePublish))
                                {
                                    return Json(new { status = -1, title = "", text = "Không thể chuyển đổi kiểu dữ liệu", obj = "" }, JsonRequestBehavior.AllowGet);
                                }

                                DateTime _datenow = DateTime.Now;

                                var dataDoc = await db_.Documents.Where(a => a.Id == model.Id).FirstOrDefaultAsync();
                                if (dataDoc != null)
                                {

                                    dataDoc.TitleDocument = model.TitleDocument;
                                    dataDoc.AbstractDocument = model.AbstractDocument;
                                    dataDoc.DatePublish = _DatePublish;
                                    dataDoc.UserUpdate = User.UserId;
                                    dataDoc.DatetimeUpdate = DateTime.Now;
                                    db_.Entry(dataDoc).State = EntityState.Modified;
                                    await db_.SaveChangesAsync();
                                };
                                if (model.files != null)
                                {

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
                                }
                                return Json(new { status = 1, title = "", text = "Cập nhật dữ liệu thành công", obj = "" }, JsonRequestBehavior.AllowGet);
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
                        return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu Abstract", obj = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = -1, title = "", text = "Không tìm thấy dữ liệu title", obj = "" }, JsonRequestBehavior.AllowGet);
                }
              

            }
            catch (Exception ex)
            {
                return Json(new { status = -1, title = "", text = ex.Message, obj = "" }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult DownloadFiles(int id)
        {
            var document = db_.Documents.FirstOrDefault(a => a.Id == id && a.isDelete == false);
            var hinhAnhList = db_.Files
               .Where(hbv => hbv.DoccumentId == document.Id && hbv.Status == true && hbv.isDelete == false)
               .Select(hbv => new { hbv.Id, hbv.UrlFile })
               .ToList();

            if (hinhAnhList == null || !hinhAnhList.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy file." });
            }

            var urlPaths = hinhAnhList.Select(hbv => new
            {
                Id = hbv.Id,
                UrlPath = hbv.UrlFile.Replace("~", "https://localhost:44338")
            }).ToList();
            var memoryStream = new MemoryStream();

            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                using (var webClient = new WebClient())
                {
                    foreach (var image in urlPaths)
                    {
                        try
                        {
                            var fileBytes = webClient.DownloadData(image.UrlPath);
                            var zipEntry = zip.CreateEntry(Path.GetFileName(image.UrlPath), CompressionLevel.Fastest);

                            using (var zipStream = zipEntry.Open())
                            {
                                zipStream.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", $"Images_{id}.zip");
        }
        public ActionResult ViewFile(int Id)
        {
            var dataFile  = db_.Files.Where(a=>a.DoccumentId == Id).ToList();
            var urlPaths = dataFile.Select(hbv => new EntityFramework.File
            {
                Id = hbv.Id,
                FileName = hbv.FileName,
                FileExtension = hbv.FileExtension,
                ParentId = hbv.ParentId,
                DoccumentId = hbv.DoccumentId,
                UrlFile = hbv.UrlFile.Replace("~", "https://localhost:44338")
            }).ToList();
            ViewBag.dataFile = urlPaths;
            ViewBag.Id = Id;
            return PartialView();
        }

    }
}