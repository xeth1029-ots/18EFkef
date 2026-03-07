using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;
using System.Net.Mail;
using WKEFSERVICE.Areas.AM.Models;
using Omu.ValueInjecter;
using System.Web;
using System.IO;
using WKEFSERVICE.Commons;
using Turbo.DataLayer;

namespace WKEFSERVICE.DataLayers
{
    public class AMDAO : BaseDAO
    {
        /// <summary>共用預設上傳路徑：/ConfigModel.WKEF/Uploads/</summary>
        public readonly string UploadPathCommon = ConfigModel.WKEF + "/Uploads/";
        /// <summary>Teacher 預設上傳路徑：/ConfigModel.WKEF/Uploads/TeacherDatas/</summary>
        public readonly string UploadPathTeacher = ConfigModel.WKEF + "/Uploads/TeacherDatas/";
        /// <summary>Teacher 預設上傳路徑：/ConfigModel.WKEF/Uploads/TeacherDatasReport/</summary>
        public readonly string UploadPathTeacherReport = ConfigModel.WKEF + "/Uploads/TeacherDatasReport/";
        /// <summary>C104M 預設上傳路徑：/ConfigModel.WKEF/Uploads/AM-C104M/</summary>
        public readonly string UploadPathC104M = ConfigModel.WKEF + "/Uploads/AM-C104M/";
        /// <summary>C107M 預設上傳路徑：/ConfigModel.WKEF/Uploads/AM-C107M/</summary>
        public readonly string UploadPathC107M = ConfigModel.WKEF + "/Uploads/AM-C107M/";
        /// <summary>C301M 預設上傳路徑：/ConfigModel.WKEF/Uploads/AM-C301M/</summary>
        public readonly string UploadPathC301M = ConfigModel.WKEF + "/Uploads/AM-C301M/";

        /// <summary>
        /// 以指定的 UserNO 取得使用者帳號資料
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public ClamUser GetUser(string userNo)
        {
            if (string.IsNullOrEmpty(userNo)) return null;
            Hashtable parms = new Hashtable { ["USERNO"] = userNo };
            return base.QueryForObject<ClamUser>("AM.getClamUser", parms);
        }

        /// <summary>
        /// 取得使用者群組角色資訊
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public ClamUserGroup GetUserGroup(string userNo)
        {
            Hashtable parms = new Hashtable { ["USERNO"] = userNo };
            return base.QueryForObject<ClamUserGroup>("AM.getClamUserGroup", parms);
        }

        /// <summary>取得角色群組權限功能清單-AMGMAPM</summary>
        /// <param name="examKind">檢定類別ID</param>
        /// <param name="role">角色群組ID</param>
        /// <param name="userNo">使用者ID</param>
        /// <param name="netID"></param>
        /// <returns></returns>
        public IList<ClamRoleFunc> GetRoleFuncs(string userNo)
        {
            Hashtable parms = new Hashtable { ["USERNO"] = userNo };
            return base.QueryForListAll<ClamRoleFunc>("AM.getClamGroupFuncs", parms);
        }

        /// <summary>
        /// 更新登入密碼錯誤次數
        /// </summary>
        /// <param name="dburm"></param>
        public void UpdateUserErrCount(AMDBURM dburm)
        {
            AMDBURM where = new AMDBURM { USERNO = dburm.USERNO };
            AMDBURM upd = new AMDBURM { USERNO = dburm.USERNO, ERRCT = dburm.ERRCT };
            base.Update<AMDBURM>(upd, where, where);
        }

        /// <summary>
        /// 鎖定使用者帳號
        /// </summary>
        /// <param name="dburm"></param>
        public void UpdateUserAccountLock(AMDBURM dburm)
        {
            AMDBURM where = new AMDBURM { USERNO = dburm.USERNO };
            AMDBURM upd = new AMDBURM { USERNO = dburm.USERNO, ERRCT = 0, AUTHSTATUS = "8" };
            base.Update<AMDBURM>(upd, where, where);
        }

        public bool SaveChangePwd_Guid(string USERNO, string EMAIL, string URL)
        {
            Guid g = Guid.NewGuid();
            var AGUID_L = new List<AMCHANGEPWD_GUID>();
            var AGUID_W = new AMCHANGEPWD_GUID { GUID = g.TONotNullString() };
            AGUID_L = base.GetRowList(AGUID_W).ToList();

            while (AGUID_L.ToCount() > 0)
            {
                g = Guid.NewGuid();
                AGUID_W.GUID = g.TONotNullString();
                AGUID_L = base.GetRowList(AGUID_W).ToList();
            }

            AMCHANGEPWD_GUID CPGUID = new AMCHANGEPWD_GUID();
            CPGUID.USERNO = USERNO;
            CPGUID.GUID = g.TONotNullString();
            CPGUID.GUIDYN = "N";
            CPGUID.MODTIME = DateTime.Now.ToString("yyyyMMddHHmmss");
            base.Insert(CPGUID);

            try
            {
                //var EMAIL = "johnnydai@turbotech.com.tw";
                var url = string.Concat(URL, "?message=", g);

                //string sBody = "請點選下方連結網址以更改密碼：" + url;
                string sBody =
                    string.Concat("親愛的會員 ", USERNO, " 您好，", "\r\n\r\n") +
                    "請點選下列連結重新設定您於【關鍵就業力課程網站】的登入密碼，無法點選時請複製下列網址至您的瀏覽器網址列。\r\n\r\n" +
                    //"<font color='red'>" + 
                    string.Concat("注意：此連結的有效期限為24小時，24小時後認證連結將自動失效。", "\r\n\r\n") +
                    //"</font>" + 
                    string.Concat(url, "\r\n\r\n") +
                    "感謝您的協助驗證\r\n" +
                    "本信件為系統自動寄發，請勿直接回覆！\r\n" +
                    "若有任何問題，請與系統管理員聯絡!\r\n" +
                    "====================================================\r\n" +
                    "勞動部勞動力發展署   關鍵就業力課程網站\r\n";

                MailMessage mailmessage = CommonsServices.NewMail(null, EMAIL, "關鍵就業力課程網站-密碼修改驗證信件", sBody, (string)null, false);

                MailSentResult mailsentresult = ConfigModel.UseSendMailws ? CommonsServices.SendMailws(mailmessage) : CommonsServices.SendMail(mailmessage);

                return mailsentresult.IsSuccess;
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                string exg = ex.ToString();
            }
            return false;
        }

