using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class MeetingAttend : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public Int64? MeetingSeq { get; set; }
        public string TeacherAccount { get; set; }
        public string Attend { get; set; }
        public string TestPassed { get; set; }
        public int? TestScore { get; set; }
        public string Remark { get; set; }
        /// <summary>強制計算（不分轄區了）</summary>
        public string MadForced { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.MeetingAttend;
        }
    }
}