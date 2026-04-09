using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class staffViewModel
    {
        
            public string StaffCD { get; set; }
            public string StaffName { get; set; }
            public string DepartmentCD { get; set; }
            public string DepartmentName { get; set; }
            public Nullable<System.DateTime> JoinedDate { get; set; }
            public Nullable<int> SeatNo { get; set; }
            public string Email { get; set; }
            public string Position { get; set; }
            public string PhoneNo { get; set; }
            public bool Enroll { get; set; }
            public string EmployeeType { get; set; }

            public string Remark { get; set; }
        
    }
}