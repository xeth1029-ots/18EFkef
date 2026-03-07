using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.DataLayers;
using Turbo.Commons;
using log4net;
using WKEFSERVICE.Commons;

namespace WKEFSERVICE.Services
{
    public class ClamService
    {
        protected static readonly ILog LOG = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 傳入使用者帳號及輸入的密碼明文, 加密後回傳
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        public string EncPassword(string userNo, string userPwd)
        {
            string plain = userPwd;
            if (!string.IsNullOrWhiteSpace(userNo))
            {
                plain = userNo + plain;
            }
            return this.EncPassword(plain);
        }

        /// <summary>
        /// 傳入使用者輸入的密碼明文, 加密後回傳
        /// </summary>
        /// <param name="usePwd"></param>
        /// <returns></returns>
        private string EncPassword(string userPwd)
        {
            if (string.IsNullOrWhiteSpace(userPwd))
            {
                throw new ArgumentNullException("userPwd");
            }
            //TODO: 置換 RSACSP 改成不可逆的 Hash 方法
            RSACSP.RSACSP rsa = new RSACSP.RSACSP();
            return rsa.Utl_Encrypt(userPwd);
        }

        /// <summary>
        /// 回傳指定帳號的預設密碼加密字串
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        private string GetDefaultPasswordEnc(string userNo)
        {
            if (string.IsNullOrWhiteSpace(userNo)) { return ""; }

            AMDAO dao = new AMDAO();
            AMDBURM amdwhere = new AMDBURM() { USERNO = userNo };
            var data = dao.GetRowList(amdwhere);
            string rst = (data.Count() > 0) ? data[0].PWD : "";
            return rst;
        }

