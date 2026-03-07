using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class AMLOGIN : IDBRow
    {
        public string L_USERNO { get; set; }
        public string L_PWD { get; set; }
        public string L_STATUS { get; set; }
        public string L_FAILREASON { get; set; }
        public string L_MODTIME { get; set; }
        public string L_MODIP { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.AMLOGIN;
        }
    }
}