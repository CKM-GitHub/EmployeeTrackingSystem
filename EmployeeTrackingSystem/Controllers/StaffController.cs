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
                             select new staffViewModel
                             {
                                 StaffCD = s.StaffCD,
                                 StaffName = s.StaffName,
                                 DepartmentCD = s.DepartmentCD,
                                 JoinedDate   =s.JoinedDate,
                                 DepartmentName = d.DepartmentName,
                                 SeatNo = s.SeatNo,
                                 Position =s.Position,
                                 PhoneNo =s.PhoneNo,
                                 Email =s.Email,
                                 Enroll =s.Enroll,
                                 Remark =s.Remark,
                                 EmployeeType =s.EmployeeType

                             }).ToList();

            //  Filter Department
            if (!string.IsNullOrEmpty(department) && department != "All")
            {
                staffList = staffList
                    .Where(x => x.DepartmentCD == department)
                    .ToList();
            }

            //  Filter Staff
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
                //  StaffCD required
                //if (string.IsNullOrWhiteSpace(model.StaffCD))
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        errors = new[] {
                //    new { field = "StaffCD", message = "スタッフCDは必須です" }
                //}
                //    });
                //}

                // Duplicate check
                

                //// StaffName required
                //if (string.IsNullOrWhiteSpace(model.StaffName))
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        errors = new[] {
                //    new { field = "StaffName", message = "スタッフ名は必須です" }
                //}
                //    });
                //}

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

                if (db.T_StaffMaster.Any(x => x.StaffCD == model.StaffCD))
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] {
                    new { field = "StaffCD", message = "このスタッフCDは既に存在します" }
                }
                    });
                }
                // Seat duplicate
                bool isExist = db.T_StaffMaster.Any(x =>
                    x.DepartmentCD == model.DepartmentCD &&
                    x.SeatNo == model.SeatNo);

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

                //  Save
                model.InsertDateTime = DateTime.Now;
                db.T_StaffMaster.Add(model);
                db.SaveChanges();

                return Json(new { success = true });
            }
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(T_StaffMaster model)
        {
            ModelState.Remove("StaffCD");
            ModelState.Remove("StaffName");



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
                    .FirstOrDefault(x => x.StaffCD == model.StaffCD);

                if (staff == null)
                {
                    return Json(new { success = false, message = "Update failed" });
                }
                if (model.StaffName != null)
                    staff.StaffName = model.StaffName;

                if (model.DepartmentCD != null )
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

                // If Enroll is nullable bool
              
                    staff.Enroll = model.Enroll;

                if (!string.IsNullOrEmpty(model.Remark))
                    staff.Remark = model.Remark;

                staff.UpdateDateTime = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, message = "登録が完了しました。" });
            }
            catch (Exception ex)
            {
               return Json(new { success = false, message = "Update failed" });
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
                    .Where(s => s.SeatNo.HasValue)
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
                            return Content($"Error: Seat {model.SeatNo} in Department {model.DepartmentCD} is already assigned to {existingStaff}");
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
                return Content("Update OK");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        public JsonResult CheckStaffCD(string staffCD)
        {
            bool exists = db.T_StaffMaster.Any(x => x.StaffCD == staffCD);

            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
        }

    }
}