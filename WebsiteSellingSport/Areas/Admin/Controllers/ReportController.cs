using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using WebsiteSellingSport.Repostitory;

namespace WebsiteSellingSport.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        ReportRepository _reportRepo;
        public ReportController(ReportRepository reportRepository)
        {
            _reportRepo = reportRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetReportData(DateTime dateStart, DateTime dateEnd)
        {
            DateTime dateStartnew = dateStart.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime dateEndnew = dateEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // Đặt giờ thành 23:59:59
            var reports = _reportRepo.GetFilterReportView(dateStartnew, dateEndnew);

            return Json(reports);
        }

    }
}
