using System;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.AM.Models;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using System.Linq;
using Omu.ValueInjecter;
using WKEFSERVICE.Services;
using Turbo.Commons;
using log4net;
using System.IO;
using System.Collections.Generic;
using static WKEFSERVICE.Commons.FileHeader;
using System.Collections;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C301MController : WKEFSERVICE.Controllers.BaseController
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //記錄Log               

        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C301MViewModel model = new C301MViewModel();
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C301MFormModel form)
        {
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC301M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            return rtn;
        }

        [HttpGet]
        public ActionResult AutoLoad()
        {
            C301MViewModel model = new C301MViewModel();
            return Index(model.Form);
        }

        [HttpGet]
        public ActionResult New()
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            C301MViewModel model = new C301MViewModel();
            model.Detail = new C301MDetailModel();
            model.Detail.IsNew = true;
            model.Detail.PicAlign = "R";
            model.Detail.CreatedUnit = sm.UserInfo.User.UNITID;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult Detail(string Seq)
        {
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            C301MViewModel model = new C301MViewModel();
            model.Detail = new C301MDetailModel();
            model.Detail = dao.DetailC301M(Seq.TONotNullString());
            model.Detail.Attacheds = dao.DetailAttachedsC301M(Seq.TONotNullString());
            model.Detail.IsNew = false;
            return View("Detail", model);
        }

        [HttpGet]
        public ActionResult DetailSignUp(string Seq, string vPage)
        {
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            C301MViewModel model = new C301MViewModel { DetailSignUp = new C301MDetailSignUpModel() };

            int iSeq = 1;
            int ivPage = 1;
            if (!int.TryParse(Seq, out iSeq))
            {
                //return View("DetailSignUp", model);
                SessionModel sm = SessionModel.Get();
                sm.LastErrorMessage = "查無資料";
                return Index(model.Form);
            }
            Seq = iSeq.ToString();
            if (!int.TryParse(vPage, out ivPage)) { ivPage = 1; }

            model.DetailSignUp = dao.DetailSignUpC301M_Mst(Seq.TONotNullString());
            model.DetailSignUp.vPage = ivPage.ToString();

            int iDefPageRows = 50;
            Hashtable r = MyCommonUtil.GET_MINMAXROW(ivPage, 50);
            int iMINROW = r.ContainsKey("MINROW") ? Convert.ToInt32(r["MINROW"]) : 1;
            int iMAXROW = r.ContainsKey("MAXROW") ? Convert.ToInt32(r["MAXROW"]) : iDefPageRows;
            model.DetailSignUp.SignUpList = dao.DetailSignUpC301M_Det(iSeq, iMINROW, iMAXROW);
            return View("DetailSignUp", model);
        }

        [HttpPost]
        public ActionResult DetailAttachedsUpload(C301MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            var file = model.Detail._UPLOAD_FILE;
            if (file == null || file.ContentLength <= 0 || string.IsNullOrWhiteSpace(file.FileName))
            {
                //sm.LastErrorMessage = "請選擇上傳檔案";
            }
            else
            {
                // 上傳檔案
                string tmpPath = Server.MapPath(dao.UploadPathC301M);  // 資料夾路徑名
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);  // 不存在則建立
                Random tmpNum = new Random();
                string tmpFileName =
                    DateTime.Now.ToString("yyyyMMddHHmmssfffff") +      // 年 + 月 + 日 + 時 + 分 + 秒 + 毫秒
                    tmpNum.Next(0, 99999).ToString().PadLeft(5, '0') +  // 亂數 5 位
                    Path.GetExtension(file.FileName);                   // 副檔名
                file.SaveAs(tmpPath + "/" + tmpFileName);
                FileLogAdd.Do(ModifyType.UPLOAD, ModifyState.SUCCESS, "AM/C301M", file.FileName, tmpFileName);
                // 資料庫新增
                dao.BeginTransaction();
                try
                {
                    #region DB Insert
                    MeetingAttached Ins = new MeetingAttached();
                    Ins.Seq = null;
                    Ins.MeetingSeq = (model.Detail.Seq.TONotNullString() == "") ? -1 : model.Detail.Seq;
                    Ins.FileNameOrg = file.FileName;
                    Ins.FileNameNew = tmpFileName;
                    Ins.FileNameType = Path.GetExtension(file.FileName);
                    Ins.UpdatedAccount = sm.UserInfo.UserNo;
                    Ins.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    int keyid = dao.Insert(Ins);
                    FuncLogAdd.Do(ModifyEvent.AFTER, ModifyType.CREATE, (keyid >= 1) ? ModifyState.SUCCESS : ModifyState.FAIL, "AM/C301M", Ins, null, "[MeetingAttached]");
                    dao.CommitTransaction();
                    #endregion
                    #region Model Insert
                    model.Detail._UPLOAD_FILE = null;
                    if (model.Detail.Attacheds == null) model.Detail.Attacheds = new List<C301MAttachedsModel>();
                    model.Detail.Attacheds.Add(new C301MAttachedsModel()
                    {
                        ROWID = model.Detail.Attacheds.ToCount() + 1,
                        Seq = keyid,
                        MeetingSeq = Ins.MeetingSeq,
                        FileNameOrg = Ins.FileNameOrg,
                        FileNameNew = Ins.FileNameNew,
                        FileNameType = Ins.FileNameType,
                        UpdatedAccount = Ins.UpdatedAccount,
                        UpdatedDatetime = Ins.UpdatedDatetime.Replace('/', '-')
                    });
                    #endregion
                    //sm.LastResultMessage = "檔案上傳成功";
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message, ex);
                    dao.RollBackTransaction();
                    if (System.IO.File.Exists(tmpPath + "/" + tmpFileName))
                    {
                        System.IO.File.Delete(tmpPath + "/" + tmpFileName);
                        FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", file.FileName, tmpFileName);
                    }
                    //sm.LastErrorMessage = "檔案上傳失敗";
                }
            }
            return PartialView("_DetailAttachedsRows", model);
        }

        [HttpPost]
        public ActionResult Save(C301MDetailModel detail)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();

            // 檢查
            if (detail.Address.TONotNullString() == "") ModelState.AddModelError("", "地址 欄位是必要項。");
            if (detail.MeetingDateS.TONotNullString() == "") ModelState.AddModelError("", "日期區間請至少填寫一項。");
            if (detail.PostalCode1.TONotNullString().Length > 3) ModelState.AddModelError("", "郵遞區號1 格式錯誤請檢查。");
            if (detail.PostalCode2.TONotNullString().Length > 3) ModelState.AddModelError("", "郵遞區號2 格式錯誤請檢查。");

            // 檢查檔案
            var file = detail.PicName_FILE;
            if (file != null)
            {
                // 檢查檔案大小
                if (file.ContentLength > 2 * 1024 * 1024)  // 位元組 *1024 *1024 = MB
                {
                    ModelState.AddModelError("", "上傳檔案格式限定 JPG、PNG、GIF，檔案大小以 2MB 為限。");
                }

                // 試做 230116
                MemoryStream ms = new MemoryStream();
                file.InputStream.CopyTo(ms);

                // 檢查檔案格式
                string[] fileArr = { ".JPEG", ".JPG", ".PNG", ".GIF" };
                if (!fileArr.Contains(Path.GetExtension(file.FileName).ToUpper()) || !CheckFileType(ms, TypeFileHeader.Image))
                {
                    ModelState.AddModelError("", "上傳檔案格式限定 JPG、PNG、GIF，檔案大小以 2MB 為限。");
                }
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                if (detail.IsNew)
                {
                    detail.CreatedAccount = sm.UserInfo.UserNo;
                    detail.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    DateTime tmpDate = HelperUtil.TransToDateTime(detail.MeetingDateS, "/") ?? DateTime.MinValue;
                    if (tmpDate != null && tmpDate != DateTime.MinValue) detail.Year = tmpDate.ToString("yyyy");
                }
                else
                {
                    detail.UpdatedAccount = sm.UserInfo.UserNo;
                    detail.UpdatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                detail._tmpPicPath = Server.MapPath(dao.UploadPathC301M);  // 資料夾路徑名
                dao.InsertOrUpdateC301M(detail);
                sm.LastResultMessage = "資料已儲存";
            }
            else
            {
                C301MViewModel model = new C301MViewModel();
                model.Detail = detail;
                #region Reset
                if (model.Detail.SignUpDateS.TONotNullString() == " ") model.Detail.SignUpDateS = null;
                if (model.Detail.SignUpDateE.TONotNullString() == " ") model.Detail.SignUpDateE = null;
                if (model.Detail.MeetingRelease.TONotNullString() == " ") model.Detail.MeetingRelease = null;
                if (HelperUtil.TransToDateTime(model.Detail.SignUpDateS, "-") == DateTime.MinValue) model.Detail.SignUpDateS = null;
                if (HelperUtil.TransToDateTime(model.Detail.SignUpDateE, "-") == DateTime.MinValue) model.Detail.SignUpDateE = null;
                if (HelperUtil.TransToDateTime(model.Detail.MeetingRelease, "-") == DateTime.MinValue) model.Detail.MeetingRelease = null;
                #endregion
                return View("Detail", model);
            }
            return (detail.IsNew) ? Index(new C301MFormModel()) : Detail(detail.Seq.TONotNullString());
        }

        [HttpPost]
        public ActionResult SaveSignUp(C301MDetailSignUpModel detailSignUp)
        {
            if (detailSignUp.SignUpList.ToCount() > 0)
            {
                AMDAO dao = new AMDAO();
                dao.InsertOrUpdateC301M_SignUp(detailSignUp);
                SessionModel sm = SessionModel.Get();
                sm.LastResultMessage = "資料已儲存";
            }
            return DetailSignUp(detailSignUp.Seq.TONotNullString(), detailSignUp.vPage);
        }

        [HttpPost]
        public ActionResult Delete(C301MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            if (form.Seq_forDel.TONotNullString() == "")
            {
                sm.LastErrorMessage = "未選擇資料";
                return Index(form);
            }
            // 刪除 檔案
            string FilePath = Server.MapPath(dao.UploadPathC301M);
            #region Meeting
            var tmpData = dao.GetRow(new Meeting() { Seq = form.Seq_forDel.TOInt64() });
            if (tmpData.PicName.TONotNullString() != "")
            {
                string FullFileName = FilePath + tmpData.PicName.TONotNullString();
                if (System.IO.File.Exists(FullFileName))
                {
                    System.IO.File.Delete(FullFileName);
                    FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", tmpData.PicName.TONotNullString(), null);
                }
            }
            #endregion
            #region MeetingAttached
            var tmpDataAtta = dao.GetRowList(new MeetingAttached() { MeetingSeq = form.Seq_forDel.TOInt64() });
            foreach (var itm in tmpDataAtta)
            {
                if (itm.FileNameNew.TONotNullString() == "") continue;
                string FullFileName = FilePath + itm.FileNameNew.TONotNullString();
                if (System.IO.File.Exists(FullFileName))
                {
                    System.IO.File.Delete(FullFileName);
                    FileLogAdd.Do(ModifyType.DELETE, ModifyState.SUCCESS, "AM/C301M", itm.FileNameOrg.TONotNullString(), itm.FileNameNew.TONotNullString());
                }
            }
            #endregion
            // 刪除 資料
            dao.DeleteC301M(form.Seq_forDel.TONotNullString(), sm.UserInfo.UserNo);
            sm.LastResultMessage = "已刪除";
            return Index(form);
        }

        [HttpPost]
        public ActionResult TeacherSearch(C301MViewModel model)
        {
            ModelState.Clear();
            AMDAO dao = new AMDAO();
            string CreatedUnit = null;
            if (model.DetailSignUp.MeetingType.TONotNullString() == "2") CreatedUnit = model.DetailSignUp.CreatedUnit.TONotNullString();
            model.DetailSignUp.TeacherSearchList = dao.GetTeacherListC301M(model.DetailSignUp.TeacherSearch.TONotNullString(), CreatedUnit);
            return PartialView("_TeacherPopupRows", model);
        }

        [HttpPost]
        public ActionResult TeacherSearchSelect(C301MViewModel model)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            if (model.DetailSignUp.TeacherSearchList.ToCount() > 0)
                foreach (var item in model.DetailSignUp.TeacherSearchList)
                {
                    if (!item.IsChecked) continue;
                    if (model.DetailSignUp.SignUpList == null) model.DetailSignUp.SignUpList = new List<C301MDetailSignUpListModel>();
                    var tmpItem = model.DetailSignUp.SignUpList.Where(x => x.TeacherAccount == item.TeacherAccount);
                    if (tmpItem.Any())  // 有發現重複時，設為啟用
                    {
                        var tmpIdx = model.DetailSignUp.SignUpList.IndexOf(tmpItem.FirstOrDefault());
                        model.DetailSignUp.SignUpList[tmpIdx].isActive = true;
                        continue;
                    }
                    // 未找到重複時，則新增
                    C301MDetailSignUpListModel NewItem = new C301MDetailSignUpListModel();
                    NewItem.ROWID = model.DetailSignUp.SignUpList.ToCount() + 1;
                    NewItem.Seq = null;
                    NewItem.MeetingSeq = model.DetailSignUp.Seq;
                    NewItem.TeacherAccount = item.TeacherAccount;
                    NewItem.TeacherName = item.TeacherName;
                    NewItem.SignUpDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    NewItem.SignUpType = "2";  // 1：自行(教師自行登入報名)  2：管理者(管理者由後台存入)
                    NewItem.UpdatedAccount = sm.UserInfo.UserNo;
                    NewItem.UpdatedDatetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    model.DetailSignUp.SignUpList.Add(NewItem);
                }
            return PartialView("_DetailSignUpRows", model);
        }

        [HttpPost]
        public ActionResult TeacherSearchSelectRemoveRow(C301MViewModel model)
        {
            ModelState.Clear();
            if (model.DetailSignUp.TempSignUpListDelete_TeacherAccount.TONotNullString() != "")
            {
                var tmpItem = model.DetailSignUp.SignUpList.Where(x => x.TeacherAccount == model.DetailSignUp.TempSignUpListDelete_TeacherAccount);
                if (tmpItem.Any())
                {
                    var tmpItemIdx = model.DetailSignUp.SignUpList.IndexOf(tmpItem.FirstOrDefault());
                    model.DetailSignUp.SignUpList[tmpItemIdx].isActive = false;
                }
            }
            return PartialView("_DetailSignUpRows", model);
        }
    }
}