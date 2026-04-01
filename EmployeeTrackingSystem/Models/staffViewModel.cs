using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class staffViewModel
    {
        public class StaffViewModel
        {
            public string StaffCD { get; set; }
            public string StaffName { get; set; }
            public string DepartmentCD { get; set; }
            public string DepartmentName { get; set; }
            public Nullable<System.DateTime> JoinedDate { get; set; }
            public Nullable<int> SeatNo { get; set; }
        }
    }
}