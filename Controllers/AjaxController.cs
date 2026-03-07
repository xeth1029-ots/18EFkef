using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using log4net;
using WKEFSERVICE.DataLayers;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Commons.Filter;
using WKEFSERVICE.Models;
using Turbo.Commons;
using WKEFSERVICE.Services;
using System.Web.Script.Serialization;
using System.Linq;
using System.IO;
using Turbo.DataLayer;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ICSharpCode.SharpZipLib.Zip;
using Turbo.Crypto;

namespace WKEFSERVICE.Controllers
{
    /// <summary>
    /// 這個類集中放置一些 Ajax 動作會用的的下拉代碼清單控制 action
    /// </summary>
    [BypassAuthorize]
    public class AjaxController : Controller
    {
        protected static readonly ILog LOG = LogManager.GetLogger(typeof(MvcApplication));

        /// <summary>師資資料</summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Api_Teachers()
        {
            string s_message = "data not found";
            string logMsg1 = "data not found.";
            LOG.Debug("AjaxController.Api_Teachers() Called.");
            var result = new AjaxResultStruct();
            // 撈 師資資料
            BaseDAO dao = new BaseDAO();
            var tmpList = dao.GetRowList(new WKEFSERVICE.Models.Entities.Teacher());
            var resDatas = new List<Api_TeachersModel>();
            if (tmpList != null && tmpList.ToCount() > 0)
            {
                s_message = "";
                logMsg1 = string.Concat(tmpList.ToCount(), " rows.");
                foreach (var row in tmpList)
                {
                    var item = new Api_TeachersModel();
                    item.plkt_name = row.TeacherName.TONotNullString();
                    item.plkt_idno = row.IDNO.TONotNullString();
                    item.plkt_company = row.ServiceUnit1.TONotNullString();
                    item.plkt_co = row.JobTitle1.TONotNullString();
                    item.plkt_company2 = row.ServiceUnit2.TONotNullString();
                    item.plkt_co2 = row.JobTitle2.TONotNullString();
                    item.plkt_dis_id = (row.UnitCode.TOInt32() + 1).TONotNullString();
                    item.plkt_year = row.JoinYear.TONotNullString();
                    item.plkt_dc = row.TeachJobAbilityDC.TONotNullString();
                    item.plkt_bc = row.TeachJobAbilityBC.TONotNullString();
                    item.plkt_kc = row.TeachJobAbilityKC.TONotNullString();
                    item.Online = row.Online.TONotNullString();
                    item.OfflineDate = row.OfflineDate.TONotNullString();
                    resDatas.Add(item);
                }
            }
            result.data = resDatas;
            result.status = (tmpList != null && tmpList.ToCount() > 0);
            result.message = s_message;
            LOG.Debug(string.Concat("AjaxController.Api_Teachers() Return: ", logMsg1));
            return Content(result.Serialize(), "application/json");
        }

        [HttpPost]
        public ActionResult Api_ReturnEnc(string plainstr1)
        {
            if (string.IsNullOrEmpty(plainstr1)) return new HttpNotFoundResult();
            return Content(new AesTk().Encrypt(plainstr1));
        }

        [HttpPost]
        public ActionResult Api_GetUtcNow()
        {
            //2023-12-13 14:00:26
            return Content(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>當年度評核結果總表</summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Api_Get_ReviewReportD1(string userid, string apitoken)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(apitoken)) return new HttpNotFoundResult();
            string decryptUser1 = MyCommonUtil.AtkDecrypt(userid);
            string decryptStr1 = MyCommonUtil.AtkDecrypt(apitoken);
            //LOG.Debug(string.Concat("#decryptUser1:", decryptUser1));
            //LOG.Debug(string.Concat("#decryptStr1:", decryptStr1));
            if (string.IsNullOrEmpty(decryptUser1) || string.IsNullOrEmpty(decryptStr1) || decryptStr1.IndexOf(',') == -1 || decryptUser1 != "wkef1206")
                return new HttpNotFoundResult();
            //LOG.Debug(string.Concat("#Api_Get_ReviewReportD1 !", ""));
            bool fg_tk1 = false;
            bool fg_tk2 = false;
            int inum = 0;
            DateTime dtime1 = DateTime.MinValue;
            //&& MyCommonUtil.IsWithin5Minutes(dtime1)
            foreach (var sVal in decryptStr1.Split(','))
            {
                inum += 1;
                //帳號檢核
                if (sVal == decryptUser1) { fg_tk1 = true; }
                //時間檢核
                if (sVal != decryptUser1 && DateTime.TryParse(sVal, out dtime1)) { fg_tk2 = MyCommonUtil.IsWithin5Minutes(dtime1); }
                if (inum > 3) { break; }
            }
            //LOG.Debug(string.Concat("#fg_tk1: ", fg_tk1));
            //LOG.Debug(string.Concat("#fg_tk2: ", fg_tk2));
            //LOG.Debug(string.Concat("#Api_Get_ReviewReportD1 !!", ""));
            if (!fg_tk1 || !fg_tk2) return new HttpNotFoundResult();
            //LOG.Debug(string.Concat("#Api_Get_ReviewReportD1 !!!", ""));
            //var result2 = new AjaxResultStruct();
            //result2.data = string.Concat("userid=", userid, ",apitoken=", apitoken);
            //return Content(result2.Serialize(), "application/json");

            string s_message = "data not found";
            string logMsg1 = "data not found.";
            LOG.Debug("AjaxController.Api_Get_ReviewReportD1() Called.");
            var result = new AjaxResultStruct();
            //當年度評核結果總表
            AMDAO dao = new AMDAO();
            Hashtable whereht = new Hashtable();
            IList<ReviewReportD1> tmpList = dao.GetReviewReportD1(whereht);
            if (tmpList != null && tmpList.ToCount() > 0)
            {
                s_message = "";
                logMsg1 = string.Concat(tmpList.ToCount(), " rows.");
            }
            result.data = tmpList;
            result.status = (tmpList != null && tmpList.ToCount() > 0);
            result.message = s_message;
            LOG.Debug(string.Concat("AjaxController.Api_Get_ReviewReportD1() Return: ", logMsg1));
            return Content(result.Serialize(), "application/json");
        }
    }
}