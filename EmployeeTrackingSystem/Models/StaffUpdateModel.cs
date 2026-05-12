using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class StaffUpdateModel
    {
        public string StaffCD { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(50)]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "*")]
        public string DepartmentCD { get; set; }

        public string DepartmentName { get; set; }

        [EmailAddress]
        [MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9.]+@gmail\.com$")]
        public string Email { get; set; }

        [MaxLength(15)]
        public string PhoneNo { get; set; }
        public DateTime? JoinedDate { get; set; }
        public string EmployeeType { get; set; }
        public bool Enroll { get; set; }
        [MaxLength(200)]
        public string Remark { get; set; }
        public string Status { get; set; }
        
        public string Note { get; set; }
       
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public Nullable<int> CurrentShop { get; set; }
    }
}