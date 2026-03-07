using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using WKEFSERVICE.DataLayers;

namespace WKEFSERVICE.Models
{
    // 這個檔案集中定義系統組態設定變數(舊系統 DefineVar 有用到的值也納入這裡)
    public class ConfigModel
    {
        /// <summary>
        /// 系統預設最高管理帳號
        /// </summary>
        public const string Admin = "superadmin";

        /// <summary>
        /// 預設分頁筆數(常數定義), 如果 web.config 中有設定 DefaultPageSize 則為以 web.config 中設定的為主
        /// </summary>
        private const int _DefaultPageSize = 20;


        /// <summary>
        /// 是否啟用壓力測試模式(AppSettings StressTestMode=Y), 
        /// 若啟用則 LoginRequired 會以設定的StressTestUserInfo
        /// 測試帳號覆寫 LoginUserInfo
        /// </summary>
        public static bool StressTestMode
        {
            get
            {
                string strTestModel = ConfigurationManager.AppSettings["StressTestMode"];
                if (string.IsNullOrEmpty(strTestModel)) { return false; }
                bool testModel = "Y".Equals(strTestModel);
                return testModel;
            }
        }

        // <summary>
        // 啟用壓力測試模式(AppSettings StressTestMode=Y)時,
        // 系統會引用模擬的使用者資訊
        // </summary>
        //public static LoginUserInfo StressTestUserInfo
        //{
        //    get
        //    {
        //        LoginUserInfo user = new LoginUserInfo();
        //        string strUserNo = ConfigurationManager.AppSettings["StressTestUserNo"];
        //        string strUserName = ConfigurationManager.AppSettings["StressTestUserName"];
        //        string strUserExamKind = ConfigurationManager.AppSettings["StressTestUserExamKind"];
        //        string strUserRole = ConfigurationManager.AppSettings["StressTestUserRole"];
        //        if (string.IsNullOrEmpty(strUserNo))
        //        {
        //            strUserNo = "nobody";
        //        }
        //        if (string.IsNullOrEmpty(strUserName))
        //        {
        //            strUserName = strUserNo;
        //        }
        //        if (string.IsNullOrEmpty(strUserExamKind))
        //        {
        //            strUserExamKind = "1";
        //        }
        //        if (string.IsNullOrEmpty(strUserRole))
        //        {
        //            strUserRole = "0";
        //        }
        //        user.UserNo = strUserNo;
        //        user.User = new ClamUser();
        //        user.User.USERNO = strUserNo;
        //        user.User.USERNAME = strUserName;
        //        user.Roles.Add(
        //            new ClamUserRole {
        //                EXAMKIND = strUserExamKind,
        //                EXAMKIND_NAME = strUserExamKind,
        //                ROLE = strUserRole,
        //                ROLE_NAME = strUserRole
        //            });
        //        return user;
        //    }
        //}

        /// <summary>網站子目錄名稱</summary>
        public static string WKEF
        {
            get
            {
                string WKEF = ConfigurationManager.AppSettings["WKEF"];
                if (string.IsNullOrEmpty(WKEF)) { return ""; }
                return string.Concat("/", WKEF);
            }
        }

        /// <summary>
        /// 主機環境角色設定: 1.內網環境, 2.外網環境
        /// </summary>
        public static string NetID
        {
            get
            {
                string netId = ConfigurationManager.AppSettings["NetID"];
                if (string.IsNullOrEmpty(netId))
                {
                    netId = "1";
                }
                return netId;
            }
        }

        /// <summary>預設分頁筆數</summary>
        public static int DefaultPageSize
        {
            get
            {
                int iPageSize;
                string pageSize = ConfigurationManager.AppSettings["DefaultPageSize"];
                if (int.TryParse(pageSize, out iPageSize))
                {
                    return iPageSize;
                }
                else
                {
                    return _DefaultPageSize;
                }
            }
        }

        // <summary>電子抽籤網站連結網址（AT/C201 畫面查詢結果資料會使用到）</summary>
        // <remarks>在 AT/C201 畫面 輸入 在校生檢定，106年度，第1梯次，術科單位1039，按下查詢即會查到資料，再按下每筆資料右邊的「連結」文字即可</remarks>
        //public static string BallotUrl
        //{
        //    get
        //    {
        //        string path = ConfigurationManager.AppSettings["BallotUrl"];
        //        if (string.IsNullOrEmpty(path)) path = (new UrlHelper()).Content("~/EUSERVICE/Ballot/Ballot.aspx");
        //        return path;
        //    }
        //}

