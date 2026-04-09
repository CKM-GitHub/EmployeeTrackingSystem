
namespace EmployeeTrackingSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class T_StaffMaster
    {
        [Required(ErrorMessage = "スタッフCDは必須です")]
        [StringLength(5, MinimumLength = 1, ErrorMessage = "Staff Code must be between 1 and 5 characters")]
        public string StaffCD { get; set; }

        [Required(ErrorMessage = "スタッフ名は必須です")]
        [MaxLength(50, ErrorMessage = "スタッフ名は50文字以内です")]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "部門CDは必須です")]
        public string DepartmentCD { get; set; }

        [MaxLength(20, ErrorMessage = "位置は20文字以内です")]
        public string Position { get; set; }

        [EmailAddress(ErrorMessage = "正しいメール形式を入力してください")]
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
