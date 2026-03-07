using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Models.Entities
{
    public class AMGMAPM : IDBRow
    {
        public string GRPID { get; set; }
        public string SYSID { get; set; }
        public string MODULES { get; set; }
        public string SUBMODULES { get; set; }
        public string PRGID { get; set; }
        public string PRG_I { get; set; }
        public string PRG_U { get; set; }
        public string PRG_D { get; set; }
        public string PRG_Q { get; set; }
        public string PRG_P { get; set; }
        public string MODUSERID { get; set; }
        public string MODUSERNAME { get; set; }
        public string MODTIME { get; set; }
        public string MODIP { get; set; }
        public DBRowTableName GetTableName()
        {
            return StaticCodeMap.TableName.AMGMAPM;
        }
    }
}