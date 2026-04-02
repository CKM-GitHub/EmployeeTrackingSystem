using EmployeeTrackingSystem.Controllers;
using EmployeeTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTrackingSystem.Controllers
{
    public class DetailController : BaseController
    {
        private Models.EmployeeTrackingDBEntities db = new EmployeeTrackingDBEntities();
        // GET: Detail
        public ActionResult DetailList()
        {
            ViewBag.ShowDropdown = true;
            LoadDropdowns();
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var deptParam = new SqlParameter("@Department", (object)DBNull.Value);
            var staffParam = new SqlParameter("@StaffName", (object)DBNull.Value);
            var frmDate = new SqlParameter("@FrmDate", startDate.ToString());
            var toDate = new SqlParameter("@ToDate", endDate.ToString());

            List<DetailViewModel> list = db.Database.SqlQuery<DetailViewModel>(
             "EXEC DetailLoggingSelect @Department, @StaffName, @FrmDate, @ToDate",
             deptParam, staffParam, frmDate, toDate
         ).ToList();

            ViewBag.Detail = "Detail";
            ViewBag.FromDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = endDate.ToString("yyyy-MM-dd");
            return View(list);
        }

        [HttpPost]
        public JsonResult GetFilteredData(string department, string staffname, string fromdate, string todate)
        {
            try
            {
                department = department == "All" ? null : department;
                staffname = staffname == "All" ? null : staffname;
                fromdate = fromdate == "" ? null : fromdate;
                todate = todate == "" ? null : todate;
                // Create SQL parameters to match your Stored Procedure
                var deptParam = new SqlParameter("@Department", department ?? (object)DBNull.Value);
                var staffParam = new SqlParameter("@StaffName", staffname ?? (object)DBNull.Value);
                var frmDate = new SqlParameter("@FrmDate", fromdate ?? (object)DBNull.Value);
                var toDate = new SqlParameter("@ToDate", todate ?? (object)DBNull.Value);

                // Execute the SP and map results to your Model
                List<DetailViewModel> list = db.Database.SqlQuery<DetailViewModel>(
                "EXEC DetailLoggingSelect @Department, @StaffName, @FrmDate, @ToDate",
                deptParam, staffParam, frmDate, toDate
                ).ToList();

                var result = list.Select(x => new {
                    x.StaffName,
                    x.DepartmentName,
                    x.Status,
                    StartDateTime = x.StartDateTime.HasValue
                        ? x.StartDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : "",
                                    ReturnDateTime = x.ReturnDateTime.HasValue
                        ? x.ReturnDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : ""
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}