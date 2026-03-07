using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using Turbo.DataLayer;

namespace WKEFSERVICE.Commons
{
    public class FileLogAdd
    {
        /// <summary>
        /// 寫入 檔案LOG                                        <br />
        /// type     => 異動類別 NONE, DELETE, UPLOAD, DOWNLOAD <br />
        /// state    => 異動狀態 NONE, SUCCESS, FAIL            <br />
        /// func     => 呼叫來源 Program name                   <br />
        /// filename => 操作之檔案名稱                           <br />
        /// Remark   => 備註                                    <br />
        /// </summary>
        public static void Do(ModifyType type, ModifyState state, string func, string filename, string Remark)
        {
            BaseDAO dao = new BaseDAO();
            int rtn = dao.Insert<filelog>(new filelog()
            {
                username = SessionModel.Get().UserInfo.UserNo,
                logtype = type.ToString(),
                createtime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                logfunc = func,
                result = state.ToString(),
                filename = filename,
                message = Remark
            });
        }
    }
}