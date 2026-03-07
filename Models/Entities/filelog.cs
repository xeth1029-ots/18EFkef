using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class filelog : IDBRow
    {
        [IdentityDBField]
        public Int64 id { get; set; }
        public string username { get; set; }
        public string logtype { get; set; }
        public string createtime { get; set; }
        public string logfunc { get; set; }
        public string result { get; set; }
        public string filename { get; set; }
        public string message { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.filelog;
        }
    }
}