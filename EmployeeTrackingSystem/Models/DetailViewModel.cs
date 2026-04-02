using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class DetailViewModel
    {
        public int SEQ { get; set; }
        public string DepartmentCD { get; set; }
        public string DepartmentName { get; set; }
        public string StaffCD { get; set; }
        public string StaffName { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> ReturnDateTime { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
        public Nullable<System.DateTime> DeleteDateTime { get; set; }
    }
}