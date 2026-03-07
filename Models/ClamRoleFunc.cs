using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Commons;
using Turbo.DataLayer;
using System.Text;

namespace WKEFSERVICE.Models
{
    /// <summary>
    /// 角色功能權限 Model
    /// </summary>
    public class ClamRoleFunc : AMFUNCM
    {
        /// <summary>功能資料字串</summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder x = new StringBuilder();
            x.Append("SHOWMENU=");
            x.Append(this.SHOWMENU);
            x.Append(", SYS_ID=");
            x.Append(this.SYSID);
            x.Append(", MODULES=");
            x.Append(this.MODULES);
            x.Append(", SUBMODULES=");
            x.Append(this.SUBMODULES);
            x.Append(", PRGID=");
            x.Append(this.PRGID);
            x.Append(", PRGNAME=");
            x.Append(this.PRGNAME);
            return x.ToString();
        }
    }
}