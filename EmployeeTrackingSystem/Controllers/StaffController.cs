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
                if (!string.IsNullOrEmpty(model.StaffName) &&
                    !IsUtf8Valid(model.StaffName, 50))
                {
                    ModelState.AddModelError("StaffName", "StaffName exceeds 50 bytes");
                }

                if (!string.IsNullOrEmpty(model.Position) &&
                    !IsUtf8Valid(model.Position, 20))
                {
                    ModelState.AddModelError("Position", "Position exceeds 20 bytes");
                }

                if (!string.IsNullOrEmpty(model.PhoneNo) &&
                  !IsUtf8Valid(model.PhoneNo, 15))
                {
                    ModelState.AddModelError("PhoneNo", "PhoneNo exceeds 15 bytes");
                }

                if (!string.IsNullOrEmpty(model.Remark) &&
                    !IsUtf8Valid(model.Remark, 200))
                {
                    ModelState.AddModelError("Remark", "Remark exceeds 200 bytes");
                }
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
                
                //// Seat duplicate
                bool isExist = db.T_StaffMaster.Any(x =>
                    x.DepartmentCD == model.DepartmentCD &&
                    x.SeatNo == model.SeatNo &&
                    x.Enroll == true);

                if (isExist)
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] {
                    new { field = "SeatNo", message = "この席番号は既に使用されています" }
                        }
                    });
                }
               
                model.CurrentShop = int.Parse(model.DepartmentCD.Substring(1, 2).ToString());

                //  Save

                try
                {
                    var entity = new T_StaffMaster
                    {
                        StaffCD = model.StaffCD,
                        StaffName = model.StaffName,
                        DepartmentCD = model.DepartmentCD,
                        Position = model.Position,
                        Email = model.Email,
                        PhoneNo = model.PhoneNo,
                        JoinedDate = model.JoinedDate,
                        EmployeeType = model.EmployeeType,
                        Enroll = model.Enroll,
                        Remark = model.Remark,
                        SeatNo = model.SeatNo,
                        CurrentShop = model.CurrentShop,
                        InsertDateTime = DateTime.Now
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
            if (!string.IsNullOrEmpty(model.StaffName) &&
              !IsUtf8Valid(model.StaffName, 50))
            {
                ModelState.AddModelError("StaffName", "StaffName exceeds 50 bytes");
            }

            if (!string.IsNullOrEmpty(model.Position) &&
                !IsUtf8Valid(model.Position, 20))
            {
                ModelState.AddModelError("Position", "Position exceeds 20 bytes");
            }

            if (!string.IsNullOrEmpty(model.PhoneNo) &&
              !IsUtf8Valid(model.PhoneNo, 15))
            {
                ModelState.AddModelError("PhoneNo", "PhoneNo exceeds 15 bytes");
            }

            if (!string.IsNullOrEmpty(model.Remark) &&
                !IsUtf8Valid(model.Remark, 200))
            {
                ModelState.AddModelError("Remark", "Remark exceeds 200 bytes");
            }


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
                bool isExist = db.T_StaffMaster.Any(x =>
                    x.DepartmentCD == model.DepartmentCD &&
                    x.SeatNo == model.SeatNo &&
                    x.StaffCD != model.StaffCD &&
                    x.Enroll == true);

                if (isExist)
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] {
                    new { field = "SeatNo", message = "この席番号は既に使用されています" }
                        }
                    });
                }
            try
            {
                var staff = db.T_StaffMaster
                    .FirstOrDefault(x => x.StaffCD == model.StaffCD && x.Enroll == true);

                if (staff == null)
                {
                    return Json(new { success = false, message = "更新失敗しました。" });
                }

                if (model.StaffName != null)
                    staff.StaffName = model.StaffName;

                if (model.DepartmentCD != null)
                    staff.DepartmentCD = model.DepartmentCD;

                if (!string.IsNullOrEmpty(model.Position))
                    staff.Position = model.Position;

                if (!string.IsNullOrEmpty(model.Email))
                    staff.Email = model.Email;

                if (!string.IsNullOrEmpty(model.PhoneNo))
                    staff.PhoneNo = model.PhoneNo;

                // If JoinedDate is nullable
                if (model.JoinedDate.HasValue)
                    staff.JoinedDate = model.JoinedDate.Value;

                if (!string.IsNullOrEmpty(model.EmployeeType))
                    staff.EmployeeType = model.EmployeeType;

                    staff.Enroll = model.Enroll;

                if (!string.IsNullOrEmpty(model.Remark))
                    staff.Remark = model.Remark;

                if ((model.DepartmentCD ?? "").Trim().StartsWith("S0"))
                {
                    staff.SeatNo = null;
                }
                else
                {
                    staff.SeatNo = model.SeatNo;
                }

                // keep existing DB value
                //staff.SeatNo = staff.SeatNo;
                

                staff.UpdateDateTime = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, message = "登録が完了しました。" });
            }
            catch (Exception ex)
            {
               return Json(new { success = false, message = "更新失敗しました。" });
            }
        }
        [HttpPost]
        public ActionResult UpdateSeat(List<staffViewModel> list)
        {
            try
            {
                // 1. Load all relevant staff in affected departments
                var deptCDs = list.Select(x => x.DepartmentCD).Distinct().ToList();
                var staffInDepartments = db.T_StaffMaster
                    .Where(s => deptCDs.Contains(s.DepartmentCD))
                    .ToList();

                // 2. Build current seat map (Department -> Seat -> Staff)
                var seatMap = staffInDepartments
                    .Where(s => s.SeatNo.HasValue && s.Enroll != false)
                    .GroupBy(s => s.DepartmentCD)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToDictionary(s => s.SeatNo.Value, s => s.StaffCD)
                    );

                // 3. Apply changes in memory
                foreach (var model in list)
                {
                    if (!seatMap.ContainsKey(model.DepartmentCD))
                        seatMap[model.DepartmentCD] = new Dictionary<int, string>();

                    var deptSeats = seatMap[model.DepartmentCD];

                    // Remove current staff seat temporarily
                    var currentStaff = staffInDepartments.FirstOrDefault(s => s.StaffCD == model.StaffCD);
                    if (currentStaff?.SeatNo != null)
                    {
                        deptSeats.Remove(currentStaff.SeatNo.Value);
                    }

                    // Check if the new seat is already assigned to another staff NOT in this batch
                    if (deptSeats.TryGetValue(model.SeatNo.Value, out string existingStaff))
                    {
                        var inBatch = list.Any(x => x.StaffCD == existingStaff);
                        if (!inBatch)
                        {
                            var deptname = db.T_Department
                              .Where(s => s.DepartmentCD == model.DepartmentCD)
                              .Select(s => new
                              {
                                DeptName = s.DepartmentName
                              }).FirstOrDefault();

                            return Content($"Error: 席 {model.SeatNo} in {deptname.DeptName} is already assigned to {existingStaff}");
                        }
                    }

                    // Assign new seat in memory map
                    deptSeats[model.SeatNo.Value] = model.StaffCD;
                }

                // 4. Save changes
                foreach (var model in list)
                {
                    var staff = staffInDepartments.FirstOrDefault(s => s.StaffCD == model.StaffCD);
                    if (staff != null)
                    {
                        staff.SeatNo = model.SeatNo;
                        staff.UpdateDateTime = DateTime.Now;
                    }
                }

                db.SaveChanges();
                return Content("登録が完了しました");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
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