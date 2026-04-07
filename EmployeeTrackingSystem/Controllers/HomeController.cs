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

        public ActionResult Dashboard()
        {
            //var members = new List<DashboardViewModel>
            //{
            //    new DashboardViewModel { Role = "社長 (President)", Name = "高間さん (Mr. Takama)" },
            //    new DashboardViewModel { Role = "取締役 (Director)", Name = "林宏一さん (Mr. Koichi Hayashi)" },
            //    new DashboardViewModel { Role = "Name", Name = "Example Member" }
            //};

            //return View(members);
                                                                    
            // Example pseudo-code
            string json1 = Get_DashboardInformation(2, "D01");
            List<DashboardViewModel> table1 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json1);
            string json2 = Get_DashboardInformation(5, "D02");
            List<DashboardViewModel> table2 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json2);
            string json3 = Get_DashboardInformation(2, "D03");
            List<DashboardViewModel> table3 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json3);
            string json4 = Get_DashboardInformation(5, "D04");
            List<DashboardViewModel> table4 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json4);
            string json5 = Get_DashboardInformation(2, "D05");
            List<DashboardViewModel> table5 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json5);
            string json6 = Get_DashboardInformation(5, "D06");
            List<DashboardViewModel> table6 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json6);
            string json7 = Get_DashboardInformation(6, "D07");
            List<DashboardViewModel> table7 = JsonConvert.DeserializeObject<List<DashboardViewModel>>(json7);

            var model = new DashboardTablesViewModel
            {
                Table1 = table1,
                Table2 = table2,
                Table3 = table3,
                Table4 = table4,
                Table5 = table5,
                Table6 = table6,
                Table7 = table7
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
        public ActionResult SaveStaff(string StaffCd, string Status, string ReturnDateTime, string Note)
        {
            var staffRecord = new DashboardViewModel
            {
                StaffCD1 = StaffCd,
                Status1 = Status,
                ReturnDatetime1 = ReturnDateTime,
                Note1 = Note
            };
           

            return Json(new { success = true });
        }

        public ActionResult StaffList()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult DetailList()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}