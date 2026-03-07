using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;


namespace WKEFSERVICE.Models.Entities
{
    //計算排程-wkef1206
    /// <summary>年度評核結果總表</summary>
    public class ReviewReport : IDBRow
    {
        [IdentityDBField]
        public long? RVSeq { get; set; }
        public string Year { get; set; }
        public string IDNO { get; set; }
        /// <summary>1:北基宜花金馬分署,2:桃竹苗分署,3:中彰投分署,4:雲嘉南分署,5:高屏澎東分署</summary>
        public string UnitCode { get; set; }
        public string MT1_ATTENDANCE { get; set; }
        public string MT1_WRITTEN_EXAM { get; set; }
        public string MT1_RESULTS { get; set; }
        public int? THOURS_TRANSDATA_D_DC { get; set; }
        public int? THOURS_TRANSDATA_D_BC { get; set; }
        public int? THOURS_TRANSDATA_D_KC { get; set; }
        public int? THOURS_TRANSDATA_S_DC { get; set; }
        public int? THOURS_TRANSDATA_S_BC { get; set; }
        public int? THOURS_TRANSDATA_S_KC { get; set; }
        public int? THOURS_TRANSDATA_K_DC { get; set; }
        public int? THOURS_TRANSDATA_K_BC { get; set; }
        public int? THOURS_TRANSDATA_K_KC { get; set; }
        /// <summary>(排除)同小人提課程</summary>
        public int? THOURS_TRANSDATA_K_DC_X { get; set; }
        /// <summary>(排除)同小人提課程</summary>
        public int? THOURS_TRANSDATA_K_BC_X { get; set; }
        /// <summary>(排除)同小人提課程</summary>
        public int? THOURS_TRANSDATA_K_KC_X { get; set; }

        public int? THOURS_TRANSDATA_D_TOTAL { get; set; }
        public int? THOURS_TRANSDATA_S_TOTAL { get; set; }
        public int? THOURS_TRANSDATA_K_TOTAL { get; set; }
        public decimal? SATISFY_TRANSDATA_D { get; set; }
        public decimal? SATISFY_TRANSDATA_S { get; set; }
        public decimal? SATISFY_TRANSDATA_K { get; set; }
        public decimal? SATISFY_TRANSDATA_DS { get; set; }
        public decimal? SATISFY_TRANSDATA_DSK { get; set; }
        public int? LEASTONEUNIT_D { get; set; }
        public int? LEASTONEUNIT_S { get; set; }
        public int? LEASTONEUNIT_K { get; set; }
        public int? MEETING_TIMES { get; set; }
        public string REPORTREC_YN { get; set; }
        public string POINT_RESULTS { get; set; }
        public string PYEARS_STATUS { get; set; }
        public string REVIEW_RESULTS { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.ReviewReport;
        }

    }

}