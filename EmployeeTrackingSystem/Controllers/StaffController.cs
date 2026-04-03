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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(T_StaffMaster model)
        {
            using (var db = new EmployeeTrackingDBEntities())
            {
                string value = model.Enroll.ToString();
                //  duplicate check
                if (db.T_StaffMaster.Any(x => x.StaffCD == model.StaffCD))
                {
                    return Json(new
                    {
                        success = false,
                        field = "StaffCD",
                        message = "このスタッフCDは既に存在します"
                    });
                }

                //  required + email check
                if (string.IsNullOrEmpty(model.StaffCD))
                    return Json(new { success = false, field = "StaffCD", message = "必須です" });

                if (string.IsNullOrEmpty(model.StaffName))
                    return Json(new { success = false, field = "StaffName", message = "必須です" });

                if (string.IsNullOrEmpty(model.DepartmentCD))
                    return Json(new { success = false, field = "DepartmentCD", message = "選択してください" });

                if (!string.IsNullOrEmpty(model.Email) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return Json(new { success = false, field = "Email", message = "メール形式が正しくありません" });
                }

                bool isExist = db.T_StaffMaster.Any(x =>
                 x.DepartmentCD == model.DepartmentCD &&
                    x.SeatNo == model.SeatNo
                        );

                if (isExist)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This seat number is already used in this department."
                    });
                }
                //  save
                model.InsertDateTime = DateTime.Now;
                db.T_StaffMaster.Add(model);
                db.SaveChanges();

                return Json(new { success = true });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(T_StaffMaster model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Validation error" });
                }

                var staff = db.T_StaffMaster
                    .FirstOrDefault(x => x.StaffCD == model.StaffCD);

                if (staff == null)
                {
                    return Json(new { success = false, message = "Staff not found" });
                }

                staff.DepartmentCD = model.DepartmentCD;
                staff.Position = model.Position;
                staff.Email = model.Email;
                staff.PhoneNo = model.PhoneNo;
                staff.JoinedDate = model.JoinedDate;
                staff.EmployeeType = model.EmployeeType;
                staff.Enroll = model.Enroll;
                staff.Remark = model.Remark;
                staff.UpdateDateTime = DateTime.Now;

                db.SaveChanges();

                return Json(new { success = true, message = "Update OK" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        //public ActionResult Update(T_StaffMaster model)
        //{
        //    // 🔍 Check ModelState FIRST
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage);

        //        return Content("Validation Error: " + string.Join(" | ", errors));
        //    }

        //    try
        //    {
        //        var staff = db.T_StaffMaster
        //            .FirstOrDefault(x => x.StaffCD == model.StaffCD);

        //        if (staff == null)
        //        {
        //            return Content("Error: Staff not found");
        //        }

        //        staff.DepartmentCD = model.DepartmentCD;
        //        staff.Position = model.Position;
        //        staff.Email = model.Email;
        //        staff.PhoneNo = model.PhoneNo;
        //        staff.JoinedDate = model.JoinedDate;
        //        staff.EmployeeType = model.EmployeeType;
        //        staff.Enroll = model.Enroll;
        //        staff.Remark = model.Remark;
        //        staff.UpdateDateTime = DateTime.Now;

        //        db.SaveChanges();

        //        return Content("Update OK"); // debug first
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content("Error: " + ex.Message);
        //    }
        //}

        [HttpPost]
        public ActionResult UpdateSeat(List<T_StaffMaster> list)
        {
            try
            {
                foreach (var model in list)
                {
                    bool exists = db.T_StaffMaster.Any(x =>
                        x.DepartmentCD == model.DepartmentCD &&
                        x.SeatNo == model.SeatNo &&
                        x.StaffCD != model.StaffCD);

                    if (exists)
                    {
                        return Content($"Error: Seat already used for {model.StaffCD}");
                    }

                    var staff = db.T_StaffMaster
                        .FirstOrDefault(x => x.StaffCD == model.StaffCD);

                    if (staff != null)
                    {
                        staff.SeatNo = model.SeatNo;
                        staff.UpdateDateTime = DateTime.Now;
                    }
                }

                db.SaveChanges();

                return Content("Update OK");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}