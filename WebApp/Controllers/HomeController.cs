using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToolsApp.App_Start;
using ToolsApp.Authentication;
using ToolsApp.EntityFramework;

namespace ToolsApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        QuanLiVanBanEntities db_ = new QuanLiVanBanEntities();
        // GET: Home
        public ActionResult Index(string Id = "")
        {
            return View();
        }
        public JsonResult LoadChart(int selectYear =2024)
        {
            var data = db_.sp_loadChart(selectYear).ToList(); 
            return Json(new { status = 1, title = "", text = "Tải dữ liệu thành công", obj = data }, JsonRequestBehavior.AllowGet);
        }
        

    }
}