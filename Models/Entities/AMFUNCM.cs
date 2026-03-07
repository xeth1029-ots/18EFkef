using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class AMFUNCM : IDBRow
    {
        [IdentityDBField]
        public Int64? Seq { get; set; }
        public string SYSID { get; set; }
        public string MODULES { get; set; }
        public string SUBMODULES { get; set; }
        public string PRGID { get; set; }
        public string PRGNAME { get; set; }
        public string PRGORDER { get; set; }
        public string OPENAUTH { get; set; }
        public string SHOWMENU { get; set; }
        public string QUERYSTRING { get; set; }
        public string MODUSERID { get; set; }
        public string MODUSERNAME { get; set; }
        public DateTime? MODTIME { get; set; }
        public string MODIP { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.AMFUNCM;
        }
    }
}