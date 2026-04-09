using EmployeeTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTrackingSystem.Controllers
{
    public class BaseController : Controller
    {
        protected EmployeeTrackingDBEntities db = new EmployeeTrackingDBEntities();

        protected void LoadDropdowns()
        {
            var departments = db.T_Department
                 .Where(d => d.DeleteDateTime == null)
                .Select(d => new
                {
                    Value = d.DepartmentCD,
                    Text = d.DepartmentName
                }).ToList();

            departments.Insert(0, new { Value = "", Text = "" });

            ViewBag.Departments = new SelectList(departments, "Value", "Text");

            var staffs = db.T_StaffMaster
                .Select(s => new
                {
                    Value = s.StaffCD,
                    Text = s.StaffName
                }).ToList();

            staffs.Insert(0, new { Value = "", Text = "" });

            ViewBag.StaffNames = new SelectList(staffs, "Value", "Text");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LoadDropdowns();
            //ViewBag.ShowDropdown = true;

            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;

            if ((controller == "Staff" && action == "StaffList") ||
                (controller == "Detail" && action == "DetailList"))
            {
                ViewBag.ShowDropdown = true;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}