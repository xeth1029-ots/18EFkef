using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class MsgPost : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public int PostType { get; set; }
        public string PostCaption { get; set; }
        public string/*DateTime*/ PostDateS { get; set; }
        public string/*DateTime*/ PostDateE { get; set; }
        public int PostTo { get; set; }
        public string PostContent { get; set; }
        public string FileNameOrg { get; set; }
        public string FileNameNew { get; set; }
        public string IsOnTop { get; set; }
        public string IsShowSignUp { get; set; }
        public Int64? MeetingSeq { get; set; }
        public string PublishedUnit { get; set; }
        public int OrderBy { get; set; }
        public string Actived { get; set; }
        public string UpdatedAccount { get; set; }
        public string/*DateTime*/ UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.MsgPost;
        }
    }
}