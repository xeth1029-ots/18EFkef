using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WKEFSERVICE.Models.Entities;

namespace WKEFSERVICE.Models
{
    public class ClamUser : AMDBURM
    {
        /// <summary>
        /// 服務單位 (分署)
        /// </summary>
        public string UNIT_NAME { get; set; }

        /// <summary>
        /// 使用者登入角色 (LoginCharacter)
        /// </summary>
        public string GRP_NAME { get; set; }

        public int? iGRPID
        {
            get
            {
                int i_GRPID = 0;
                return !int.TryParse(this.GRPID, out i_GRPID) ? (int?)null : i_GRPID;
            }
        }
    }
}