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
using System.IO;
using static WKEFSERVICE.Commons.FileHeader;

namespace WKEFSERVICE.Areas.AM.Controllers
{
    public class C201MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            C201MViewModel model = new C201MViewModel();
            model.Form.FilterTab = "1";
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C201MFormModel form)
        {
            AMDAO dao = new AMDAO();
            ActionResult rtn = View("Index", form);
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                // 設定查詢分頁資訊
                dao.SetPageInfo(form.rid, form.p);
                // 查詢結果
                form.Grid = dao.QueryC201M(form);
                // 有 result id 資訊, 分頁連結, 返回 GridRows Partial View
                if (!string.IsNullOrEmpty(form.rid) && form.useCache == 0) rtn = PartialView("_GridRows", form);
                // 設定分頁元件(_PagingLink partial view)所需的資訊
                base.SetPagingParams(form, dao, "Index");
            }
            if (form.FilterTab.TONotNullString() == "") form.FilterTab = "1";
            return rtn;
        }

        [HttpPost]
        public ActionResult DoBack(C201MViewModel model)
        {
            var FilterArr = model.Detail.FilterStr.TONotNullString().Split(new string[] { "||" }, StringSplitOptions.None).ToList();
            if (FilterArr.ToCount() == 9)
            {
                model.Form.FilterTab = FilterArr[0];
                model.Form.Email = FilterArr[1];
                model.Form.TeacherName = FilterArr[2];
                model.Form.IDNO = FilterArr[3];
                model.Form.Keyword = FilterArr[4];
                model.Form.TeachUnit = FilterArr[5];
                model.Form.Industry = FilterArr[6];
                model.Form.TeachArea = FilterArr[7];
                model.Form.LiveArea = FilterArr[8];
            }
            return Index(model.Form);
        }

        [HttpGet]
        public ActionResult Edit(string Seq, string FilterStr)
        {
            ModelState.Clear();
            C201MViewModel model = new C201MViewModel();
            AMDAO dao = new AMDAO();
            model.Detail = dao.DetailC201M(Seq);
            model.Detail.FilterStr = FilterStr;
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Save(C201MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            AMDAO dao = new AMDAO();
            if (IsValid(model))
            {
                ModelState.Clear();
                model.Detail._tmpPicPath = Server.MapPath(dao.UploadPathTeacher);  // 資料夾路徑名

                // 檢查檔案
                var file = model.Detail.Pic_FILE;
                if (file != null)
                {
                    // 檢查檔案大小
                    if (file.ContentLength > 2 * 1024 * 1024)  // 位元組 *1024 *1024 = MB
                    {
                        sm.LastErrorMessage = "上傳檔案格式限定 JPG、PNG、GIF，檔案大小以 2MB 為限。";
                        return View("Edit", model);
                    }

                    // 試做 230116
                    MemoryStream ms = new MemoryStream();
                    file.InputStream.CopyTo(ms);

                    // 檢查檔案格式
                    string[] fileArr = { ".JPEG", ".JPG", ".PNG", ".GIF" };
                    if (!fileArr.Contains(Path.GetExtension(file.FileName).ToUpper()) || !CheckFileType(ms, TypeFileHeader.Image))
                    {
                        sm.LastErrorMessage = "上傳檔案格式限定 JPG、PNG、GIF，檔案大小以 2MB 為限。";
                        return View("Edit", model);
                    }
                }

                dao.BeginTransaction();
                try
                {
                    dao.SaveC201M(model);
                    dao.CommitTransaction();
                    sm.LastResultMessage = "資料已儲存";
                    model.Detail.EditAreaIdx = null;
                    return Edit(model.Detail.Seq.ToString(), model.Detail.FilterStr.TONotNullString());  // 儲存成功則重載
                    //return Redirect("./Edit?Seq=" + model.Detail.Seq.ToString() + "&FilterStr=" + model.Detail.FilterStr.TONotNullString());
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message, ex);
                    dao.RollBackTransaction();
                    sm.LastErrorMessage = "儲存發生錯誤";
                }
            }
            return View("Edit", model);
        }

        /// <summary>
        /// 手動檢查區塊必填欄位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsValid(C201MViewModel model)
        {
            // 依區塊檢查
            bool Result = true;
            string Idx = model.Detail.EditAreaIdx.TONotNullString();
            if (Idx == "1")
            {
                if (model.Detail.TeacherName.TONotNullString() == "") { ModelState.AddModelError("", "姓名(中文) 是必填欄位"); Result = false; }
                if (model.Detail.Sex.TONotNullString() == "") { ModelState.AddModelError("", "性別 是必填欄位"); Result = false; }
                if (model.Detail.Birthday.TONotNullString() == "") { ModelState.AddModelError("", "出生日期 是必填欄位"); Result = false; }
                if (model.Detail.Email.TONotNullString() == "") { ModelState.AddModelError("", "Email 是必填欄位"); Result = false; }
            }
            else if (Idx == "2") { }
            else if (Idx == "3") { }
            else if (Idx == "4") { }
            else if (Idx == "5")
            {
                if (model.Detail.UnitCode.TONotNullString() == "") { ModelState.AddModelError("", "所屬轄區 是必填欄位"); Result = false; }
            }
            else if (Idx == "6") { }
            else if (Idx == "7") { }
            else if (Idx == "8") { }
            else if (Idx == "9") { }
            else if (Idx == "10") { }
            else if (Idx == "11") { }
            else if (Idx == "12") { }
            else if (Idx == "13")
            {
                if (model.Detail.Online.TONotNullString() == "") { ModelState.AddModelError("", "在線/下線 是必填欄位"); Result = false; }
            }
            return Result;
        }

        public ActionResult GetJsonIndustry()
        {
            AMDAO dao = new AMDAO();
            var Datas = dao.GetJsonIndustry();
            return Json(Datas, JsonRequestBehavior.AllowGet);
        }
    }
}