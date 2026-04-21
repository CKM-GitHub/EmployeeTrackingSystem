using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class StaffUpdateModel
    {
       
        [MaxLength(5)]
        public string StaffCD { get; set; }

       
        
        public string StaffName { get; set; }

        
        public string DepartmentCD { get; set; }

        public string DepartmentName { get; set; }

        
        public string Position { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        
        public string PhoneNo { get; set; }

        public Nullable<System.DateTime> JoinedDate { get; set; }
        public string EmployeeType { get; set; }
        public bool Enroll { get; set; }

        [MaxLength(200)]
        public string Remark { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> ReturnDateTime { get; set; }
        public string Note { get; set; }

        [RegularExpression(@"^[0-9]+$")]
        public Nullable<int> SeatNo { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public Nullable<System.DateTime> DeleteDateTime { get; set; }

        public Nullable<int> CurrentShop { get; set; }
    }
}