using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class MsgPostAttached : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public Int64? MsgPostSeq { get; set; }
        public string FileNameOrg { get; set; }
        public string FileNameNew { get; set; }
        public string FileNameType { get; set; }
        public string UpdatedAccount { get; set; }
        public string/*DateTime*/ UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.MsgPostAttached;
        }
    }
}