using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class Notices : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public string Caption { get; set; }
        public string DateS { get; set; }
        public string DateE { get; set; }
        public string IsOnTop { get; set; }
        public string UpdatedAccount { get; set; }
        public string UpdatedDatetime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.Notices;
        }
    }
}