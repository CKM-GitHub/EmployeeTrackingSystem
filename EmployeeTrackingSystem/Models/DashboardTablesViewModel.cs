using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTrackingSystem.Models
{
    public class DashboardTablesViewModel
    {
        public IEnumerable<DashboardViewModel> Table1 { get; set; }
        public IEnumerable<DashboardViewModel> Table2 { get; set; }
        public IEnumerable<DashboardViewModel> Table3 { get; set; }
        public IEnumerable<DashboardViewModel> Table4 { get; set; }
        public IEnumerable<DashboardViewModel> Table5 { get; set; }
        public IEnumerable<DashboardViewModel> Table6 { get; set; }
        public IEnumerable<DashboardViewModel> Table7 { get; set; }
        public IEnumerable<DashboardViewModel> Table8 { get; set; }
        public IEnumerable<DashboardViewModel> Table9 { get; set; }
        public IEnumerable<DashboardViewModel> Table10 { get; set; }
        public IEnumerable<DashboardViewModel> Table11 { get; set; }

        public List<T_Department> AvailableShops { get; set; }
    }
}