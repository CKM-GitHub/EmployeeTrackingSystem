
namespace EmployeeTrackingSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class T_StaffMaster
    {
        [Required(ErrorMessage = "*")]
        [MaxLength(5, ErrorMessage = "スタッフCDは5文字以内です")]
        public string StaffCD { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "スタッフ名は50文字以内です")]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "*")]
        public string DepartmentCD { get; set; }

        [MaxLength(20, ErrorMessage = "位置は20文字以内です")]
        public string Position { get; set; }

        [EmailAddress(ErrorMessage = "正しいメールを入力してください")]
        public string Email { get; set; }

        [MaxLength(15, ErrorMessage = "電話番号は15文字以内です")]
        public string PhoneNo { get; set; }

        public Nullable<System.DateTime> JoinedDate { get; set; }
        public string EmployeeType { get; set; }
        public bool Enroll { get; set; }

        [MaxLength(200, ErrorMessage = "リマークは200文字以内です")]
        public string Remark { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> ReturnDateTime { get; set; }
        public string Note { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "SeatNo must be numbers only")]
        public Nullable<int> SeatNo { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public Nullable<System.DateTime> DeleteDateTime { get; set; }
    }
}
