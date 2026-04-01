using EmployeeTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static EmployeeTrackingSystem.Models.staffViewModel;

namespace EmployeeTrackingSystem.Controllers
{
    public class StaffController : BaseController
    {
      
        public ActionResult StaffList(string department, string staff)
        {
            ViewBag.ShowDropdown = true;
            LoadDropdowns();

            // JOIN Staff + Department
            var staffList = (from s in db.T_StaffMaster
                             join d in db.T_Department
                             on s.DepartmentCD equals d.DepartmentCD
                             select new StaffViewModel
                             {
                                 StaffCD = s.StaffCD,
                                 StaffName = s.StaffName,
                                 DepartmentCD = s.DepartmentCD,
                                 DepartmentName = d.DepartmentName,
                                 SeatNo = s.SeatNo
                             }).ToList();

            // 🔍 Filter Department
            if (!string.IsNullOrEmpty(department) && department != "All")
            {
                staffList = staffList
                    .Where(x => x.DepartmentCD == department)
                    .ToList();
            }

            // 🔍 Filter Staff
            if (!string.IsNullOrEmpty(staff) && staff != "All")
            {
                staffList = staffList
                    .Where(x => x.StaffCD == staff)
                    .ToList();
            }
           
            return View(staffList);
        }
    }
}