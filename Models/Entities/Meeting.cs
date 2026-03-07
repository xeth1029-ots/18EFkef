using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class Meeting : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public string Year { get; set; }
        public string MeetingType { get; set; }
        public string MeetingName { get; set; }
        public string MeetingDateS { get; set; }
        public string MeetingDateE { get; set; }
        public string MeetingRelease { get; set; }
        public int? Days { get; set; }
        public string TimeS { get; set; }
        public string TimeE { get; set; }
        public string Location { get; set; }
        public string PostalCode1 { get; set; }
        public string PostalCode2 { get; set; }
        public string Address { get; set; }
        public int? MaximumPeople { get; set; }
        public string SignUpDateS { get; set; }
        public string SignUpDateE { get; set; }
        public string Remark { get; set; }
        public string PicName { get; set; }
        public string PicHint { get; set; }
        public string PicAlign { get; set; }
        public string SignUpState { get; set; }
        public string CreatedUnit { get; set; }
        public string CreatedAccount { get; set; }
        public string CreatedDatetime { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.Meeting;
        }
    }
}