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
using System.Collections;

namespace WKEFSERVICE.Areas.A2.Models
{
    public class C601MViewModel
    {
        public C601MViewModel() { this.Form = new C601MFormModel(); }

        public C601MFormModel Form { get; set; }
    }

    public class C601MFormModel : PagingResultsViewModel
    {
        [Control(Mode = Control.Hidden)]
        [Display(Name = "關鍵字")]
        public string KeyWord { get; set; }

        public string DateS_A { get; set; }

        [Display(Name = "公告日期起")]
        [Control(Mode = Control.DatePicker)]
        public string DateS_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(DateS_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateS_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string DateS_B { get; set; }

        [Display(Name = "公告日期迄")]
        [Control(Mode = Control.DatePicker)]
        public string DateS_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(DateS_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                DateS_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public IList<C601MGridModel> Grid { get; set; }
    }

    public class C601MGridModel : Notices
    {
        [NotDBField]
        public string DateS_Name
        {
            get
            {
                //if (DateS.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(DateS) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (DateS.TONotNullString() == "") return "";
                string[] tmpList = DateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        public IList<C601MGridAttachedModel> GridAttached { get; set; }
    }

    public class C601MGridAttachedModel : NoticesAttached
    {

    }
}