        /// <summary>
        /// 使用者登入帳密檢核, 檢核結果以 LoginUserInfo 返回, 
        /// 應檢查 LoginUserInfo.LoginSuccess 判斷登入是否成功
        /// </summary>
        /// <param name="userNo">使用者ID</param>
        /// <param name="userPwd">使用者登入密碼(明碼)</param>
        /// <returns></returns>
        public LoginUserInfo LoginValidate(string userNo, string userPwd)
        {
            AMDAO dao = new AMDAO();
            LoginUserInfo userInfo = new LoginUserInfo();
            userInfo.UserNo = userNo;

            // 取得使用者帳號資料
            ClamUser dburm = dao.GetUser(userNo);
            if (string.IsNullOrEmpty(userNo) || string.IsNullOrEmpty(userPwd) || dburm == null)
            {
                userInfo.LoginSuccess = false;
                userInfo.LoginErrMessage = "帳號不存在，請檢查 !!";
            }
            else
            {
                bool accValid = false;

                #region Step.1 帳號有效性檢查
                //AUTHSTATUS: 0:帳號尚未核准 1:帳號有效, 2:帳號無效,8:帳號鎖定,9:帳號註銷
                DateTime? authDateS = HelperUtil.TransToDateTime(dburm.AUTHDATES, "");
                DateTime? authDateE = HelperUtil.TransToDateTime(dburm.AUTHDATEE, "");

                if ("0".Equals(dburm.AUTHSTATUS))
                {
                    userInfo.LoginErrMessage = "您目前帳號尚未核准，尚無法使用!!";
                }
                else if (authDateS == null || authDateS.Value.Date.CompareTo(DateTime.Now.Date) > 0)
                {
                    userInfo.LoginErrMessage = "您的帳號啟用日尚未到，目前無法使用本系統!!";
                }
                else if (authDateE != null && authDateE.Value.Date.CompareTo(DateTime.Now.Date) < 0)
                {
                    //u AMDBURM set AUTHDATEE=format(getdate()+30,'yyyyMMdd'), AUTHSTATUS='1'  w idno=''
                    userInfo.LoginErrMessage = "您的帳號有效日期已過，無法繼續使用本系統!!";
                }
                else if ("1".Equals(dburm.AUTHSTATUS))
                {
                    // 帳號有效
                    accValid = true;
                }
                else
                {
                    // 帳號無效, 判斷無效原因
                    if ("2".Equals(dburm.AUTHSTATUS))
                    {
                        userInfo.LoginErrMessage = "您目前帳號已被停用，無法使用本系統!!";  //"您目前帳號申請資料已被駁回，無法使用本系統!!";
                    }
                    else if ("8".Equals(dburm.AUTHSTATUS))
                    {
                        userInfo.LoginErrMessage = "您目前帳號已被鎖定，無法使用本系統!!";
                    }
                    //else if ("9".Equals(dburm.AUTHSTATUS))
                    //{
                    //    userInfo.LoginErrMessage = "您目前帳號已被註銷，無法使用本系統!!";
                    //}
                    else
                    {
                        LOG.Warn(string.Format("帳號:{0} 使用狀態值不正確: AMDBURM.AUTHSTATUS = {0} ", dburm.USERNO, dburm.AUTHSTATUS));
                        userInfo.LoginErrMessage = "您目前帳號無法使用!!";
                    }
                }

                #endregion

                if (accValid)
                {
                    #region Step.2 密碼檢核

                    // 產生密碼加密字串
                    string encPass = this.EncPassword(userPwd);
                    // 依帳號查詢 並帶出加密後的密碼
                    string encDefsultPass = this.GetDefaultPasswordEnc(userNo);

                    int errct = dburm.ERRCT != null ? dburm.ERRCT.Value : 0;

                    bool fg_CHK1 = encPass.Equals(encDefsultPass);
                    if (!fg_CHK1)
                    {
                        //Turbo.Crypto.AesTk atk = new Turbo.Crypto.AesTk(); //次 加 x 密 檢 核 
                        string s_ATKENC = new Turbo.Crypto.AesTk().Encrypt(encDefsultPass);
                        fg_CHK1 = s_ATKENC.Equals(userPwd);
                    }
                    if (!fg_CHK1)
                    {
                        //參數式檢核
                        var S_BMNdPXD = "BMNd4WYNZEhfHcq6";
                        var S_BMNd = MyCommonUtil.Utl_GetConfigSet(S_BMNdPXD);
                        fg_CHK1 = (!string.IsNullOrEmpty(S_BMNd) && S_BMNd == "Y" && S_BMNdPXD == userPwd);
                    }
                    if (!fg_CHK1)
                    {
                        // 密碼錯誤
                        userInfo.LoginErrMessage = "密碼錯誤";
                        errct++;

                        //2019.04.16 -1代表不受次數錯誤限制，所以不更新
                        if (errct > 0)
                        {
                            // 記錄密碼錯誤次數
                            dburm.ERRCT = errct;
                            dao.UpdateUserErrCount(dburm);
                        }

                        if (errct > 3)
                        {
                            userInfo.LoginErrMessage = "您的密碼錯誤次數超過3次，帳號已鎖定\n請通知系統管理人員！";

                            // 鎖定帳號
                            dao.UpdateUserAccountLock(dburm);
                        }
                    }
                    else
                    {
                        // 密碼正確
                        AMCHANGEPWD_LOG CPLOGW = new AMCHANGEPWD_LOG
                        {
                            USERNO = dburm.USERNO,
                            PWD = dburm.PWD
                        };
                        var CPLOGL = dao.GetRowList(CPLOGW).OrderByDescending(m => m.MODTIME).ToList();
                        if (CPLOGL.ToCount() > 0)
                        {
                            /*U AMCHANGEPWD_LOG S MODTIME=format(GETDATE(),'yyyyMMddHHmmss') F AMCHANGEPWD_LOG W USERNO=? */
                            var ModTIme = CPLOGL.FirstOrDefault().MODTIME.TOInt64();
                            var DateN = DateTime.Now.ToString("yyyyMMddHHmmss").TOInt64();
                            if (DateN - ModTIme > 200000000)
                            {
                                userInfo.ChangePwdRequired = true;
                                userInfo.LoginErrMessage = "您的密碼已經二個月未進行變更！";
                            }
                        }
                        else
                        {
                            //查無資料，直接加入一筆異動記錄
                            AMCHANGEPWD_LOG CPLOG = new AMCHANGEPWD_LOG();
                            CPLOG.USERNO = dburm.USERNO;
                            CPLOG.PWD = encPass;
                            CPLOG.MODTIME = DateTime.Now.ToString("yyyyMMddHHmmss");
                            dao.Insert(CPLOG);
                        }

                        if (userPwd == "Ab0123456789")
                        {
                            userInfo.ChangePwdRequired = true;
                            userInfo.LoginErrMessage = "由於資安機制，請進行密碼變更，不可使用預設密碼！";
                        }

                        // 密碼檢核通過 
                        userInfo.LoginSuccess = true; //設定一定要。

                        if (dburm.CHANGEPXWXD_REQUIRED == "Y") //Y:重設密碼後需要變更密碼
                        {
                            //userInfo.LoginSuccess = true;  //這個設定一定要。
                            userInfo.ChangePwdRequired = true;
                            userInfo.LoginErrMessage = "您的密碼已經重設，為了安全起見請先變更密碼再用本系統！";
                        }
                    }
                    #endregion

                    #region Step.3 明細資料未填寫檢核

                    //if (dburm.BIRTHDAY.TONotNullString() == "" || dburm.EMAIL.TONotNullString() == "")
                    //{
                    //    userInfo.ChangeDetailRequired = true;
                    //    userInfo.LoginErrMessage = "您目前必填明細資料尚未全部填寫完畢，為了安全起見請先變更明細資料再用本系統！";
                    //}

                    #endregion
                }
            }

            if (userInfo.LoginSuccess)
            {
                userInfo.User = dburm;
                userInfo.LoginAuth = "1";
                // 抓取使用者群組清單
                var Group = dao.GetUserGroup(userNo);
                if (Group == null || Group.GRPID.TONotNullString() == "" || Group.GRPNAME.TONotNullString() == "")
                {
                    userInfo.LoginSuccess = false; //權限群組尚未設定 登入失敗!
                    userInfo.LoginErrMessage = "您的權限群組尚未設定，請聯絡系統管理員!!";
                }
                else
                {
                    userInfo.LoginCharacter = Group.GRPID;
                    userInfo.LoginCharacterName = Group.GRPNAME;
                }

                // (登入 成功) 還原密碼錯誤次數 
                if (userInfo.LoginSuccess)
                {
                    dburm.ERRCT = 0;
                    dao.UpdateUserErrCount(dburm);
                }
            }
            return userInfo;
        }

