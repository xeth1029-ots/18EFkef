using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Services;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using System.Web.Mvc;
using Turbo.Commons;
using System.Linq;
using System;
using System.Web;
using WKEFSERVICE.DataLayers;

namespace WKEFSERVICE.Areas.A3.Models
{
    public class C501MViewModel
    {
        public C501MViewModel() { this.Form = new C501MFormModel(); }

        public C501MFormModel Form { get; set; }
    }

    public class C501MFormModel //: PagingResultsViewModel
    {
        public C501MFormModel() { this.ExportFormat = "0"; }

        [Display(Name = "所屬轄區")]
        public string Unit { get; set; }

        public IList<SelectListItem> Unit_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "請選擇", Value = "" };
                var list = model.UNIT_List;
                list.Insert(0, itemAll);
                return list;
            }
        }

        [Display(Name = "是否在線")]
        public string Online { get; set; }

        public IList<SelectListItem> Online_list
        {
            get
            {
                var dictionary = new Dictionary<string, string> {
                    { "",  "不區分" },
                    { "Y", "在線" },
                    { "N", "下線" }
                };
                return MyCommonUtil.ConvertSelItems(dictionary);
            }
        }

        [Display(Name = "匯出欄位")]
        public string ExportFields { get; set; }

        public string[] ExportFields_SHOW
        {
            get
            {
                return (this.ExportFields != null) ? this.ExportFields.Replace("'", "").Split(',') : new string[0];
            }
            set
            {
                if (value != null)
                    this.ExportFields = string.Join(",", value.ToList());
            }
        }

        public IList<CheckBoxListItem> ExportFields_SHOW_list
        {
            get
            {
                List<CheckBoxListItem> Result = new List<CheckBoxListItem>();
                string[] tmpList = {
                    "帳號",
                    //"身分證字號",  分署人員不可匯出此欄位
                    "姓名(英文)",
                    "性別",
                    "生日",
                    "聯絡電話",
                    "聯絡手機",
                    "電子郵件",
                    "電子郵件(工作)",
                    "聯絡地址",
                    "最高學歷",
                    "學歷背景",
                    "服務單位/職稱",
                    "專長領域",
                    "專長類別標籤",
                    "可授課職能類別",
                    "授課區域",
                    "授課產業別",
                    "產學類別",
                    "工作經歷",
                    "專業證照",
                    "自我簡介",
                    "加入年度",
                    "服務單位屬性",
                    "是否在線",
                    "下線日期",
                    "下線原因",
                    "下線原因(其他)",
                    "是否在首頁輪播"
                };
                int Idx = 0;
                foreach (var tmpStr in tmpList)
                {
                    Result.Add(new CheckBoxListItem(Idx.ToString(), tmpStr, false));
                    Idx++;
                }
                return Result;
            }
        }

        [Display(Name = "匯出檔案格式")]
        public string ExportFormat { get; set; }

        public IList<C501MGridModel> Grid { get; set; }
    }

    public class C501MGridModel : Teacher
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string Sex_Name { get { return (this.Sex.TONotNullString() == "F") ? "女" : "男"; } }

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

        [NotDBField]
        public string EduLevelHighest_Name { get; set; }

        [NotDBField]
        public string EduLevel1_Name { get; set; }

        [NotDBField]
        public string EduLevel2_Name { get; set; }

        [NotDBField]
        public string EduLevel3_Name { get; set; }

        [NotDBField]
        public string Expertise_Str { get; set; }

        [NotDBField]
        public string TeachArea_Name { get { return this.TeachArea.TONotNullString().Replace(",", "、"); } }

        [NotDBField]
        public string IndustryStr { get; set; }

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

        [NotDBField]
        public bool Is_TeachJobAbilityDC
        {
            get { return TeachJobAbilityDC == "Y"; }
            set { TeachJobAbilityDC = value ? "Y" : "N"; }
        }

        [NotDBField]
        public bool Is_TeachJobAbilityBC
        {
            get { return TeachJobAbilityBC == "Y"; }
            set { TeachJobAbilityBC = value ? "Y" : "N"; }
        }

        [NotDBField]
        public bool Is_TeachJobAbilityKC
        {
            get { return TeachJobAbilityKC == "Y"; }
            set { TeachJobAbilityKC = value ? "Y" : "N"; }
        }

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
        public string HomePageCarousel_Name { get { return (this.HomePageCarousel.TONotNullString() == "Y") ? "是" : "否"; } }

        [NotDBField]
        [Display(Name = "共通核心職能老師")]
        public string PublicCore_Name { get { return (this.PublicCore.TONotNullString() == "Y" ? "是" : (this.PublicCore.TONotNullString() == "N" ? "否" : "")); } }

        [NotDBField]
        public string Online_Name { get { return (this.Online.TONotNullString() == "Y") ? "是" : "否"; } }

        [NotDBField]
        public string ServiceUnitProperties_Name
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
    }

}