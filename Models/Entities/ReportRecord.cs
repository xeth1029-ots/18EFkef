using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class ReportRecord : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public string Year { get; set; }
        public string TeacherAccount { get; set; }
        public string ReportType { get; set; }
        public string JobAbilityCode { get; set; }
        public string ReportName { get; set; }
        public string FileNameRemark { get; set; }
        public string FileNameOrg { get; set; }
        public string FileNameNew { get; set; }
        public string FileNameType { get; set; }
        public string FirstTime { get; set; }
        public string AuditStatus { get; set; }
        public string AuditAccount { get; set; }
        public string AuditDatetime { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.ReportRecord;
        }
    }
}