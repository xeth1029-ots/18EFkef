using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using WKEFSERVICE.Services;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System;
using Turbo.Commons;
using WKEFSERVICE.DataLayers;

namespace WKEFSERVICE.Areas.AM.Models
{
    public class C201MViewModel
    {
        public C201MViewModel() { this.Form = new C201MFormModel(); }

        public C201MFormModel Form { get; set; }

        public C201MDetailModel Detail { get; set; }
    }

    public class C201MFormModel : PagingResultsViewModel
    {
        #region Filter Page 1

        [Control(Mode = Control.Hidden)]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "姓名")]
        public string TeacherName { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "身分證字號")]
        public string IDNO { get; set; }

        #endregion

        #region Filter Page 2

        [Display(Name = "關鍵字")]
        [Control(Mode = Control.Hidden)]
        public string Keyword { get; set; }

        [Display(Name = "授課單元")]
        [Control(Mode = Control.Hidden)]
        public string TeachUnit { get; set; }

        public IList<SelectListItem> TeachUnit_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.TeachUnit_list();
                list.Insert(0, itemAll);
                return list;
            }
        }

        public string TeachUnit_forSQL
        {
            get
            {
                if (this.TeachUnit.TONotNullString() == "") return null;
                return this.TeachUnit.Substring(0, 2);
            }
        }

        [Display(Name = "授課區域")]
        [Control(Mode = Control.Hidden)]
        public string TeachArea { get; set; }

        public string TeachArea_forSQL
        {
            get
            {
                if (this.TeachArea.TONotNullString() == "") return null;
                string tmpStr = this.TeachArea.TONotNullString().Replace("全區", "");
                tmpStr = tmpStr + "," + tmpStr.Replace("臺", "台");
                return string.Join(",", tmpStr.Split(',').Distinct().ToArray());
            }
        }

        [Display(Name = "產業別")]
        [Control(Mode = Control.Hidden)]
        public string Industry { get; set; }

        public IList<SelectListItem> Industry_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全部", Value = "" };
                var list = model.Industry_list;
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "居住地")]
        [Control(Mode = Control.Hidden)]
        public string LiveArea { get; set; }

        public string LiveArea_forSQL
        {
            get
            {
                if (this.LiveArea.TONotNullString() == "") return null;
                string tmpStr = this.LiveArea.TONotNullString().Replace("全區", "");
                tmpStr = tmpStr + "," + tmpStr.Replace("臺", "台");
                return string.Join(",", tmpStr.Split(',').Distinct().ToArray());
            }
        }

        #endregion

        public string FilterTab { get; set; }

        public IList<C201MGridModel> Grid { get; set; }
    }

    public class C201MGridModel : Teacher
    {
        [NotDBField]
        public long? ROWID { get; set; }

        /// <summary>組合中英文姓名 (顯示用)</summary>
        [NotDBField]
        public string CombineName
        {
            get
            {
                string tmpEName = "";
                if (this.TeacherEName.TONotNullString() != "") tmpEName = " (" + this.TeacherEName + ")";
                return this.TeacherName + tmpEName;
            }
        }
    }

    public class C201MDetailModel : Teacher
    {
        /// <summary>記錄瀏覽頁的查詢條件，以 || 分隔</summary>
        public string FilterStr { get; set; }

        /// <summary>編輯區塊之代號</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public string EditAreaIdx { get; set; }

        #region for View

        /// <summary>所屬轄區 (分署別)</summary>
        [NotDBField]
        public string UnitName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                string tmpStr = "";
                var tmp = model.UNIT_All_List.Where(x => x.Value == this.UnitCode.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        /// <summary>組合中英文姓名 (顯示用)</summary>
        [NotDBField]
        public string CombineName
        {
            get
            {
                string tmpEName = "";
                if (this.TeacherEName.TONotNullString() != "") tmpEName = " (" + this.TeacherEName + ")";
                return this.TeacherName + tmpEName;
            }
        }

        /// <summary>最高學歷</summary>
        [NotDBField]
        public string EduLevelHighest_Name { get; set; }

        /// <summary>學歷1</summary>
        [NotDBField]
        public string EduLevel1_Name { get; set; }

        /// <summary>學歷2</summary>
        [NotDBField]
        public string EduLevel2_Name { get; set; }

        /// <summary>學歷3</summary>
        [NotDBField]
        public string EduLevel3_Name { get; set; }

        /// <summary>專長領域 (逗號分隔)</summary>
        [NotDBField]
        public string Expertise_Str { get; set; }

        /// <summary>專長領域 (Expertise_Str 切割後清單)</summary>
        [NotDBField]
        public string[] Expertise_List
        {
            get
            {
                if (this.Expertise_Str.TONotNullString() == "") return null;
                return this.Expertise_Str.Split(',').ToArray();
            }
        }

        /// <summary>授課區域</summary>
        [NotDBField]
        public string TeachArea_Name { get { return this.TeachArea.TONotNullString().Replace(",", "、"); } }

        /// <summary>授課產業別</summary>
        [NotDBField]
        public string IndustryStr { get; set; }

        /// <summary>授課產業別 (顯示用)</summary>
        [NotDBField]
        public string Industry_Name
        {
            get
            {
                if (this.IndustryStr.TONotNullString() == "") return null;
                string[] list = this.IndustryStr.TONotNullString().Split(',');
                return string.Join("、", list.Where(x => x != "").ToArray());
            }
        }

        /// <summary>(前1年度)授課時數</summary>
        [NotDBField]
        public int? R1_THOURS_TRANSDATA_DC { get; set; }
        /// <summary>(前1年度)授課時數</summary>
        [NotDBField]
        public int? R1_THOURS_TRANSDATA_BC { get; set; }
        /// <summary>(前1年度)授課時數</summary>
        [NotDBField]
        public int? R1_THOURS_TRANSDATA_KC { get; set; }
        /// <summary>(前1年度)授課時數</summary>
        [NotDBField]
        public int? R1_THOURS_TRANSDATA_TOTAL { get; set; }

        /// <summary>(當年度)授課時數</summary>
        [NotDBField]
        public int? R2_THOURS_TRANSDATA_DC { get; set; }
        /// <summary>(當年度)授課時數</summary>
        [NotDBField]
        public int? R2_THOURS_TRANSDATA_BC { get; set; }
        /// <summary>(當年度)授課時數</summary>
        [NotDBField]
        public int? R2_THOURS_TRANSDATA_KC { get; set; }
        /// <summary>(當年度)授課時數</summary>
        [NotDBField]
        public int? R2_THOURS_TRANSDATA_TOTAL { get; set; }
        #endregion

        #region for Edit

        /// <summary>資料夾路徑名 (暫存，程式用)</summary>
        [NotDBField]
        public string _tmpPicPath { get; set; }

        [NotDBField]
        [Display(Name = "頭像圖示/照片")]
        public HttpPostedFileBase Pic_FILE { get; set; }

        //[Required] 改為手動各別檢查 (區分Area)
        [Display(Name = "姓名(中文)")]
        public string TeacherName { get; set; }

        [Display(Name = "姓名(英文)")]
        public string TeacherEName { get; set; }

        //[Required] 改為手動各別檢查 (區分Area)
        [Display(Name = "性別")]
        public string Sex { get; set; }

        [Display(Name = "聯絡電話")]
        public string Tel { get; set; }

        [Display(Name = "聯絡手機")]
        public string Phone { get; set; }

        //[Required] 改為手動各別檢查 (區分Area)
        [Display(Name = "出生日期")]
        [Control(Mode = Control.DatePicker)]
        public string Birthday_AD
        {
            get
            {
                if (Birthday.TONotNullString() == "") return "";
                DateTime tmp = HelperUtil.TransToDateTime(Birthday, "/") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                Birthday = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        //[Required] 改為手動各別檢查 (區分Area)
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Email(工作)")]
        public string EmailWork { get; set; }

        [Display(Name = "聯絡地址")]
        public string Address { get; set; }

        [Display(Name = "前台顯示")]
        public bool IsShow_Tel
        {
            get { return IsShowTel == "Y"; }
            set { IsShowTel = value ? "Y" : "N"; }
        }

        [Display(Name = "前台顯示")]
        public bool IsShow_Phone
        {
            get { return IsShowPhone == "Y"; }
            set { IsShowPhone = value ? "Y" : "N"; }
        }

        [Display(Name = "前台顯示")]
        public bool IsShow_Birthday
        {
            get { return IsShowBirthday == "Y"; }
            set { IsShowBirthday = value ? "Y" : "N"; }
        }

        [Display(Name = "前台顯示")]
        public bool IsShow_Email
        {
            get { return IsShowEmail == "Y"; }
            set { IsShowEmail = value ? "Y" : "N"; }
        }

        [Display(Name = "前台顯示")]
        public bool IsShow_EmailWork
        {
            get { return IsShowEmailWork == "Y"; }
            set { IsShowEmailWork = value ? "Y" : "N"; }
        }

        [Display(Name = "前台顯示")]
        public bool IsShow_Address
        {
            get { return IsShowAddress == "Y"; }
            set { IsShowAddress = value ? "Y" : "N"; }
        }

        //[Required] 改為手動各別檢查 (區分Area)
        [Display(Name = "最高學歷")]
        public string EduLevelHighest { get; set; }

        public IList<SelectListItem> EduLevelHighest_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.EduLevel_List;
                tmp.Insert(0, new SelectListItem() { Value = "", Text = "<請選擇>" });
                return tmp;
            }
        }

        [Display(Name = "學歷 1")]
        public string EduLevel1 { get; set; }

        public IList<SelectListItem> EduLevel1_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.EduLevel_List;
                tmp.Insert(0, new SelectListItem() { Value = "", Text = "<請選擇>" });
                return tmp;
            }
        }

        [Display(Name = "學歷 2")]
        public string EduLevel2 { get; set; }

        public IList<SelectListItem> EduLevel2_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.EduLevel_List;
                tmp.Insert(0, new SelectListItem() { Value = "", Text = "<請選擇>" });
                return tmp;
            }
        }

        [Display(Name = "學歷 3")]
        public string EduLevel3 { get; set; }

        public IList<SelectListItem> EduLevel3_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmp = model.EduLevel_List;
                tmp.Insert(0, new SelectListItem() { Value = "", Text = "<請選擇>" });
                return tmp;
            }
        }

        [Display(Name = "學校 1")]
        public string EduSchool1 { get; set; }

        [Display(Name = "學校 2")]
        public string EduSchool2 { get; set; }

        [Display(Name = "學校 3")]
        public string EduSchool3 { get; set; }

        [Display(Name = "科系 1")]
        public string EduDept1 { get; set; }

        [Display(Name = "科系 2")]
        public string EduDept2 { get; set; }

        [Display(Name = "科系 3")]
        public string EduDept3 { get; set; }

        [Display(Name = "服務單位 1")]
        public string ServiceUnit1 { get; set; }

        [Display(Name = "服務單位 2")]
        public string ServiceUnit2 { get; set; }

        [Display(Name = "職稱 1")]
        public string JobTitle1 { get; set; }

        [Display(Name = "職稱 2")]
        public string JobTitle2 { get; set; }

        [Display(Name = "所屬轄區")]
        public string UnitCode { get; set; }

        [NotDBField]
        public IList<SelectListItem> UnitCode_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                return model.UNIT_List;
            }
        }

        [Display(Name = "專長描述")]
        public string ExpertiseDesc { get; set; }

        [Display(Name = "專長類別標籤")]
        public string ExpertiseCode { get; set; }

        [NotDBField]
        public string[] ExpertiseCode_SHOW
        {
            get
            {
                return (this.ExpertiseCode != null) ? this.ExpertiseCode.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.ExpertiseCode = string.Join(",", value.ToList());
            }
        }

        [NotDBField]
        public IList<CheckBoxListItem> ExpertiseCode_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>
                // 　　轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.Expertise_List;
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        [NotDBField]
        [Display(Name = "職能類別")]
        public string TeachJobAbilitys { get; set; }

        [NotDBField]
        [Display(Name = "動機職能(DC)")]
        public bool Is_TeachJobAbilityDC
        {
            get { return TeachJobAbilityDC == "Y"; }
            set { TeachJobAbilityDC = value ? "Y" : "N"; }
        }

        [NotDBField]
        [Display(Name = "行為職能(BC)")]
        public bool Is_TeachJobAbilityBC
        {
            get { return TeachJobAbilityBC == "Y"; }
            set { TeachJobAbilityBC = value ? "Y" : "N"; }
        }

        [NotDBField]
        [Display(Name = "知識職能(KC)")]
        public bool Is_TeachJobAbilityKC
        {
            get { return TeachJobAbilityKC == "Y"; }
            set { TeachJobAbilityKC = value ? "Y" : "N"; }
        }

        [Display(Name = "授課區域")]
        public string TeachArea { get; set; }

        [NotDBField]
        [Display(Name = "授課產業別")]
        public string TeachIndustryDetName
        {
            get { return this.IndustryStr; }
            set
            {
                this.TeachIndustryDetCode = null;
                if (value.TONotNullString() == "") return;
                // 逆推
                // 由中文名，反推回代碼，儲存使用
                AMDAO dao = new AMDAO();
                var Datas = dao.GetJsonIndustry();  // 資料集
                var Values = value.TONotNullString().Split(',');  // 使用者選擇
                var Result = value;  // 回寫用變數
                // 轉換成編號
                foreach (var item in Values)
                {
                    int tmpID = Datas.Where(x => x.DETNAME == item).Select(s => s.DETID).FirstOrDefault() ?? 0;
                    if (tmpID <= 0) continue;
                    Result = Result.Replace(item, tmpID.ToString());
                }
                // 檢查是否選擇 "全區"
                foreach (var item in Result.Split(','))
                {
                    if (!item.Contains("全區")) continue;
                    string tmpStr = item.Replace("全區", "");
                    var tmpList = Datas.Where(x => x.MSTNAME == tmpStr).Select(s => s.DETID).ToArray();
                    Result = Result.Replace(item, "");  // 移除全區字樣
                    Result = Result.Replace(",,", ",");  // 整理逗號
                    if (Result != "" && Result.First() == ',') Result = Result.Remove(0, 1);  // 去掉頭逗號
                    if (Result != "" && Result.Last() != ',') Result = Result + ",";  // 檢查是否有尾逗號
                    Result = Result + string.Join(",", tmpList);
                }
                this.TeachIndustryDetCode = string.Join(",", Result.Split(',').Distinct().ToArray());
            }
        }

        [Display(Name = "產學類別")]
        public string IndustryAcademicType { get; set; }

        [NotDBField]
        public string IndustryAcademicType_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                string tmpStr = "";
                var tmp = model.IndustryAcademicType_List().Where(x => x.Value == this.IndustryAcademicType.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [NotDBField]
        public IList<SelectListItem> IndustryAcademicType_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.IndustryAcademicType_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "工作經歷")]
        public string WorkHistory { get; set; }

        [Display(Name = "專業證照")]
        public string ProLicense { get; set; }

        [Display(Name = "自我簡介")]
        public string SelfIntroduction { get; set; }

        #endregion

        #region for Other

        [Display(Name = "加入時間")]
        public string JoinYear { get; set; }

        [Display(Name = "共通核心職能老師")]
        public string PublicCore_Name { get { return (this.PublicCore.TONotNullString() == "Y" ? "是" : (this.PublicCore.TONotNullString() == "N" ? "否" : "")); } }

        [Display(Name = "在線/下線")]
        public string Online { get; set; }

        public string Online_Name { get { return (this.Online.TONotNullString() == "Y") ? "在線" : "下線"; } }

        [Display(Name = "下線日期")]
        public string OfflineDate { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string OfflineDate_AD
        {
            get
            {
                if (OfflineDate.TONotNullString() == "") return "";
                DateTime tmp = HelperUtil.TransToDateTime(OfflineDate, "/") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                OfflineDate = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "/");  // YYYMMDD 民國年 使用者看到
            }
        }

        [Display(Name = "下線原因")]
        [Control(Mode = Control.Hidden)]
        public string OfflineReason { get; set; }

        public string OfflineReason_Str
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var AllList = model.OfflineReason_List();
                var Result = this.OfflineReason.TONotNullString().Split(',').ToArray();
                for (int i = 0; i <= Result.Length - 1; i++)
                {
                    var tmpStr = AllList.Where(x => x.Value == Result[i]);
                    if (!tmpStr.Any()) continue;
                    Result[i] = (i + 1).ToString() + ". " + tmpStr.FirstOrDefault().Text;
                }
                return string.Join("\r\n", Result);
            }
        }

        [NotDBField]
        public string[] OfflineReason_SHOW
        {
            get
            {
                return (this.OfflineReason != null) ? this.OfflineReason.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.OfflineReason = string.Join(",", value.ToList());
            }
        }

        [NotDBField]
        public IList<CheckBoxListItem> OfflineReason_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>
                // 　　轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.OfflineReason_List();
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        [Display(Name = "下線原因(其他)")]
        [Control(Mode = Control.Hidden)]
        public string OfflineReasonRemark { get; set; }

        [Display(Name = "是否在首頁輪播")]
        public string HomePageCarousel { get; set; }

        public string HomePageCarousel_Name { get { return (this.HomePageCarousel.TONotNullString() == "Y") ? "是" : "否"; } }

        [Display(Name = "服務單位屬性")]
        [Control(Mode = Control.Hidden)]
        public string ServiceUnitProperties { get; set; }

        public string ServiceUnitProperties_Str
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var AllList = model.ServiceUnitProperties_List();
                var Result = this.ServiceUnitProperties.TONotNullString().Split(',').ToArray();
                for (int i = 0; i <= Result.Length - 1; i++)
                {
                    var tmpStr = AllList.Where(x => x.Value == Result[i]);
                    if (!tmpStr.Any()) continue;
                    Result[i] = /*(i + 1).ToString() + ". " +*/ tmpStr.FirstOrDefault().Text;
                }
                return string.Join(", ", Result);
            }
        }

        [NotDBField]
        public string[] ServiceUnitProperties_SHOW
        {
            get
            {
                return (this.ServiceUnitProperties != null) ? this.ServiceUnitProperties.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.ServiceUnitProperties = string.Join(",", value.ToList());
            }
        }

        [NotDBField]
        public IList<CheckBoxListItem> ServiceUnitProperties_SHOW_list
        {
            get
            {
                // 把型態從 List<SelectListItem>
                // 　　轉成 List<CheckBoxListItem>
                ShareCodeListModel model = new ShareCodeListModel();
                IList<SelectListItem> ListOrg = model.ServiceUnitProperties_List();
                IList<CheckBoxListItem> ListNew = new List<CheckBoxListItem>();
                foreach (var item in ListOrg) ListNew.Add(new CheckBoxListItem(item.Value, item.Text, false));
                return ListNew;
            }
        }

        #endregion
    }

    /// <summary>
    /// 抓取產業別大小類用 API 回傳類別
    /// </summary>
    public class JsonIndustry
    {
        /// <summary>IndustryMst Code 產業別(大類) 代碼</summary>
        public int? MSTID { get; set; }

        /// <summary>IndustryDet Code 產業別(小類) 代碼</summary>
        public int? DETID { get; set; }

        /// <summary>IndustryMst Name 產業別(大類) 名稱</summary>
        public string MSTNAME { get; set; }

        /// <summary>IndustryDet Name 產業別(小類) 名稱</summary>
        public string DETNAME { get; set; }
    }
}