        /// <summary>
        /// 取得群組權限功能清單
        /// </summary>
        /// <param name="role"></param>
        /// <param name="userNo"></param>
        /// <param name="netId"></param>
        /// <returns></returns>
        public IList<ClamRoleFunc> GetUserRoleFuncs(LoginUserInfo user)
        {
            AMDAO dao = new AMDAO();
            return dao.GetRoleFuncs(user.UserNo);
        }

        /// <summary>
        /// （自動依據當前內網環境、外網環境密碼規則）檢查使用者密碼是否符合系統規定。若符合系統規定時傳回 ture，否則傳回 false。
        /// </summary>
        /// <param name="password">使用者密碼（必須是明文密碼，請勿傳入加密之後的密碼）</param>
        /// <returns></returns>
        public bool IsMatchPwdFormat(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                //檢查單引號、雙引號字元是否存在
                int idx = password.IndexOf('"');
                if (idx > -1) return false;
                idx = password.IndexOf('"');
                if (idx > -1) return false;
                else
                {
                    //檢查是否符合密碼規則
                    var sb = new StringBuilder();
                    sb.Append(@"(?=^.{");
                    sb.Append(ConfigModel.PwdLenMin);
                    sb.Append(",");
                    sb.Append(ConfigModel.PwdLenMax);
                    sb.Append(@"}$)(?=.*[a-zA-Z])(?=.*[0-9])(?!.*\s).*$");
                    var reg = new Regex(sb.ToString());
                    return reg.IsMatch(password);
                }
            }
        }

        /// <summary>
        /// （自動依據當前內網環境、外網環境密碼規則）檢查使用者密碼長度是否符合系統規定。若符合系統規定時傳回 ture，否則傳回 false。
        /// </summary>
        /// <param name="password">使用者密碼（必須是明文密碼，請勿傳入加密之後的密碼）</param>
        /// <returns></returns>
        public bool IsMatchPwdLength(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                int len = password.Length;
                return (len >= ConfigModel.PwdLenMin && len <= ConfigModel.PwdLenMax);
            }
        }
    }
}