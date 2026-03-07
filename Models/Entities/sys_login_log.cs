using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class sys_login_log : IDBRow
    {
        [IdentityDBField]
        public Int64 seq { get; set; }
        public string logintype { get; set; }
        public string userid { get; set; }
        public string remoteip { get; set; }
        public string accesstime { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.sys_login_log;
        }
    }
}