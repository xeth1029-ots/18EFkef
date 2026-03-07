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
    /// 後台首頁
    /// </summary>
    public class BackendController : BaseController  // 繼承自 BaseController 會去做登入判斷
    {
        protected static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 後台首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo != null)
            {
                if (sm.UserInfo.User.GRPID.TOInt32() <= 1) return RedirectToAction("Index", "Home");            // 1. 一般使用者
                if (sm.UserInfo.User.GRPID.TOInt32() == 2) return RedirectToAction("indexTeacher", "Backend");  // 2. 教師使用者
                if (sm.UserInfo.User.GRPID.TOInt32() == 3) return RedirectToAction("indexBranch", "Backend");   // 3. 分署使用者
                if (sm.UserInfo.User.GRPID.TOInt32() == 4) return RedirectToAction("indexAdmin", "Backend");    // 4. 管理者
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult indexTeacher()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo != null && sm.UserInfo.User.GRPID == "2")
            {
                var model = new HomeModel();
                model.TeachersAdminModel = new TeachersAdminModel();
                return View("indexTeacher", model);
            }
            else return RedirectToAction("Index", "Home");
        }

        public ActionResult indexBranch()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo != null && sm.UserInfo.User.GRPID == "3")
            {
                var model = new HomeModel();
                model.BranchsAdminModel = new BranchsAdminModel();
                return View("indexBranch", model);
            }
            else return RedirectToAction("Index", "Home");
        }

        public ActionResult indexAdmin()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo != null && sm.UserInfo.User.GRPID == "4")
            {
                var model = new HomeModel();
                model.AdminModel = new AdminModel();
                return View("indexAdmin", model);
            }
            else return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            SessionModel sm = SessionModel.Get();
            HomeController.SetLoginLog(HttpContext.Request, sm.UserInfo.UserNo, "LOGOUT", "登出成功");
            // 沒什麼用...
            //sm.UserInfo = null;
            //sm.RoleFuncs = null;
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult GetNewsDatas(string PostType, string PostTo)
        {
            // 取得訊息公告
            HomeModel model = new HomeModel();
            AMDAO dao = new AMDAO();
            model.NewsGrid = dao.GetNewsDatas(PostType, PostTo).Take(5).ToList();
            return View(model);
        }

        public ActionResult GetMeetingsDatas(string MeetingType)
        {
            // 取得會議清單
            HomeModel model = new HomeModel();
            AMDAO dao = new AMDAO();
            model.MeetingsGrid = dao.GetMeetingsDatas(MeetingType).Take(5).ToList();
            return View(model);
        }

        public ActionResult GetAuditReportsDatas(string AuditStatus)
        {
            // 取自我審核報告
            HomeModel model = new HomeModel();
            AMDAO dao = new AMDAO();
            SessionModel sm = SessionModel.Get();
            string UnitCode = null;
            if (sm.UserInfo != null && sm.UserInfo.LoginCharacter != "4") UnitCode = sm.UserInfo.User.UNITID;
            if (UnitCode == "0") UnitCode = null;
            model.AuditReportsGrid = dao.GetAuditReportsDatas(AuditStatus, UnitCode);
            return View(model);
        }

        public ActionResult NewsAll(WKEFSERVICE.Areas.A1.Models.C102MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("NewsAll", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.PostType = "";
                form.PostTo = sm.UserInfo.LoginCharacter;
                form.Grid = dao.GetNewsDatas(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRowsNewsAll_Backend", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "NewsAll");
            }
            return rtn;
        }

        public ActionResult NewsDetail(string Seq)
        {
            int i_Seq = 0;
            if (!int.TryParse(Seq, out i_Seq)) { return new HttpNotFoundResult(); }
            SessionModel sm = SessionModel.Get();
            A1DAO dao = new A1DAO();
            var dataNews = dao.DetailC102M(Seq);  // 取得公告資料
            if (dataNews == null) return Index();  // 檢查是否確實有此公告
            int iLoginCharacter = (sm.UserInfo == null) ? 1 : sm.UserInfo.LoginCharacter.TOInt32();
            if (dataNews.PostTo > iLoginCharacter)  // 檢查閱覽權限
            {
                sm.LastErrorMessage = "權限不足";
                return Index();
            }

            // 借用 A1/C102M 的 Model 來做顯示
            // 但頁面 還是獨立頁面
            WKEFSERVICE.Areas.A1.Models.C102MViewModel model = new WKEFSERVICE.Areas.A1.Models.C102MViewModel();
            //model.Detail = dao.DetailC102M(Seq);  // 取得公告資料
            model.Detail = dataNews;  // 取得公告資料

            model.Detail.Attacheds = dao.DetailAttachedsC102M(Seq);  // 取得公告附檔資料
            return View("NewsDetail", model);
        }

        public ActionResult GetTeacherPercent()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo == null || sm.UserInfo.LoginCharacter != "2") return new HttpNotFoundResult();
            A2DAO dao = new A2DAO();
            Teacher tmpTea = dao.GetRow<Teacher>(new Teacher() { ACCOUNT = sm.UserInfo.UserNo });
            if (tmpTea == null) return new HttpNotFoundResult();

            // 開始計算教師填寫百分比
            string[] fillFields = {  // 必填欄位
                "SelfPicPath",
                "TeacherName",
                "Sex",
                "Birthday",
                "Email",
                "ServiceUnit1",
                "JobTitle1",
                "ExpertiseDesc",
                "ExpertiseCode",
                "TeachArea",
                "TeachIndustryDetCode",
                "IndustryAcademicType",
                "WorkHistory",
                "ProLicense",
                "SelfIntroduction"
            };
            int fillCount = 0;  // 已填總欄位數
            foreach (string f in fillFields)
            {
                string tmpStr = tmpTea.GetType().GetProperty(f).GetValue(tmpTea, null).TONotNullString();
                if (tmpStr != "") fillCount++;
            }
            double rtnPercent = fillFields.Count() > 0 ? (fillCount / fillFields.Count()) : 0;
            rtnPercent = Math.Floor(rtnPercent * 100);
            return Content(rtnPercent.ToString());
        }

        /// <summary>取得授課時數</summary>
        /// <returns></returns>
        public ActionResult GetTeacherTeachHour()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo == null || sm.UserInfo.LoginCharacter != "2") return new HttpNotFoundResult();
            A2DAO dao = new A2DAO();
            //var h1 = dao.GetRowList(new TransData_D_Class() { TeacherID = sm.UserInfo.User.IDNO }).Select(x => x.TeachHours).Sum() ?? 0;
            //var h2 = dao.GetRowList(new TransData_S_Class() { TeacherID = sm.UserInfo.User.IDNO }).Select(x => x.TeachHours).Sum() ?? 0;
            Hashtable parmas_D = new Hashtable { { "TeacherID", sm.UserInfo.User.IDNO } };
            var h1 = dao.QueryForListAll<TransData_D_Class>("A2.GetTransData_D_Class_TeacherTeachHour", parmas_D).ToList().Select(x => x.TeachHours).Sum() ?? 0;
            Hashtable parmas_S = new Hashtable { { "TeacherID", sm.UserInfo.User.IDNO } };
            var h2 = dao.QueryForListAll<TransData_S_Class>("A2.GetTransData_S_Class_TeacherTeachHour", parmas_S).ToList().Select(x => x.TeachHours).Sum() ?? 0;
            Hashtable parmas_K = new Hashtable { { "TeacherID", sm.UserInfo.User.IDNO } };
            var h3 = dao.QueryForListAll<TransData_S_Class>("A2.GetTransData_K_Class_TeacherTeachHour", parmas_K).ToList().Select(x => x.TeachHours).Sum() ?? 0;
            var rtn = h1 + h2 + h3;
            return Content(rtn.ToString());
        }

        public ActionResult GetTeacherTeachHour2()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo == null || sm.UserInfo.LoginCharacter != "2") return new HttpNotFoundResult();
            int ih1 = 0;
            int ih2 = 0;
            int ih3 = 0;

            A2DAO dao = new A2DAO();
            Hashtable parmas_D = new Hashtable { { "TeacherID", sm.UserInfo.User.IDNO } };
            var lth1 = dao.QueryForListAll<TransData_D_Class>("A2.GetTransData_D_Class_TeacherTeachHour", parmas_D);
            foreach (var h1 in lth1)
            {
                if (h1.TeachHours.HasValue)
                {
                    if ((h1.CourseUnitCode == "DC1" || h1.CourseUnitCode == "KC1" || h1.CourseUnitCode == "BC1") && h1.TeachHours >= 3)
                    {
                        ih1 += 1;
                    }
                    else if ((h1.CourseUnitCode == "DC2" || h1.CourseUnitCode == "KC2" || h1.CourseUnitCode == "BC2"
                        || h1.CourseUnitCode == "DC3" || h1.CourseUnitCode == "KC3" || h1.CourseUnitCode == "BC3") && h1.TeachHours >= 6)
                    {
                        ih1 += 1;
                    }
                }
            }
            Hashtable parmas_S = new Hashtable { { "TeacherID", sm.UserInfo.User.IDNO } };
            var lth2 = dao.QueryForListAll<TransData_S_Class>("A2.GetTransData_S_Class_TeacherTeachHour", parmas_S);
            foreach (var h2 in lth2)
            {
                if (h2.TeachHours.HasValue)
                {
                    if ((h2.CourseUnitCode == "DC1" || h2.CourseUnitCode == "KC1" || h2.CourseUnitCode == "BC1") && h2.TeachHours >= 3)
                    {
                        ih2 += 1;
                    }
                    else if ((h2.CourseUnitCode == "DC2" || h2.CourseUnitCode == "KC2" || h2.CourseUnitCode == "BC2"
                        || h2.CourseUnitCode == "DC3" || h2.CourseUnitCode == "KC3" || h2.CourseUnitCode == "BC3") && h2.TeachHours >= 6)
                    {
                        ih2 += 1;
                    }
                }
            }

            Hashtable parmas_K = new Hashtable { { "TeacherID", sm.UserInfo.User.IDNO } };
            var lth3 = dao.QueryForListAll<TransData_K_Class>("A2.GetTransData_K_Class_TeacherTeachHour", parmas_K);
            foreach (var h3 in lth3)
            {
                if (h3.TeachHours.HasValue)
                {
                    if ((h3.CourseUnitCode == "DC1" || h3.CourseUnitCode == "KC1" || h3.CourseUnitCode == "BC1") && h3.TeachHours >= 3)
                    {
                        ih3 += 1;
                    }
                    else if ((h3.CourseUnitCode == "DC2" || h3.CourseUnitCode == "KC2" || h3.CourseUnitCode == "BC2"
                        || h3.CourseUnitCode == "DC3" || h3.CourseUnitCode == "KC3" || h3.CourseUnitCode == "BC3") && h3.TeachHours >= 6)
                    {
                        ih3 += 1;
                    }
                }
            }

            var rtn = (ih1 + ih2 + ih3);
            return Content(rtn.ToString());
        }

        /// <summary>授課滿意度</summary>
        /// <returns></returns>
        public ActionResult GetTeacherTeachSatisfy()
        {
            SessionModel sm = SessionModel.Get();
            if (sm.UserInfo == null || sm.UserInfo.LoginCharacter != "2") return new HttpNotFoundResult();
            A2DAO dao = new A2DAO();

            Hashtable parmas_D = new Hashtable() { ["TeacherID"] = sm.UserInfo.User.IDNO };
            var tmp1 = dao.QueryForObject<Hashtable>("A2.getTeacherTeachSatisfy_D", parmas_D);
            double s1 = (tmp1["Satisfy"] ?? -1).ToDouble();

            Hashtable parmas_S = new Hashtable() { ["TeacherID"] = sm.UserInfo.User.IDNO };
            var tmp2 = dao.QueryForObject<Hashtable>("A2.getTeacherTeachSatisfy_S", parmas_S);
            double s2 = (tmp2["Satisfy"] ?? -1).ToDouble();

            Hashtable parmas_K = new Hashtable { ["TeacherID"] = sm.UserInfo.User.IDNO };
            var tmp3 = dao.QueryForObject<Hashtable>("A2.getTeacherTeachSatisfy_K", parmas_K);
            double s3 = (tmp3["Satisfy"] ?? -1).ToDouble();

            string s_Rtn = "";
            double iALL = 3;
            //3者，都有值取平均,//任1無值，取最大
            if (s1 > 0 && s2 > 0 && s3 > 0)
            {
                //3者，都有值取平均
                s_Rtn = ((s1 + s2 + s3) / iALL).ToString();
            }
            else if (s1 > s2 && s1 > s3)
            {
                s_Rtn = s1.ToString();
            }
            else if (s2 > s1 && s2 > s3)
            {
                s_Rtn = s2.ToString();
            }
            else if (s3 > s1 && s3 > s2)
            {
                s_Rtn = s3.ToString();
            }
            return Content(s_Rtn);
        }

        /// <summary>後端使用者操作，需紀錄LOG (檔案下載LOG)</summary>
        [HttpPost]
        public ActionResult FileDownloadLog(string Func, string Org, string New)
        {
            FileLogAdd.Do(ModifyType.DOWNLOAD, ModifyState.SUCCESS, Func, Org, New);
            return Content("OK");
        }
    }
}