        public Hashtable GetBackendAddr(string PRGID)
        {
            Hashtable parms = new Hashtable { ["PRGID"] = PRGID };
            return base.QueryForObject<Hashtable>("AM.GetBackendAddr", parms);
        }

        public string GetBackendAddrName3(string CurrentUrl)
        {
            //string CurrentUrl = new Uri(Request.Url.AbsoluteUri).PathAndQuery;
            string Name3 = null;
            string ConfigModelWKEF = (ConfigModel.WKEF == "") ? "20221128" : ConfigModel.WKEF;
            string Url = "";
            string[] SplitUrl = CurrentUrl.Replace(ConfigModelWKEF, "").Split('/');
            if (SplitUrl.ToCount() >= 3) { Url = SplitUrl[1] + "/" + SplitUrl[2]; }
            string[] CheckList = new string[] { "Backend/indexTeacher", "Backend/indexBranch", "Backend/indexAdmin" };
            bool isHome = CheckList.Contains(Url);
            if (isHome) { return Name3; }
            //string Name1 = ""; //string Name2 = ""; //AMDAO dao = new AMDAO();
            var GetAddr = this.GetBackendAddr(Url);
            if (GetAddr != null)
            {
                //Name1 = GetAddr["Name1"].TONotNullString(); //Name2 = GetAddr["Name2"].TONotNullString();
                Name3 = GetAddr["Name3"].TONotNullString();
                return Name3;
            }
            return Name3;
        }

        /// <summary>
        /// 傳入使用者輸入的密碼明文, 加密後回傳
        /// </summary>
        /// <param name="usePwd"></param>
        /// <returns></returns>
        public string EncPassword(string userPwd)
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
        /// AM/C101M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C101MGridModel> QueryC101M(C101MFormModel parms)
        {
            IList<C101MGridModel> rst = null;

            // 帳號長度超過2碼(不含2碼)，使用like搜尋方式
            if (!string.IsNullOrEmpty(parms.USERNO) && parms.USERNO.Length > 2)
            {
                rst = base.QueryForList<C101MGridModel>("AM.queryC101M_UN3", parms);
            }
            else
            {
                rst = base.QueryForList<C101MGridModel>("AM.queryC101M", parms);
            }
            return rst;
        }

        /// <summary>
        /// AM/C101M Append
        /// </summary>
        /// <param name="model"></param>
        public void AppendC101M(C101MDetailModel model)
        {
            SessionModel sm = SessionModel.Get();
            AMDBURM InsModel = new AMDBURM();
            InsModel.InjectFrom(model);
            int ins = base.Insert(InsModel);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, ModifyState.SUCCESS, "AM/C101M", InsModel, null, "[AMDBURM]");
            // 如果是申請教師帳號，則順道也新增教師資料
            if (model.GRPID == "2")  // 2.教師使用者
            {
                Teacher InsTeacher = new Teacher
                {
                    ACCOUNT = model.USERNO,
                    IDNO = model.IDNO,
                    TeacherName = model.USERNAME,
                    UnitCode = model.UNITID,
                    Birthday = model.BIRTHDAY,
                    Email = model.EMAIL,
                    Online = "N",
                    CreatedAccount = sm.UserInfo.UserNo,
                    CreatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
                };
                int rtn = base.Insert(InsTeacher);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (rtn >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C101M", InsTeacher, null, "[Teacher]");
            }
        }

