using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WKEFSERVICE.Models
{
    public class ReviewReportD1
    {
        public string TEACHERNAME { get; set; } // nvarchar(20) NOT NULL
        public string IDNO { get; set; } // nvarchar(30) NOT NULL
        public string ACCOUNT { get; set; } // nvarchar(15) NOT NULL
        public string UNITCODE { get; set; } // nvarchar(3) NOT NULL
        public string YEARS { get; set; } // int NULL
        public int? MT1_ATTENDANCE_CNT { get; set; } // int NULL
        public int? MT1_WRITTEN_EXAM { get; set; } // int NULL
        public int? MT1_RESULTS { get; set; } // int NULL
        public int? THOURS_TRANSDATA_D_DC { get; set; } // int NULL
        public int? THOURS_TRANSDATA_D_BC { get; set; } // int NULL
        public int? THOURS_TRANSDATA_D_KC { get; set; } // int NULL
        public int? THOURS_TRANSDATA_S_DC { get; set; } // int NULL
        public int? THOURS_TRANSDATA_S_BC { get; set; } // int NULL
        public int? THOURS_TRANSDATA_S_KC { get; set; } // int NULL
        public int? THOURS_TRANSDATA_D_TOTAL { get; set; } // int NULL
        public int? THOURS_TRANSDATA_S_TOTAL { get; set; } // int NULL
        public string SATISFY_TRANSDATA_D { get; set; } // nvarchar(30) NULL
        public string SATISFY_TRANSDATA_S { get; set; } // nvarchar(30) NULL
        public int? MEETING_TIMES { get; set; } // int NULL
        public int? REPORTREC_YN_CNT { get; set; } // int NULL    
    }
}