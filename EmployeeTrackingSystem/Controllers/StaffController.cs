using EmployeeTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
            var departmentParam = string.IsNullOrEmpty(department) ? (object)DBNull.Value : department;
            var staffParam = string.IsNullOrEmpty(staff) ? (object)DBNull.Value : staff;

            var staffList = db.Database.SqlQuery<staffViewModel>(
                "EXEC GetStaffList @departmentcd, @staffcd",
                new SqlParameter("@departmentcd", departmentParam),
                new SqlParameter("@staffcd", staffParam)
            ).ToList();

            
            return View(staffList);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(staffViewModel model)
        {
            using (var db = new EmployeeTrackingDBEntities())
            {
               
                // Other ModelState validation (optional)
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            field = x.Key.Replace("model.", ""),
                            message = x.Value.Errors.First().ErrorMessage
                        }).ToList();

                    return Json(new { success = false, errors = errors });
                }
                var existsActive = db.T_StaffMaster
                    .Any(x => x.StaffCD == model.StaffCD  && x.Enroll == true);

                var existsInactive = db.T_StaffMaster
                    .Any(x => x.StaffCD == model.StaffCD  && x.Enroll == false);

                if (existsInactive)
                {
                    return Json(new
                    {
                        success = false,
                        errorType = "inactive",
                        message = "Inactive CD already exist"
                    });
                }

                if (existsActive)
                {
                    return Json(new
                    {
                        success = false,
                        errorType = "duplicate",
                        errors = new[]
                        {
                    new { field = "StaffCD", message = "" }
                         }
                    });
                }

                int maxSeatNo = 0;
                if (!string.IsNullOrEmpty(model.DepartmentCD) &&
                    model.DepartmentCD.StartsWith("S0") &&
                    int.TryParse(model.DepartmentCD.Substring(1, 2), out int shop))
                {
                    model.CurrentShop = shop;
                }
                else
                {
                    maxSeatNo = db.T_StaffMaster
                  .Where(x => x.DepartmentCD == model.DepartmentCD)
                  .Max(x => x.SeatNo).GetValueOrDefault();
                }

                try
                {
                    var entity = new T_StaffMaster
                    {
                        StaffCD = model.StaffCD,
                        StaffName = model.StaffName,
                        DepartmentCD = model.DepartmentCD,
                        Email = model.Email,
                        PhoneNo = model.PhoneNo,
                        JoinedDate = model.JoinedDate,
                        EmployeeType = model.EmployeeType,
                        Status= "帰宅",
                        Enroll = model.Enroll,
                        Remark = model.Remark,
                        CurrentShop = model.CurrentShop,
                        InsertDateTime = DateTime.Now,
                        SeatNo = maxSeatNo + 1
                    };

                    db.T_StaffMaster.Add(entity);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                catch(Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
              
            }
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(StaffUpdateModel model)
        {
           
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                       .Where(x => x.Value.Errors.Count > 0)
                       .Select(x => new {
                           field = x.Key,
                           message = x.Value.Errors.First().ErrorMessage
                       }).ToList();

                return Json(new { success = false, errors = errors });;
            }
                
            try
            {
                var staff = db.T_StaffMaster
                    .FirstOrDefault(x => x.StaffCD == model.StaffCD && x.Enroll == true);

                if (staff == null)
                {
                    return Json(new { success = false, message = "更新失敗しました。" });
                }

                
                    staff.StaffName = model.StaffName;
                    staff.DepartmentCD = model.DepartmentCD;
                    staff.Email = model.Email;
                    staff.PhoneNo = model.PhoneNo;
                    staff.JoinedDate = model.JoinedDate;
                    staff.EmployeeType = model.EmployeeType;
                    staff.Enroll = model.Enroll;
                    staff.Remark = model.Remark;

                var dept = (model.DepartmentCD ?? "").Trim();

                if (dept.StartsWith("S0") && 
                    int.TryParse(dept.Substring(1, 2), out int shop))
                {
                    
                    staff.CurrentShop = shop;
                }
                else
                {
                    staff.CurrentShop = null;
                    
                }
                staff.UpdateDateTime = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, message = "登録が完了しました。" });
            }
            catch (Exception ex)
            {
               return Json(new { success = false, message = "更新失敗しました。" });
            }
        }

        public JsonResult CheckStaffCD(string staffCD)
        {
            var active = db.T_StaffMaster
                .Any(x => x.StaffCD == staffCD && x.Enroll == true);

            var inactive = db.T_StaffMaster
                .Any(x => x.StaffCD == staffCD && x.Enroll == false);

            if (inactive)
            {
                return Json(new
                {
                    exists = true,
                    errorType = "inactive",
                    message = "Inactive CD already exist"

                }, JsonRequestBehavior.AllowGet);
            }

            if (active)
            {
                return Json(new
                {
                    exists = true,
                    errorType = "duplicate",
                    message = "Invalid use"

                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                status = "ok"
            }, JsonRequestBehavior.AllowGet);
        }
       
        bool IsUtf8Valid(string value, int maxBytes)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            return Encoding.UTF8.GetByteCount(value) <= maxBytes;
        }
    }
}