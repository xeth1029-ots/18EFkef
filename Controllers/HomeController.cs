using System.IO;
using System.Linq;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using System;
using WKEFSERVICE.Models.Entities;
using log4net;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Services;
using System.Text;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Web;
using System.Collections;

namespace WKEFSERVICE.Controllers
{
    /// <summary>
    /// 首頁
    /// </summary>
    public class HomeController : Controller
    {
        protected static readonly ILog LOG = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string GUID_ATTACKS_CODE = "GUID_ATTACKS_CODE";
        /// <summary> 首頁 </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            if (Session[GUID_ATTACKS_CODE] == null || string.IsNullOrEmpty((string)Session[GUID_ATTACKS_CODE]) || sm.CHECK_GUID_attacks || (string)Session[GUID_ATTACKS_CODE] != sm.GUID_attacks)
            {
                sm.GUID_attacks = Guid.NewGuid().TONotNullString();
                Session[GUID_ATTACKS_CODE] = sm.GUID_attacks;
            }

            A1DAO dao = new A1DAO();
            var tmpList = dao.QueryC102M(null).Take(5).ToList();

            HomeModel model = new HomeModel();
            model.NewsGrid = new List<NewsModel>();
            foreach (var item in tmpList)
            {
                NewsModel newObj = new NewsModel();
                newObj.InjectFrom(item);
                model.NewsGrid.Add(newObj);
            }

            //model.TeachersGrid = dao.GetTeacher_HomePageCarousel(100, false);  // 先給他顯示前100位
            model.TeachersGrid = dao.GetTeacher_HomePageCarousel(10, true);  // 隨機10位

            return View("Index", model);
        }

        /// <summary>顯示 Login 登入頁面</summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            SessionModel sm = SessionModel.Get();
            if (Session[GUID_ATTACKS_CODE] == null || string.IsNullOrEmpty((string)Session[GUID_ATTACKS_CODE]) || sm.CHECK_GUID_attacks || (string)Session[GUID_ATTACKS_CODE] != sm.GUID_attacks)
            {
                sm.GUID_attacks = Guid.NewGuid().TONotNullString();
                Session[GUID_ATTACKS_CODE] = sm.GUID_attacks;
            }

            LoginModel viewModel = new LoginModel();

            ActionResult rtn = View(viewModel);