        /// <summary>
        /// AM/C101M Update
        /// </summary>
        /// <param name="model"></param>
        public void UpdateC101M(C101MDetailModel model)
        {
            SessionModel sm = SessionModel.Get();
            AMDBURM WhrModel = new AMDBURM { USERNO = model.USERNO };
            AMDBURM UpdModel = new AMDBURM();
            UpdModel.InjectFrom(model);

            ClearFieldMap cfmModel = new ClearFieldMap();
            cfmModel.Add((AMDBURM x) => x.AUTHDATEE);

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C101M", base.GetRow(WhrModel), WhrModel, "[AMDBURM]");
            int upd = base.Update(UpdModel, WhrModel, cfmModel);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (upd == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C101M", UpdModel, WhrModel, "[AMDBURM]");

            // 如果是申請教師帳號，則順道也更新教師資料
            if (model.GRPID == "2")  // 2.教師使用者
            {
                Hashtable parms = new Hashtable
                {
                    ["ACCOUNT"] = model.USERNO,
                    ["IDNO"] = model.IDNO,
                    ["TeacherName"] = model.USERNAME,
                    ["UnitCode"] = model.UNITID,
                    ["Birthday"] = model.BIRTHDAY,
                    ["Email"] = model.EMAIL,
                    ["UpdatedAccount"] = sm.UserInfo.UserNo,
                    ["UpdatedDatetime"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
                };

                Hashtable tmpHash = new Hashtable { ["ACCOUNT"] = model.USERNO };
                FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C101M", base.QueryForListAll<Hashtable>("AM.LogGet__Teacher", tmpHash), tmpHash, "AM.updateC101M_Teacher");
                int rtn = base.Update("AM.updateC101M_Teacher", parms);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C101M", parms, tmpHash, "AM.updateC101M_Teacher");
            }
        }

        /// <summary>
        /// AM/C102M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C102MGridModel> QueryC102M(C102MFormModel parms)
        {
            return base.QueryForList<C102MGridModel>("AM.queryC102M", parms);
        }

        public IList<C102MSetAuthGridModel> GetC102MAuthList(C102MSetAuthModel parms)
        {
            return base.QueryForList<C102MSetAuthGridModel>("AM.getC102MAuthList", parms);
        }

        public void UpdateOrAppendC102M(IList<C102MSetAuthGridModel> parms)
        {
            base.BeginTransaction();
            try
            {
                foreach (C102MSetAuthGridModel item in parms)
                {
                    C102MSetAuthGridModel where = new C102MSetAuthGridModel
                    {
                        GRPID = item.GRPID,
                        SYSID = item.SYSID,
                        MODULES = item.MODULES,
                        SUBMODULES = item.SUBMODULES,
                        PRGID = item.PRGID
                    };
                    base.InsertOrUpdate<C102MSetAuthGridModel>(item, where);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C102M", item, where, "base.InsertOrUpdate<C102MSetAuthGridModel>");
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                base.RollBackTransaction();
                LOG.Error(ex.Message, ex);
                throw new Exception("Update Or Append fail #C102M failed: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// AM/C104M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C104MGridModel> QueryC104M(C104MFormModel parms)
        {
            return base.QueryForList<C104MGridModel>("AM.queryC104M", parms);
        }

        public C104MDetailModel DetailC104M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForObject<C104MDetailModel>("AM.detailC104M", parms);
        }

        public IList<C104MAttachedsModel> DetailAttachedsC104M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForListAll<C104MAttachedsModel>("AM.detailAttachedsC104M", parms);
        }

        public void InsertOrUpdateC104M(C104MDetailModel model)
        {
            base.BeginTransaction();
            try
            {
                if (model.IsNew)
                {
                    // 主檔 MsgPost
                    MsgPost Ins = new MsgPost();
                    Ins.InjectFrom(model);
                    int MstKey = base.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (MstKey >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C104M", Ins, null, "[MsgPost]");
                    // 明細 MsgPostAttached
                    if (model.Attacheds.ToCount() > 0)
                    {
                        foreach (var row in model.Attacheds)
                        {
                            MsgPostAttached where = new MsgPostAttached() { Seq = row.Seq };
                            MsgPostAttached Upd = new MsgPostAttached();
                            Upd.InjectFrom(row);
                            Upd.MsgPostSeq = MstKey;
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C104M", base.GetRow(where), where, "[MsgPostAttached]");
                            int UpdInt = base.Update(Upd, where);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C104M", Upd, where, "[MsgPostAttached]");
                        }
                    }
                }
                else
                {
                    // 主檔 MsgPost
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C104M", base.QueryForListAll<Hashtable>("AM.LogGet__MsgPost", model), model, "AM.updateC104M_MsgPost");
                    int UpdInt = base.Update("AM.updateC104M_MsgPost", model);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C104M", model, model, "AM.updateC104M_MsgPost");
                    // 明細 MsgPostAttached
                    // => UPDATE 不需要，已經即時異動了
                }
                // 明細 MsgPostAttached 移除 isActive = False
                if (model.Attacheds.ToCount() > 0)
                {
                    foreach (var row in model.Attacheds.Where(x => x.isActive == false).ToList())
                    {
                        MsgPostAttached where2 = new MsgPostAttached() { Seq = row.Seq };
                        FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C104M", base.GetRow(where2), where2, "[MsgPostAttached]");
                        int DelInt = base.Delete<MsgPostAttached>(where2);
                        FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (DelInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C104M", null, where2, "[MsgPostAttached]");
                        // 移除實體檔案
                        string UploadPath = HttpContext.Current.Server.MapPath(this.UploadPathC104M);
                        string FullFileName = UploadPath + row.FileNameNew;
                        if (System.IO.File.Exists(FullFileName))
                        {
                            System.IO.File.Delete(FullFileName);
                            FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C104M", row.FileNameOrg.TONotNullString(), row.FileNameNew.TONotNullString());
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                base.RollBackTransaction();
                LOG.Error(ex.Message, ex);
                throw new Exception("Insert or Update C104M failed: " + ex.Message, ex);
            }
        }

        public void DeleteC104M(string Seq, string UserNo)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq, ["UpdatedAccount"] = UserNo };
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C104M", base.QueryForListAll<Hashtable>("AM.LogGet__MsgPost", parms), parms, "AM.deleteC104M");
            var i = base.Delete("AM.deleteC104M", parms);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (i == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C104M", null, parms, "AM.deleteC104M");
        }

        /// <summary>
        /// AM/C105M Query-1
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<C105MGridModel> QueryC105M_1(C105MFormModel parms)
        {
            return base.QueryForList<C105MGridModel>("AM.queryC105M_1", parms);
        }
        /// <summary>
        /// AM/C105M Query-2
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<C105MGridModel> QueryC105M_2(C105MFormModel parms)
        {
            return base.QueryForList<C105MGridModel>("AM.queryC105M_2", parms);
        }
        /// <summary>
        /// AM/C105M Query-3
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<C105MGridModel> QueryC105M_3(C105MFormModel parms)
        {
            return base.QueryForList<C105MGridModel>("AM.queryC105M_3", parms);
        }
        /// <summary>匯出每月登入異常日誌</summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<C105MGrid2Model> QueryC105M_1B(C105MFormModel parms)
        {
            return base.QueryForList<C105MGrid2Model>("AM.queryC105M_1B", parms);
        }

        /// <summary>
        /// AM/C107M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C107MGridModel> QueryC107M(C107MFormModel parms)
        {
            return base.QueryForList<C107MGridModel>("AM.queryC107M", parms);
        }

        public C107MDetailModel DetailC107M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForObject<C107MDetailModel>("AM.detailC107M", parms);
        }

        public IList<C107MAttachedsModel> DetailAttachedsC107M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForListAll<C107MAttachedsModel>("AM.detailAttachedsC107M", parms);
        }

        public void InsertOrUpdateC107M(C107MDetailModel model)
        {
            base.BeginTransaction();
            try
            {
                if (model.IsNew)
                {
                    // 主檔 Notices
                    Notices Ins = new Notices();
                    Ins.InjectFrom(model);
                    int MstKey = base.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (MstKey >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C107M", Ins, null, "[Notices]");
                    // 明細 NoticesAttached
                    if (model.Attacheds.ToCount() > 0)
                    {
                        foreach (var row in model.Attacheds)
                        {
                            NoticesAttached where = new NoticesAttached() { Seq = row.Seq };
                            NoticesAttached Upd = new NoticesAttached();
                            Upd.InjectFrom(row);
                            Upd.NoticesSeq = MstKey;
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C107M", base.GetRow(where), where, "[NoticesAttached]");
                            int UpdInt = base.Update(Upd, where);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C107M", Upd, where, "[NoticesAttached]");
                        }
                    }
                }
                else
                {
                    // 主檔 Notices
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C107M", base.QueryForListAll<Hashtable>("AM.LogGet__Notices", model), model, "AM.updateC107M_Notices");
                    int UpdInt = base.Update("AM.updateC107M_Notices", model);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C107M", model, model, "AM.updateC107M_Notices");
                    // 明細 NoticesAttached
                    // => UPDATE 不需要，已經即時異動了
                }
                // 明細 NoticesAttached 移除 isActive = False
                if (model.Attacheds.ToCount() > 0)
                {
                    foreach (var row in model.Attacheds.Where(x => x.isActive == false).ToList())
                    {
                        NoticesAttached where2 = new NoticesAttached() { Seq = row.Seq };
                        FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", base.GetRow(where2), where2, "[NoticesAttached]");
                        int DelInt = base.Delete<NoticesAttached>(where2);
                        FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (DelInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C107M", null, where2, "[NoticesAttached]");
                        // 移除實體檔案
                        string UploadPath = HttpContext.Current.Server.MapPath(this.UploadPathC107M);
                        string FullFileName = UploadPath + row.FileNameNew;
                        if (System.IO.File.Exists(FullFileName))
                        {
                            System.IO.File.Delete(FullFileName);
                            FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", row.FileNameOrg.TONotNullString(), row.FileNameNew.TONotNullString());
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                base.RollBackTransaction();
                LOG.Error(ex.Message, ex);
                throw new Exception("Insert or Update C107M failed: " + ex.Message, ex);
            }
        }

        public void DeleteC107M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };

            var listNoticesAttached = base.GetRowList<NoticesAttached>(new NoticesAttached() { NoticesSeq = i_Seq }).ToList();
            foreach (var naRow in listNoticesAttached)
            {
                string UploadPath = HttpContext.Current.Server.MapPath(this.UploadPathC107M);
                string FullFileName = UploadPath + naRow.FileNameNew;
                if (System.IO.File.Exists(FullFileName))
                {
                    System.IO.File.Delete(FullFileName);
                    FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", naRow.FileNameOrg.TONotNullString(), naRow.FileNameNew.TONotNullString());
                }
            }

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", base.QueryForListAll<Hashtable>("AM.LogGet__NoticesAttached", parms), parms, "AM.deleteC107M_NoticesAttached");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", base.QueryForListAll<Hashtable>("AM.LogGet__Notices", parms), parms, "AM.deleteC107M_Notices");

            int i1 = base.Delete("AM.deleteC107M_NoticesAttached", parms);
            int i2 = base.Delete("AM.deleteC107M_Notices", parms);

            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", null, parms, "AM.deleteC107M_NoticesAttached");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C107M", null, parms, "AM.deleteC107M_Notices");
        }

        /// <summary>
        /// AM/C201M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C201MGridModel> QueryC201M(C201MFormModel form)
        {
            return base.QueryForList<C201MGridModel>("AM.queryC201M", form);
        }

        public C201MDetailModel DetailC201M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForObject<C201MDetailModel>("AM.detailC201M", parms);
        }

        public void SaveC201M(C201MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            // 準備儲存物件
            Teacher where = new Teacher { Seq = model.Detail.Seq };
            Teacher update = new Teacher
            {
                UpdatedAccount = sm.UserInfo.UserNo,
                UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
            };
            // 清空需設為 null 欄位
            ClearFieldMap cfmModel = new ClearFieldMap();  //cfmModel.Add((Teacher x) => x.);
            // 依區塊儲存
            string Idx = model.Detail.EditAreaIdx.TONotNullString();
            if (Idx == "1")
            {
                #region Area 1
                update.TeacherName = model.Detail.TeacherName;
                update.TeacherEName = model.Detail.TeacherEName;
                update.Sex = model.Detail.Sex;
                update.Tel = model.Detail.Tel;
                update.Phone = model.Detail.Phone;
                update.Birthday = model.Detail.Birthday;
                update.Email = model.Detail.Email;
                update.EmailWork = model.Detail.EmailWork;
                update.PostalCode1 = model.Detail.PostalCode1;
                update.PostalCode2 = model.Detail.PostalCode2;
                update.Address = model.Detail.Address;
                // 前台顯示
                update.IsShowTel = model.Detail.IsShowTel;
                update.IsShowPhone = model.Detail.IsShowPhone;
                update.IsShowBirthday = model.Detail.IsShowBirthday;
                update.IsShowEmail = model.Detail.IsShowEmail;
                update.IsShowEmailWork = model.Detail.IsShowEmailWork;
                update.IsShowAddress = model.Detail.IsShowAddress;
                // 圖檔上傳
                var file = model.Detail.Pic_FILE;
                if (file != null && file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    if (!Directory.Exists(model.Detail._tmpPicPath)) Directory.CreateDirectory(model.Detail._tmpPicPath);  // 不存在則建立
                    Random tmpNum = new Random();
                    string tmpFileName =
                        DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                        tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                        Path.GetExtension(file.FileName);                   // 副檔名
                    file.SaveAs(model.Detail._tmpPicPath + "/" + tmpFileName);
                    FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "AM/C201M", file.FileName, tmpFileName);
                    // 變更 model 欄位 - 檔名
                    update.SelfPicPath = tmpFileName;
                }
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeacherName);
                cfmModel.Add((Teacher x) => x.TeacherEName);
                cfmModel.Add((Teacher x) => x.Sex);
                cfmModel.Add((Teacher x) => x.Tel);
                cfmModel.Add((Teacher x) => x.Phone);
                cfmModel.Add((Teacher x) => x.Birthday);
                cfmModel.Add((Teacher x) => x.Email);
                cfmModel.Add((Teacher x) => x.EmailWork);
                cfmModel.Add((Teacher x) => x.PostalCode1);
                cfmModel.Add((Teacher x) => x.PostalCode2);
                cfmModel.Add((Teacher x) => x.Address);
                cfmModel.Add((Teacher x) => x.IsShowTel);
                cfmModel.Add((Teacher x) => x.IsShowPhone);
                cfmModel.Add((Teacher x) => x.IsShowBirthday);
                cfmModel.Add((Teacher x) => x.IsShowEmail);
                cfmModel.Add((Teacher x) => x.IsShowEmailWork);
                cfmModel.Add((Teacher x) => x.IsShowAddress);
                //cfmModel.Add((Teacher x) => x.SelfPicPath);
                #endregion
            }
            else if (Idx == "2")
            {
                #region Area 2
                update.EduLevelHighest = model.Detail.EduLevelHighest;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.EduLevelHighest);
                #endregion
            }
            else if (Idx == "3")
            {
                #region Area 3
                update.EduSchool1 = model.Detail.EduSchool1;
                update.EduSchool2 = model.Detail.EduSchool2;
                update.EduSchool3 = model.Detail.EduSchool3;
                update.EduDept1 = model.Detail.EduDept1;
                update.EduDept2 = model.Detail.EduDept2;
                update.EduDept3 = model.Detail.EduDept3;
                bool isEdu1 = (model.Detail.EduSchool1.TONotNullString() != "" && model.Detail.EduDept1.TONotNullString() != "");
                bool isEdu2 = (model.Detail.EduSchool2.TONotNullString() != "" && model.Detail.EduDept2.TONotNullString() != "");
                bool isEdu3 = (model.Detail.EduSchool3.TONotNullString() != "" && model.Detail.EduDept3.TONotNullString() != "");
                update.EduLevel1 = isEdu1 ? model.Detail.EduLevel1 : null;
                update.EduLevel2 = isEdu2 ? model.Detail.EduLevel2 : null;
                update.EduLevel3 = isEdu3 ? model.Detail.EduLevel3 : null;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.EduSchool1);
                cfmModel.Add((Teacher x) => x.EduSchool2);
                cfmModel.Add((Teacher x) => x.EduSchool3);
                cfmModel.Add((Teacher x) => x.EduDept1);
                cfmModel.Add((Teacher x) => x.EduDept2);
                cfmModel.Add((Teacher x) => x.EduDept3);
                cfmModel.Add((Teacher x) => x.EduLevel1);
                cfmModel.Add((Teacher x) => x.EduLevel2);
                cfmModel.Add((Teacher x) => x.EduLevel3);
                #endregion
            }
            else if (Idx == "4")
            {
                #region Area 4
                update.ServiceUnit1 = model.Detail.ServiceUnit1;
                update.JobTitle1 = model.Detail.JobTitle1;
                update.ServiceUnit2 = model.Detail.ServiceUnit2;
                update.JobTitle2 = model.Detail.JobTitle2;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.ServiceUnit1);
                cfmModel.Add((Teacher x) => x.JobTitle1);
                cfmModel.Add((Teacher x) => x.ServiceUnit2);
                cfmModel.Add((Teacher x) => x.JobTitle2);
                #endregion
            }
            else if (Idx == "5")
            {
                #region Area 5
                update.UnitCode = model.Detail.UnitCode;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.UnitCode);
                #endregion
            }
            else if (Idx == "6")
            {
                #region Area 6
                update.ExpertiseDesc = model.Detail.ExpertiseDesc;
                update.ExpertiseCode = model.Detail.ExpertiseCode;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.ExpertiseDesc);
                cfmModel.Add((Teacher x) => x.ExpertiseCode);
                #endregion
            }
            else if (Idx == "7")
            {
                #region Area 7
                update.TeachJobAbilityDC = model.Detail.TeachJobAbilityDC;
                update.TeachJobAbilityBC = model.Detail.TeachJobAbilityBC;
                update.TeachJobAbilityKC = model.Detail.TeachJobAbilityKC;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeachJobAbilityDC);
                cfmModel.Add((Teacher x) => x.TeachJobAbilityBC);
                cfmModel.Add((Teacher x) => x.TeachJobAbilityKC);
                #endregion
            }
            else if (Idx == "8")
            {
                #region Area 8
                update.TeachArea = model.Detail.TeachArea;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeachArea);
                #endregion
            }
            else if (Idx == "9")
            {
                #region Area 9
                update.TeachIndustryDetCode = model.Detail.TeachIndustryDetCode;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.TeachIndustryDetCode);
                #endregion
            }
            else if (Idx == "10")
            {
                #region Area 10
                update.WorkHistory = model.Detail.WorkHistory;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.WorkHistory);
                #endregion
            }
            else if (Idx == "11")
            {
                #region Area 11
                update.ProLicense = model.Detail.ProLicense;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.ProLicense);
                #endregion
            }
            else if (Idx == "12")
            {
                #region Area 12
                update.SelfIntroduction = model.Detail.SelfIntroduction;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.SelfIntroduction);
                #endregion
            }
            else if (Idx == "13")
            {
                #region Area 13
                update.JoinYear = model.Detail.JoinYear;
                update.ServiceUnitProperties = model.Detail.ServiceUnitProperties;
                update.PublicCore = model.Detail.PublicCore;
                update.Online = model.Detail.Online;
                update.OfflineDate = model.Detail.OfflineDate;
                update.OfflineReason = model.Detail.OfflineReason;
                update.OfflineReasonRemark = model.Detail.OfflineReasonRemark;
                update.HomePageCarousel = model.Detail.HomePageCarousel;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.JoinYear);
                cfmModel.Add((Teacher x) => x.ServiceUnitProperties);
                cfmModel.Add((Teacher x) => x.PublicCore);
                cfmModel.Add((Teacher x) => x.Online);
                cfmModel.Add((Teacher x) => x.OfflineDate);
                cfmModel.Add((Teacher x) => x.OfflineReason);
                cfmModel.Add((Teacher x) => x.OfflineReasonRemark);
                cfmModel.Add((Teacher x) => x.HomePageCarousel);
                #endregion
            }
            else if (Idx == "14")
            {
                #region Area 14
                update.IndustryAcademicType = model.Detail.IndustryAcademicType;
                // 清空需設為 null 欄位
                cfmModel.Add((Teacher x) => x.IndustryAcademicType);
                #endregion
            }

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C201M", base.GetRow(where), where, "[Teacher]");
            int rtn = base.Update(update, where, cfmModel);
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C201M", update, where, "[Teacher]");
        }

        public IList<JsonIndustry> GetJsonIndustry()
        {
            return base.QueryForListAll<JsonIndustry>("AM.getJsonIndustry", null);
        }

        /// <summary>AM/C202M - 管理者首頁>師資管理>教師授課資料：匯出師資個人授課總表</summary>
        /// <param name="parms"></param>
        /// <param name="PlanDataType"></param>
        /// <param name="GetAll"></param>
        /// <returns></returns>
        public IList<C202MGridModel> QueryC202M(C202MFormModel parms, string PlanDataType, bool GetAll)
        {
            if (PlanDataType.TONotNullString() == "") PlanDataType = "0";
            string statementId = string.Concat("AM.queryC202M_", PlanDataType);
            if (GetAll)
                return base.QueryForListAll<C202MGridModel>(statementId, parms);
            else
                return base.QueryForList<C202MGridModel>(statementId, parms);
        }

        /// <summary>
        /// AM/C301M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C301MGridModel> QueryC301M(C301MFormModel parms)
        {
            return base.QueryForList<C301MGridModel>("AM.queryC301M", parms);
        }

        public C301MDetailModel DetailC301M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForObject<C301MDetailModel>("AM.detailC301M", parms);
        }

        public IList<C301MAttachedsModel> DetailAttachedsC301M(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForListAll<C301MAttachedsModel>("AM.detailAttachedsC301M", parms);
        }

        public C301MDetailSignUpModel DetailSignUpC301M_Mst(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForObject<C301MDetailSignUpModel>("AM.detailSignUpC301M_Mst", parms);
        }

        public IList<C301MDetailSignUpListModel> DetailSignUpC301M_Det(int iSeq, int iMINROW, int iMAXROW)
        {
            Hashtable parms = new Hashtable { ["Seq"] = iSeq, ["MINROW"] = iMINROW, ["MAXROW"] = iMAXROW };
            return base.QueryForListAll<C301MDetailSignUpListModel>("AM.detailSignUpC301M_Det", parms);
        }

        private void DoMstUploadC301M(C301MDetailModel model)
        {
            var file = model.PicName_FILE;
            if (file != null && file.ContentLength > 0 && !string.IsNullOrWhiteSpace(file.FileName))
            {
                // 上傳檔案
                if (!Directory.Exists(model._tmpPicPath)) Directory.CreateDirectory(model._tmpPicPath);  // 不存在則建立
                Random tmpNum = new Random();
                string tmpFileName =
                    DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                    tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                    Path.GetExtension(file.FileName);                   // 副檔名
                file.SaveAs(model._tmpPicPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "AM/C301M", file.FileName, tmpFileName);
                // 變更 model 欄位 - 檔名
                model.PicName = tmpFileName;
            }
        }

        public void InsertOrUpdateC301M(C301MDetailModel model)
        {
            base.BeginTransaction();
            try
            {
                if (model.IsNew)
                {
                    // 主檔 Meeting 的 圖檔上傳
                    DoMstUploadC301M(model);
                    // 主檔 Meeting
                    Meeting Ins = new Meeting();
                    Ins.InjectFrom(model);
                    int MstKey = base.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (MstKey >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", Ins, null, "[Meeting]");
                    // 明細 MeetingAttached
                    if (model.Attacheds.ToCount() > 0)
                    {
                        foreach (var row in model.Attacheds)
                        {
                            MeetingAttached where = new MeetingAttached() { Seq = row.Seq };
                            MeetingAttached Upd = new MeetingAttached();
                            Upd.InjectFrom(row);
                            Upd.MeetingSeq = MstKey;
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C301M", base.GetRow(where), where, "[MeetingAttached]");
                            int UpdInt = base.Update(Upd, where);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", Upd, where, "[MeetingAttached]");
                        }
                    }
                }
                else
                {
                    // 主檔 Meeting 的 圖檔上傳
                    DoMstUploadC301M(model);
                    // 主檔 Meeting
                    FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__Meeting_All", model), model, "updateC301M_Meeting");
                    int UpdInt = base.Update("AM.updateC301M_Meeting", model);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (UpdInt >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", model, model, "updateC301M_Meeting");
                    // 明細 MeetingAttached
                    // => UPDATE 不需要，已經即時異動了
                }
                // 明細 MeetingAttached 移除 isActive = False
                if (model.Attacheds.ToCount() > 0)
                {
                    foreach (var row in model.Attacheds.Where(x => x.isActive == false).ToList())
                    {
                        MeetingAttached where2 = new MeetingAttached() { Seq = row.Seq };
                        FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.GetRow(where2), where2, "[MeetingAttached]");
                        int DelInt = base.Delete<MeetingAttached>(where2);
                        FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (DelInt == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", null, where2, "[MeetingAttached]");
                        // 移除實體檔案
                        string UploadPath = HttpContext.Current.Server.MapPath(this.UploadPathC301M);
                        string FullFileName = UploadPath + row.FileNameNew;
                        if (System.IO.File.Exists(FullFileName))
                        {
                            System.IO.File.Delete(FullFileName);
                            FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", row.FileNameOrg.TONotNullString(), row.FileNameNew.TONotNullString());
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                base.RollBackTransaction();
                throw new Exception("Insert or Update C301M failed: " + ex.Message, ex);
            }
        }

        public void DeleteC301M(string Seq, string UserNo)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            var i = 0;

            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend_All", parms), parms, "AM.deleteC301M_MeetingAttend_All");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingSignUp_All", parms), parms, "AM.deleteC301M_MeetingSignUp_All");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttached_All", parms), parms, "AM.deleteC301M_MeetingAttached_All");
            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__Meeting_All", parms), parms, "AM.deleteC301M_Meeting_All");

            i = base.Delete("AM.deleteC301M_MeetingAttend_All", parms);
            i = base.Delete("AM.deleteC301M_MeetingSignUp_All", parms);
            i = base.Delete("AM.deleteC301M_MeetingAttached_All", parms);
            i = base.Delete("AM.deleteC301M_Meeting_All", parms);

            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", null, parms, "AM.deleteC301M_MeetingAttend_All");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", null, parms, "AM.deleteC301M_MeetingSignUp_All");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", null, parms, "AM.deleteC301M_MeetingAttached_All");
            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", null, parms, "AM.deleteC301M_Meeting_All");
        }

        public IList<C301MTeacherPopupListModel> GetTeacherListC301M(string TeacherSearch, string UnitCode)
        {
            Hashtable parms = new Hashtable { ["TeacherName"] = TeacherSearch, ["UnitCode"] = UnitCode };
            var model = base.QueryForListAll<C301MTeacherPopupListModel>("AM.getTeacherListC301M", parms);
            return model;
            //base.PageSize = 5;
            //base.m_default_pagesize = 5;
            //return base.PagingList("AM.getTeacherListC301M", model);
        }

        public void InsertOrUpdateC301M_SignUp(C301MDetailSignUpModel model)
        {
            base.BeginTransaction();
            try
            {
                foreach (var item in model.SignUpList)
                {
                    if (item.isActive)
                    {
                        if (item.Seq.TONotNullString() == "")
                        {
                            // Insert - MeetingSignUp
                            MeetingSignUp ins = new MeetingSignUp();
                            ins.InjectFrom(item);
                            ins.MeetingSeq = model.Seq;
                            int res = base.Insert(ins);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (res >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", ins, null, "[MeetingSignUp]");

                            // Insert - MeetingAttend
                            MeetingAttend ins2 = new MeetingAttend();
                            ins2.InjectFrom(ins);
                            ins2.Attend = "N";
                            ins2.TestPassed = "N";
                            int res2 = base.Insert(ins2);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (res2 >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", ins2, null, "[MeetingAttend]");
                        }
                    }
                    else
                    {
                        if (item.Seq.TONotNullString() != "")
                        {
                            // Delete - MeetingSignUp
                            Hashtable del = new Hashtable { ["Seq"] = item.Seq };
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingSignUp2", del), del, "AM.deleteC301M_MeetingSignUp");
                            int res = base.Delete("AM.deleteC301M_MeetingSignUp", del);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (res == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", null, del, "AM.deleteC301M_MeetingSignUp");

                            // Delete - MeetingAttend
                            Hashtable del2 = new Hashtable { ["MeetingSeq"] = item.MeetingSeq, ["TeacherAccount"] = item.TeacherAccount };
                            FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend", del2), del2, "AM.deleteC301M_MeetingAttend");

                            int res2 = base.Delete("AM.deleteC301M_MeetingAttend", del2);
                            FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.DELETE, (res2 == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", null, del2, "AM.deleteC301M_MeetingAttend");
                        }
                    }
                }
                base.CommitTransaction();
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message, ex);
                base.RollBackTransaction();
                throw new Exception("Insert or Update C301M SignUpList failed: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// AM/C302M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C302MGridModel> QueryC302M(C302MFormModel parms)
        {
            return base.QueryForList<C302MGridModel>("AM.queryC302M", parms);
        }

        public C302MDetailModel DetailC302M_Mst(string Seq)
        {
            Hashtable parms = new Hashtable { ["Seq"] = Seq };
            return base.QueryForObject<C302MDetailModel>("AM.detailC302M_Mst", parms);
        }

        public IList<C302MDetailRowsModel> DetailC302M_Det(int iSeq, int iMINROW, int iMAXROW)
        {
            Hashtable parms = new Hashtable { ["Seq"] = iSeq, ["MINROW"] = iMINROW, ["MAXROW"] = iMAXROW };
            return base.QueryForListAll<C302MDetailRowsModel>("AM.detailC302M_Det", parms);
        }

        public void SaveC302M(C302MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            if (model.Detail.DetailRows == null) { return; }

            foreach (var item in model.Detail.DetailRows)
            {
                item.UpdatedAccount = sm.UserInfo.UserNo;
                item.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

                Hashtable tmpHash = new Hashtable { ["Seq"] = item.Seq };
                string s_statementId = (model.Detail.MeetingType == "1") ? "AM.updateC302M_MeetingAttend_1" : (model.Detail.MeetingType == "2") ? "AM.updateC302M_MeetingAttend_2" : "AM.updateC302M_MeetingAttend";

                FuncLogAdd.Do(ModifyEvent.BEFORE, ModifyType.UPDATE, ModifyState.SUCCESS, "AM/C302M", base.QueryForListAll<Hashtable>("AM.LogGet__MeetingAttend2", tmpHash), tmpHash, s_statementId);
                int rtn = base.Update(s_statementId, item);
                FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, (rtn == 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C302M", item, tmpHash, s_statementId);

                //強制計算（不分轄區了）
                string s_statementId2 = "AM.updateC302M_MeetingAttend_MadForced";
                int rtn2 = base.Update(s_statementId2, item);
            }

        }

        public IList<NewsModel> GetNewsDatas(string PostType, string PostTo)
        {
            Hashtable parms = new Hashtable { ["PostType"] = PostType, ["PostTo"] = PostTo };
            SessionModel sm = SessionModel.Get();
            parms["CreatedUnit"] = sm.UserInfo.User.UNITID;
            return base.QueryForListAll<NewsModel>("AM.getNewsDatas", parms);
        }

        public IList<MeetingsModel> GetMeetingsDatas(string MeetingType)
        {
            Hashtable parms = new Hashtable { ["MeetingType"] = MeetingType };
            return base.QueryForListAll<MeetingsModel>("AM.getMeetingsDatas", parms);
        }

        public IList<AuditReportsModel> GetAuditReportsDatas(string AuditStatus, string UnitCode)
        {
            Hashtable parms = new Hashtable { ["AuditStatus"] = AuditStatus, ["UnitCode"] = UnitCode };
            return base.QueryForListAll<AuditReportsModel>("AM.getAuditReportsDatas", parms);
        }

        public IList<WKEFSERVICE.Areas.A1.Models.C102MGridModel> GetNewsDatas(WKEFSERVICE.Areas.A1.Models.C102MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            form.CreatedUnit = sm.UserInfo.User.UNITID;
            return base.QueryForList<WKEFSERVICE.Areas.A1.Models.C102MGridModel>("AM.getNewsDatas2", form);
        }

        /// <summary>
        /// AM/C401M Query for Export
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<C401MGridModel> QueryC401M(C401MFormModel model)
        {
            Hashtable parms = new Hashtable { ["Unit"] = model.Unit, ["Online"] = model.Online };
            return base.QueryForListAll<C401MGridModel>("AM.queryC401M", parms);
        }

        /// <summary>
        /// AM/C901M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C901MGridModel> QueryC901M(C901MFormModel form)
        {
            return base.QueryForList<C901MGridModel>("AM.queryC901M", form);
        }

        /// <summary>
        /// AM/C902M Query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<C902MGridModel> QueryC902M(C902MFormModel form)
        {
            //UnitCode/Year/Teacher_Name/FirstTime_S/FirstTime_E/AuditStatus
            return base.QueryForList<C902MGridModel>("AM.queryC902M", form);
        }

        public C902MDetailModel DetailC902M(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return null; /*throw new ArgumentNullException("不可為空。");*/ }

            Hashtable parms = new Hashtable { ["Seq"] = i_Seq };
            return base.QueryForObject<C902MDetailModel>("AM.detailC902M", parms);
        }

        /// <summary>當年度評核結果總表</summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public IList<ReviewReportD1> GetReviewReportD1(Hashtable ht)
        {
            Hashtable parms = ht; //Hashtable parms = new Hashtable();
            return base.QueryForListAll<ReviewReportD1>("AM.getReviewReportData1", parms);
        }

        public IList<C403MGridModel1> QueryC403M1(string Year)
        {
            Hashtable parms = new Hashtable { ["ACTUALYEAR"] = Year };
            return base.QueryForListAll<C403MGridModel1>("AM.QueryC403M1", parms);
        }
        public IList<C403MGridModel2> QueryC403M2(string Year)
        {
            Hashtable parms = new Hashtable { ["Year"] = Year };
            return base.QueryForListAll<C403MGridModel2>("AM.QueryC403M2", parms);
        }

        public IList<C403MGridModel3> QueryC403M3(string Year)
        {
            Hashtable parms = new Hashtable { ["Year"] = Year };
            return base.QueryForListAll<C403MGridModel3>("AM.QueryC403M3", parms);
        }

    }
}