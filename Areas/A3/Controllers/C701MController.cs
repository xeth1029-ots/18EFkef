using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WKEFSERVICE.Areas.A3.Models;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Models;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Services;


namespace WKEFSERVICE.Areas.A3.Controllers
{
    public class C701MController : WKEFSERVICE.Controllers.BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionModel sm = SessionModel.Get();
            C701MViewModel model = new C701MViewModel();
            model.Form.TeacherUnit = sm.UserInfo.User.UNITID;
            model.Form.Year = $"{DateTime.Now.Year - 1}";
            model.Form.FinalReportCode = "1";
            return View(model.Form);
        }

        [HttpPost]
        public ActionResult Index(C701MFormModel form)
        {
            SessionModel sm = SessionModel.Get();
            ModelState.Clear();
            ActionResult rtn = View("Index", form);
            var fmYear = form.Year;
            //0.全,1.北,2.桃,3.中,4.南,5.高,,1.師資個人授課總表,2.師資授課情形統計表,3.滿意度調查表,4.年度評核結果總表,
            var fmTeacherUnit = sm.UserInfo.User.UNITID; //form.TeacherUnit ?? "0";
            var fmFinalReportCode = form.FinalReportCode;

            if (fmYear.TONotNullString() == "") { sm.LastErrorMessage = "請選擇計畫年度！"; return rtn; }
            if (fmFinalReportCode.TONotNullString() == "") { sm.LastErrorMessage = "請選擇定版報表！"; return rtn; }
            //計畫年度：113年開始，往後至當年度(跟現在一樣，逆排序)
            //所屬轄區：署可以選全部，分署鎖定自己的分署
            //定版報表：師資個人授課總表、師資授課情形統計表、滿意度調查表、年度評核結果總表
            var wfinrpt = new FinalReport { Years = fmYear, UnitCode = fmTeacherUnit, FinalReportCode = fmFinalReportCode };

            AMDAO dao = new AMDAO();
            var lstFinalReport = dao.GetRowList(wfinrpt).ToList();
            if (lstFinalReport == null || lstFinalReport.Count == 0)
            {
                var errmsg1 = $"查無此定版報表-{fmYear}-{fmTeacherUnit}-{fmFinalReportCode}-";
                LOG.Error(errmsg1);
                sm.LastErrorMessage = errmsg1; return rtn;
            }

            //下載檔案
            //SVN\WDAIIP\111\05_關鍵就業力課程網站\07_SDS\trunk\Uploads\20250224_113年_1\
            var aFinalReport = lstFinalReport[0];
            var filepath1 = aFinalReport.FilePath1 ?? "~/Exception/"; //"~/Uploads/20250224_113年_1/";
            var filename1 = aFinalReport.FileName1;
            var fileExt1 = System.IO.Path.GetExtension(filename1).ToLowerInvariant();

            //關鍵_113年_年度評核結果總表(中)
            var RptCodelist = form.FinalReportCode_list;
            var Report_N = RptCodelist.Where(t => t.Value == fmFinalReportCode).FirstOrDefault().Text;
            var sclm = new ShareCodeListModel();
            var UNIT_N = sclm.UNIT_Shot_List.Where(t => t.Value == fmTeacherUnit).FirstOrDefault().Text;
            var fmYear_ROC = $"{Convert.ToInt32(fmYear) - 1911}";
            var a_fileName = $"關鍵-{fmYear_ROC}年-{Report_N}{UNIT_N}-{DateTime.Now.ToString("fffddssmmHH")}{fileExt1}";

            if (!filepath1.EndsWith("/")) { filepath1 += "/"; }
            var filePath2 = Server.MapPath(string.Concat(filepath1, filename1));
            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath2, FileMode.Open)) { stream.CopyTo(memory); }
                memory.Position = 0;
                return File(memory, form.GetContentType(filePath2), a_fileName);
            }
            catch (FileNotFoundException ex)
            {
                var errmsg1 = $"檔案不存在-{fmYear}-{fmTeacherUnit}-{fmFinalReportCode}-";
                LOG.Error($"{ex.Message},{errmsg1}", ex);
                sm.LastErrorMessage = errmsg1;
                return rtn;
                //return HttpNotFound(); // 檔案不存在
            }
            catch (Exception ex)
            {
                // 記錄錯誤
                var errmsg2 = $"下載檔案時發生錯誤-{fmYear}-{fmTeacherUnit}-{fmFinalReportCode}-";
                LOG.Error($"{ex.Message},{errmsg2}", ex);
                sm.LastErrorMessage = errmsg2;// errmsg1;
                return rtn;
            }

        }
    }
}
