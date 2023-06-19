using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XyTech.Models
{
    public class MonthlyAttendanceViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string FloorName { get; set; }
        public int AttendanceCount { get; set; }
    }

}