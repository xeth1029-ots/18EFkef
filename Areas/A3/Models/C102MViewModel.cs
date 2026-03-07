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
    public class C102MViewModel
    {
        public C102MViewModel() { this.Form = new C102MFormModel(); }

        public C102MFormModel Form { get; set; }

        public C102MDetailModel Detail { get; set; }
    }

    public class C102MFormModel //: PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        public string UnitCode { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "轄區")]
        public string UnitName { get; set; }

        [Control(Mode = Control.Hidden)]
        [Display(Name = "年度")]
        public string Year { get; set; }

        public IList<SelectListItem> Year_list
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var itemAll = new SelectListItem() { Text = "全顯示", Value = "" };
                var list = model.ReportRecordYears_List();
                list.Insert(0, itemAll);
                return list;
            }
        }

        public IList<C102MGridModel> Grid { get; set; }
    }

    public class C102MGridModel : Report
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string Year_Name
        {
            get
            {
                if (this.Year.TONotNullString() == "") return "";
                int tmp = this.Year.TONotNullString().TOInt32() - 1911;
                return tmp.ToString() + " (" + this.Year.TONotNullString() + ")";
            }
        }

        [NotDBField]
        public string DateS_Name
        {
            get
            {
                if (this.DateS.TONotNullString() == "") return "";
                var tmp = this.DateS.Split(' ');
                return tmp[0];
            }
        }

        [NotDBField]
        public string DateE_Name
        {
            get
            {
                if (this.DateE.TONotNullString() == "") return "";
                var tmp = this.DateE.Split(' ');
                return tmp[0];
            }
        }
    }

    public class C102MDetailModel : Report
    {
        public C102MDetailModel() { this.IsNew = false; }

        /// <summary>Detail必要控件(Hidden)</summary>
        [Control(Mode = Control.Hidden)]
        [NotDBField]
        public bool IsNew { get; set; }

        [NotDBField]
        [Display(Name = "年度")]
        public string Year_Name
        {
            get
            {
                if (this.Year.TONotNullString() == "") return "";
                int tmp = this.Year.TONotNullString().TOInt32() - 1911;
                return tmp.ToString() + " (" + this.Year.TONotNullString() + ")";
            }
        }

        [NotDBField]
        [Display(Name = "轄區")]
        public string UnitName { get; set; }

        public string DateS { get; set; }

        [Required]
        [Display(Name = "繳交起始日")]
        [Control(Mode = Control.DatePicker)]
        public string DateS_AD
        {
            get
            {
                if (DateS.TONotNullString() == "") return "";
                string[] tmpList = DateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateS = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string DateE { get; set; }

        [Required]
        [Display(Name = "繳交截止日")]
        [Control(Mode = Control.DatePicker)]
        public string DateE_AD
        {
            get
            {
                if (DateE.TONotNullString() == "") return "";
                string[] tmpList = DateE.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return HelperUtil.DateTimeToString(tmp, "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateE = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "-");  // YYYMMDD 民國年 使用者看到
            }
        }
    }
}