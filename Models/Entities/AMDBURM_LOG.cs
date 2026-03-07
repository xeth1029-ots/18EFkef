using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class AMDBURM_LOG : IDBRow
    {
        public string USERNO { get; set; }
        public string PWD { get; set; }
        public string USERNAME { get; set; }
        public string IDNO { get; set; }
        public string BIRTHDAY { get; set; }
        /// <summary>服務單位 (分署) 編號</summary>
        public string UNITID { get; set; }
        /// <summary>使用者角色 (1.一般, 2.教師, 3.分署, 4.管理者) 編號</summary>
        public string GRPID { get; set; }
        public string EMAIL { get; set; }
        public string AUTHSTATUS { get; set; }
        public string AUTHDATES { get; set; }
        public string AUTHDATEE { get; set; }
        public int? ERRCT { get; set; }
        public string APPDATE { get; set; }
        public string MODUSERID { get; set; }
        public string MODUSERNAME { get; set; }
        public string MODTIME { get; set; }
        public string MODIP { get; set; }
        public string CHANGEPWD_REQUIRED { get; set; }
        public DateTime? LASTLOGINTIME { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.AMDBURM_LOG;
        }
    }
}