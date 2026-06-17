using EmployeeTrackingSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTrackingSystem.Controllers
{
    public class HomeController : Controller
    {
        public string conStr = ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString;
        private EmployeeTrackingDBEntities db = new EmployeeTrackingDBEntities();
        List<DashboardViewModel> table1 = null;
        List<DashboardViewModel> table2 = null;
        List<DashboardViewModel> table3 = null;
        List<DashboardViewModel> table4 = null;
        List<DashboardViewModel> table5 = null;
        List<DashboardViewModel> table6 = null;
        List<DashboardViewModel> table7 = null;
        List<DashboardViewModel> tableshop1 = null;
        List<DashboardViewModel> tableshop2 = null;
        List<DashboardViewModel> tableshop3 = null;
        public ActionResult Dashboard()
        {
            string json1 = Get_DashboardInformation(2, "D01");
            table1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json1);
            string json2 = Get_DashboardInformation(4, "D02");
             table2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json2);
            string json3 = Get_DashboardInformation(2, "D03");
             table3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json3);
            string json4 = Get_DashboardInformation(2, "D04");
             table4 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json4);
            string json5 = Get_DashboardInformation(2, "D05");
            table5 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json5);
            string json6 = Get_DashboardInformation(2, "D06");
             table6 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json6);
            string json7 = Get_DashboardInformation(4, "D07");
             table7 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json7);

            string json_shop1 = Get_DashboardInformation(2, "S01");
             tableshop1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop1);

            string json_shop2 = Get_DashboardInformation(2, "S02");
             tableshop2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop2);

            string json_shop3 = Get_DashboardInformation(2, "S03");
            tableshop3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop3);
            ViewBag.CurDate = DateTime.Now.ToString("yyyy-MM-dd");
           var model = new DashboardTablesViewModel
            {
                Table1 = table1,
                Table2 = table2,
                Table3 = table3,
                Table4 = table4,
                Table5 = table5,
                Table6 = table6,
                Table7 = table7,
                Table9 = tableshop1,
                Table10 = tableshop2,
                Table11 = tableshop3,
                AvailableShops = db.T_Department.Where(s => s.DepartmentCD.StartsWith("S")).ToList(),
                Alldepartments = db.T_Department.ToList()               
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult GetRefreshDepartment(string departmentCd) // 20260601 ttw
        {

            DashboardTablesViewModel model = new DashboardTablesViewModel();
            if (departmentCd == "D01")
            {
                string json1 = Get_DashboardInformation(2, "D01");
                table1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json1);
                model.divtb = table1;
            }
            else if (departmentCd == "D02")
            {
                string json2 = Get_DashboardInformation(4, "D02");
                table2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json2);
                model.divtb = table2;
            }
            else if (departmentCd == "D03")
            {
                string json3 = Get_DashboardInformation(2, "D03");
                table3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json3);
                model.divtb = table3;
            }
            else if (departmentCd == "D04")
            {
                string json4 = Get_DashboardInformation(2, "D04");
                table4 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json4);
                model.divtb = table4;
            }
            else if (departmentCd == "D05")
            {
                string json5 = Get_DashboardInformation(2, "D05");
                table5 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json5);
                model.divtb = table5;
            }
            else if (departmentCd == "D06")
            {
                string json6 = Get_DashboardInformation(2, "D06");
                table6 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json6);
                model.divtb = table6;
            }
            else if (departmentCd == "D07")
            {
                string json7 = Get_DashboardInformation(4, "D07");
                table7 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json7);
                model.divtb = table7;
            }
            else if(departmentCd == "S01")
            {
                string json_shop1 = Get_DashboardInformation(2, "S01");
                tableshop1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop1);
                model.divtb = tableshop1;
            }
            else if (departmentCd == "S02")
            {
                string json_shop2 = Get_DashboardInformation(2, "S02");
                tableshop2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop2);
                model.divtb = tableshop2;
            }
            else if (departmentCd == "S03")
            {
                string json_shop3 = Get_DashboardInformation(2, "S03");
                tableshop3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop3);
                model.divtb = tableshop3;
            }           

            string partialview = string.Empty;
            if (departmentCd == "D02" || departmentCd == "D07")
            {
                partialview = "_DashboardTable5Col";
            }
            else
                partialview = "_DashboardTable";
            
            return PartialView(partialview, model.divtb);
        }
        public string Get_DashboardInformation(int show_column, string DepartmentCD)
        {
            string JSONString = string.Empty;
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@ColumnMode", SqlDbType.VarChar) { Value = show_column };
            prms[1] = new SqlParameter("@DepartmentCD", SqlDbType.VarChar) { Value = DepartmentCD };

            JSONString = JsonConvert.SerializeObject(SelectData("Select_Dashboard_Data", prms));
            return JSONString;
        }
        public DataTable SelectData(string sSQL, params SqlParameter[] para)
        {
            DataTable dt = new DataTable();
            var newCon = new SqlConnection(conStr);
            using (var adapt = new SqlDataAdapter(sSQL, newCon))
            {
                newCon.Open();
                adapt.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (para != null)
                    adapt.SelectCommand.Parameters.AddRange(para);
                adapt.Fill(dt);
                newCon.Close();
            }
            return dt;
        }
        [HttpPost]
        public ActionResult SaveStaff(string DepartmentCD, string StaffCd, string Status, string ReturnDateTime, string Note, string CurrentShop)
        {
            var staffRecord = new DashboardViewModel
            {
                DepartmentCD = DepartmentCD,
                StaffCD1 = StaffCd,
                Status1 = Status,
                ReturnDatetime1 = ReturnDateTime,
                Note1 = Note,
                CurrentShop= CurrentShop
            };
            bool insertflag = true;
            if (!string.IsNullOrEmpty(Status))
            {
                insertflag = Dashobard_StaffName_Click_Save(staffRecord);
                if (insertflag)
                {
                    TempData["Message"] = "登録しました。";
                }
                else
                {
                    TempData["Message"] = "登録失敗しました。";
                }
            }
            else
            {
                insertflag = false;
                TempData["Message"] = "更新するステータスを少なくとも1つ選択してください！";
            }
            return Json(new { success = insertflag, message = TempData["Message"] });
        }
        public bool Dashobard_StaffName_Click_Save(DashboardViewModel model)
        {
            try
            {
                DataTable dtinfo = new DataTable();
                SqlParameter[] prms = new SqlParameter[6];
                
                prms[0] = new SqlParameter("@DepartmentCD", SqlDbType.VarChar) { Value = model.DepartmentCD };
                prms[1] = new SqlParameter("@StaffCD", SqlDbType.VarChar) { Value = model.StaffCD1 };

                if (!String.IsNullOrWhiteSpace(model.Status1))
                    prms[2] = new SqlParameter("@Status", SqlDbType.NVarChar) { Value = model.Status1 };
                else
                    prms[2] = new SqlParameter("@Status", SqlDbType.NVarChar) { Value = DBNull.Value };

                if (!String.IsNullOrWhiteSpace(model.ReturnDatetime1))
                    prms[3] = new SqlParameter("@ReturnDatetime", SqlDbType.VarChar) { Value = model.ReturnDatetime1 };
                else
                    prms[3] = new SqlParameter("@ReturnDatetime", SqlDbType.VarChar) { Value = DBNull.Value };

                if (!String.IsNullOrWhiteSpace(model.Note1))
                    prms[4] = new SqlParameter("@Note", SqlDbType.NVarChar) { Value = model.Note1 };
                else
                    prms[4] = new SqlParameter("@Note", SqlDbType.NVarChar) { Value = DBNull.Value };
                
                if (!String.IsNullOrWhiteSpace(model.CurrentShop))
                    prms[5] = new SqlParameter("@CurrentShop", SqlDbType.NVarChar) { Value = model.CurrentShop };
                else
                    prms[5] = new SqlParameter("@CurrentShop", SqlDbType.NVarChar) { Value = DBNull.Value };

                InsertUpdateDeleteData("Dashobard_StaffName_Insert", prms);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void InsertUpdateDeleteData(string sSQL, params SqlParameter[] para)
        {
            var newCon = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(sSQL, newCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(para);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }
        [HttpPost]
        public JsonResult GetPlanData(string id)
        {
            try
            {
                DataTable dt = new DataTable();
                var newCon = new SqlConnection(conStr);              
                var staffCDPara = new SqlParameter("@StaffCD", id ?? (object)DBNull.Value);
                using (var adapt = new SqlDataAdapter("Select_Plan_Data", newCon))
                {
                    newCon.Open();
                    adapt.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (staffCDPara != null)
                        adapt.SelectCommand.Parameters.Add(staffCDPara);
                    adapt.Fill(dt);
                    newCon.Close();
                }                
                return Json(JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SavePlanData(List<PlanViewModel> list)
        {
            foreach (var item in list)
            {
                var existingRecord = db.T_Plan.FirstOrDefault(p => p.PlanID == item.PlanID);
                if (existingRecord != null)
                {
                    // UPDATE: Record exists, just change the data
                    existingRecord.PlanDateTime = item.PlanDateTime;
                    existingRecord.ReturnDateTime = item.ReturnDateTime;
                    existingRecord.Note = item.Note;
                    existingRecord.Status = item.Status;
                    existingRecord.UpdateDateTime = DateTime.Now;

                }
                else if (item.Note != null)
                {
                    //save to DB
                    db.T_Plan.Add(new T_Plan
                    {
                        StaffCD = item.StaffCD,
                        PlanDateTime = item.PlanDateTime,
                        ReturnDateTime = item.ReturnDateTime,
                        Status = item.Status,
                        Note = item.Note,
                        InsertDateTime = DateTime.Now
                    });
                }
            }

            try
            {

                int sv = db.SaveChanges();
                bool res = false;
                if (sv > 0)
                    res = true;
                return Json(new { success = res });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    detail = ex.InnerException?.Message
                });
            }
        }


        [HttpPost]
        public JsonResult UpdateSeatChange(string staff1,int? seat1,string staff2,int? seat2,string dept1, string dept2)
        {
            try
            {
                using (var db = new EmployeeTrackingDBEntities())
                {
                    var s1 = db.T_StaffMaster
                               .FirstOrDefault(x => x.StaffCD == staff1);

                    var s2 = db.T_StaffMaster
                               .FirstOrDefault(x => x.StaffCD == staff2);

                    //if (s1 != null)
                    //{
                    //    seat1 = s1.SeatNo;
                    //    s1.SeatNo = s2.SeatNo;
                    //    s1.DepartmentCD = dept2;
                    //}

                    //if (s2 != null)
                    //{
                    //    s2.SeatNo = seat1;
                    //    s2.DepartmentCD = dept1;
                    //}
                    //if(dept1.StartsWith("S") && dept2.StartsWith("S"))
                    //{                       
                    //    s1.CurrentShop = int.Parse(s1.DepartmentCD.Substring(1));
                    //    s2.CurrentShop = int.Parse(s2.DepartmentCD.Substring(1));
                    //}

                    if (s1 != null && s2 != null)
                    {
                        if (dept1.StartsWith("S") && dept2.StartsWith("S"))
                        {
                            s1.CurrentShop = s2.CurrentShop;    //int.Parse(s2.DepartmentCD.Substring(1));                          
                            s2.CurrentShop = s1.CurrentShop;    //int.Parse(s1.DepartmentCD.Substring(1));                          
                        }
                        else
                        {                            
                            s1.DepartmentCD = dept2;
                            s2.DepartmentCD = dept1;
                        }
                        seat1 = s1.SeatNo;
                        s1.SeatNo = s2.SeatNo;
                        s2.SeatNo = seat1;
                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        success = true
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult ChangeDepartment(string staffCD,string draggedDept,string targetDept)
        {
            using (var db = new EmployeeTrackingDBEntities())
            {
                var staff = db.T_StaffMaster
                    .FirstOrDefault(x => x.StaffCD == staffCD);

                if (staffCD == null || staffCD == "")
                {
                    return Json(new
                    {
                        success = false,
                        message = "スタッフが見つかりません。"
                    });
                }
                //20260514 ttw 

                if (draggedDept != targetDept)
                {
                    if (draggedDept.StartsWith("S") && targetDept.StartsWith("S"))
                    {                     
                        staff.CurrentShop = int.Parse(targetDept.Substring(1));
                    }
                    else
                    {
                        // old department reorder
                        var oldList = db.T_StaffMaster
                            .Where(s =>
                                s.DepartmentCD == draggedDept &&
                                s.StaffCD != staffCD &&
                                s.Enroll != false)
                            .OrderBy(s => s.SeatNo)
                            .ToList();

                        int no = 1;

                        foreach (var s in oldList)
                        {
                            s.SeatNo = no++;
                        }
                        // new department max seat
                        int maxSeatNo = db.T_StaffMaster
                            .Where(x =>
                                x.DepartmentCD == targetDept &&
                                x.Enroll != false)
                            .Max(x => (int?)x.SeatNo) ?? 0;

                        // move current staff
                        staff.DepartmentCD = targetDept;
                        staff.SeatNo = maxSeatNo + 1;
                    }
                    db.SaveChanges();
                }

                return Json(new
                {
                    success = true
                });
            }
        }
        [HttpPost]
        public ActionResult DeleteSelectedPlan(List<string> ids)
        {
            using (var db = new EmployeeTrackingDBEntities())
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        int planid = int.Parse(ids[i].ToString());
                        var Plantd = db.T_Plan
                          .FirstOrDefault(x => x.PlanID == planid);
                        Plantd.DeleteDateTime = DateTime.Now;
                    }
                }
                db.SaveChanges();
            }
            return Json(new { success = true });
        }
    }
}