            return rtn;
        }

        /// <summary>執行 Login 登入</summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult doLogin(LoginModel parmsModel)
        {
            SHAREDAO dao = new SHAREDAO();
            ActionResult rtn;
            try
            {
                // 檢查驗證碼及輸入欄位
                this.InputValidate(parmsModel.form);

                // 因需求為 要可使用帳號或電子郵件做登入，故
                // 先拿輸入的去找 USERNO，如果找不到，換找 EMAIL，目的要抓出對應的 USERNO
                string getUserNo = "";
                AMDBURM tmpObj = dao.GetRow<AMDBURM>(new AMDBURM() { USERNO = parmsModel.form.UserNo_EMAIL });
                if (tmpObj != null)
                {
                    getUserNo = tmpObj.USERNO.TONotNullString();
                }
                else
                {
                    tmpObj = dao.GetRow<AMDBURM>(new AMDBURM() { EMAIL = parmsModel.form.UserNo_EMAIL });
                    if (tmpObj != null) getUserNo = tmpObj.USERNO.TONotNullString();
                }

                // 系統管理邏輯
                ClamService service = new ClamService();

                // Add by.Senya 221226
                // 帳密輸入錯誤後鎖定，超過15分鐘解鎖可登入
                var tmpObj2 = dao.GetRow<AMDBURM>(new AMDBURM() { USERNO = getUserNo });
                if (getUserNo != "" && tmpObj2 != null && tmpObj2.AUTHSTATUS == "8")
                {
                    Hashtable tmpParms = new Hashtable { ["username"] = getUserNo };
                    var getLockLog = dao.QueryForListAll<loginlog>("AM.GetLockLog", tmpParms).ToList();
                    DateTime TimeLock = DateTime.ParseExact(getLockLog.FirstOrDefault().createtime, "yyyy-MM-dd HH:mm:ss", null);
                    DateTime TimeNow = DateTime.Now;
                    //long tmpTime = new TimeSpan(TimeNow.Ticks - TimeLock.Ticks).TotalSeconds.TOInt64();
                    double tmpTime = new TimeSpan(TimeNow.Ticks - TimeLock.Ticks).TotalSeconds;
                    if (tmpTime >= 15 * 60)
                    {
                        // 超過15分鐘解鎖可登入
                        AMDBURM upd = new AMDBURM() { AUTHSTATUS = "1", ERRCT = 0 };
                        AMDBURM whe = new AMDBURM() { USERNO = getUserNo };
                        int tmprtn = dao.Update(upd, whe);
                        //FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.UPDATE, ModifyState.SUCCESS, "Home/Login", upd, whe, "帳號解鎖");
                    }
                }

                // 登入帳密檢核, 並取得使用者帳號及權限角色清單資料
                LoginUserInfo userInfo = service.LoginValidate(getUserNo, parmsModel.form.UserPwd);
                userInfo.LoginIP = HttpContext.Request.UserHostAddress;

                // 登入失敗, 丟出錯誤訊息
                if (!userInfo.LoginSuccess)
                {
                    throw new LoginExceptions(userInfo.LoginErrMessage);
                }

                // 將登入者資訊保存在 SessionModel 中
                SessionModel sm = SessionModel.Get();
                sm.UserInfo = userInfo;

                // 將登入者群組權限功能清單保存在 SessionModel 中
                sm.RoleFuncs = service.GetUserRoleFuncs(userInfo);

                //userInfo.ChangePwdRequired = true;  測試用

                // 登入成功時記錄登入時間
                AMDBURM newdata = new AMDBURM() { LASTLOGINTIME = DateTime.Now };
                AMDBURM where = new AMDBURM() { USERNO = getUserNo };
                dao.Update(newdata, where);

                // 寫入LOG
                // this.InsLog(getUserNo, parmsModel.form.UserPwd, "Y", "");
                SetLoginLog(HttpContext.Request, getUserNo, "LOGIN", "登入成功");

                // 若需要立即變更密碼時，就直接導向密碼變更畫面
                if (userInfo.ChangePwdRequired)
                {
                    string msg = userInfo.LoginErrMessage;
                    sm.LastErrorMessage = string.IsNullOrEmpty(msg) ? "為了安全起見，請您先變更密碼再用本系統！" : msg;
                    sm.RedirectUrlAfterBlock = Url.Action("PasswordChange", "Home", new { useCache = "2" });
                    return Index();
                    //var vm = new LoginModel();
                    //vm.form = parmsModel.form;
                    //return View("Index", vm);
                }
                else
                {
                    // 加入密碼長度不足時要顯示訊息，並直接導至密碼修改畫面
                    if (!service.IsMatchPwdLength(parmsModel.form.UserPwd))
                    {
                        //string.Concat("您的密碼長度未符合系統規定，請立即修改密碼。", "預設長度-最短:", ConfigModel.PwdLenMin, ",最長:", ConfigModel.PwdLenMax);
                        sm.LastErrorMessage = string.Concat("您的密碼長度未符合系統規定，請立即修改密碼。");
                        sm.RedirectUrlAfterBlock = Url.Action("PasswordChange", "Home", new { useCache = "2" });
                        return Index();
                        //var vm = new LoginModel();
                        //vm.form = parmsModel.form;
                        //return View("Index", vm);
                    }
                }

                rtn = RedirectToAction("Index", "Backend");
            }
            catch (LoginExceptions ex)
            {
                LOG.Info("Login(" + parmsModel.form.UserNo_EMAIL + ") Failed from " + Request.UserHostAddress + ": " + ex.Message);
                // 清除不想要 Cache POST data 的欄位
                ModelState.Remove("form.UserNo_EMAIL");
                ModelState.Remove("form.UserPwd");
                ModelState.Remove("form.ValidateCode");

                LoginModel model = new LoginModel();
                model.form.UserNo_EMAIL = parmsModel.form.UserNo_EMAIL;
                model.ErrorMessage = ex.Message;
                ModelState.AddModelError("ErrorMessage", ex.Message);  // 補丁
                rtn = View("Login", model);
                // 寫入LOG 排除驗證碼及畫面欄位未填寫
                if (!ex.Message.Contains("驗證碼輸入錯誤") && !ex.Message.Contains("請輸入 帳號及密碼 !!"))
                {
                    //this.InsLog(parmsModel.form.UserNo_EMAIL, parmsModel.form.UserPwd, "N", ex.Message);
                    SetLoginLog(HttpContext.Request, parmsModel.form.UserNo_EMAIL, "LOGINFAIL", ex.Message);
                }
                return rtn;
            }
            return rtn;
        }

        /// <summary>
        /// 變更密碼頁面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PasswordChange()
        {
            SessionModel sm = SessionModel.Get();
            LoginModel model = new LoginModel();
            model.PwdChange.IsNew = false;
            model.PwdChange.EMAIL = sm.UserInfo.User.EMAIL;
            model.PwdChange.UserNo = sm.UserInfo.User.USERNO;
            return View("PwdChange", model.PwdChange);
        }

        /// <summary>
        /// 執行 變更密碼
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult doPasswordChange(PwdChangeFormModel form)
        {
            ActionResult rtn = View("PwdChange", form);
            SessionModel sm = SessionModel.Get();
            SHAREDAO dao = new SHAREDAO();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                AMDBURM where = new AMDBURM();
                where.USERNO = form.UserNo;
                where.EMAIL = form.EMAIL;
                AMDBURM newdata = new AMDBURM();
                string encPass = this.EncPassword(form.UserPWD); //將密碼加密
                newdata.PWD = encPass;
                newdata.CHANGEPXWXD_REQUIRED = "N";

                var data = dao.GetRowList(where);

                if (data.ToCount() == 1)
                {
                    AMCHANGEPWD_LOG CPLOGW = new AMCHANGEPWD_LOG();
                    CPLOGW.USERNO = data.FirstOrDefault().USERNO;
                    var CPLOGL = dao.GetRowList(CPLOGW).OrderByDescending(m => m.MODTIME).ToList();
                    //if(CPLOGL.ToCount() > 0)
                    //{
                    var CPLOG3 = CPLOGL.Take(3).ToList();
                    var CPLOG_CK = string.Join(",", CPLOG3.Select(m => m.PWD));
                    if (CPLOG_CK.Contains(encPass))
                    {
                        sm.LastErrorMessage = "不得與前三次密碼相同，請檢查!";
                    }
                    else
                    {
                        AMCHANGEPWD_LOG CPLOG = new AMCHANGEPWD_LOG();
                        CPLOG.USERNO = data.FirstOrDefault().USERNO;
                        CPLOG.PWD = encPass;
                        CPLOG.MODTIME = DateTime.Now.ToString("yyyyMMddHHmmss");

                        dao.Insert(CPLOG);

                        dao.Update(newdata, where);

                        HomeController.SetLoginLog(HttpContext.Request, sm.UserInfo.UserNo, "LOGOUT", "登出成功");
                        Session.RemoveAll();
                        sm.LastResultMessage = "密碼重設完畢，請使用新密碼登入!!";
                        rtn = RedirectToAction("Login", "Home");
                    }
                }
                else
                {
                    sm.LastErrorMessage = "未查詢到此帳號，請聯絡管理員確認帳號是否存在!";
                }
            }
            return rtn;
        }

        /// <summary>
        /// 顯示 ForgetPwd 忘記密碼頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgetPwd()
        {
            LoginModel viewModel = new LoginModel();
            return View("ForgetPwd", viewModel);
        }

        /// <summary>
        /// 執行 ForgetPwd 忘記密碼
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult doForgetPwd(ForgetPasswdModel form)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                string IDNO = form.IDNO.TONotNullString();
                string EMAIL = form.EMAIL.TONotNullString();
                string Birthday = form.BIRTHDAY.TONotNullString();
                string ValidateCode = form.ValidateCode.TONotNullString();

                /*開始CLASM01M的sUtl_DoServerCheck的等效部分--資料檢查*/
                StringBuilder sbErrorMessage = new StringBuilder();
                AMDAO dao = new AMDAO();
                if (!CommonsServices.CheckEMail(EMAIL))
                {
                    sbErrorMessage.Append("電子郵件信箱EMail格式錯誤\n");
                }
                //AMDBURM clamdburm = dao.GetRow(new AMDBURM { USERNO = UserNo });
                AMDBURM clamdburm = dao.GetRow(new AMDBURM { IDNO = IDNO/*, EMAIL = EMAIL, BIRTHDAY = Birthday*/ });
                if (clamdburm == null)
                {
                    sbErrorMessage.Append("使用者帳號不存在，請重新輸入\n");
                }
                else
                {
                    if (!Birthday.Equals(clamdburm.BIRTHDAY))
                    {
                        sbErrorMessage.Append("使用者生日比對不正確\n");
                    }
                    if (!IDNO.Equals(clamdburm.IDNO))
                    {
                        sbErrorMessage.Append("使用者身分證號比對不正確\n");
                    }
                    if (!EMAIL.Equals(clamdburm.EMAIL))
                    {
                        sbErrorMessage.Append("使用者電子郵件信箱(EMAIL)比對不正確\n");
                    }
                    // 20201111 - 增加每日只能更改一次密碼
                    // if (UserNo != "")
                    if (clamdburm.USERNO != "")
                    {
                        AMCHANGEPWD_GUID guidw = new AMCHANGEPWD_GUID();
                        //guidw.USERNO = UserNo;
                        guidw.USERNO = clamdburm.USERNO;
                        var guidL = dao.GetRowList(guidw).Where(m => m.GUIDYN == "Y").OrderByDescending(m => m.MODTIME);
                        if (guidL.ToCount() > 0)
                        {
                            var guidF = guidL.FirstOrDefault();
                            var guidFdate = guidF.MODTIME.SubstringTo(0, 8);
                            if (guidFdate == DateTime.Now.ToString("yyyyMMdd"))
                            {
                                sbErrorMessage.Append("今日已變更密碼成功一次，請於明日再執行忘記密碼功能!\n");
                            }
                        }
                    }
                }
                if (!ValidateCode.Equals(SessionModel.Get().LoginValidateCode))
                {
                    sbErrorMessage.Append("驗證碼輸入錯誤，請重新輸入\n");
                }
                if (!string.IsNullOrEmpty(sbErrorMessage.ToString()))
                {
                    string msg = sbErrorMessage.ToString();
                    string msg2 = msg.Replace("\r\n", "，").Replace("\n", "，"); ;
                    var data = new { ERRMSG = msg, RESULT = "N", ERRMSG2 = msg2 };
                    return MyCommonUtil.BuildAjaxResult(data, false);
                }

                var IsSuccess = dao.SaveChangePwd_Guid(clamdburm.USERNO/*UserNo*/, EMAIL, Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Url.Content("~/Home/MailPasswordChange"));
                /*sendProcess的等效部分END,「確定」鍵流程順利結束*/
                if (IsSuccess)
                {
                    return MyCommonUtil.BuildAjaxResult(new { RESULT = "Y" }, true);
                }
                else
                {
                    string msg2 = "寄信失敗!!";
                    return MyCommonUtil.BuildAjaxResult(new { ERRMSG = msg2, RESULT = "N", ERRMSG2 = msg2 }, false);
                }
            }
            else
            {
                string msg = MyCommonUtil.GetModelStateErrors(ModelState, "\r\n");
                string msg2 = msg.Replace("\r\n", "，"); ;
                return MyCommonUtil.BuildAjaxResult(new { ERRMSG = msg, RESULT = "N", ERRMSG2 = msg2 }, false);
            }
            //return View("Index");
        }

        /// <summary>
        /// 變更密碼頁面 - for 認證信使用 (忘記密碼)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MailPasswordChange(string message)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            LoginModel model = new LoginModel();
            ActionResult rtn = View("ForgetPwdChange", model.ForgetPasswdChange);

            AMCHANGEPWD_GUID GID = new AMCHANGEPWD_GUID();
            GID.GUID = message;
            var GL = dao.GetRowList(GID).ToList();

            if (GL.ToCount() > 0)
            {
                GL = GL.OrderByDescending(m => m.MODTIME).ToList();
                var GI = GL.FirstOrDefault();
                var GTIME = GI.MODTIME;
                var DNOW = DateTime.Now.ToString("yyyyMMddHHmmss");
                //var DDF = DNOW.TOInt64() - GTIME.TOInt64();
                DateTime DT1 = DateTime.ParseExact(GTIME, "yyyyMMddHHmmss", null);
                DateTime DT2 = DateTime.ParseExact(DNOW, "yyyyMMddHHmmss", null);
                //long DDF = new TimeSpan(DT2.Ticks - DT1.Ticks).TotalSeconds.TOInt64();
                double DDF = new TimeSpan(DT2.Ticks - DT1.Ticks).TotalSeconds;
                if (DDF > (60 * 60 * 24))  // 24hr 86,400
                {
                    sm.LastErrorMessage = "該驗證已經過期，請使用忘記密碼重新寄發驗證信!";
                    rtn = RedirectToAction("Index");
                }

                AMCHANGEPWD_GUID GID2 = new AMCHANGEPWD_GUID();
                GID2.USERNO = GI.USERNO;
                var GL2 = dao.GetRowList(GID2).OrderByDescending(m => m.MODTIME).ToList();
                if (GL2.ToCount() > 0)
                {
                    var GI2 = GL2.FirstOrDefault();
                    if (GI2.GUID != GI.GUID || GI2.GUIDYN == "Y")
                    {
                        sm.LastErrorMessage = "該驗證已經失效，請使用忘記密碼重新寄發驗證信!";
                        rtn = RedirectToAction("Index");
                    }
                    else
                    {
                        AMDBURM userW = new AMDBURM();
                        userW.USERNO = GI.USERNO;
                        var userL = dao.GetRowList(userW);
                        if (userL.ToCount() > 0)
                        {
                            var user = userL.FirstOrDefault();
                            model.ForgetPasswdChange.EMAIL = user.EMAIL;
                            model.ForgetPasswdChange.USERNO = user.USERNO;
                            model.ForgetPasswdChange.USERNAME = user.USERNAME;
                            model.ForgetPasswdChange.GUID = GI2.GUID;
                        }
                        else
                        {
                            sm.LastErrorMessage = "輸入使用者資訊有誤!";
                            rtn = RedirectToAction("Index");
                        }
                    }
                }
            }
            else
            {
                sm.LastErrorMessage = "驗證信件，查無資料!";
                rtn = RedirectToAction("Index");
            }
            return rtn;
        }

        /// <summary>
        /// 執行 變更密碼 - for 認證信使用 (忘記密碼)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult doMailPasswordChange(ForgetPasswdChangeModel form)
        {
            ActionResult rtn = View("ForgetPwdChange", form);
            SessionModel sm = SessionModel.Get();
            SHAREDAO dao = new SHAREDAO();
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                AMDBURM where = new AMDBURM();
                where.USERNO = form.USERNO;
                where.EMAIL = form.EMAIL;
                AMDBURM newdata = new AMDBURM();
                string encPass = this.EncPassword(form.PWD); //將密碼加密
                newdata.PWD = encPass;

                var data = dao.GetRowList(where);

                if (data.ToCount() == 1)
                {
                    AMCHANGEPWD_LOG CPLOGW = new AMCHANGEPWD_LOG();
                    CPLOGW.USERNO = form.USERNO;
                    var CPLOGL = dao.GetRowList(CPLOGW).OrderByDescending(m => m.MODTIME).ToList();
                    //if(CPLOGL.ToCount() > 0)
                    //{
                    var CPLOG3 = CPLOGL.Take(3).ToList();
                    var CPLOG_CK = string.Join(",", CPLOG3.Select(m => m.PWD));
                    if (CPLOG_CK.Contains(encPass))
                    {
                        sm.LastErrorMessage = "不得與前三次密碼相同，請檢查!";
                    }
                    else
                    {
                        AMCHANGEPWD_GUID GUW = new AMCHANGEPWD_GUID { GUID = form.GUID };
                        AMCHANGEPWD_GUID GU = new AMCHANGEPWD_GUID { GUIDYN = "Y" };
                        dao.Update(GU, GUW);
                        AMCHANGEPWD_LOG CPLOG = new AMCHANGEPWD_LOG();
                        CPLOG.USERNO = form.USERNO;
                        CPLOG.GUID = form.GUID;
                        CPLOG.PWD = encPass;
                        CPLOG.MODTIME = DateTime.Now.ToString("yyyyMMddHHmmss");

                        dao.Insert(CPLOG);

                        dao.Update(newdata, where);

                        sm.LastResultMessage = "密碼重設完畢，請使用新密碼登入!!";
                        rtn = RedirectToAction("Login", "Home");
                    }
                }
                else
                {
                    sm.LastErrorMessage = "未查詢到此帳號，請聯絡管理員確認帳號是否存在!";
                }
            }
            return rtn;
        }

        /// <summary>
        /// 重新產生並回傳驗證碼圖片檔案內容
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetValidateCode()
        {
            Turbo.Commons.ValidateCode vc = new Turbo.Commons.ValidateCode();
            string vCode = vc.CreateValidateCode(4);
            SessionModel.Get().LoginValidateCode = vCode;

            MemoryStream stream = vc.CreateValidateGraphic(vCode);
            return File(stream.ToArray(), "image/jpeg");
        }

        /// <summary>
        /// 圖型驗證碼轉語音撥放頁
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VCodeAudio()
        {
            return View();
        }

        /// <summary>將當前的驗證碼轉成 Wav audio 輸出</summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetValidateCodeAudio()
        {
            string vCode = SessionModel.Get().LoginValidateCode;

            if (string.IsNullOrEmpty(vCode))
            {
                //LOG.Warn("#GetValidateCodeAudio , IsNullOrEmpty(vCode)!");
                return HttpNotFound();
            }
            else
            {
                //LOG.Debug( string.Concat("#GetValidateCodeAudio , vCode: ", vCode));
                string audioPath = HttpContext.Server.MapPath("~/Content/audio/");
                Turbo.Commons.ValidateCode vc = new Turbo.Commons.ValidateCode();
                MemoryStream stream = vc.CreateValidateAudio(vCode, audioPath);
                return File(stream.ToArray(), "audio/wav");
            }
        }

        private void InputValidate(LoginFormModel form)
        {
            if (string.IsNullOrEmpty(form.UserNo_EMAIL) || string.IsNullOrEmpty(form.UserPwd))
            {
                LoginExceptions ex = new LoginExceptions("請輸入 帳號及密碼 !!");
                throw ex;
            }
            if (string.IsNullOrEmpty(form.ValidateCode) || !form.ValidateCode.Equals(SessionModel.Get().LoginValidateCode))
            {
                LoginExceptions ex = new LoginExceptions("驗證碼輸入錯誤");
                throw ex;
            }
        }

        /// <summary>
        /// 傳入使用者輸入的密碼明文, 加密後回傳
        /// </summary>
        /// <param name="userPwd"></param>
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
        /// 將使用者登入狀態寫入LOG
        /// </summary>
        /// <param UserNo="使用者帳號"></param>
        /// <param userPwd="使用者密碼"></param>
        /// <param Status="登入狀態 成功:Y;失敗:N"></param>
        /// <param FAILREASON="登入失敗時錯誤訊息"></param>
        /// <returns></returns>
        //private void InsLog(string UserNo, string userPwd, string Status, string FAILREASON)
        //{
        //    SHAREDAO dao = new SHAREDAO();
        //    AMLOGIN SIGINLOG = new AMLOGIN();
        //    SIGINLOG.L_USERNO = UserNo;
        //    SIGINLOG.L_PWD = this.EncPassword(userPwd);  // 將密碼加密
        //    SIGINLOG.L_STATUS = Status;
        //    SIGINLOG.L_FAILREASON = FAILREASON == null ? "" : FAILREASON;
        //    SIGINLOG.L_MODIP = HttpContext.Request.UserHostAddress;
        //    SIGINLOG.L_MODTIME = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    dao.Insert(SIGINLOG);
        //}

        /// <summary>
        /// 將使用者登入狀態寫入LOG - 新版 221215
        /// </summary>
        /// <param name="userno"></param>
        /// <param name="logtype"></param>
        /// <param name="result"></param>
        [HttpPost]
        public static void SetLoginLog(HttpRequestBase req, string userno, string logtype, string result)
        {
            string userAgent = req.UserAgent;
            userAgent = userAgent.Count() <= 200 ? userAgent : userAgent.Substring(0, 200);
            string ip = req.UserHostAddress;

            loginlog Ins = new loginlog();
            Ins.username = userno;
            Ins.logtype = logtype;
            Ins.createtime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            Ins.ip = ip;
            Ins.message = result;
            Ins.useragent = userAgent;

            SHAREDAO dao = new SHAREDAO();
            dao.BeginTransaction();
            try
            {
                dao.Insert(Ins);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                LOG.Warn(ex.Message, ex);
                dao.RollBackTransaction();
                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Temp_IntroCourse() { return View("Temp_IntroCourse/IntroCourse"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_DC1() { return View("Temp_IntroCourse/IntroCourse_DC1"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_DC2() { return View("Temp_IntroCourse/IntroCourse_DC2"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_DC3() { return View("Temp_IntroCourse/IntroCourse_DC3"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_BC1() { return View("Temp_IntroCourse/IntroCourse_BC1"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_BC2() { return View("Temp_IntroCourse/IntroCourse_BC2"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_BC3() { return View("Temp_IntroCourse/IntroCourse_BC3"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_KC1() { return View("Temp_IntroCourse/IntroCourse_KC1"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_KC2() { return View("Temp_IntroCourse/IntroCourse_KC2"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult IntroCourse_KC3() { return View("Temp_IntroCourse/IntroCourse_KC3"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Footer_Page1() { return View("Temp_Page/Footer_Page1"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Footer_Page2() { return View("Temp_Page/Footer_Page2"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult SiteGuide() { return View("Temp_Page/SiteGuide"); }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult JsonFiles1()
        {
            //SessionModel sm = SessionModel.Get();
            //if (Session[GUID_ATTACKS_CODE] == null || string.IsNullOrEmpty((string)Session[GUID_ATTACKS_CODE]) || sm.CHECK_GUID_attacks || (string)Session[GUID_ATTACKS_CODE] != sm.GUID_attacks)
            //{
            //    return new HttpNotFoundResult();
            //}
            return Content(System.IO.File.ReadAllText(Server.MapPath(Url.Content("~/JsonFiles/citys.json"))));
        }

        //[AllowAnonymous] [HttpGet] public ActionResult JsonFiles2() { return Content(System.IO.File.ReadAllText(Server.MapPath(ConfigModel.WKEF + "/JsonFiles/zipCode.json"))); }
        //[AllowAnonymous] [HttpGet] public ActionResult JsonFiles3() { return Content(System.IO.File.ReadAllText(Server.MapPath(ConfigModel.WKEF + "/JsonFiles/taiwan_zipCode.json"))); }
    }
}