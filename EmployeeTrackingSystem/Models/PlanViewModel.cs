using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class PlanViewModel
    {
        public int PlanID { get; set; }
        public string StaffCD { get; set; }
        public System.DateTime PlanDate { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public Nullable<System.DateTime> DeleteDateTime { get; set; }
    }
}