using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;

namespace WKEFSERVICE.Models
{
    /// <summary>
    /// 提供跨Session共用資料的存取類
    /// </summary>
    public class ApplicationModel : ApplicationBaseModel
    {
        private static object _lock = new object();
        private static ApplicationModel _instance = null;

        private ApplicationModel() { }

        private static ApplicationModel GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null) _instance = new ApplicationModel();
                return _instance;
            }
        }

        /// <summary>
        /// 取得系統最外層清單(沒有PRGID)<br />
        /// 參數 queryStr 可傳入：<br />
        /// A2 = 教師使用者<br />
        /// A3 = 分署使用者<br />
        /// AM = 管理者<br />
        /// </summary>
        /// <returns></returns>
        public static IList<AMFUNCM> GetClamFuncsOutAll(string queryStr)
        {
            const string _KEY = "TblCLAMFUNCMAll";
            ApplicationModel model = GetInstance();
            object value = model.GetApplicationVar(_KEY);

            // 不需要這段 - by.Senya
            // 每次進來都應重載，否則換帳號登入時，可能存在舊資料 (登出時，Session.RemoveAll(); 無法清除乾淨)
            //if (value != null && value is IList<AMFUNCM>)
            //{
            //    // 已存在, 直接返回
            //    return (IList<AMFUNCM>)value;
            //}
            //else
            //{
            // 不存在或過期, 從DB中載入
            BaseDAO dao = new BaseDAO();
            // 載出所有程式代碼(顯示在清單的)
            IList<AMFUNCM> list = dao.QueryForListAll<AMFUNCM>("AM.getClamFuncsOutAll_for" + queryStr, null);

            // 將 list 儲存至 ApplictionModel 中
            // 並設定有效時間至系統時間當天的 23:59:59 
            // (也就是隔天的 00:00:00 失效)
            DateTime now = DateTime.Now;
            now = now.AddDays(1);
            DateTime expire = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            model.SetApplicationVar(_KEY, list, expire);
            return list;
            //}
        }

        /// <summary>
        /// 取得系統已啟用的全部功能清單(已排序)
        /// </summary>
        /// <returns></returns>
        public static IList<AMFUNCM> GetClamFuncsAll()
        {
            const string _KEY = "TblCLAMFUNCM";
            ApplicationModel model = GetInstance();
            object value = model.GetApplicationVar(_KEY);

            if (value != null && value is IList<AMFUNCM>)
            {
                // 已存在, 直接返回
                return (IList<AMFUNCM>)value;
            }
            else
            {
                // 不存在或過期, 從DB中載入
                BaseDAO dao = new BaseDAO();
                // 載出所有程式代碼(顯示在清單的)
                IList<AMFUNCM> list = dao.QueryForListAll<AMFUNCM>("AM.getClamFuncsAll", null);

                // 將 list 儲存至 ApplictionModel 中
                // 並設定有效時間至系統時間當天的 23:59:59 
                // (也就是隔天的 00:00:00 失效)
                DateTime now = DateTime.Now;
                now = now.AddDays(1);
                DateTime expire = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

                model.SetApplicationVar(_KEY, list, expire);
                return list;
            }
        }

        /// <summary>
        /// 組合路徑_主要
        /// </summary>
        /// <returns></returns>
        public static string GetHeader()
        {
            SessionModel sm = SessionModel.Get();
            var dao = new BaseDAO();
            var FUNCMSTR = "首頁";

            if (sm.LastActionFunc != null)
            {
                var SYSID = sm.LastActionFunc.SYSID;
                var MODULES = sm.LastActionFunc.MODULES;
                var SUBMODULES = sm.LastActionFunc.SUBMODULES;
                var PRGID = sm.LastActionFunc.PRGID;

                // 第一層
                FUNCMSTR = GetFUNCM(SYSID).PRGNAME;
                // 第二層(Y/N)
                FUNCMSTR += MODULES.TONotNullString().Trim() != "" ? "＞" + GetFUNCM(SYSID, MODULES).PRGNAME : "";
                // 第三層(Y/N)
                FUNCMSTR += SUBMODULES.TONotNullString().Trim() != "" ? "＞" + GetFUNCM(SYSID, MODULES, SUBMODULES).PRGNAME : "";
                // 程式名稱(Y/N)
                FUNCMSTR += "＞" + sm.LastActionFunc.PRGNAME;
            }
            return FUNCMSTR;
        }

        /// <summary>
        /// 取得程式代碼相關物件
        /// </summary>
        /// <param name="SYSID"></param>
        /// <returns></returns>
        public static AMFUNCM GetFUNCM(string SYSID, string MODULES = " ", string SUBMODULES = " ", string PRGID = " ")
        {
            SessionModel sm = SessionModel.Get();
            AMFUNCM where = new AMFUNCM();
            where.SYSID = SYSID;
            where.MODULES = MODULES;
            where.SUBMODULES = SUBMODULES;
            where.PRGID = " ";
            var dao = new BaseDAO();
            var AMFUNCM = dao.GetRow(where);
            return AMFUNCM;
        }
    }
}