using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turbo.DataLayer;
using WKEFSERVICE.Commons;
using WKEFSERVICE.Models.Entities;
using WKEFSERVICE.Models;
using System.Web.Mvc;
using Turbo.Commons;
using System.Linq;
using WKEFSERVICE.Services;

namespace WKEFSERVICE.Areas.A1.Models
{
    public class C101MViewModel
    {
        public C101MViewModel() { this.Form = new C101MFormModel(); }

        public C101MFormModel Form { get; set; }

        public C101MDetailModel Detail { get; set; }
    }

    public class C101MFormModel : PagingResultsViewModel
    {
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


        [Display(Name = "授課區域")]
        [Control(Mode = Control.Hidden)]
        public string ddlTeachArea { get; set; }

        [Display(Name = "居住地")]
        [Control(Mode = Control.Hidden)]
        public string ddlLiveArea { get; set; }

        public IList<C101MGridModel> Grid { get; set; }
    }

    public class C101MGridModel : Teacher
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string forKeyword { get; set; }

        [NotDBField]
        public int? Find_TeachArea { get; set; }

        [NotDBField]
        public int? Find_LiveArea { get; set; }

        [NotDBField]
        public int? Find_IndustryMstCode { get; set; }

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

        /// <summary>服務單位</summary>
        [NotDBField]
        public string ServiceUnit_Name
        {
            get
            {
                string SU1 = ""; string SU2 = ""; string Result = "";
                if (this.ServiceUnit1.TONotNullString() != "" && this.JobTitle1.TONotNullString() != "") SU1 = this.ServiceUnit1 + " / " + this.JobTitle1;
                if (this.ServiceUnit2.TONotNullString() != "" && this.JobTitle2.TONotNullString() != "") SU2 = this.ServiceUnit2 + " / " + this.JobTitle2;
                if (SU1 != "") Result = SU1;
                if (SU2 != "") Result = Result + ", " + SU2;
                return Result;
            }
        }

        /// <summary>職能類別</summary>
        [NotDBField]
        public string TeachJobAbilityStr { get; set; }

        /// <summary>職能類別 (顯示用)</summary>
        [NotDBField]
        public string TeachJobAbility_Name
        {
            get
            {
                if (this.TeachJobAbilityStr.TONotNullString() == "") return null;
                return this.TeachJobAbilityStr.TONotNullString().Substring(0, this.TeachJobAbilityStr.TONotNullString().Length - 1);
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

        /// <summary>產學類別</summary>
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
    }

    public class C101MDetailModel : Teacher
    {
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

        /// <summary>產學類別</summary>
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

    }
}