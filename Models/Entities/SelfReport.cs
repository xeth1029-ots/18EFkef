using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class SelfReport : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public string Year { get; set; }
        /// <summary>1:北基宜花金馬分署,2:桃竹苗分署,3:中彰投分署,4:雲嘉南分署,5:高屏澎東分署</summary>
        public string UnitCode { get; set; }
        public string TeacherAccount { get; set; }
        public string JobAbilityCode { get; set; }
        public string CourseUnitCode { get; set; }
        public string OBJECT_TYPE { get; set; }
        public int? StudNum { get; set; }
        public string TObject { get; set; }
        public string TMethod { get; set; }
        public string TslidesOrg { get; set; }
        public string TslidesNew { get; set; }
        public string TslidesType { get; set; }
        public string Other { get; set; }
        public string StudyGReportOrg { get; set; }
        public string StudyGReportNew { get; set; }
        public string StudyGReportType { get; set; }
        public string WorksheetsOrg { get; set; }
        public string WorksheetsNew { get; set; }
        public string WorksheetsType { get; set; }
        public string HomeWorkOrg { get; set; }
        public string HomeWorkNew { get; set; }
        public string HomeWorkType { get; set; }
        public string ExpReportOrg { get; set; }
        public string ExpReportNew { get; set; }
        public string ExpReportType { get; set; }
        public string ExamOrg { get; set; }
        public string ExamNew { get; set; }
        public string ExamType { get; set; }
        public string Other1Org { get; set; }
        public string Other1New { get; set; }
        public string Other1Type { get; set; }
        public string Other1Remark { get; set; }
        public string Other2Org { get; set; }
        public string Other2New { get; set; }
        public string Other2Type { get; set; }
        public string Other2Remark { get; set; }
        public string Effect { get; set; }
        public string PlanImprove { get; set; }
        public string ExamImprove { get; set; }
        public string materialsImprove { get; set; }
        public string Other3 { get; set; }
        public string FirstTime { get; set; }
        public string AuditStatus { get; set; }
        /// <summary>退回原因說明</summary>
        public string AuditReason { get; set; }
        public string AuditAccount { get; set; }
        public string AuditDatetime { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.SelfReport;
        }
    }
}