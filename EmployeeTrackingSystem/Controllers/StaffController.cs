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
                                 JoinedDate   =s.JoinedDate,
                                 DepartmentName = d.DepartmentName,
                                 SeatNo = s.SeatNo
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
                // 1️⃣ StaffCD required
                if (string.IsNullOrWhiteSpace(model.StaffCD))
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] {
                    new { field = "StaffCD", message = "スタッフCDは必須です" }
                }
                    });
                }

                // 2️⃣ Duplicate check
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

                // 3️⃣ StaffName required
                if (string.IsNullOrWhiteSpace(model.StaffName))
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] {
                    new { field = "StaffName", message = "スタッフ名は必須です" }
                }
                    });
                }

                // 4️⃣ Other ModelState validation (optional)
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

                // 5️⃣ Seat duplicate
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

                // ✅ Save
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

                //staff.DepartmentCD = model.DepartmentCD;
                //staff.Position = model.Position;
                //staff.Email = model.Email;
                //staff.PhoneNo = model.PhoneNo;
                //staff.JoinedDate = model.JoinedDate;
                //staff.EmployeeType = model.EmployeeType;
                //staff.Enroll = model.Enroll;
                //staff.Remark = model.Remark;
                //staff.UpdateDateTime = DateTime.Now;

                db.SaveChanges();
                return Json(new { success = true, message = "Updated successfully" });
               /* return Content("Update OK"); /*/
            }
            catch (Exception ex)
            {
               return Json(new { success = false, message = "Update failed" });
            }
        }

        [HttpPost]
        public ActionResult UpdateSeat( List<StaffViewModel> list)
        {
            try
            {
                // Step 1: Load all relevant staff
                var deptCDs = list.Select(x => x.DepartmentCD).Distinct().ToList();
                var staffInDepartments = db.T_StaffMaster
                    .Where(x => deptCDs.Contains(x.DepartmentCD))
                    .ToList();

                // Step 2: Build in-memory seat map
                var seatMap = staffInDepartments
                    .Where(s => s.SeatNo.HasValue)
                    .GroupBy(s => s.DepartmentCD)
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToDictionary(s => s.SeatNo.Value, s => s.StaffCD)
                    );

                // Step 3: Process updates
                foreach (var model in list)
                {
                    if (!seatMap.ContainsKey(model.DepartmentCD))
                        seatMap[model.DepartmentCD] = new Dictionary<int, string>();

                    var deptSeats = seatMap[model.DepartmentCD];

                    // Check if target seat is used by another staff
                    if (deptSeats.ContainsKey(model.SeatNo.Value))
                    {
                        var otherStaffCD = deptSeats[model.SeatNo.Value];

                        if (otherStaffCD != model.StaffCD)
                        {
                            // Check if the other staff is also swapping in this batch
                            var otherModel = list.FirstOrDefault(x => x.StaffCD == otherStaffCD);
                            if (otherModel == null || otherModel.SeatNo == model.SeatNo)
                            {
                                return Content($"Error: Seat {model.SeatNo} in Department {model.DepartmentCD} is already used by {otherStaffCD}");
                            }
                        }
                    }

                    // Update seat map in memory
                    var staffCurrentSeat = staffInDepartments.FirstOrDefault(s => s.StaffCD == model.StaffCD)?.SeatNo;
                    if (staffCurrentSeat.HasValue)
                        deptSeats.Remove(staffCurrentSeat.Value);

                    deptSeats[model.SeatNo.Value] = model.StaffCD;

                    // Update DB object
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
        
    }
}