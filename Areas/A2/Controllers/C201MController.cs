using System;
using System.Web.Mvc;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Areas.A2.Models;
using WKEFSERVICE.Models;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using System.Linq;
using Omu.ValueInjecter;
using WKEFSERVICE.Services;
using Turbo.Commons;
using System.IO;
using static WKEFSERVICE.Commons.FileHeader;

/// <summary>
/// 注意！
/// 提示：此 A2/C201M 之語法取自 AM/C201M，並做功能性刪減
/// </summary>
namespace WKEFSERVICE.Areas.A2.Controllers
{
    public class C201MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            var getSeq = dao.GetRow<Teacher>(new Teacher() { ACCOUNT = sm.UserInfo.UserNo });
            if (getSeq == null) return Redirect(ConfigModel.WKEF + "/Backend");
            return Edit(getSeq.Seq.ToString());
        }

        [HttpGet]
        public ActionResult Edit(string Seq)
        {
            ModelState.Clear();
            SessionModel sm = SessionModel.Get();
            C201MViewModel model = new C201MViewModel();
            A2DAO dao = new A2DAO();
            var checkSeq = dao.GetRow<Teacher>(new Teacher() { ACCOUNT = sm.UserInfo.UserNo }).Seq ?? 0;
            if (checkSeq.ToString() != Seq) return Redirect(ConfigModel.WKEF + "/Backend");
            model.Detail = dao.DetailC201M(Seq);
            if (model.Detail == null) return new HttpNotFoundResult();
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Save(C201MViewModel model)
        {
            SessionModel sm = SessionModel.Get();
            A2DAO dao = new A2DAO();
            if (IsValid(model))
            {
                ModelState.Clear();
                model.Detail._tmpPicPath = Server.MapPath(new AMDAO().UploadPathTeacher);  // 資料夾路徑名

                // 檢查檔案
                var file = model.Detail.Pic_FILE;
                if (file != null)
                {
                    // 檢查檔案大小
                    if (file.ContentLength > (2 * 1024 * 1024))  // 位元組 2*1024*1024 = MB
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
                    return Edit(model.Detail.Seq.ToString());  // 儲存成功則重載
                    //return Redirect("./Edit?Seq=" + model.Detail.Seq.ToString());
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
            A2DAO dao = new A2DAO();
            var Datas = dao.GetJsonIndustry();
            return Json(Datas, JsonRequestBehavior.AllowGet);
        }
    }
}