        /// <summary> 暫存路徑 </summary>
        public static string TempPath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["TempPath"];
                if (string.IsNullOrEmpty(path))
                {
                    path = (new UrlHelper()).Content("~/App_Data/Temp");
                }
                return path;
            }
        }

        /// <summary>
        /// 上傳檔案暫存路徑
        /// </summary>
        public static string UploadTempPath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["UpLoadFile"];
                if (string.IsNullOrEmpty(path))
                {
                    path = (new UrlHelper()).Content("~/Uploads");
                }
                return path;
            }
        }

        //取得在角色選擇頁中, 指定檢定類別所要使用的圖示檔名
        //public static string GetExamkindIcon(string examkind)
        //{
        //    string icon = "icon-type-default.png";
        //    switch (examkind)
        //    {

        //    }
        //    return icon;
        //}

        // 判斷登入帳號是否為技能檢定中心角色 檢定類別代碼 角色代碼
        //public static bool IsWdaseRole(string examkind, string role)
        //{
        //    bool bFlag = false;
        //    目前只判斷 role 為 0 :勞動部勞動力發展署技檢中心，
        //    examking 預留參數 已後可能會使用到
        //    if (role == "0")
        //    {
        //        bFlag = true;
        //    }
        //    return bFlag;
        //}

        /// <summary> 系統電子郵件服務主機 IP </summary>
        public static string MailServer
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailServer"];
                return (string.IsNullOrEmpty(value)) ? "127.0.0.1" : value;
            }
        }

        /// <summary> 系統電子郵件服務主機 IP </summary>
        public static string MailSenderUserNo
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailSenderUserNo"];
                return (string.IsNullOrEmpty(value)) ? "service@turbotech.com.tw" : value;
            }
        }

        /// <summary> 系統寄件者電子郵件地址 </summary>
        public static string MailSenderAddr
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailSenderAddr"];
                return (string.IsNullOrEmpty(value)) ? "service@turbotech.com.tw" : value;
            }
        }

        /// <summary> 系統寄件者電子郵件密碼 </summary>
        public static string MailSenderPwd
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailSenderPWD"];
                return (string.IsNullOrEmpty(value)) ? "" : value;
            }
        }

        /// <summary> 系統電子郵件服務主機 IP </summary>
        public static int MailServerPort
        {
            get
            {
                int i_value = 25;
                string value = ConfigurationManager.AppSettings["MailServerPort"];
                if (string.IsNullOrEmpty(value)) { return i_value; }
                if (int.TryParse(value, out i_value)) { return i_value; }
                return i_value;
            }
        }

        /// <summary> 寄信啟用ssl </summary>
        public static string MailEnableSsl
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailEnableSsl"];
                return (string.IsNullOrEmpty(value)) ? "" : value;
            }
        }

        /// <summary> 寄件者 emailaddress</summary>
        public static string from_emailaddress
        {
            get
            {
                string value = ConfigurationManager.AppSettings["from_emailaddress"];
                return (string.IsNullOrEmpty(value)) ? "ojt@msa.wda.gov.tw" : value;
            }
        }

        /// <summary> 寄件者 登入帳號 </summary>
        public static string MailuserName
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailuserName"];
                return (string.IsNullOrEmpty(value)) ? "ojt@msa.wda.gov.tw" : value;
            }
        }

        /// <summary> 寄件者 登入帳號 PXXWD </summary>
        public static string MailuserPwd
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MailuserPwd"];
                return (string.IsNullOrEmpty(value)) ? "" : value;
            }
        }

        /// <summary> 寄信數限制 </summary>
        public static int MaxCanMailCount
        {
            get
            {
                int i_value = 6;
                string value = ConfigurationManager.AppSettings["MaxCanMailCount"];
                if (string.IsNullOrEmpty(value)) { return i_value; }
                if (int.TryParse(value, out i_value)) { return i_value; }
                return i_value;
            }
        }

        /// <summary> 使用webservice 寄信功能 </summary>
        public static bool UseSendMailws
        {
            get
            {
                string value = ConfigurationManager.AppSettings["SendMailws"];
                return (string.IsNullOrEmpty(value)) ? false : (value == "Y" ? true : false);
            }
        }

        /// <summary> 測試網站檢核 </summary>
        public static bool UseTestWeb1
        {
            get
            {
                string value = ConfigurationManager.AppSettings["TestWeb1"];
                return (string.IsNullOrEmpty(value)) ? false : (value == "Y" ? true : false);
            }
        }

        /// <summary>
        /// 系統寄件者電子郵件密碼
        /// </summary>
        public static string FtpNasUser
        {
            get
            {
                string value = ConfigurationManager.AppSettings["FtpNasUser"];
                return (string.IsNullOrEmpty(value)) ? "labor" : value;
            }
        }

        /// <summary> 系統寄件者電子郵件密碼 </summary>
        public static string FtpNasServer
        {
            get
            {
                string value = ConfigurationManager.AppSettings["FtpNasServer"];
                return (string.IsNullOrEmpty(value)) ? "" : value;
            }
        }

        /// <summary>系統寄件者電子郵件密碼</summary>
        public static string FtpNasPassword
        {
            get
            {
                string value = ConfigurationManager.AppSettings["FtpNasPassword"];
                return (string.IsNullOrEmpty(value)) ? "labor22595700" : value;
            }
        }

        // 命題人員登入/更新API, 密碼字串加密的 AES KEY, REST API 會使用到這個設定
        //public static string WRS_AES_KEY
        //{
        //    get
        //    {
        //        return "WRS001004Staff";
        //    }
        //}

        // 系統主功能表樣式（0: 系統預設功能表，1: 樹型功能表）
        //public static string MainMenuType
        //{
        //    get
        //    {
        //        try
        //        {
        //            string value = ConfigurationManager.AppSettings["MainMenuType"];
        //            return string.IsNullOrEmpty(value) ? "0" : value;
        //        }
        //        catch
        //        {
        //            return "0";
        //        }
        //    }
        //}

        // 系統主功能表樣式（0: 系統預設功能表，1: 樹型功能表）
        //public static string OnOrOff
        //{
        //    get
        //    {
        //        try
        //        {
        //            string value = ConfigurationManager.AppSettings["OnOrOff"];
        //            return string.IsNullOrEmpty(value) ? "" : value == "0" ? "" : "EUSERVICE/";
        //        }
        //        catch
        //        {
        //            return "";
        //        }
        //    }
        //}

        // 系統主功能表樣式（0: 系統預設功能表，1: 樹型功能表）
        //public static string OnOrOffData
        //{
        //    get
        //    {
        //        try
        //        {
        //            string value = "0"; /*WKEFSERVICE.DataLayers.SHAREDAO.GetSetup("OnOrOffData");*/
        //            return string.IsNullOrEmpty(value) ? "" : value;
        //        }
        //        catch
        //        {
        //            return "";
        //        }
        //    }
        //}

        /// <summary>
        /// 是否啟用內網使用者「12-20 碼密碼長度」檢查。（true: 啟用，false: 不啟用 (即維持原 8-20 碼密碼長度)）
        /// </summary>
        public static bool Net1UserPwdLen1220Check
        {
            get
            {
                string value = ConfigurationManager.AppSettings["Net1UserPwdLen1220Check"];
                return string.IsNullOrEmpty(value) ? false : (value == "Y");
            }
        }

        /// <summary>自動依據當前「內網環境、外網環境」傳回使用者登入密碼長度最小值。</summary>
        /// <remarks>依據 20180521 署方來信，署資安管控條件。</remarks>
        public static int PwdLenMin
        {
            get
            {
                if (ConfigModel.Net1UserPwdLen1220Check)
                {
                    return (ConfigModel.NetID == "2") ? 8 : 12;
                }
                else
                {
                    return (ConfigModel.NetID == "2") ? 8 : 8;
                }
            }
        }

        /// <summary>自動依據當前「內網環境、外網環境」傳回使用者登入密碼長度最大值。</summary>
        public static int PwdLenMax
        {
            get { return (ConfigModel.NetID == "2") ? 20 : 20; }
        }

        //網站無障礙標章（適用「網站無障礙規範(110.07)」）,https://accessibility.moda.gov.tw/Download/Detail/1747?Category=28
        /// <summary>無障礙網站</summary>
        public static string AccessibleWebsite { get { return "https://accessibility.moda.gov.tw/Applications/Detail?category=20251209095039"; } }

        public static string strItemDCtxt = "動機職能 DC　　　";
        public static string strItemBCtxt = "行為職能 BC　　　";
        public static string strItemKCtxt = "知識職能 KC　　　";
    }
}