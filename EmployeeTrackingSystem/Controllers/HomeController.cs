using EmployeeTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EmployeeTrackingSystem.Models.staffViewModel;

namespace EmployeeTrackingSystem.Controllers
{
    public class HomeController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult DetailList()
        {
            ViewBag.ShowDropdown = true;
            LoadDropdowns();
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}