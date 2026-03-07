using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class AMCHANGEPWD_GUID : IDBRow
    {
        [IdentityDBField]
        public Int64? ID { get; set; }
        public string USERNO { get; set; }
        public string GUID { get; set; }
        public string GUIDYN { get; set; }
        public string MODTIME { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.AMCHANGEPWD_GUID;
        }
    }
}