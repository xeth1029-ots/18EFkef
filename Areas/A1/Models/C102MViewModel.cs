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
using System;

namespace WKEFSERVICE.Areas.A1.Models
{
    public class C102MViewModel
    {
        public C102MViewModel() { this.Form = new C102MFormModel(); }

        public C102MFormModel Form { get; set; }

        public C102MDetailModel Detail { get; set; }
    }

    public class C102MFormModel : PagingResultsViewModel
    {
        public string PostType { get; set; }

        public string PostTo { get; set; }

        public string CreatedUnit { get; set; }

        [Control(Mode = Control.Hidden)]
        public string KeyWord { get; set; }

        public string PostDate_A { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string PostDate_A_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(PostDate_A, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDate_A = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public string PostDate_B { get; set; }

        [Control(Mode = Control.DatePicker)]
        public string PostDate_B_AD
        {
            get
            {
                return HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(PostDate_B, ""), "/");  // YYYYMMDD 回傳給系統
            }
            set
            {
                PostDate_B = HelperUtil.DateTimeToString(HelperUtil.TransToDateTime(value), "");  // YYYMMDD 民國年 使用者看到
            }
        }

        public IList<C102MGridModel> Grid { get; set; }
    }

    public class C102MGridModel : MsgPost
    {
        [NotDBField]
        public long? ROWID { get; set; }

        [NotDBField]
        public string PostType_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.PostType_list().Where(x => x.Value == this.PostType.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [NotDBField]
        public string PostTo_Name
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                var tmpStr = "";
                var tmp = model.GRPID_List.Where(x => x.Value == this.PostTo.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        [NotDBField]
        public string PostDateS_Name
        {
            get
            {
                //if (PostDateS.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(PostDateS) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (PostDateS.TONotNullString() == "") return "";
                string[] tmpList = PostDateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string PostDateE_Name
        {
            get
            {
                //if (PostDateE.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(PostDateE) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (PostDateE.TONotNullString() == "") return "";
                string[] tmpList = PostDateE.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string UpdatedDatetime_Name
        {
            get
            {
                //if (UpdatedDatetime.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(UpdatedDatetime) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (UpdatedDatetime.TONotNullString() == "") return "";
                string[] tmpList = UpdatedDatetime.ToString().Split(' ');
                string tmpDate = tmpList[0];
                string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd") + " " + tmpTime;
            }
        }

        [NotDBField]
        public string PublishedUnitName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                string tmpStr = "";
                var tmp = model.UNIT_All_List.Where(x => x.Value == this.PublishedUnit.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }
        
        /// <summary>是否可以報名</summary>
        [NotDBField]
        public int isCanSignUp { get; set; }
    }

    public class C102MDetailModel : MsgPost
    {
        [NotDBField]
        public string PostDateS_Name
        {
            get
            {
                //if (PostDateS.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(PostDateS) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (PostDateS.TONotNullString() == "") return "";
                string[] tmpList = PostDateS.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string PostDateE_Name
        {
            get
            {
                //if (PostDateE.TONotNullString() == "") return "";
                //DateTime tmp = HelperUtil.TransToDateTime(PostDateE) ?? DateTime.MinValue;
                //return tmp.AddYears(-1911).ToString("yyy/MM/dd");
                if (PostDateE.TONotNullString() == "") return "";
                string[] tmpList = PostDateE.ToString().Split(' ');
                string tmpDate = tmpList[0];
                //string tmpTime = tmpList[1];
                DateTime tmp = HelperUtil.TransToDateTime(tmpDate, "-") ?? DateTime.MinValue;
                return tmp.AddYears(-1911).ToString("yyy/MM/dd");// + " " + tmpTime;
            }
        }

        [NotDBField]
        public string PublishedUnitName
        {
            get
            {
                ShareCodeListModel model = new ShareCodeListModel();
                string tmpStr = "";
                var tmp = model.UNIT_All_List.Where(x => x.Value == this.PublishedUnit.TONotNullString());
                if (tmp.Any()) tmpStr = tmp.FirstOrDefault().Text;
                return tmpStr;
            }
        }

        /// <summary>是否可以報名</summary>
        [NotDBField]
        public int isCanSignUp { get; set; }

        public IList<C102MAttachedsModel> Attacheds { get; set; }
    }

    public class C102MAttachedsModel : MsgPostAttached
    {
        [NotDBField]
        public long? ROWID { get; set; }
    }
}