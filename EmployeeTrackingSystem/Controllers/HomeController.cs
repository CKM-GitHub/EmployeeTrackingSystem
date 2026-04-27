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
        public ActionResult Dashboard()
        {            
            string json1 = Get_DashboardInformation(2, "D01");
            List<DashboardViewModel> table1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json1);
            string json2 = Get_DashboardInformation(4, "D02");
            List<DashboardViewModel> table2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json2);
            string json3 = Get_DashboardInformation(2, "D03");
            List<DashboardViewModel> table3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json3);
            string json4 = Get_DashboardInformation(2, "D04");
            List<DashboardViewModel> table4 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json4);
            string json5 = Get_DashboardInformation(2, "D05");
            List<DashboardViewModel> table5 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json5);
            string json6 = Get_DashboardInformation(2, "D06");
            List<DashboardViewModel> table6 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json6);
            string json7 = Get_DashboardInformation(4, "D07");
            List<DashboardViewModel> table7 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json7);
           // string json8 = Get_DashboardInformation(2, "D08");
           // List<DashboardViewModel> table8 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json8);

            string json_shop1 = Get_DashboardInformation(2, "S01");
            List<DashboardViewModel> tableshop1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop1);

            string json_shop2 = Get_DashboardInformation(2, "S02");
            List<DashboardViewModel> tableshop2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop2);

            string json_shop3 = Get_DashboardInformation(2, "S03");
            List<DashboardViewModel> tableshop3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json_shop3);
            
            var model = new DashboardTablesViewModel
            {
                Table1 = table1,
                Table2 = table2,
                Table3 = table3,
                Table4 = table4,
                Table5 = table5,
                Table6 = table6,
                Table7 = table7,
               // Table8 = table8,

                Table9 = tableshop1,
                Table10 = tableshop2,
                Table11 = tableshop3,
                AvailableShops = db.T_Department.Where(s => s.DepartmentCD.StartsWith("S")).ToList()
            };
          
            return View(model);
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
            Boolean insertflag = true;
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
                TempData["Message"] = "Plz choose at least one status to update!!";
            }
            return Json(new { success = insertflag, message = TempData["Message"] });
        }
        public Boolean Dashobard_StaffName_Click_Save(DashboardViewModel model)
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
                var existingRecord = db.T_Plan.FirstOrDefault(p => p.PlanDate == item.PlanDate && p.StaffCD == item.StaffCD);
                if (existingRecord != null)
                {
                    if (string.IsNullOrEmpty(item.Note))
                    {
                        db.T_Plan.Remove(existingRecord);
                    }
                    else
                    {
                        // UPDATE: Record exists, just change the remark
                        existingRecord.Note = item.Note;
                        existingRecord.UpdateDateTime = DateTime.Now;
                    }
                }
                else if (item.Note != null)
                {
                    //save to DB
                    db.T_Plan.Add(new T_Plan
                    {
                        StaffCD = item.StaffCD,
                        PlanDate = item.PlanDate,
                        Note = item.Note,
                        InsertDateTime = DateTime.Now
                    });
                }
            }
            int sv = db.SaveChanges();
            bool res = false;
            if (sv > 0)
                res = true;
            return Json(new { success = res });
        }
    }
}